import { usersApi } from "./axiosConfig";

const authApi = {
  register: async (userData) => {
    const response = await usersApi.post("/auth/register", userData);
    return response.data;
  },

  login: async (credentials) => {
    const response = await usersApi.post("/auth/login", credentials);
    return response.data;
  },

  confirmEmail: async (token) => {
    const response = await usersApi.get(`/auth/confirm-email?token=${token}`);
    return response.data;
  },

  forgotPassword: async (email) => {
    const response = await usersApi.post("/auth/forgot-password", { email });
    return response.data;
  },

  resetPassword: async (data) => {
    const response = await usersApi.post("/auth/reset-password", data);
    return response.data;
  },

  getCurrentUser: async () => {
    const response = await usersApi.get("/auth/me");
    return response.data;
  },
};

export default authApi;
