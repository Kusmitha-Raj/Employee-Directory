// src/pages/Departments/DepartmentList.tsx
import { useEffect, useMemo, useState } from "react";
import { getDepartments, deleteDepartment } from "../../api/departmentApi";
import DepartmentForm from "./DepartmentForm";
import { useAuth } from "../../context/AuthContext";
import "./department.css";

export default function DepartmentList() {
  const { user } = useAuth();
  const isAdmin = user?.role === "Admin";

  const [departments, setDepartments] = useState<{ id: number; name: string }[]>(
    []
  );
  const [q, setQ] = useState("");
  const [loading, setLoading] = useState(false);
  const [showForm, setShowForm] = useState(false);
  const [mode, setMode] = useState<"create" | "edit">("create");
  const [editing, setEditing] = useState<any | null>(null);
  const [error, setError] = useState<string | null>(null);

  const load = async () => {
    setLoading(true);
    setError(null);
    try {
      const data = await getDepartments();
      setDepartments(data);
    } catch (e: any) {
      setError(e?.message ?? "Load failed");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    load();
  }, []);

  const filtered = useMemo(() => {
    const s = q.trim().toLowerCase();
    if (!s) return departments;
    return departments.filter((d) => d.name.toLowerCase().includes(s));
  }, [departments, q]);

  const totalDepartments = departments.length;
  const totalMatching = filtered.length;

  const openCreate = () => {
    setMode("create");
    setEditing(null);
    setShowForm(true);
  };
  const openEdit = (d: any) => {
    setMode("edit");
    setEditing(d);
    setShowForm(true);
  };

  const onSaved = (saved: any) => {
    setDepartments((prev) => {
      const exists = prev.find((p) => p.id === saved.id);
      if (exists) return prev.map((p) => (p.id === saved.id ? saved : p));
      return [saved, ...prev];
    });
    setShowForm(false);
  };

  const handleDelete = async (id: number) => {
    if (!isAdmin) {
      alert("Not allowed");
      return;
    }
    if (!confirm("Delete department?")) return;
    try {
      await deleteDepartment(id);
      setDepartments((prev) => prev.filter((p) => p.id !== id));
    } catch (e: any) {
      alert(e?.message ?? "Delete failed");
    }
  };

  return (
    <main className="page-wrap table-page">
      {/* toolbar sits above card, same width */}
      <div className="toolbar">
        <h2>Departments</h2>
        <div className="toolbar-right">
          {/* ✅ count pill added */}
          <span className="count-pill">
            {q
              ? `${totalMatching} / ${totalDepartments} departments`
              : `${totalDepartments} departments`}
          </span>

          <input
            className="search"
            value={q}
            onChange={(e) => setQ(e.target.value)}
            placeholder="Search..."
          />
          <button className="btn secondary" onClick={load}>
            Refresh
          </button>
          {isAdmin && (
            <button className="btn" onClick={openCreate}>
              New Department
            </button>
          )}
        </div>
      </div>

      {/* glass card with pill rows */}
      {loading ? (
        <div className="card table-card">Loading...</div>
      ) : error ? (
        <div className="card table-card">
          <div className="error">{error}</div>
        </div>
      ) : (
        <div className="card table-card">
          <table className="table">
            <thead>
              <tr>
                <th>ID</th>
                <th>Name</th>
                {isAdmin && <th>Actions</th>}
              </tr>
            </thead>
            <tbody>
              {filtered.length === 0 ? (
                <tr>
                  <td colSpan={isAdmin ? 3 : 2} className="muted">
                    No matching department found.
                  </td>
                </tr>
              ) : (
                filtered.map((d) => (
                  <tr key={d.id}>
                    <td>{d.id}</td>
                    <td>{d.name}</td>
                    {isAdmin ? (
                      <td>
                        <button
                          className="btn small"
                          onClick={() => openEdit(d)}
                        >
                          Edit
                        </button>
                        <button
                          className="btn small danger"
                          onClick={() => handleDelete(d.id)}
                        >
                          Delete
                        </button>
                      </td>
                    ) : (
                      <td>
                        <span className="muted">Restricted</span>
                      </td>
                    )}
                  </tr>
                ))
              )}
            </tbody>
          </table>
        </div>
      )}

      {/* modal for create / edit */}
      {showForm && (
        <div className="modal-overlay">
          <div className="modal-card" role="dialog" aria-modal="true">
            <h3 style={{ marginTop: 0 }}>
              {mode === "create" ? "Create Department" : "Edit Department"}
            </h3>
            <DepartmentForm
              mode={mode}
              initial={editing ?? undefined}
              onSaved={onSaved}
              onCancel={() => setShowForm(false)}
            />
          </div>
        </div>
      )}
    </main>
  );
}
