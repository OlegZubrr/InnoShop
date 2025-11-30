import React, { useState, useEffect } from "react";
import { Package } from "lucide-react";
import productsApiService from "../api/productsApi";
import ProductCard from "../components/products/ProductCard";
import ProductFilters from "../components/products/ProductFilters";
import Modal from "../components/common/Modal";
import Loader from "../components/common/Loader";
import ErrorMessage from "../components/common/ErrorMessage";
import { formatPrice, formatDate, getErrorMessage } from "../utils/helpers";
import { DEFAULT_PAGE_SIZE } from "../utils/constants";

const HomePage = () => {
  const [products, setProducts] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [selectedProduct, setSelectedProduct] = useState(null);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [pagination, setPagination] = useState({
    totalCount: 0,
    pageNumber: 1,
    pageSize: DEFAULT_PAGE_SIZE,
    totalPages: 0,
  });

  const fetchProducts = async (filters = {}) => {
    setLoading(true);
    setError("");

    try {
      const params = {
        ...filters,
        pageNumber: filters.pageNumber || 1,
        pageSize: filters.pageSize || DEFAULT_PAGE_SIZE,
      };

      const response = await productsApiService.searchProducts(params);

      setProducts(response.products || []);
      setPagination({
        totalCount: response.totalCount || 0,
        pageNumber: response.pageNumber || 1,
        pageSize: response.pageSize || DEFAULT_PAGE_SIZE,
        totalPages: response.totalPages || 0,
      });
    } catch (err) {
      setError(getErrorMessage(err));
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchProducts();
  }, []);

  const handleFilter = (filters) => {
    fetchProducts(filters);
  };

  const handleReset = () => {
    fetchProducts();
  };

  const handleProductClick = (product) => {
    setSelectedProduct(product);
    setIsModalOpen(true);
  };

  const handleCloseModal = () => {
    setIsModalOpen(false);
    setSelectedProduct(null);
  };

  if (loading) {
    return (
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-12">
        <Loader fullScreen message="Loading products..." />
      </div>
    );
  }

  return (
    <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      <div className="mb-8">
        <h1 className="text-3xl font-bold text-gray-900 mb-2">
          Product Catalog
        </h1>
        <p className="text-gray-600">
          Total products found: {pagination.totalCount}
        </p>
      </div>

      <ProductFilters onFilter={handleFilter} onReset={handleReset} />

      {error && (
        <ErrorMessage message={error} onRetry={() => fetchProducts()} />
      )}

      {products.length === 0 && !loading && !error ? (
        <div className="text-center py-12">
          <Package className="w-16 h-16 text-gray-400 mx-auto mb-4" />
          <h3 className="text-xl font-semibold text-gray-900 mb-2">
            No products found
          </h3>
          <p className="text-gray-600">Try changing your search filters</p>
        </div>
      ) : (
        <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6">
          {products.map((product) => (
            <ProductCard
              key={product.id}
              product={product}
              onClick={() => handleProductClick(product)}
            />
          ))}
        </div>
      )}

      <Modal
        isOpen={isModalOpen}
        onClose={handleCloseModal}
        title={selectedProduct?.name}
        size="medium"
      >
        {selectedProduct && (
          <div className="space-y-4">
            <div className="h-64 bg-gradient-to-br from-primary-400 to-primary-600 rounded-lg flex items-center justify-center">
              <Package className="w-24 h-24 text-white opacity-50" />
            </div>

            <div>
              <h3 className="font-semibold text-gray-900 mb-2">Description</h3>
              <p className="text-gray-600">{selectedProduct.description}</p>
            </div>

            <div className="grid grid-cols-2 gap-4 py-4 border-t border-b">
              <div>
                <p className="text-sm text-gray-500 mb-1">Price</p>
                <p className="text-2xl font-bold text-primary-600">
                  {formatPrice(selectedProduct.price)}
                </p>
              </div>
              <div>
                <p className="text-sm text-gray-500 mb-1">Availability</p>
                <span
                  className={`inline-block px-3 py-1 text-sm font-medium rounded-full ${
                    selectedProduct.isAvailable
                      ? "bg-green-100 text-green-700"
                      : "bg-red-100 text-red-700"
                  }`}
                >
                  {selectedProduct.isAvailable ? "In Stock" : "Out of Stock"}
                </span>
              </div>
            </div>

            <div className="text-sm text-gray-500">
              <p>Created on: {formatDate(selectedProduct.createdAt)}</p>
            </div>
          </div>
        )}
      </Modal>
    </div>
  );
};

export default HomePage;
