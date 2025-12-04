import { render, screen, waitFor } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import EmployeeList from "./Employees/EmployeeList";
import type { EmployeeView } from "../api/employeeApi";

jest.mock("../api/employeeApi", () => ({
  getEmployees: jest.fn(),
  getEmployeeById: jest.fn(),
  createEmployee: jest.fn(),
  updateEmployee: jest.fn(),
  deleteEmployee: jest.fn(),
}));

jest.mock("../context/AuthContext", () => ({
  useAuth: jest.fn(),
}));

jest.mock("react-toastify", () => ({
  toast: {
    success: jest.fn(),
    error: jest.fn(),
  },
}));


jest.mock("./Employees/EmployeeForm", () => {
 
  return (props: any) => (
    <div data-testid="employee-form">
      <button
        onClick={() =>
          props.onSave?.({ firstName: "Raksha", lastName: "Achary" })
        }
      >
        Mock Save
      </button>
      {props.onCancel && <button onClick={props.onCancel}>Mock Cancel</button>}
    </div>
  );
});

const {
  getEmployees,
  getEmployeeById,
  createEmployee,
  updateEmployee,
  deleteEmployee,
} = jest.requireMock("../api/employeeApi");

const { useAuth } = jest.requireMock("../context/AuthContext");
const { toast } = jest.requireMock("react-toastify");

const mockEmployees: EmployeeView[] = [
  {
    id: 1,
    firstName: "Raksha",
    lastName: "Achary",
    email: "raksha@test.com",
    phone: "7656456789",
    departmentName: "HR",
    jobRole: "Developer",
    gender: "Female",
    name: "Raksha Achary",
  } as any,
  {
    id: 2,
    firstName: "Kavana",
    lastName: "K",
    email: "kavana@test.com",
    phone: "9876564534",
    departmentName: "IT",
    jobRole: "Engineer",
    gender: "Female",
    name: "Kavana K",
  } as any,
];

describe("EmployeeList", () => {
  const user = userEvent.setup();

  beforeAll(() => {
    // @ts-ignore
    global.confirm = jest.fn(() => true);
  });

  beforeEach(() => {
    jest.clearAllMocks();

    (useAuth as jest.Mock).mockReturnValue({
      user: { role: "Admin" },
    });

    (getEmployees as jest.Mock).mockResolvedValue(mockEmployees);
  });

  it("loads and renders employees", async () => {
    render(<EmployeeList />);

    expect(screen.getByText(/loading/i)).toBeInTheDocument();

    expect(await screen.findByText("Raksha Achary")).toBeInTheDocument();
    expect(screen.getByText("Kavana K")).toBeInTheDocument();
    expect(screen.getByText(/2 employees/i)).toBeInTheDocument();
  });

  it("shows error when load fails", async () => {
    (getEmployees as jest.Mock).mockRejectedValueOnce(
      new Error("Server down")
    );

    render(<EmployeeList />);
    expect(await screen.findByText(/server down/i)).toBeInTheDocument();
  });

  it("filters by search text", async () => {
    render(<EmployeeList />);

    await screen.findByText("Raksha Achary");

    const search = screen.getByPlaceholderText(/search employees/i);
    await user.type(search, "raksha");

    expect(screen.getByText("Raksha Achary")).toBeInTheDocument();
    expect(screen.queryByText("Kavana K")).not.toBeInTheDocument();
    expect(screen.getByText(/1 \/ 2 employees/i)).toBeInTheDocument();
  });

  it("does not show admin actions when user is not admin", async () => {
    (useAuth as jest.Mock).mockReturnValue({ user: { role: "Employee" } });

    render(<EmployeeList />);
    await screen.findByText("Raksha Achary");

    expect(
      screen.queryByRole("button", { name: /create employee/i })
    ).not.toBeInTheDocument();
  });

  it("opens create modal and saves new employee", async () => {
    (createEmployee as jest.Mock).mockResolvedValue({
      id: 3,
      firstName: "Raksha",
      lastName: "Achary",
      name: "Raksha Achary",
    });

    render(<EmployeeList />);
    await screen.findByText("Raksha Achary");

    await user.click(screen.getByRole("button", { name: /create employee/i }));
    expect(screen.getByTestId("employee-form")).toBeInTheDocument();

    await user.click(screen.getByText(/mock save/i));

    await waitFor(() => expect(createEmployee).toHaveBeenCalled());
    await waitFor(() => expect(toast.success).toHaveBeenCalled());

    expect(screen.queryByTestId("employee-form")).not.toBeInTheDocument();
  });

  it("opens edit modal and saves changes", async () => {
    (getEmployeeById as jest.Mock).mockResolvedValue(mockEmployees[0]);
    (updateEmployee as jest.Mock).mockResolvedValue(mockEmployees[0]);

    render(<EmployeeList />);
    await screen.findByText("Raksha Achary");

    await user.click(screen.getAllByRole("button", { name: /edit/i })[0]);

    await waitFor(() =>
      expect(getEmployeeById).toHaveBeenCalledWith(1)
    );

    expect(await screen.findByTestId("employee-form")).toBeInTheDocument();

    await user.click(screen.getByText(/mock save/i));

    await waitFor(() =>
      expect(updateEmployee).toHaveBeenCalledWith(1, {
        firstName: "Raksha",
        lastName: "Achary",
      })
    );
  });

  it("deletes an employee", async () => {
    (deleteEmployee as jest.Mock).mockResolvedValue({});

    render(<EmployeeList />);
    await screen.findByText("Raksha Achary");

    await user.click(screen.getAllByRole("button", { name: /delete/i })[0]);

    await waitFor(() =>
      expect(deleteEmployee).toHaveBeenCalledWith(1)
    );
  });

  it("supports pagination when > pageSize", async () => {
    const bigList: EmployeeView[] = Array.from({ length: 10 }).map((_, i) => ({
      id: i + 1,
      firstName: `Emp${i + 1}`,
      lastName: "Test",
      name: `Emp${i + 1} Test`,
    })) as any;

    (getEmployees as jest.Mock).mockResolvedValueOnce(bigList);

    render(<EmployeeList />);

    await screen.findByText("Emp1 Test");
    expect(screen.getByText(/page 1 of 2/i)).toBeInTheDocument();

    await user.click(screen.getByRole("button", { name: /next/i }));
    expect(screen.getByText(/page 2 of 2/i)).toBeInTheDocument();
  });
});
