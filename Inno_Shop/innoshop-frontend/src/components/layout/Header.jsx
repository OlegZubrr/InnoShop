import React from "react";
import { Link, useNavigate } from "react-router-dom";
import { ShoppingBag, User, LogOut, Users, Package } from "lucide-react";
import { useAuth } from "../../hooks/useAuth";
import Button from "../common/Button";
import { ROUTES, USER_ROLES } from "../../utils/constants";
import toast from "react-hot-toast";

const Header = () => {
  const navigate = useNavigate();
  const { isAuthenticated, user, logout, isAdmin } = useAuth();

  const handleLogout = () => {
    logout();
    toast.success("You have successfully logged out");
    navigate(ROUTES.LOGIN);
  };

  return (
    <header className="bg-white shadow-sm sticky top-0 z-40">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div className="flex justify-between items-center h-16">
          <Link to={ROUTES.HOME} className="flex items-center gap-2">
            <ShoppingBag className="w-8 h-8 text-primary-600" />
            <span className="text-xl font-bold text-gray-900">InnoShop</span>
          </Link>

          <nav className="flex items-center gap-4">
            <Link
              to={ROUTES.HOME}
              className="text-gray-700 hover:text-primary-600 font-medium transition-colors"
            >
              Products
            </Link>

            {isAuthenticated ? (
              <>
                <Link
                  to={ROUTES.MY_PRODUCTS}
                  className="flex items-center gap-2 text-gray-700 hover:text-primary-600 font-medium transition-colors"
                >
                  <Package className="w-4 h-4" />
                  My Products
                </Link>

                {isAdmin() && (
                  <Link
                    to={ROUTES.ADMIN_USERS}
                    className="flex items-center gap-2 text-gray-700 hover:text-primary-600 font-medium transition-colors"
                  >
                    <Users className="w-4 h-4" />
                    Users
                  </Link>
                )}

                <div className="flex items-center gap-3 ml-4 pl-4 border-l">
                  <div className="flex items-center gap-2">
                    <User className="w-4 h-4 text-gray-500" />
                    <span className="text-sm text-gray-700">{user?.name}</span>
                    {isAdmin() && (
                      <span className="px-2 py-0.5 text-xs bg-primary-100 text-primary-700 rounded-full">
                        Admin
                      </span>
                    )}
                  </div>

                  <Button
                    onClick={handleLogout}
                    variant="secondary"
                    className="flex items-center gap-2"
                  >
                    <LogOut className="w-4 h-4" />
                    Logout
                  </Button>
                </div>
              </>
            ) : (
              <div className="flex items-center gap-3 ml-4">
                <Link to={ROUTES.LOGIN}>
                  <Button variant="outline">Login</Button>
                </Link>
                <Link to={ROUTES.REGISTER}>
                  <Button>Register</Button>
                </Link>
              </div>
            )}
          </nav>
        </div>
      </div>
    </header>
  );
};

export default Header;
