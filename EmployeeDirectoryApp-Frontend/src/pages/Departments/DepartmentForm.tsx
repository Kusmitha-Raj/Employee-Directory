import React, { useEffect, useState } from "react";
import { createDepartment, updateDepartment } from "../../api/departmentApi";
import { useAuth } from "../../context/AuthContext";
import type { DepartmentView } from "../../api/departmentApi";

type Props = {
  mode?: "create" | "edit";
  initial?: Partial<DepartmentView>;
  onSaved?: (d: DepartmentView) => void;
  onCancel?: () => void;
};

export default function DepartmentForm({ mode = "create", initial, onSaved, onCancel }: Props) {
  const { user } = useAuth();
  const isAdmin = user?.role === "Admin";
  const [name, setName] = useState("");
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const computeDisplayName = (src: any) => {
    const i = src || {};
    const n = (i.Name ?? i.name ?? "").toString().trim();
    const id = i.DepartmentId ?? i.departmentId ?? i.id;
    return n || (id ? `Department ${id}` : "");
  };

  useEffect(() => {
    const i = initial as any || {};
    const nameVal = i.name ?? i.Name ?? "";
    setName(nameVal);
    setError(null);
  }, [initial]);

  const submit = async (e?: React.FormEvent) => {
    e?.preventDefault();
    setError(null);

    if (!name.trim()) { setError("Name required"); return; }
    if (!isAdmin) { setError("Permission denied"); return; }

    setLoading(true);
    try {
      if (mode === "create") {
        const saved = await createDepartment({ name: name.trim() });
        onSaved?.(saved);
      } else {
        if (!initial?.id) throw new Error("Missing id");
        const saved = await updateDepartment(initial.id, { name: name.trim() });
        onSaved?.(saved);
      }
    } catch (err: any) {
      setError(err?.response?.data?.message ?? err?.message ?? "Save failed");
    } finally {
      setLoading(false);
    }
  };

  const displayName = computeDisplayName(initial);

  return (
    <form className="card" onSubmit={submit} style={{ minWidth: 320, maxWidth: 420 }}>
      {displayName ? (
        <div style={{ marginBottom: 10, fontWeight: 600 }}>
          Department: <span style={{ color: "#1e3a8a" }}>{displayName}</span>
        </div>
      ) : null}

      <h3 style={{ marginTop: 0 }}>{mode === "create" ? "Create Department" : "Edit Department"}</h3>

      <label className="field">
        <span>Name</span>
        <input value={name} onChange={(e) => setName(e.target.value)} disabled={!isAdmin || loading} />
      </label>

      {error && <div className="error">{error}</div>}

      <div className="form-actions">
        <button className="btn" type="submit" disabled={loading || !isAdmin}>
          {loading ? "Saving..." : "Save"}
        </button>
        <button className="btn secondary" type="button" onClick={() => onCancel?.()} disabled={loading}>
          Cancel
        </button>
      </div>
    </form>
  );
}
