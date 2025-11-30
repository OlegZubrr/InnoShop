import { usersApi } from "./axiosConfig";

const usersApiService = {
  getAllUsers: async () => {
    const response = await usersApi.get("/users");
    return response.data;
  },

  getUserById: async (id) => {
    const response = await usersApi.get(`/users/${id}`);
    return response.data;
  },

  createUser: async (userData) => {
    const response = await usersApi.post("/users", userData);
    return response.data;
  },

  updateUser: async (id, userData) => {
    const response = await usersApi.put(`/users/${id}`, userData);
    return response.data;
  },

  deleteUser: async (id) => {
    const response = await usersApi.delete(`/users/${id}`);
    return response.data;
  },

  deactivateUser: async (id) => {
    const response = await usersApi.patch(`/users/${id}/deactivate`);
    return response.data;
  },

  activateUser: async (id) => {
    const response = await usersApi.patch(`/users/${id}/activate`);
    return response.data;
  },

  changePassword: async (id, passwordData) => {
    const response = await usersApi.post(
      `/users/${id}/change-password`,
      passwordData
    );
    return response.data;
  },

  updateUserRole: async (id, role) => {
    const response = await usersApi.patch(`/users/${id}/role`, { role });
    return response.data;
  },
};

export default usersApiService;
