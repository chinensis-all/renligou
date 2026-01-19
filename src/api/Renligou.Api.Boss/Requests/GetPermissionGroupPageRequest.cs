namespace Renligou.Api.Boss.Requests
{
    public class GetPermissionGroupPageRequest
    {
        public string? GroupName { get; set; }

        public string? DisplayName { get; set; }

        public int Page { get; set; } = 1;

        public int PageSize { get; set; } = 10;
    }
}
