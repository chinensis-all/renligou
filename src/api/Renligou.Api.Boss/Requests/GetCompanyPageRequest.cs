using System.ComponentModel;

namespace Renligou.Api.Boss.Requests
{
    public sealed record GetCompanyPageRequest
    {
        [Description("公司类型")]
        public string? CompanyType { get; init; }

        [Description("公司名称，模糊搜索")]
        public string? CompanyName { get; init; }

        [Description("省份Id")]
        public long? ProvinceId { get; init; }

        [Description("城市Id")]
        public string? Status { get; init; }

        [Description("是否启用")]
        public bool? Actived { get; init; }

        [Description("页码，默认1")]
        public int Page { get; init; } = 1;

        [Description("页大小，默认15")]
        public int PageSize { get; init; } = 15;
    }
}
