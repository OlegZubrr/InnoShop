import React, { useState, useEffect } from "react";
import { Plus, Edit, Trash2, Package } from "lucide-react";
import { useAuth } from "../hooks/useAuth";
import productsApiService from "../api/productsApi";
import ProductCard from "../components/products/ProductCard";
import ProductForm from "../components/products/ProductForm";
import Modal from "../components/common/Modal";
import Button from "../components/common/Button";
import Loader from "../components/common/Loader";
import ErrorMessage from "../components/common/ErrorMessage";
import { getErrorMessage, formatPrice, formatDate } from "../utils/helpers";
import toast from "react-hot-toast";

const MyProductsPage = () => {
  const { user } = useAuth();
  const [products, setProducts] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [isCreateModalOpen, setIsCreateModalOpen] = useState(false);
  const [isEditModalOpen, setIsEditModalOpen] = useState(false);
  const [isDetailModalOpen, setIsDetailModalOpen] = useState(false);
  const [selectedProduct, setSelectedProduct] = useState(null);
  const [isSubmitting, setIsSubmitting] = useState(false);

  const fetchMyProducts = async () => {
    if (!user?.id) return;

    setLoading(true);
    setError("");

    try {
      const data = await productsApiService.getProductsByUserId(user.id);
      setProducts(data || []);
    } catch (err) {
      setError(getErrorMessage(err));
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchMyProducts();
  }, [user]);

  const handleCreate = async (productData) => {
    setIsSubmitting(true);

    try {
      await productsApiService.createProduct(productData);
      toast.success("Product created successfully!");
      setIsCreateModalOpen(false);
      fetchMyProducts();
    } catch (err) {
      toast.error(getErrorMessage(err));
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleUpdate = async (productData) => {
    if (!selectedProduct) return;

    setIsSubmitting(true);

    try {
      await productsApiService.updateProduct(selectedProduct.id, productData);
      toast.success("Product updated successfully!");
      setIsEditModalOpen(false);
      setSelectedProduct(null);
      fetchMyProducts();
    } catch (err) {
      toast.error(getErrorMessage(err));
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleDelete = async (productId) => {
    if (!window.confirm("Are you sure you want to delete this product?")) {
      return;
    }

    try {
      await productsApiService.deleteProduct(productId);
      toast.success("Product deleted successfully!");
      fetchMyProducts();
    } catch (err) {
      toast.error(getErrorMessage(err));
    }
  };

  const handleProductClick = (product) => {
    setSelectedProduct(product);
    setIsDetailModalOpen(true);
  };

  const handleEdit = (product, e) => {
    e.stopPropagation();
    setSelectedProduct(product);
    setIsEditModalOpen(true);
  };

  const handleDeleteClick = (product, e) => {
    e.stopPropagation();
    handleDelete(product.id);
  };

  if (loading) {
    return <Loader fullScreen message="Loading your products..." />;
  }

  return (
    <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      <div className="flex justify-between items-center mb-8">
        <div>
          <h1 className="text-3xl font-bold text-gray-900 mb-2">My Products</h1>
          <p className="text-gray-600">Manage your products</p>
        </div>
        <Button
          onClick={() => setIsCreateModalOpen(true)}
          className="flex items-center gap-2"
        >
          <Plus className="w-5 h-5" />
          Add Product
        </Button>
      </div>

      {error && <ErrorMessage message={error} onRetry={fetchMyProducts} />}

      {products.length === 0 && !loading && !error ? (
        <div className="text-center py-12 bg-white rounded-lg shadow-md">
          <Package className="w-16 h-16 text-gray-400 mx-auto mb-4" />
          <h3 className="text-xl font-semibold text-gray-900 mb-2">
            You have no products yet
          </h3>
          <p className="text-gray-600 mb-6">Create your first product now!</p>
          <Button onClick={() => setIsCreateModalOpen(true)}>
            Add Product
          </Button>
        </div>
      ) : (
        <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6">
          {products.map((product) => (
            <div key={product.id} className="relative group">
              <ProductCard
                product={product}
                onClick={() => handleProductClick(product)}
              />
              <div className="absolute top-2 right-2 flex gap-2 opacity-0 group-hover:opacity-100 transition-opacity">
                <button
                  onClick={(e) => handleEdit(product, e)}
                  className="p-2 bg-white rounded-full shadow-lg hover:bg-blue-50 transition-colors"
                >
                  <Edit className="w-4 h-4 text-blue-600" />
                </button>
                <button
                  onClick={(e) => handleDeleteClick(product, e)}
                  className="p-2 bg-white rounded-full shadow-lg hover:bg-red-50 transition-colors"
                >
                  <Trash2 className="w-4 h-4 text-red-600" />
                </button>
              </div>
            </div>
          ))}
        </div>
      )}

      <Modal
        isOpen={isCreateModalOpen}
        onClose={() => setIsCreateModalOpen(false)}
        title="Create Product"
        size="medium"
      >
        <ProductForm
          onSubmit={handleCreate}
          onCancel={() => setIsCreateModalOpen(false)}
          isLoading={isSubmitting}
        />
      </Modal>

      <Modal
        isOpen={isEditModalOpen}
        onClose={() => {
          setIsEditModalOpen(false);
          setSelectedProduct(null);
        }}
        title="Edit Product"
        size="medium"
      >
        <ProductForm
          product={selectedProduct}
          onSubmit={handleUpdate}
          onCancel={() => {
            setIsEditModalOpen(false);
            setSelectedProduct(null);
          }}
          isLoading={isSubmitting}
        />
      </Modal>

      <Modal
        isOpen={isDetailModalOpen}
        onClose={() => {
          setIsDetailModalOpen(false);
          setSelectedProduct(null);
        }}
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
                  {selectedProduct.isAvailable ? "Available" : "Out of stock"}
                </span>
              </div>
            </div>

            <div className="text-sm text-gray-500">
              <p>Created: {formatDate(selectedProduct.createdAt)}</p>
            </div>

            <div className="flex gap-3 pt-4">
              <Button
                onClick={() => {
                  setIsDetailModalOpen(false);
                  setIsEditModalOpen(true);
                }}
                variant="outline"
                className="flex-1"
              >
                <Edit className="w-4 h-4 mr-2" />
                Edit
              </Button>
              <Button
                onClick={() => {
                  setIsDetailModalOpen(false);
                  handleDelete(selectedProduct.id);
                }}
                variant="danger"
                className="flex-1"
              >
                <Trash2 className="w-4 h-4 mr-2" />
                Delete
              </Button>
            </div>
          </div>
        )}
      </Modal>
    </div>
  );
};

export default MyProductsPage;
