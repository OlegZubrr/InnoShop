export const validateRegisterForm = (formData) => {
  const errors = {};

  if (!formData.name || formData.name.trim().length < 2) {
    errors.name = "Name must contain at least 2 characters";
  }

  if (!formData.email) {
    errors.email = "Email is required";
  } else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(formData.email)) {
    errors.email = "Enter a valid email";
  }

  if (!formData.password) {
    errors.password = "Password is required";
  } else if (formData.password.length < 8) {
    errors.password = "Password must contain at least 8 characters";
  } else if (!/(?=.*[a-z])(?=.*[A-Z])(?=.*\d)/.test(formData.password)) {
    errors.password =
      "Password must contain uppercase, lowercase letters and numbers";
  }

  if (formData.password !== formData.confirmPassword) {
    errors.confirmPassword = "Passwords do not match";
  }

  return errors;
};

export const validateLoginForm = (formData) => {
  const errors = {};

  if (!formData.email) {
    errors.email = "Email is required";
  } else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(formData.email)) {
    errors.email = "Enter a valid email";
  }

  if (!formData.password) {
    errors.password = "Password is required";
  }

  return errors;
};

export const validateProductForm = (formData) => {
  const errors = {};

  if (!formData.name || formData.name.trim().length < 3) {
    errors.name = "Name must contain at least 3 characters";
  }

  if (!formData.description || formData.description.trim().length < 10) {
    errors.description = "Description must contain at least 10 characters";
  }

  if (!formData.price || formData.price <= 0) {
    errors.price = "Price must be greater than 0";
  }

  return errors;
};

export const validatePasswordChange = (formData) => {
  const errors = {};

  if (!formData.currentPassword) {
    errors.currentPassword = "Enter your current password";
  }

  if (!formData.newPassword) {
    errors.newPassword = "Enter a new password";
  } else if (formData.newPassword.length < 8) {
    errors.newPassword = "Password must contain at least 8 characters";
  } else if (!/(?=.*[a-z])(?=.*[A-Z])(?=.*\d)/.test(formData.newPassword)) {
    errors.newPassword =
      "Password must contain uppercase, lowercase letters and numbers";
  }

  if (formData.newPassword !== formData.confirmPassword) {
    errors.confirmPassword = "Passwords do not match";
  }

  return errors;
};

export const validateResetPassword = (formData) => {
  const errors = {};

  if (!formData.password) {
    errors.password = "Password is required";
  } else if (formData.password.length < 8) {
    errors.password = "Password must contain at least 8 characters";
  } else if (!/(?=.*[a-z])(?=.*[A-Z])(?=.*\d)/.test(formData.password)) {
    errors.password =
      "Password must contain uppercase, lowercase letters and numbers";
  }

  if (formData.password !== formData.confirmPassword) {
    errors.confirmPassword = "Passwords do not match";
  }

  return errors;
};
