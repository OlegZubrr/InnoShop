import React from "react";
import { ShoppingBag } from "lucide-react";
import RegisterForm from "../components/auth/RegisterForm";

const RegisterPage = () => {
  return (
    <div className="min-h-screen flex items-center justify-center bg-gradient-to-br from-primary-50 to-primary-100 py-12 px-4 sm:px-6 lg:px-8">
      <div className="max-w-md w-full">
        <div className="text-center mb-8">
          <div className="flex justify-center mb-4">
            <div className="bg-primary-600 p-3 rounded-2xl">
              <ShoppingBag className="w-10 h-10 text-white" />
            </div>
          </div>
          <h2 className="text-3xl font-bold text-gray-900 mb-2">
            Create an Account
          </h2>
          <p className="text-gray-600">Join InnoShop today</p>
        </div>

        <div className="bg-white rounded-2xl shadow-xl p-8">
          <RegisterForm />
        </div>
      </div>
    </div>
  );
};

export default RegisterPage;
