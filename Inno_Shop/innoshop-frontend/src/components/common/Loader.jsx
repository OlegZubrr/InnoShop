import React from "react";
import { Loader2 } from "lucide-react";

const Loader = ({ size = "medium", fullScreen = false, message = "" }) => {
  const sizeClasses = {
    small: "w-4 h-4",
    medium: "w-8 h-8",
    large: "w-12 h-12",
  };

  const loader = (
    <div className="flex flex-col items-center justify-center gap-3">
      <Loader2
        className={`${sizeClasses[size]} animate-spin text-primary-600`}
      />
      {message && <p className="text-gray-600 text-sm">{message}</p>}
    </div>
  );

  if (fullScreen) {
    return (
      <div className="fixed inset-0 bg-white bg-opacity-90 flex items-center justify-center z-50">
        {loader}
      </div>
    );
  }

  return loader;
};

export default Loader;
