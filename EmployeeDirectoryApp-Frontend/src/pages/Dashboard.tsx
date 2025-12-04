// src/pages/Dashboard.tsx
import { useEffect, useState } from "react";
import { useAuth } from "../context/AuthContext";
import { getEmployees } from "../api/employeeApi";
import { getDepartments } from "../api/departmentApi";

export default function Dashboard() {
  const { user } = useAuth();
  const displayName = user?.username?.split("@")[0].toUpperCase();

  const [employeeCount, setEmployeeCount] = useState<number | null>(null);
  const [departmentCount, setDepartmentCount] = useState<number | null>(null);
  const [loadingStats, setLoadingStats] = useState(false);
  const [statsError, setStatsError] = useState<string | null>(null);

  useEffect(() => {
    (async () => {
      setLoadingStats(true);
      setStatsError(null);
      try {
        const [emps, deps] = await Promise.all([
          getEmployees(),
          getDepartments(),
        ]);
        setEmployeeCount(emps.length);
        setDepartmentCount(deps.length);
      } catch (e: any) {
        setStatsError(e?.message ?? "Failed to load dashboard stats");
      } finally {
        setLoadingStats(false);
      }
    })();
  }, []);

  return (
    <main
      className="page-wrap"
      style={{
        display: "flex",
        flexDirection: "column",
        alignItems: "center",
        gap: "25px",
        minHeight: "75vh",
        padding: "20px",
      }}
    >
      {/* CARD 1 — GREETING + STATS */}
      <div
        className="card"
        style={{
          padding: "25px 35px",
          background: "#eaf1ff",
          borderRadius: "12px",
          boxShadow: "0 8px 20px rgba(0,0,0,0.08)",
          maxWidth: "900px",
          width: "100%",
          display: "flex",
          alignItems: "stretch",
          justifyContent: "space-between",
          gap: "24px",
        }}
      >
        {/* LEFT: GREETING */}
        <div
          style={{
            flex: 1,
            display: "flex",
            flexDirection: "column",
            justifyContent: "center",
          }}
        >
          <h2 style={{ margin: 0, fontWeight: 700, color: "#1e3a8a" }}>
            <span style={{ color: "#2563eb" }}>Hi {displayName}</span>
          </h2>
          <p
            style={{
              marginTop: "10px",
              fontSize: "1.1rem",
              color: "#374151",
            }}
          >
            Welcome to Employee Directory App!
          </p>
          {statsError && (
            <p style={{ marginTop: "8px", color: "#b91c1c", fontSize: "0.9rem" }}>
              {statsError}
            </p>
          )}
        </div>

        {/* RIGHT: COLOURFUL STATS CARDS */}
        <div
          style={{
            display: "flex",
            flexDirection: "row",
            gap: "16px",
            alignItems: "stretch",
          }}
        >
          {/* Employees card */}
          <div
            style={{
              minWidth: "150px",
              padding: "14px 16px",
              borderRadius: "14px",
              background:
                "linear-gradient(135deg, #2563eb, #4f46e5)", // blue-purple
              color: "#f9fafb",
              display: "flex",
              flexDirection: "column",
              justifyContent: "space-between",
            }}
          >
            <span
              style={{
                fontSize: "0.85rem",
                opacity: 0.9,
              }}
            >
              Total Employees
            </span>
            <span
              style={{
                fontSize: "1.8rem",
                fontWeight: 800,
                lineHeight: 1.1,
                marginTop: "6px",
              }}
            >
              {loadingStats ? "…" : employeeCount ?? 0}
            </span>
          </div>

          {/* Departments card */}
          <div
            style={{
              minWidth: "150px",
              padding: "14px 16px",
              borderRadius: "14px",
              background:
                "linear-gradient(135deg, #16a34a, #22c55e)", // green gradient
              color: "#f9fafb",
              display: "flex",
              flexDirection: "column",
              justifyContent: "space-between",
            }}
          >
            <span
              style={{
                fontSize: "0.85rem",
                opacity: 0.9,
              }}
            >
              Total Departments
            </span>
            <span
              style={{
                fontSize: "1.8rem",
                fontWeight: 800,
                lineHeight: 1.1,
                marginTop: "6px",
              }}
            >
              {loadingStats ? "…" : departmentCount ?? 0}
            </span>
          </div>
        </div>
      </div>

      {/* CARD 2 — IMAGE + INFO */}
      <div
        className="card"
        style={{
          padding: "30px",
          borderRadius: "12px",
          boxShadow: "0 8px 20px rgba(0,0,0,0.08)",
          maxWidth: "900px",
          width: "100%",
          display: "flex",
          alignItems: "center",
          gap: "25px",
          background: "#ffffff",
        }}
      >
        {/* LEFT: TEXT */}
        <div style={{ flex: 1 }}>
          <h3 style={{ margin: 0, fontWeight: 700, color: "#1e3a8a" }}>
            Employee Directory
          </h3>

          <p
            style={{
              marginTop: "12px",
              lineHeight: "1.6",
              color: "#4b5563",
            }}
          >
            Employee Directory helps to quickly find, manage, and update employee
            information across the organization. Admin can add, edit, and remove
            employees, while regular employees get view of their profile and
            colleagues.
          </p>
        </div>

        {/* RIGHT: IMAGE */}
        <div>
          <img
            src="https://th.bing.com/th/id/OIP.5sE_dCbt70M6gxK3QXKcOAHaEJ?w=328&h=184&c=7&r=0&o=7&dpr=1.5&pid=1.7&rm=3"
            alt="Dashboard Visual"
            style={{
              width: "300px",
              borderRadius: "12px",
            }}
          />
        </div>
      </div>
    </main>
  );
}
