
import api from "./axios";
export interface DepartmentView {
  id: number;
  name: string;
}

type BackendDepartment = {
  DepartmentId?: number;
  departmentId?: number;
  id?: number;

  Name?: string;
  name?: string;
};

function mapBackend(d: BackendDepartment): DepartmentView {
  return {
    id: d.DepartmentId ?? d.departmentId ?? d.id ?? 0,
    name: d.Name ?? d.name ?? ""
  };
}
function mapCreateUpdate(payload: { name: string }) {
  return {
    Name: payload.name
  };
}

export async function getDepartments(): Promise<DepartmentView[]> {
  const { data } = await api.get<BackendDepartment[]>("/api/Department");
  return data.map(mapBackend);
}

export async function getDepartmentById(id: number): Promise<DepartmentView> {
  const { data } = await api.get<BackendDepartment>(`/api/Department/${id}`);
  return mapBackend(data);
}

export async function createDepartment(payload: { name: string }): Promise<DepartmentView> {
  const backendPayload = mapCreateUpdate(payload);
  const { data } = await api.post<BackendDepartment>("/api/Department", backendPayload);
  return mapBackend(data);
}

export async function updateDepartment(id: number, payload: { name: string }): Promise<DepartmentView> {
  const backendPayload = mapCreateUpdate(payload);
  const { data } = await api.put<BackendDepartment>(`/api/Department/${id}`, backendPayload);
  return mapBackend(data);
}

export async function deleteDepartment(id: number): Promise<void> {
  await api.delete(`/api/Department/${id}`);
}
