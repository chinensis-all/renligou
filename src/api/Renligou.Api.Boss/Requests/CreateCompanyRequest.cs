using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Renligou.Api.Boss.Requests
{
    public sealed record CreateCompanyRequest
    {
        [Description("公司类型(HEADQUARTER / BRANCH / SUBSIDIARY)")]
        [Required(ErrorMessage = "公司类型不能为空")]
        public string CompanyType { get; init; } = default!;

        [Description("公司编码")]
        [MaxLength(64, ErrorMessage = "公司编码长度不能超过 64")]
        public string CompanyCode { get; init; } = default!;

        [Description("公司名称")]
        [Required(ErrorMessage = "公司名称不能为空")]
        [MaxLength(128, ErrorMessage = "公司名称长度不能超过 128")]
        public string CompanyName { get; init; } = default!;

        [Description("公司简称")]
        [Required(ErrorMessage = "公司简称不能为空")]
        [MaxLength(64, ErrorMessage = "公司简称长度不能超过 64")]
        public string CompanyShortName { get; init; } = default!;

        [Description("法人姓名")]
        [MaxLength(64, ErrorMessage = "法人姓名长度不能超过 64")]
        public string LegalPersonName { get; init; } = default!;

        [Description("统一社会信用代码")]
        [MaxLength(32, ErrorMessage = "统一社会信用代码长度不能超过 32")]
        public string CreditCode { get; init; } = default!;

        [Description("注册地址")]
        [MaxLength(256, ErrorMessage = "注册地址长度不能超过 256")]
        public string RegisteredAddress { get; init; } = default!;

        [Description("备注")]
        [MaxLength(512, ErrorMessage = "备注长度不能超过 512")]
        public string Remark { get; init; } = default!;

        [Description("所在省份Id")]
        [Required(ErrorMessage = "请选择所在省份")]
        [Range(1, long.MaxValue, ErrorMessage = "请选择所在省份")]
        public long ProvinceId { get; init; }

        [Description("所在城市Id")]
        [Required(ErrorMessage = "请选择所在城市")]
        [Range(1, long.MaxValue, ErrorMessage = "请选择所在城市")]
        public long CityId { get; init; }

        [Description("所在区县Id")]
        [Required(ErrorMessage = "请选择所在区县")]
        [Range(1, long.MaxValue, ErrorMessage = "请选择所在曲线")]
        public long DistrictId { get; init; }

        [Description("详细地址")]
        [MaxLength(256, ErrorMessage = "详细地址长度不能超过 256")]
        public string CompletedAddress { get; init; } = default!;

        [Description]
        public bool Enabled { get; init; } = false;

        [Description("生效日期，格式：yyyy-MM-dd")]
        [RegularExpression(
            @"^\d{4}-\d{2}-\d{2}$",
            ErrorMessage = "生效日期格式必须为 yyyy-MM-dd"
        )]
        public string? EffectiveDate { get; init; }

        [Description("失效日期，格式：yyyy-MM-dd")]
        [RegularExpression(
            @"^\d{4}-\d{2}-\d{2}$",
            ErrorMessage = "失效日期格式必须为 yyyy-MM-dd"
        )]
        public string? ExpiredDate { get; init; }
    }
}
