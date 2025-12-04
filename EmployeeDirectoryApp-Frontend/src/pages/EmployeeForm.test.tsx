import { render, screen } from "@testing-library/react";
import EmployeeForm from "./Employees/EmployeeForm";

describe("EmployeeForm", () => {
  it("renders employee form fields", () => {
    const onSave = jest.fn();

    render(<EmployeeForm onSave={onSave} />);

    expect(screen.getByText(/first name/i)).toBeInTheDocument();
    expect(screen.getByText(/last name/i)).toBeInTheDocument();
    expect(screen.getByText(/email/i)).toBeInTheDocument();
    expect(screen.getByText(/phone/i)).toBeInTheDocument();
    expect(screen.getByText(/job role/i)).toBeInTheDocument();
    expect(screen.getByText(/gender/i)).toBeInTheDocument();
    expect(screen.getByText(/department/i)).toBeInTheDocument();

    expect(
      screen.getByRole("button", { name: /save/i })
    ).toBeInTheDocument();
  });

  it("renders initial values when provided", () => {
    const onSave = jest.fn();

    render(
      <EmployeeForm
        onSave={onSave}
        initial={{
          firstName: "Existing",
          lastName: "Employee",
          email: "existing@test.com",
          // ✅ IMPORTANT: use phone, NOT phoneNo
          phone: "99999",
        }}
      />
    );

    expect(screen.getByDisplayValue("Existing")).toBeInTheDocument();
    expect(screen.getByDisplayValue("Employee")).toBeInTheDocument();
    expect(
      screen.getByDisplayValue("existing@test.com")
    ).toBeInTheDocument();
    expect(screen.getByDisplayValue("99999")).toBeInTheDocument();
  });
});
