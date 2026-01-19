using System.ComponentModel;

namespace Renligou.Core.Application.Enterprise.Queries
{
    public sealed record CompanyListDto
    {
        [Description("公司ID")]
        public string CompanyId { get; init; }

        [Description("公司编码")]
        public string CompanyCode { get; init; }

        [Description("公司名称")]
        public string CompanyName { get; init; }

        [Description("公司简称")]
        public string CompanyShortName { get; init; }

        [Description("公司类型")]
        public string CompanyType { get; init; }
    }
}
