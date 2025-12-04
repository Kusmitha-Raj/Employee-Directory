import { screen } from "@testing-library/react";
import { renderWithProviders } from "../test-utils";
import EmployeeList from "./Employees/EmployeeList";
import * as employeeApi from "../api/employeeApi";

jest.mock("../api/employeeApi");

describe("EmployeeList page", () => {
  it("renders employees returned from API", async () => {
    (employeeApi.getEmployees as jest.Mock).mockResolvedValue([
      {
        employeeId: 1,
        firstName: "Kusmitha",
        lastName: "Raj",
        departmentName: "IT",
        email: "k@test.com",
        jobRole: "SDE Intern",
        phoneNo: null,
        gender: null,
      },
      {
        employeeId: 2,
        firstName: "John",
        lastName: "Doe",
        departmentName: "HR",
        email: "j@test.com",
        jobRole: "HR Manager",
        phoneNo: null,
        gender: null,
      },
    ]);

    renderWithProviders(<EmployeeList />);

    // Wait for one known piece of data
    expect(await screen.findByText("k@test.com")).toBeInTheDocument();
    expect(screen.getByText("j@test.com")).toBeInTheDocument();

    expect(screen.getByText("IT")).toBeInTheDocument();

// old: expect(screen.getByText(/hr/i)).toBeInTheDocument();
    expect(screen.getByText("HR")).toBeInTheDocument(); // ✅ exact department text


    expect(screen.getByText(/sde intern/i)).toBeInTheDocument();
    expect(screen.getByText(/hr manager/i)).toBeInTheDocument();
  });

  it("shows empty state when no employees", async () => {
    (employeeApi.getEmployees as jest.Mock).mockResolvedValue([]);

    renderWithProviders(<EmployeeList />);

    const empty = await screen.findByText(/no matching employee found/i);
    expect(empty).toBeInTheDocument();
  });
});
