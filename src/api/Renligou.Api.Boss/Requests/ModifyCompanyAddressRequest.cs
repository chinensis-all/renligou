using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Renligou.Api.Boss.Requests
{
    public sealed record ModifyCompanyAddressRequest
    {
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
    }
}
