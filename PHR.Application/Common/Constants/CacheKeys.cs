namespace PHR.Application.Common.Constants;
public static class CacheKeys
{
    public const string USER_PERMISSIONS = "user_permissions_{0}";
    public const string USER_ROLES = "user_roles_{0}";
    public const string ROLE_PERMISSIONS = "role_permissions_{0}";
    public const string PATIENT_RECORD = "patient_record_{0}";
    public const string ACCESS_REQUESTS = "access_requests_{0}";
    public const string USER_PROFILE = "user_profile_{0}";
    public static string UserPermissions(int userId) => string.Format(USER_PERMISSIONS, userId);
    public static string UserRoles(int userId) => string.Format(USER_ROLES, userId);
    public static string RolePermissions(int roleId) => string.Format(ROLE_PERMISSIONS, roleId);
    public static string PatientRecord(int recordId) => string.Format(PATIENT_RECORD, recordId);
    public static string AccessRequests(int userId) => string.Format(ACCESS_REQUESTS, userId);
    public static string UserProfile(int userId) => string.Format(USER_PROFILE, userId);
}