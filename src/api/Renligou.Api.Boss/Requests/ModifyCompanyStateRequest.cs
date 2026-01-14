using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Renligou.Api.Boss.Requests
{
    public sealed record ModifyCompanyStateRequest 
    {
        [Description]
        [Required(ErrorMessage = "请选择是否启用")]
        public bool Enabled { get; init; }

        [Description("生效日期，格式：yyyy-MM-dd")]
        [Required(ErrorMessage = "请选择生效日期")]
        [RegularExpression(
            @"^\d{4}-\d{2}-\d{2}$",
            ErrorMessage = "生效日期格式必须为 yyyy-MM-dd"
        )]
        public string? EffectiveDate { get; init; }

        [Description("失效日期，格式：yyyy-MM-dd")]
        [Required(ErrorMessage = "请选择失效日期")]
        [RegularExpression(
            @"^\d{4}-\d{2}-\d{2}$",
            ErrorMessage = "失效日期格式必须为 yyyy-MM-dd"
        )]
        public string? ExpiredDate { get; init; }
    }
}
