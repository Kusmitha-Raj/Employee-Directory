import { render, screen } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { MemoryRouter } from "react-router-dom";
import { AuthProvider } from "../context/AuthContext";
import Login from "./Login";

function renderWithProviders(ui: React.ReactElement) {
  return render(
    <MemoryRouter>
      <AuthProvider>{ui}</AuthProvider>
    </MemoryRouter>
  );
}

describe("Login page", () => {
  it("renders login form with inputs and button", () => {
    renderWithProviders(<Login />);

    // From your DOM snapshot:
    // <div class="login-header">Login Form</div>
    expect(screen.getByText(/login form/i)).toBeInTheDocument();

    expect(
      screen.getByPlaceholderText(/email address/i)
    ).toBeInTheDocument();
    expect(screen.getByPlaceholderText(/password/i)).toBeInTheDocument();

    // Initial login button (may be "Login" or "Sign in")
    // Adjust if your initial text is different
    const button = screen.getByRole("button", { name: /login|sign in|signing in/i });
    expect(button).toBeInTheDocument();
  });

  it("disables button while submitting", async () => {
    const user = userEvent.setup();
    renderWithProviders(<Login />);

    const emailInput = screen.getByPlaceholderText(/email address/i);
    const passwordInput = screen.getByPlaceholderText(/password/i);

    await user.type(emailInput, "x@test.com");
    await user.type(passwordInput, "wrong");

    const button = screen.getByRole("button", {
      name: /login|sign in|signing in/i,
    });

    await user.click(button);

    // We just assert that it's disabled after click
    // (matching your DOM where it shows "Signing in..." and disabled)
    expect(button).toBeDisabled();
  });
});
