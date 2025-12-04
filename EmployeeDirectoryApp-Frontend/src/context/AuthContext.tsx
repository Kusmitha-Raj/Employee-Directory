import React, { createContext, useContext, useEffect, useState } from "react";
import api from "../api/axios";

export interface AuthUser {
  username: string;
  role: "Admin" | "Employee" | "User";
  token: string;
  email?: string;
}

interface AuthContextType {
  user: AuthUser | null;
  login: (email: string, password: string) => Promise<AuthUser>;
  logout: () => Promise<void>;
}
export const AuthContext = createContext<AuthContextType | undefined>(undefined);

export const AuthProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const [user, setUser] = useState<AuthUser | null>(() => {
    const stored = localStorage.getItem("authUser");
    if (!stored) return null;
    try {
      const parsed = JSON.parse(stored) as AuthUser;
      if (parsed?.token) {
        api.defaults.headers.common["Authorization"] = `Bearer ${parsed.token}`;
      }
      return parsed;
    } catch {
      return null;
    }
  });
  useEffect(() => {
  }, []);
  const login = async (email: string, password: string): Promise<AuthUser> => {
    const resp = await api.post<{ message?: string; token?: string; role?: string }>(
      "/api/User/Login",
      { email, password }
    );

    const data = resp.data ?? {};
    const token = data.token;
    const roleFromServer = data.role ?? "Employee";

    if (!token) {
      throw new Error(data.message ?? "Login failed: no token returned");
    }

    const role = roleFromServer === "Admin" ? "Admin" : roleFromServer === "Employee" ? "Employee" : "User";

    const authUser: AuthUser = {
      username: email, 
      role,
      token,
      email,
    };
    setUser(authUser);
    try {
      localStorage.setItem("authUser", JSON.stringify(authUser));
    } catch {
    }
    api.defaults.headers.common["Authorization"] = `Bearer ${token}`;

    return authUser;
  };

  const logout = async (): Promise<void> => {
    try {
      await api.post("/api/auth/revoke");
    } catch {
      
    } finally {
      setUser(null);
      try {
        localStorage.removeItem("authUser");
      } catch {}
      try {
        delete api.defaults.headers.common["Authorization"];
      } catch {
        api.defaults.headers.common["Authorization"] = "";
      }
      
    }
  };

  return <AuthContext.Provider value={{ user, login, logout }}>{children}</AuthContext.Provider>;
};

export function useAuth() {
  const ctx = useContext(AuthContext);
  if (!ctx) throw new Error("useAuth must be used within AuthProvider");
  return ctx;
}
