export const USERS_API_URL =
  process.env.REACT_APP_USERS_API_URL || "http://localhost:5000/api";
export const PRODUCTS_API_URL =
  process.env.REACT_APP_PRODUCTS_API_URL || "http://localhost:5001/api";

export const TOKEN_KEY = "innoshop_token";
export const USER_KEY = "innoshop_user";

export const USER_ROLES = {
  USER: 0,
  ADMIN: 1,
};

export const USER_ROLE_NAMES = {
  0: "User",
  1: "Admin",
};

export const PRODUCT_AVAILABILITY = {
  AVAILABLE: true,
  NOT_AVAILABLE: false,
};

export const DEFAULT_PAGE_SIZE = 12;
export const PAGE_SIZE_OPTIONS = [6, 12, 24, 48];

export const ROUTES = {
  HOME: "/",
  LOGIN: "/login",
  REGISTER: "/register",
  EMAIL_CONFIRM: "/confirm-email",
  FORGOT_PASSWORD: "/forgot-password",
  RESET_PASSWORD: "/reset-password",
  PROFILE: "/profile",
  MY_PRODUCTS: "/my-products",
  ADMIN_USERS: "/admin/users",
};

export const HTTP_STATUS = {
  OK: 200,
  CREATED: 201,
  NO_CONTENT: 204,
  BAD_REQUEST: 400,
  UNAUTHORIZED: 401,
  FORBIDDEN: 403,
  NOT_FOUND: 404,
  INTERNAL_SERVER_ERROR: 500,
};

export const TOAST_DURATION = 3000;
