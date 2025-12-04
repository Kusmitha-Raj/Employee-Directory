import { render, screen } from "@testing-library/react";
import App from "./App";
import { AuthProvider } from "./context/AuthContext";
import { MemoryRouter } from "react-router-dom";

test("renders the app root content", () => {
  render(
    <MemoryRouter>
      <AuthProvider>
        <App />
      </AuthProvider>
    </MemoryRouter>
  );

  // 🔁 Change this text to something that actually appears in your UI.
  // For example "Login", "Employees", your app name, etc.
  const element = screen.getByText(/employee directory/i);

  expect(element).toBeInTheDocument();
});
