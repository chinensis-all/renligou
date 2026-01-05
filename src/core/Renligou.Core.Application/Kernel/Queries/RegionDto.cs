using System.ComponentModel;

namespace Renligou.Core.Application.Kernel.Queries
{
    public class RegionDto
    {
        [Description("行政区划ID")]
        public string Id { get; set; }

        [Description("邮编")]
        public string PostalCode { get; set; } = string.Empty;

        [Description("区号")]
        public string AreaCode { get; set; } = string.Empty;

        [Description("名称")]
        public string RegionName { get; set; } = string.Empty;

        [Description("合并名称")]
        public string MergeName { get; set; } = string.Empty;

        [Description("经度")]
        public float Longitude { get; set; } = 0;

        [Description("纬度")]
        public float Latitude { get; set; } = 0;
    }
}
