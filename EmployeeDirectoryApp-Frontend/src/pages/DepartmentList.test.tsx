import { screen } from "@testing-library/react";
import { renderWithProviders } from "../test-utils";
import DepartmentList from "./Departments/DepartmentList"; // adjust path
import * as departmentApi from "../api/departmentApi";

jest.mock("../api/departmentApi");

describe("DepartmentList page", () => {
  it("renders departments from API", async () => {
    (departmentApi.getDepartments as jest.Mock).mockResolvedValue([
      { departmentId: 1, name: "IT" },
      { departmentId: 2, name: "HR" },
    ]);

    renderWithProviders(<DepartmentList />);

    expect(await screen.findByText(/it/i)).toBeInTheDocument();
    expect(screen.getByText(/hr/i)).toBeInTheDocument();
  });

  it("shows empty state when there are no departments", async () => {
    (departmentApi.getDepartments as jest.Mock).mockResolvedValue([]);

    renderWithProviders(<DepartmentList />);

    const empty = await screen.findByText(/no matching department found/i);
    expect(empty).toBeInTheDocument();
  });
});
