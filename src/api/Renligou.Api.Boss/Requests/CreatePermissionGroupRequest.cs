namespace Renligou.Api.Boss.Requests
{
    public class CreatePermissionGroupRequest
    {
        public string GroupName { get; set; } = string.Empty;

        public string DisplayName { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;
    }
}
