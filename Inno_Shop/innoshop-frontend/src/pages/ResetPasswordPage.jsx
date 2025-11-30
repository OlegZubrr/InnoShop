import React, { useState } from "react";
import { useSearchParams, useNavigate, Link } from "react-router-dom";
import { CheckCircle } from "lucide-react";
import authApi from "../api/authApi";
import Input from "../components/common/Input";
import Button from "../components/common/Button";
import ErrorMessage from "../components/common/ErrorMessage";
import { ROUTES } from "../utils/constants";
import { getErrorMessage } from "../utils/helpers";
import { validateResetPassword } from "../utils/validators";
import toast from "react-hot-toast";

const ResetPasswordPage = () => {
  const [searchParams] = useSearchParams();
  const navigate = useNavigate();
  const token = searchParams.get("token");

  const [formData, setFormData] = useState({
    password: "",
    confirmPassword: "",
  });

  const [errors, setErrors] = useState({});
  const [apiError, setApiError] = useState("");
  const [isLoading, setIsLoading] = useState(false);
  const [isSuccess, setIsSuccess] = useState(false);

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData((prev) => ({ ...prev, [name]: value }));

    if (errors[name]) {
      setErrors((prev) => ({ ...prev, [name]: "" }));
    }
    if (apiError) {
      setApiError("");
    }
  };

  const handleSubmit = async (e) => {
    e.preventDefault();

    if (!token) {
      setApiError("Password reset token is missing");
      return;
    }

    const validationErrors = validateResetPassword(formData);
    if (Object.keys(validationErrors).length > 0) {
      setErrors(validationErrors);
      return;
    }

    setIsLoading(true);
    setApiError("");

    try {
      await authApi.resetPassword({
        token,
        newPassword: formData.password,
      });
      setIsSuccess(true);
      toast.success("Password successfully changed!");
      setTimeout(() => {
        navigate(ROUTES.LOGIN);
      }, 2000);
    } catch (err) {
      setApiError(getErrorMessage(err));
      toast.error(getErrorMessage(err));
    } finally {
      setIsLoading(false);
    }
  };

  if (!token) {
    return (
      <div className="min-h-screen flex items-center justify-center bg-gradient-to-br from-primary-50 to-primary-100 py-12 px-4">
        <div className="max-w-md w-full bg-white rounded-2xl shadow-xl p-8 text-center">
          <h2 className="text-2xl font-bold text-gray-900 mb-2">
            Invalid Link
          </h2>
          <p className="text-gray-600 mb-6">
            The password reset link is invalid or expired
          </p>
          <Link to={ROUTES.FORGOT_PASSWORD}>
            <Button fullWidth>Request a New Link</Button>
          </Link>
        </div>
      </div>
    );
  }

  if (isSuccess) {
    return (
      <div className="min-h-screen flex items-center justify-center bg-gradient-to-br from-primary-50 to-primary-100 py-12 px-4">
        <div className="max-w-md w-full bg-white rounded-2xl shadow-xl p-8 text-center">
          <CheckCircle className="w-16 h-16 text-green-500 mx-auto mb-4" />
          <h2 className="text-2xl font-bold text-gray-900 mb-2">
            Password Changed!
          </h2>
          <p className="text-gray-600 mb-6">
            Your password has been successfully changed. You will be redirected
            to the login page...
          </p>
          <Link to={ROUTES.LOGIN}>
            <Button fullWidth>Login Now</Button>
          </Link>
        </div>
      </div>
    );
  }

  return (
    <div className="min-h-screen flex items-center justify-center bg-gradient-to-br from-primary-50 to-primary-100 py-12 px-4">
      <div className="max-w-md w-full">
        <div className="bg-white rounded-2xl shadow-xl p-8">
          <div className="text-center mb-6">
            <h2 className="text-2xl font-bold text-gray-900 mb-2">
              Create a New Password
            </h2>
            <p className="text-gray-600">
              Enter a new password for your account
            </p>
          </div>

          <form onSubmit={handleSubmit} className="space-y-4">
            {apiError && <ErrorMessage message={apiError} />}

            <Input
              label="New Password"
              name="password"
              type="password"
              value={formData.password}
              onChange={handleChange}
              placeholder="••••••••"
              error={errors.password}
              required
              disabled={isLoading}
            />

            <Input
              label="Confirm Password"
              name="confirmPassword"
              type="password"
              value={formData.confirmPassword}
              onChange={handleChange}
              placeholder="••••••••"
              error={errors.confirmPassword}
              required
              disabled={isLoading}
            />

            <div className="text-xs text-gray-500">
              Password must be at least 8 characters long, including uppercase
              and lowercase letters, and numbers.
            </div>

            <Button type="submit" fullWidth disabled={isLoading}>
              {isLoading ? "Saving..." : "Change Password"}
            </Button>
          </form>
        </div>
      </div>
    </div>
  );
};

export default ResetPasswordPage;
