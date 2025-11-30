import { jwtDecode } from "jwt-decode";
import { TOKEN_KEY, USER_KEY, USER_ROLE_NAMES } from "./constants";

export const saveToken = (token) => {
  localStorage.setItem(TOKEN_KEY, token);
};

export const getToken = () => {
  return localStorage.getItem(TOKEN_KEY);
};

export const removeToken = () => {
  localStorage.removeItem(TOKEN_KEY);
};

export const isTokenExpired = (token) => {
  if (!token) return true;

  try {
    const decoded = jwtDecode(token);
    const currentTime = Date.now() / 1000;
    return decoded.exp < currentTime;
  } catch (error) {
    return true;
  }
};

export const getTokenData = (token) => {
  if (!token) return null;

  try {
    const decoded = jwtDecode(token);
    console.log("Decoded token data:", decoded);
    return decoded;
  } catch (error) {
    console.error("Token decode error:", error);
    return null;
  }
};

export const saveUser = (user) => {
  localStorage.setItem(USER_KEY, JSON.stringify(user));
};

export const getUser = () => {
  const user = localStorage.getItem(USER_KEY);
  return user ? JSON.parse(user) : null;
};

export const removeUser = () => {
  localStorage.removeItem(USER_KEY);
};

export const formatPrice = (price) => {
  return new Intl.NumberFormat("en-US", {
    style: "currency",
    currency: "USD",
  }).format(price);
};

export const formatDate = (date) => {
  return new Intl.DateTimeFormat("en-US", {
    year: "numeric",
    month: "long",
    day: "numeric",
    hour: "2-digit",
    minute: "2-digit",
  }).format(new Date(date));
};

export const formatShortDate = (date) => {
  return new Intl.DateTimeFormat("en-US", {
    year: "numeric",
    month: "short",
    day: "numeric",
  }).format(new Date(date));
};

export const truncateText = (text, maxLength = 100) => {
  if (text.length <= maxLength) return text;
  return text.substring(0, maxLength) + "...";
};

export const capitalizeFirst = (str) => {
  return str.charAt(0).toUpperCase() + str.slice(1);
};

export const getQueryParams = (search) => {
  return new URLSearchParams(search);
};

export const buildQueryString = (params) => {
  const query = new URLSearchParams();

  Object.keys(params).forEach((key) => {
    if (
      params[key] !== null &&
      params[key] !== undefined &&
      params[key] !== ""
    ) {
      query.append(key, params[key]);
    }
  });

  return query.toString();
};

export const getErrorMessage = (error) => {
  if (error.response) {
    return (
      error.response.data?.message ||
      error.response.data?.title ||
      "An error occurred"
    );
  }

  if (error.request) {
    return "Unable to connect to the server";
  }

  return error.message || "An unknown error occurred";
};

export const isValidEmail = (email) => {
  const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
  return emailRegex.test(email);
};

export const isValidPassword = (password) => {
  const passwordRegex = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[a-zA-Z\d@$!%*?&]{8,}$/;
  return passwordRegex.test(password);
};

export const getRoleName = (roleNumber) => {
  return USER_ROLE_NAMES[roleNumber] || "Unknown";
};

export const isAdminRole = (roleNumber) => {
  return roleNumber === 1;
};

export const isUserRole = (roleNumber) => {
  return roleNumber === 0;
};
