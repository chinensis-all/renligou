using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Renligou.Core.Infrastructure.Persistence.Pos
{
    [Table("companies")]
    public class CompanyPo
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public string CompanyType { get; set; }

        [Required]
        public string CompanyCode { get; set; }

        [Required]
        public string CompanyName { get; set; }

        public string CompanyShortName { get; set; } = string.Empty;

        public string LegalPersonName { get; set; } = string.Empty;

        public string CreditCode { get; set; } = string.Empty;

        public string RegisteredAddress { get; set; } = string.Empty;

        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset UpdatedAt { get; set; }

        public long ProvinceId { get; set; } = 0;

        public string Province { get; set; } = string.Empty;

        public long CityId { get; set; } = 0;

        public string City { get; set; } = string.Empty;

        public long DistrictId { get; set; } = 0;

        public string District { get; set; } = string.Empty;

        public string CompletedAddress { get; set; } = string.Empty;

        public short Enabled { get; set; } = 1;

        public DateOnly? EffectiveDate { get; set; }

        public DateOnly? ExpiredDate { get; set; }

        public string Remark { get; set; } = string.Empty;
    }
}
