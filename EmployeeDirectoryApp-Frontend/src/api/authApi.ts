import api from './axios';

export type LoginResponse = {
  token?: string;
  role?: string;
  message?: string;
};

export async function login(email: string, password: string): Promise<LoginResponse> {
  const resp = await api.post<LoginResponse>("/api/auth/login", { email, password });
  const data = resp.data;
  if (data?.token) {
    localStorage.setItem("token", data.token);
  }
  if (data?.role) {
    localStorage.setItem("role", data.role);
  }
  return data;
}

export async function logout(): Promise<void> {
  try {
    await api.post("/api/User/revoke");
  } catch (err) {
  } finally {
    localStorage.removeItem("token");
    localStorage.removeItem("role");
    window.location.href = "/login";
  }
}
