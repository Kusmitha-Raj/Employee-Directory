// src/api/axios.ts
import axios from "axios";
import type {
  AxiosResponse,
  AxiosError,
  InternalAxiosRequestConfig, 
} from "axios";

type RefreshQueueItem = {
  resolve: (token: string) => void;
  reject: (err: any) => void;
};

const api = axios.create({
  baseURL: "http://localhost:7240",
  withCredentials: true,
});

api.interceptors.request.use((config: InternalAxiosRequestConfig) => {
  try {
    const stored = localStorage.getItem("authUser");
    if (stored) {
      const parsed = JSON.parse(stored);
      const token = parsed?.token;
      if (token) {
        if (!config.headers) config.headers = {} as InternalAxiosRequestConfig["headers"];
        (config.headers as any)["Authorization"] = `Bearer ${token}`;
      }
    }
  } catch {
  }
  return config;
});

let isRefreshing = false;
let failedQueue: RefreshQueueItem[] = [];

const processQueue = (error: any, token: string | null = null) => {
  failedQueue.forEach((p) => {
    if (error) p.reject(error);
    else p.resolve(token as string);
  });
  failedQueue = [];
};
api.interceptors.response.use(
  (response: AxiosResponse) => response,
  async (err: AxiosError & { config?: InternalAxiosRequestConfig }) => {
    const originalRequest = err.config;
    if (!originalRequest) return Promise.reject(err);

    const status = err.response?.status;

    if (status === 401 && !(originalRequest as any)._retry) {
      (originalRequest as any)._retry = true;

      if (isRefreshing) {
        return new Promise<string>((resolve, reject) => {
          failedQueue.push({ resolve, reject });
        }).then((token) => {
          originalRequest.headers = originalRequest.headers ?? ({} as InternalAxiosRequestConfig["headers"]);
          (originalRequest.headers as any)["Authorization"] = `Bearer ${token}`;
          return api(originalRequest);
        });
      }

      isRefreshing = true;

      try {
        const refreshResponse = await api.post("/api/auth/refresh");
        const newToken = (refreshResponse.data as any)?.token;
        if (!newToken) throw new Error("Invalid refresh response: no token");

        
        try {
          const stored = localStorage.getItem("authUser");
          if (stored) {
            const parsed = JSON.parse(stored);
            parsed.token = newToken;
            localStorage.setItem("authUser", JSON.stringify(parsed));
          } else {
            localStorage.setItem("token", newToken);
          }
        } catch {}

        processQueue(null, newToken);

        originalRequest.headers = originalRequest.headers ?? ({} as InternalAxiosRequestConfig["headers"]);
        (originalRequest.headers as any)["Authorization"] = `Bearer ${newToken}`;

        return api(originalRequest);
      } catch (refreshError) {
        processQueue(refreshError, null);
        try { localStorage.removeItem("authUser"); localStorage.removeItem("token"); } catch {}
        window.location.href = "/login";
        return Promise.reject(refreshError);
      } finally {
        isRefreshing = false;
      }
    }

    return Promise.reject(err);
  }
);

export default api;
