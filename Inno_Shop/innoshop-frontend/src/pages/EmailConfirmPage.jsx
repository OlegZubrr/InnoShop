import React, { useEffect, useState } from "react";
import { useSearchParams, Link } from "react-router-dom";
import { CheckCircle, XCircle, Loader2 } from "lucide-react";
import authApi from "../api/authApi";
import Button from "../components/common/Button";
import { ROUTES } from "../utils/constants";
import { getErrorMessage } from "../utils/helpers";

const EmailConfirmPage = () => {
  const [searchParams] = useSearchParams();
  const [status, setStatus] = useState("loading");
  const [message, setMessage] = useState("");

  useEffect(() => {
    const confirmEmail = async () => {
      const token = searchParams.get("token");

      if (!token) {
        setStatus("error");
        setMessage("Confirmation token is missing");
        return;
      }

      try {
        await authApi.confirmEmail(token);
        setStatus("success");
        setMessage("Email successfully confirmed! You can now log in.");
      } catch (error) {
        setStatus("error");
        setMessage(getErrorMessage(error));
      }
    };

    confirmEmail();
  }, [searchParams]);

  return (
    <div className="min-h-screen flex items-center justify-center bg-gradient-to-br from-primary-50 to-primary-100 py-12 px-4">
      <div className="max-w-md w-full bg-white rounded-2xl shadow-xl p-8 text-center">
        {status === "loading" && (
          <>
            <Loader2 className="w-16 h-16 text-primary-600 animate-spin mx-auto mb-4" />
            <h2 className="text-2xl font-bold text-gray-900 mb-2">
              Confirming Email...
            </h2>
            <p className="text-gray-600">Please wait</p>
          </>
        )}

        {status === "success" && (
          <>
            <CheckCircle className="w-16 h-16 text-green-500 mx-auto mb-4" />
            <h2 className="text-2xl font-bold text-gray-900 mb-2">
              Email Confirmed!
            </h2>
            <p className="text-gray-600 mb-6">{message}</p>
            <Link to={ROUTES.LOGIN}>
              <Button fullWidth>Log In</Button>
            </Link>
          </>
        )}

        {status === "error" && (
          <>
            <XCircle className="w-16 h-16 text-red-500 mx-auto mb-4" />
            <h2 className="text-2xl font-bold text-gray-900 mb-2">
              Confirmation Error
            </h2>
            <p className="text-gray-600 mb-6">{message}</p>
            <Link to={ROUTES.LOGIN}>
              <Button fullWidth variant="secondary">
                Back to Login
              </Button>
            </Link>
          </>
        )}
      </div>
    </div>
  );
};

export default EmailConfirmPage;
