import React, { createContext, useState, useEffect, useCallback } from "react";
import authApi from "../api/authApi";
import {
  saveToken,
  getToken,
  removeToken,
  saveUser,
  getUser,
  removeUser,
  isTokenExpired,
  getTokenData,
} from "../utils/helpers";
import { USER_ROLES } from "../utils/constants";

export const AuthContext = createContext(null);

export const AuthProvider = ({ children }) => {
  const [user, setUser] = useState(null);
  const [loading, setLoading] = useState(true);
  const [isAuthenticated, setIsAuthenticated] = useState(false);

  useEffect(() => {
    const initAuth = async () => {
      const token = getToken();
      const savedUser = getUser();

      if (token && !isTokenExpired(token) && savedUser) {
        setUser(savedUser);
        setIsAuthenticated(true);
      } else {
        logout();
      }

      setLoading(false);
    };

    initAuth();
  }, []);

  const login = async (credentials) => {
    try {
      const response = await authApi.login(credentials);
      const { token, user: userData } = response;

      saveToken(token);
      saveUser(userData);
      setUser(userData);
      setIsAuthenticated(true);

      return { success: true, user: userData };
    } catch (error) {
      return {
        success: false,
        error: error.response?.data?.message || "Login error",
      };
    }
  };

  const register = async (userData) => {
    try {
      const response = await authApi.register(userData);
      return { success: true, data: response };
    } catch (error) {
      return {
        success: false,
        error: error.response?.data?.message || "Registration error",
      };
    }
  };

  const logout = useCallback(() => {
    removeToken();
    removeUser();
    setUser(null);
    setIsAuthenticated(false);
  }, []);

  const updateUser = useCallback((updatedUser) => {
    setUser(updatedUser);
    saveUser(updatedUser);
  }, []);

  const hasRole = useCallback(
    (role) => {
      return user?.role === role;
    },
    [user]
  );

  const isAdmin = useCallback(() => {
    return hasRole(USER_ROLES.ADMIN);
  }, [hasRole]);

  const value = {
    user,
    loading,
    isAuthenticated,
    login,
    register,
    logout,
    updateUser,
    hasRole,
    isAdmin,
  };

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
};
