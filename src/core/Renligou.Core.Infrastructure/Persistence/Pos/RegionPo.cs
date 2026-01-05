using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Renligou.Core.Infrastructure.Persistence.Pos
{
    /// <summary>
    /// Po: 中国行政区划表
    /// </summary>
    [Table("regions")]
    public class RegionPo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public long ParentId { get; set; } = 0;

        public short RegionLevel { get; set; } = 0;

        public string PostalCode { get; set; } = string.Empty;

        public string AreaCode { get; set; } = string.Empty;

        public string RegionName { get; set; } = string.Empty;

        public string NamePinyin { get; set; } = string.Empty;

        public string ShortName { get; set; } = string.Empty;

        public string MergeName { get; set; } = string.Empty;

        public float Longitude { get; set; } = 0;

        public float Latitude { get; set; } = 0;
    }
}
