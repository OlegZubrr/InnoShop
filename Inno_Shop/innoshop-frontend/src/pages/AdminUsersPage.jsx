import React, { useState, useEffect } from "react";
import { Users, UserCheck, UserX, Trash2, Shield } from "lucide-react";
import usersApiService from "../api/usersApi";
import Button from "../components/common/Button";
import Loader from "../components/common/Loader";
import ErrorMessage from "../components/common/ErrorMessage";
import { getErrorMessage, formatDate } from "../utils/helpers";
import toast from "react-hot-toast";

const AdminUsersPage = () => {
  const [users, setUsers] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  const fetchUsers = async () => {
    setLoading(true);
    setError("");

    try {
      const data = await usersApiService.getAllUsers();
      setUsers(data || []);
    } catch (err) {
      setError(getErrorMessage(err));
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchUsers();
  }, []);

  const handleActivate = async (userId) => {
    try {
      await usersApiService.activateUser(userId);
      toast.success("User activated!");
      fetchUsers();
    } catch (err) {
      toast.error(getErrorMessage(err));
    }
  };

  const handleDeactivate = async (userId) => {
    if (!window.confirm("Are you sure you want to deactivate this user?")) {
      return;
    }

    try {
      await usersApiService.deactivateUser(userId);
      toast.success("User deactivated!");
      fetchUsers();
    } catch (err) {
      toast.error(getErrorMessage(err));
    }
  };

  const handleDelete = async (userId) => {
    if (
      !window.confirm(
        "Are you sure you want to delete this user? This action is irreversible!"
      )
    ) {
      return;
    }

    try {
      await usersApiService.deleteUser(userId);
      toast.success("User deleted!");
      fetchUsers();
    } catch (err) {
      toast.error(getErrorMessage(err));
    }
  };

  if (loading) {
    return <Loader fullScreen message="Loading users..." />;
  }

  return (
    <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      <div className="mb-8">
        <h1 className="text-3xl font-bold text-gray-900 mb-2">
          User Management
        </h1>
        <p className="text-gray-600">Total users: {users.length}</p>
      </div>

      {error && <ErrorMessage message={error} onRetry={fetchUsers} />}

      {users.length === 0 && !loading && !error ? (
        <div className="text-center py-12 bg-white rounded-lg shadow-md">
          <Users className="w-16 h-16 text-gray-400 mx-auto mb-4" />
          <h3 className="text-xl font-semibold text-gray-900 mb-2">
            No users found
          </h3>
        </div>
      ) : (
        <div className="bg-white rounded-lg shadow-md overflow-hidden">
          <div className="overflow-x-auto">
            <table className="w-full">
              <thead className="bg-gray-50 border-b">
                <tr>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    User
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Email
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Role
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Status
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Registration Date
                  </th>
                  <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Actions
                  </th>
                </tr>
              </thead>
              <tbody className="divide-y divide-gray-200">
                {users.map((user) => (
                  <tr
                    key={user.id}
                    className="hover:bg-gray-50 transition-colors"
                  >
                    <td className="px-6 py-4 whitespace-nowrap">
                      <div className="flex items-center">
                        <div className="flex-shrink-0 h-10 w-10 bg-primary-100 rounded-full flex items-center justify-center">
                          <span className="text-primary-600 font-semibold">
                            {user.name.charAt(0).toUpperCase()}
                          </span>
                        </div>
                        <div className="ml-4">
                          <div className="text-sm font-medium text-gray-900">
                            {user.name}
                          </div>
                        </div>
                      </div>
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap">
                      <div className="text-sm text-gray-900">{user.email}</div>
                      {user.emailConfirmed && (
                        <div className="text-xs text-green-600">
                          ✓ Confirmed
                        </div>
                      )}
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap">
                      <span
                        className={`inline-flex items-center gap-1 px-2.5 py-0.5 rounded-full text-xs font-medium ${
                          user.role === "Admin"
                            ? "bg-purple-100 text-purple-800"
                            : "bg-blue-100 text-blue-800"
                        }`}
                      >
                        {user.role === "Admin" && (
                          <Shield className="w-3 h-3" />
                        )}
                        {user.role}
                      </span>
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap">
                      <span
                        className={`px-2.5 py-0.5 rounded-full text-xs font-medium ${
                          user.isActive
                            ? "bg-green-100 text-green-800"
                            : "bg-red-100 text-red-800"
                        }`}
                      >
                        {user.isActive ? "Active" : "Inactive"}
                      </span>
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                      {formatDate(user.createdAt)}
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
                      <div className="flex items-center justify-end gap-2">
                        {user.isActive ? (
                          <button
                            onClick={() => handleDeactivate(user.id)}
                            className="text-orange-600 hover:text-orange-900 p-2 hover:bg-orange-50 rounded transition-colors"
                            title="Deactivate"
                          >
                            <UserX className="w-4 h-4" />
                          </button>
                        ) : (
                          <button
                            onClick={() => handleActivate(user.id)}
                            className="text-green-600 hover:text-green-900 p-2 hover:bg-green-50 rounded transition-colors"
                            title="Activate"
                          >
                            <UserCheck className="w-4 h-4" />
                          </button>
                        )}

                        <button
                          onClick={() => handleDelete(user.id)}
                          className="text-red-600 hover:text-red-900 p-2 hover:bg-red-50 rounded transition-colors"
                          title="Delete"
                        >
                          <Trash2 className="w-4 h-4" />
                        </button>
                      </div>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </div>
      )}
    </div>
  );
};

export default AdminUsersPage;
