import React from "react";
import {
  User,
  Mail,
  Calendar,
  Shield,
  CheckCircle,
  XCircle,
} from "lucide-react";
import { formatDate } from "../../utils/helpers";

const UserCard = ({ user, onActivate, onDeactivate, onDelete }) => {
  return (
    <div className="bg-white rounded-lg shadow-md hover:shadow-xl transition-all duration-300 overflow-hidden">
      <div className="bg-gradient-to-r from-primary-500 to-primary-600 p-6">
        <div className="flex items-center justify-center mb-2">
          <div className="w-20 h-20 bg-white rounded-full flex items-center justify-center">
            <span className="text-3xl font-bold text-primary-600">
              {user.name.charAt(0).toUpperCase()}
            </span>
          </div>
        </div>
        <h3 className="text-xl font-bold text-white text-center">
          {user.name}
        </h3>
      </div>

      <div className="p-6 space-y-4">
        <div className="flex items-start gap-3">
          <Mail className="w-5 h-5 text-gray-400 flex-shrink-0 mt-0.5" />
          <div className="flex-1 min-w-0">
            <p className="text-sm text-gray-500">Email</p>
            <p className="text-sm font-medium text-gray-900 truncate">
              {user.email}
            </p>
            {user.emailConfirmed && (
              <span className="inline-flex items-center gap-1 text-xs text-green-600 mt-1">
                <CheckCircle className="w-3 h-3" />
                Verified
              </span>
            )}
          </div>
        </div>

        <div className="flex items-center gap-3">
          <Shield className="w-5 h-5 text-gray-400 flex-shrink-0" />
          <div>
            <p className="text-sm text-gray-500">Role</p>
            <span
              className={`inline-flex items-center gap-1 px-2.5 py-1 rounded-full text-xs font-medium mt-1 ${
                user.role === "Admin"
                  ? "bg-purple-100 text-purple-800"
                  : "bg-blue-100 text-blue-800"
              }`}
            >
              {user.role === "Admin" && <Shield className="w-3 h-3" />}
              {user.role}
            </span>
          </div>
        </div>

        <div className="flex items-center gap-3">
          {user.isActive ? (
            <CheckCircle className="w-5 h-5 text-green-500 flex-shrink-0" />
          ) : (
            <XCircle className="w-5 h-5 text-red-500 flex-shrink-0" />
          )}
          <div>
            <p className="text-sm text-gray-500">Status</p>
            <span
              className={`inline-block px-2.5 py-1 rounded-full text-xs font-medium mt-1 ${
                user.isActive
                  ? "bg-green-100 text-green-800"
                  : "bg-red-100 text-red-800"
              }`}
            >
              {user.isActive ? "Active" : "Inactive"}
            </span>
          </div>
        </div>

        <div className="flex items-center gap-3 pt-3 border-t">
          <Calendar className="w-5 h-5 text-gray-400 flex-shrink-0" />
          <div>
            <p className="text-sm text-gray-500">Registered</p>
            <p className="text-sm font-medium text-gray-900">
              {formatDate(user.createdAt)}
            </p>
          </div>
        </div>

        <div className="flex gap-2 pt-4 border-t">
          {user.isActive ? (
            <button
              onClick={() => onDeactivate(user.id)}
              className="flex-1 px-4 py-2 bg-orange-100 text-orange-700 rounded-lg hover:bg-orange-200 transition-colors text-sm font-medium"
            >
              Deactivate
            </button>
          ) : (
            <button
              onClick={() => onActivate(user.id)}
              className="flex-1 px-4 py-2 bg-green-100 text-green-700 rounded-lg hover:bg-green-200 transition-colors text-sm font-medium"
            >
              Activate
            </button>
          )}

          <button
            onClick={() => onDelete(user.id)}
            className="flex-1 px-4 py-2 bg-red-100 text-red-700 rounded-lg hover:bg-red-200 transition-colors text-sm font-medium"
          >
            Delete
          </button>
        </div>
      </div>
    </div>
  );
};

export default UserCard;
