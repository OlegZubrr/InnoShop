import React from "react";
import { AlertCircle } from "lucide-react";

const ErrorMessage = ({ message, onRetry }) => {
  if (!message) return null;

  return (
    <div className="bg-red-50 border border-red-200 rounded-lg p-4 flex items-start gap-3">
      <AlertCircle className="w-5 h-5 text-red-600 flex-shrink-0 mt-0.5" />
      <div className="flex-1">
        <p className="text-red-800">{message}</p>
        {onRetry && (
          <button
            onClick={onRetry}
            className="mt-2 text-sm text-red-600 hover:text-red-800 font-medium"
          >
            Try again
          </button>
        )}
      </div>
    </div>
  );
};

export default ErrorMessage;
