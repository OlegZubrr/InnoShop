import React, { useState } from "react";
import { Shield, AlertCircle } from "lucide-react";
import Modal from "../common/Modal";
import Button from "../common/Button";
import { USER_ROLES, USER_ROLE_NAMES } from "../../utils/constants";

const ChangeRoleModal = ({ isOpen, onClose, user, onConfirm, isLoading }) => {
  const [selectedRole, setSelectedRole] = useState(
    user?.role || USER_ROLES.USER
  );

  const handleSubmit = () => {
    onConfirm(selectedRole);
  };

  if (!user) return null;

  const currentRoleName = USER_ROLE_NAMES[user.role];
  const newRoleName = USER_ROLE_NAMES[selectedRole];
  const isRoleChanged = selectedRole !== user.role;

  return (
    <Modal
      isOpen={isOpen}
      onClose={onClose}
      title="Change User Role"
      size="small"
    >
      <div className="space-y-6">
        <div className="bg-gray-50 rounded-lg p-4">
          <div className="flex items-center gap-3">
            <div className="w-12 h-12 bg-primary-100 rounded-full flex items-center justify-center">
              <span className="text-lg font-bold text-primary-600">
                {user.fullName.charAt(0).toUpperCase()}
              </span>
            </div>
            <div>
              <p className="font-semibold text-gray-900">{user.fullName}</p>
              <p className="text-sm text-gray-600">{user.email}</p>
            </div>
          </div>
        </div>

        <div>
          <label className="block text-sm font-medium text-gray-700 mb-2">
            Current Role
          </label>
          <div className="flex items-center gap-2">
            <Shield className="w-5 h-5 text-gray-400" />
            <span
              className={`px-3 py-1 rounded-full text-sm font-medium ${
                user.role === USER_ROLES.ADMIN
                  ? "bg-purple-100 text-purple-800"
                  : "bg-blue-100 text-blue-800"
              }`}
            >
              {currentRoleName}
            </span>
          </div>
        </div>

        <div>
          <label className="block text-sm font-medium text-gray-700 mb-2">
            New Role
          </label>
          <select
            value={selectedRole}
            onChange={(e) => setSelectedRole(parseInt(e.target.value))}
            disabled={isLoading}
            className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-primary-500 focus:border-transparent disabled:bg-gray-100 disabled:cursor-not-allowed"
          >
            <option value={USER_ROLES.USER}>
              {USER_ROLE_NAMES[USER_ROLES.USER]} - Regular User
            </option>
            <option value={USER_ROLES.ADMIN}>
              {USER_ROLE_NAMES[USER_ROLES.ADMIN]} - Administrator
            </option>
          </select>
        </div>

        {isRoleChanged && (
          <div className="bg-yellow-50 border border-yellow-200 rounded-lg p-4 flex gap-3">
            <AlertCircle className="w-5 h-5 text-yellow-600 flex-shrink-0 mt-0.5" />
            <div className="text-sm text-yellow-800">
              {selectedRole === USER_ROLES.ADMIN ? (
                <>
                  <strong>Warning!</strong> The user will receive administrator
                  rights and will be able to:
                  <ul className="list-disc list-inside mt-2 space-y-1">
                    <li>Manage all users</li>
                    <li>Activate/deactivate accounts</li>
                    <li>Delete users</li>
                    <li>Change roles of other users</li>
                  </ul>
                </>
              ) : (
                <>
                  <strong>Warning!</strong> The user will lose administrator
                  rights and will no longer be able to manage the system.
                </>
              )}
            </div>
          </div>
        )}

        <div className="flex gap-3 pt-4 border-t">
          <Button
            onClick={onClose}
            variant="secondary"
            disabled={isLoading}
            className="flex-1"
          >
            Cancel
          </Button>
          <Button
            onClick={handleSubmit}
            disabled={isLoading || !isRoleChanged}
            className="flex-1"
          >
            {isLoading ? "Saving..." : "Change Role"}
          </Button>
        </div>
      </div>
    </Modal>
  );
};

export default ChangeRoleModal;
