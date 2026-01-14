using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Renligou.Api.Boss.Requests
{
    public sealed record ModifyCompanyBasicRequest
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
    }
}
