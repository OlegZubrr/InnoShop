import React, { useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import { useAuth } from "../../hooks/useAuth";
import Input from "../common/Input";
import Button from "../common/Button";
import ErrorMessage from "../common/ErrorMessage";
import { validateRegisterForm } from "../../utils/validators";
import { ROUTES } from "../../utils/constants";
import toast from "react-hot-toast";

const RegisterForm = () => {
  const navigate = useNavigate();
  const { register } = useAuth();

  const [formData, setFormData] = useState({
    fullName: "",
    email: "",
    password: "",
    confirmPassword: "",
  });

  const [errors, setErrors] = useState({});
  const [isLoading, setIsLoading] = useState(false);
  const [apiError, setApiError] = useState("");

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

    const validationErrors = validateRegisterForm(formData);
    if (Object.keys(validationErrors).length > 0) {
      setErrors(validationErrors);
      return;
    }

    setIsLoading(true);
    setApiError("");

    try {
      const result = await register({
        fullName: formData.fullName,
        email: formData.email,
        password: formData.password,
      });

      if (result.success) {
        toast.success(
          "Registration successful! Please check your email to confirm."
        );
        navigate(ROUTES.LOGIN);
      } else {
        setApiError(result.error);
        toast.error(result.error);
      }
    } catch (error) {
      setApiError("An error occurred during registration");
      toast.error("An error occurred during registration");
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <form onSubmit={handleSubmit} className="space-y-4">
      {apiError && <ErrorMessage message={apiError} />}

      <Input
        label="Full Name"
        name="fullName"
        type="text"
        value={formData.fullName}
        onChange={handleChange}
        placeholder="Your full name"
        error={errors.fullName}
        required
        disabled={isLoading}
      />

      <Input
        label="Email"
        name="email"
        type="email"
        value={formData.email}
        onChange={handleChange}
        placeholder="your@email.com"
        error={errors.email}
        required
        disabled={isLoading}
      />

      <Input
        label="Password"
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
        Password must be at least 8 characters long, including uppercase,
        lowercase letters, and numbers.
      </div>

      <Button type="submit" fullWidth disabled={isLoading}>
        {isLoading ? "Registering..." : "Register"}
      </Button>

      <p className="text-center text-sm text-gray-600">
        Already have an account?{" "}
        <Link
          to={ROUTES.LOGIN}
          className="text-primary-600 hover:text-primary-700 font-medium"
        >
          Login
        </Link>
      </p>
    </form>
  );
};

export default RegisterForm;
