import React, { useState, useEffect } from "react";
import { Users } from "lucide-react";
import usersApiService from "../api/usersApi";
import UserList from "../components/users/UserList";
import ChangeRoleModal from "../components/users/ChangeRoleModal";
import Loader from "../components/common/Loader";
import ErrorMessage from "../components/common/ErrorMessage";
import { getErrorMessage } from "../utils/helpers";
import toast from "react-hot-toast";

const AdminUsersPage = () => {
  const [users, setUsers] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  const [isRoleModalOpen, setIsRoleModalOpen] = useState(false);
  const [selectedUser, setSelectedUser] = useState(null);
  const [isUpdatingRole, setIsUpdatingRole] = useState(false);

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
        "Are you sure you want to delete this user? This action cannot be undone!"
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

  const handleOpenRoleModal = (user) => {
    setSelectedUser(user);
    setIsRoleModalOpen(true);
  };

  const handleCloseRoleModal = () => {
    setIsRoleModalOpen(false);
    setSelectedUser(null);
  };

  const handleChangeRole = async (newRole) => {
    if (!selectedUser) return;

    setIsUpdatingRole(true);

    try {
      await usersApiService.updateUserRole(selectedUser.id, newRole);
      toast.success("User role updated successfully!");
      handleCloseRoleModal();
      fetchUsers();
    } catch (err) {
      toast.error(getErrorMessage(err));
    } finally {
      setIsUpdatingRole(false);
    }
  };

  if (loading) {
    return <Loader fullScreen message="Loading users..." />;
  }

  return (
    <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      <div className="mb-8">
        <div className="flex items-center gap-3 mb-2">
          <Users className="w-8 h-8 text-primary-600" />
          <h1 className="text-3xl font-bold text-gray-900">User Management</h1>
        </div>
        <p className="text-gray-600">
          Activate, deactivate, change roles, or delete system users
        </p>
      </div>

      {error && <ErrorMessage message={error} onRetry={fetchUsers} />}

      <UserList
        users={users}
        onActivate={handleActivate}
        onDeactivate={handleDeactivate}
        onDelete={handleDelete}
        onChangeRole={handleOpenRoleModal}
        loading={loading}
      />

      <ChangeRoleModal
        isOpen={isRoleModalOpen}
        onClose={handleCloseRoleModal}
        user={selectedUser}
        onConfirm={handleChangeRole}
        isLoading={isUpdatingRole}
      />
    </div>
  );
};

export default AdminUsersPage;
