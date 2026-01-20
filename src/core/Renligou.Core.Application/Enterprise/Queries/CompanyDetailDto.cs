using System.ComponentModel;

namespace Renligou.Core.Application.Enterprise.Queries
{
    public sealed record CompanyDetailDto
    {
        [Description("公司ID")]
        public string CompanyId { get; init; }
        
        [Description("公司编码")]
        public string CompanyCode { get; init; }

        [Description("公司类型")]
        public string CompanyType { get; init; }

        [Description("公司名称")]
        public string CompanyName { get; init; }

        [Description("公司简称")]
        public string CompanyShortName { get; init; }

        [Description("法人姓名")]
        public string LegalPersonName { get; init; }
        
        [Description("统一社会信用代码")]
        public string CreditCode { get; init; }
        
        [Description("注册地址")]
        public string RegisteredAddress { get; init; }

        [Description("联系电话")]
        public string ProvinceId { get; init; }

        [Description("省份")]
        public string Province { get; init; }

        [Description("城市ID")]
        public string CityId { get; init; }

        [Description("城市")]
        public string City { get; init; }

        [Description("区县ID")]
        public string DistrictId { get; init; }

        [Description("区县")]
        public string District { get; init; }

        [Description("详细地址")]
        public string CompletedAddress { get; init; }

        [Description("是否启用")]
        public bool Enabled { get; init; }

        [Description("营业执照图片URL")]
        public string EffectiveDate { get; init; }

        [Description("营业执照图片URL")]
        public string ExpiredDate { get; init; }

        [Description("备注")]
        public string Remark { get; init; }

        [Description("创建时间")]
        public string CreatedAt { get; init; }
        
        [Description("更新时间")]
        public string UpdatedAt { get; init; }
    }
}
