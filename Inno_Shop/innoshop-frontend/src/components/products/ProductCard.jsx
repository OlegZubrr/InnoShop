import React from "react";
import { Calendar, Tag } from "lucide-react";
import {
  formatPrice,
  formatShortDate,
  truncateText,
} from "../../utils/helpers";

const ProductCard = ({ product, onClick }) => {
  return (
    <div
      onClick={onClick}
      className="bg-white rounded-lg shadow-md hover:shadow-xl transition-all duration-300 cursor-pointer overflow-hidden group"
    >
      <div className="h-48 bg-gradient-to-br from-primary-400 to-primary-600 flex items-center justify-center">
        <Tag className="w-16 h-16 text-white opacity-50" />
      </div>

      <div className="p-4">
        <h3 className="text-lg font-semibold text-gray-900 mb-2 group-hover:text-primary-600 transition-colors">
          {product.name}
        </h3>

        <p className="text-sm text-gray-600 mb-4 line-clamp-2">
          {truncateText(product.description, 80)}
        </p>

        <div className="flex items-center justify-between mb-3">
          <span className="text-2xl font-bold text-primary-600">
            {formatPrice(product.price)}
          </span>
          <span
            className={`px-3 py-1 text-xs font-medium rounded-full ${
              product.isAvailable
                ? "bg-green-100 text-green-700"
                : "bg-red-100 text-red-700"
            }`}
          >
            {product.isAvailable ? "Available" : "Out of stock"}
          </span>
        </div>

        <div className="flex flex-col gap-1 text-xs text-gray-500 pt-3 border-t">
          <div className="flex items-center gap-1">
            <Calendar className="w-3 h-3" />
            <span>{formatShortDate(product.createdAt)}</span>
          </div>
        </div>
      </div>
    </div>
  );
};

export default ProductCard;
