import { productsApi } from "./axiosConfig";
import { buildQueryString } from "../utils/helpers";

const productsApiService = {
  getAllProducts: async () => {
    const response = await productsApi.get("/products");
    return response.data;
  },

  getProductById: async (id) => {
    const response = await productsApi.get(`/products/${id}`);
    return response.data;
  },

  getProductsByUserId: async (userId) => {
    const response = await productsApi.get(`/products/user/${userId}`);
    return response.data;
  },

  searchProducts: async (filters) => {
    const queryString = buildQueryString(filters);
    const response = await productsApi.get(`/products/search?${queryString}`);
    return response.data;
  },

  createProduct: async (productData) => {
    const response = await productsApi.post("/products", productData);
    return response.data;
  },

  updateProduct: async (id, productData) => {
    const response = await productsApi.put(`/products/${id}`, productData);
    return response.data;
  },

  deleteProduct: async (id) => {
    const response = await productsApi.delete(`/products/${id}`);
    return response.data;
  },
};

export default productsApiService;
