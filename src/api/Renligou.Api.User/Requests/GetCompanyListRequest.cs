using System.ComponentModel;

namespace Renligou.Api.User.Requests
{
    public sealed record GetCompanyListRequest
    {
        [Description("公司名称，模糊搜索")]
        public string? CompanyName { get; init; }
    }
}
