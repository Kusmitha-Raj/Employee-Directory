// src/api/employeeApi.ts
import api from "./axios";

// What the frontend should work with:
export interface EmployeeView {
  id: number;
  firstName?: string;
  lastName?: string;
  name?: string;               // used as fallback only
  email?: string;
  phone?: string;
  departmentId?: number;
  departmentName?: string;
  jobRole?: string;
  gender?: string;
  raw?: any;
}

// What we get from backend (varies)
type BackendEmployee = Record<string, any>;

function mapBackendToFrontend(e: BackendEmployee): EmployeeView {
  const id = e.EmployeeId ?? e.employeeId ?? e.id ?? 0;

  const first = (e.FirstName ?? e.firstName ?? e.first ?? "").toString().trim();
  const last = (e.LastName ?? e.lastName ?? e.last ?? "").toString().trim();

  // 🟢 Build full name only from first + last
  const nameFromParts =
    (first || last ? `${first} ${last}`.trim() : "") || undefined;

  // 🛑 REMOVED email username based fallback ❌
  const name =
    nameFromParts || (id ? `Employee ${id}` : "Unknown");

  return {
    id,
    firstName: first || undefined,
    lastName: last || undefined,
    name,
    email: e.Email ?? e.email ?? undefined,
    phone: e.PhoneNo ?? e.phoneNo ?? e.Phone ?? e.phone ?? undefined,
    departmentId: e.DepartmentId ?? e.departmentId ?? undefined,
    departmentName: e.DepartmentName ?? e.departmentName ?? undefined,
    jobRole: e.JobRole ?? e.jobRole ?? undefined,
    gender: e.Gender ?? e.gender ?? undefined,
    raw: e, // debugging if needed
  };
}

export async function getEmployees(): Promise<EmployeeView[]> {
  const resp = await api.get<BackendEmployee[]>("/api/Employee");
  return (resp.data ?? []).map(mapBackendToFrontend);
}

export async function getEmployeeById(
  id: number
): Promise<EmployeeView> {
  const resp = await api.get<BackendEmployee>(`/api/Employee/${id}`);
  return mapBackendToFrontend(resp.data ?? {});
}

export async function createEmployee(payload: {
  firstName: string;
  lastName: string;
  email?: string;
  phone?: string;
  departmentId?: number;
  jobRole?: string;
  gender?: string;
}): Promise<EmployeeView | null> {
  const body = {
    FirstName: payload.firstName,
    LastName: payload.lastName,
    Email: payload.email,
    PhoneNo: payload.phone,
    DepartmentId: payload.departmentId,
    JobRole: payload.jobRole,
    Gender: payload.gender,
  };

  const resp = await api.post("/api/User/AddEmployee", body);
  return resp?.data ? mapBackendToFrontend(resp.data) : null;
}

export async function updateEmployee(
  id: number,
  payload: {
    firstName?: string;
    lastName?: string;
    email?: string;
    phone?: string;
    departmentId?: number;
    jobRole?: string;
    gender?: string;
  }
): Promise<EmployeeView | null> {
  const body: any = {};

  if (payload.firstName !== undefined) body.FirstName = payload.firstName;
  if (payload.lastName !== undefined) body.LastName = payload.lastName;
  if (payload.email !== undefined) body.Email = payload.email;
  if (payload.phone !== undefined) body.PhoneNo = payload.phone;
  if (payload.departmentId !== undefined) body.DepartmentId = payload.departmentId;
  if (payload.jobRole !== undefined) body.JobRole = payload.jobRole;
  if (payload.gender !== undefined) body.Gender = payload.gender;

  const resp = await api.put(`/api/Employee/${id}`, body);
  return resp?.data ? mapBackendToFrontend(resp.data) : null;
}

export async function deleteEmployee(id: number): Promise<void> {
  await api.delete(`/api/Employee/${id}`);
}
