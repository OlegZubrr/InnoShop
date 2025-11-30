import React, { useState } from "react";
import { Link } from "react-router-dom";
import { Mail, ArrowLeft } from "lucide-react";
import authApi from "../api/authApi";
import Input from "../components/common/Input";
import Button from "../components/common/Button";
import ErrorMessage from "../components/common/ErrorMessage";
import { ROUTES } from "../utils/constants";
import { getErrorMessage, isValidEmail } from "../utils/helpers";
import toast from "react-hot-toast";

const ForgotPasswordPage = () => {
  const [email, setEmail] = useState("");
  const [error, setError] = useState("");
  const [isLoading, setIsLoading] = useState(false);
  const [isSuccess, setIsSuccess] = useState(false);

  const handleSubmit = async (e) => {
    e.preventDefault();

    if (!email) {
      setError("Email is required");
      return;
    }

    if (!isValidEmail(email)) {
      setError("Please enter a valid email");
      return;
    }

    setIsLoading(true);
    setError("");

    try {
      await authApi.forgotPassword(email);
      setIsSuccess(true);
      toast.success("Password reset email sent!");
    } catch (err) {
      setError(getErrorMessage(err));
      toast.error(getErrorMessage(err));
    } finally {
      setIsLoading(false);
    }
  };

  if (isSuccess) {
    return (
      <div className="min-h-screen flex items-center justify-center bg-gradient-to-br from-primary-50 to-primary-100 py-12 px-4">
        <div className="max-w-md w-full bg-white rounded-2xl shadow-xl p-8 text-center">
          <div className="bg-green-100 rounded-full p-4 w-16 h-16 mx-auto mb-4 flex items-center justify-center">
            <Mail className="w-8 h-8 text-green-600" />
          </div>
          <h2 className="text-2xl font-bold text-gray-900 mb-2">
            Check your email
          </h2>
          <p className="text-gray-600 mb-6">
            We have sent password reset instructions to <strong>{email}</strong>
          </p>
          <Link to={ROUTES.LOGIN}>
            <Button fullWidth variant="secondary">
              Back to Login
            </Button>
          </Link>
        </div>
      </div>
    );
  }

  return (
    <div className="min-h-screen flex items-center justify-center bg-gradient-to-br from-primary-50 to-primary-100 py-12 px-4">
      <div className="max-w-md w-full">
        <Link
          to={ROUTES.LOGIN}
          className="inline-flex items-center gap-2 text-primary-600 hover:text-primary-700 mb-6"
        >
          <ArrowLeft className="w-4 h-4" />
          Back to Login
        </Link>

        <div className="bg-white rounded-2xl shadow-xl p-8">
          <div className="text-center mb-6">
            <h2 className="text-2xl font-bold text-gray-900 mb-2">
              Forgot Password?
            </h2>
            <p className="text-gray-600">
              Enter your email, and we will send reset instructions
            </p>
          </div>

          <form onSubmit={handleSubmit} className="space-y-4">
            {error && <ErrorMessage message={error} />}

            <Input
              label="Email"
              name="email"
              type="email"
              value={email}
              onChange={(e) => {
                setEmail(e.target.value);
                setError("");
              }}
              placeholder="your@email.com"
              required
              disabled={isLoading}
            />

            <Button type="submit" fullWidth disabled={isLoading}>
              {isLoading ? "Sending..." : "Send Instructions"}
            </Button>
          </form>
        </div>
      </div>
    </div>
  );
};

export default ForgotPasswordPage;
