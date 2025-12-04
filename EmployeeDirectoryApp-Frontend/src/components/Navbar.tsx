import { Link, useNavigate } from "react-router-dom";
import { useAuth } from "../context/AuthContext";

export default function Navbar() {
  const { user, logout } = useAuth();
  const navigate = useNavigate();
  
  const handleLogout = () => {
    logout();
    navigate("/login", { replace: true });
  };

  return (
    <header className="nav">
      <div className="nav-left">
        <Link to="/" className="brand"><strong>Employee Directory</strong></Link>
      </div>
  <nav className="links nav-center clean-nav">
  {user && (
    <>
      <Link to="/" className="nav-link">Home</Link>
      <Link to="/employees" className="nav-link">Employees</Link>
      <Link to="/departments" className="nav-link">Departments</Link>
    </>
  )}
</nav>
      <div className="nav-right">
        {user ? (
          <>
            <span className="greet">
              {user.role}
            </span>

            {/* ✅ Show Change Password only when user is logged in */}
           
            <button className="btn small" onClick={handleLogout}>Logout</button>
          </>
        ) : (
          <>
            <Link to="/login" className="btn small">Login</Link>

            {/* If you want Change Password visible even before login, uncomment below */}
            {/* <ChangePassword /> */}
          </>
        )}
      </div>
    </header>
  );
}
