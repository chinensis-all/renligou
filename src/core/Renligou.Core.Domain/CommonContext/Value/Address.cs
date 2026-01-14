namespace Renligou.Core.Domain.CommonContext.Value
{
    public readonly record struct Address(
        long ProvinceId = 0,
        string Province = "",
        long CityId = 0,
        string City = "",
        long DistrictId = 0,
        string District = "",
        string CompletedAddress = ""
    ) { }
}
