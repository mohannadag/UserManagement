namespace UserManagement.API.Models.Enums
{
    public static class SystemPermissions
    {
        public static class Users
        {
            public const string View = "Permissions.Users.View";
            public const string Create = "Permissions.Users.Create";
            public const string Edit = "Permissions.Users.Edit";
            public const string Delete = "Permissions.Users.Delete";
            public const string Restore = "Permissions.Users.Restore";
            public const string AssignRoles = "Permissions.Users.AssignRoles";
            public const string AssignPermissions = "Permissions.Users.AssignPermissions";
            public const string ToggleStatus = "Permissions.Users.ToggleStatus";
        }

        public static class Roles
        {
            public const string View = "Permissions.Roles.View";
            public const string Create = "Permissions.Roles.Create";
            public const string Edit = "Permissions.Roles.Edit";
            public const string Delete = "Permissions.Roles.Delete";
            public const string ManagePermissions = "Permissions.Roles.ManagePermissions";
        }

        public static class Permissions
        {
            public const string View = "Permissions.Permissions.View";
        }

        public static List<string> GetAllPermissions()
        {
            return new List<string>
            {
                Users.View,
                Users.Create,
                Users.Edit,
                Users.Delete,
                Users.Restore,
                Users.AssignRoles,
                Users.AssignPermissions,
                Users.ToggleStatus,

                Roles.View,
                Roles.Create,
                Roles.Edit,
                Roles.Delete,
                Roles.ManagePermissions,

                Permissions.View
            };
        }
    }
}
