import axios from "axios";
import { USERS_API_URL, PRODUCTS_API_URL } from "../utils/constants";
import { getToken, removeToken, removeUser } from "../utils/helpers";

export const usersApi = axios.create({
  baseURL: USERS_API_URL,
  headers: {
    "Content-Type": "application/json",
  },
});

export const productsApi = axios.create({
  baseURL: PRODUCTS_API_URL,
  headers: {
    "Content-Type": "application/json",
  },
});

const requestInterceptor = (config) => {
  const token = getToken();

  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }

  return config;
};

const responseErrorInterceptor = (error) => {
  if (error.response) {
    const { status } = error.response;

    if (status === 401) {
      removeToken();
      removeUser();

      const publicPaths = [
        "/login",
        "/register",
        "/confirm-email",
        "/forgot-password",
        "/reset-password",
      ];
      const currentPath = window.location.pathname;

      if (!publicPaths.includes(currentPath)) {
        window.location.href = "/login";
      }
    }

    if (status === 403) {
      console.error(
        "Access denied: You do not have permission to access this resource"
      );
    }
  }

  return Promise.reject(error);
};

usersApi.interceptors.request.use(requestInterceptor);
usersApi.interceptors.response.use(
  (response) => response,
  responseErrorInterceptor
);

productsApi.interceptors.request.use(requestInterceptor);
productsApi.interceptors.response.use(
  (response) => response,
  responseErrorInterceptor
);

export default { usersApi, productsApi };
