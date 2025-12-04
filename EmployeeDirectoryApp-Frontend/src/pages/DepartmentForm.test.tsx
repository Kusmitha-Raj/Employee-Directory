import { screen } from "@testing-library/react";
import { renderWithProviders } from "../test-utils";
import DepartmentForm from "./Departments/DepartmentForm";

describe("DepartmentForm", () => {
  it("renders create department form", () => {
    const onSaved = jest.fn();

    renderWithProviders(<DepartmentForm onSaved={onSaved} />);

    expect(screen.getByText(/create department/i)).toBeInTheDocument();
    expect(screen.getByText(/name/i)).toBeInTheDocument();
    expect(
      screen.getByRole("button", { name: /save/i })
    ).toBeInTheDocument();
  });

  it("shows initial department name when provided", () => {
    const onSaved = jest.fn();

    renderWithProviders(
      <DepartmentForm
        onSaved={onSaved}
        initial={{ name: "Existing Dept" }}
      />
    );

    // input value is "Existing Dept"
    expect(screen.getByDisplayValue("Existing Dept")).toBeInTheDocument();
  });
});
