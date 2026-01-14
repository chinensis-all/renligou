namespace Renligou.Core.Application.Enterprise.Queries
{
    public sealed record CompanyListDto
    {
        public string CompanyId { get; init; }
        public string CompanyCode { get; init; }
        public string CompanyName { get; init; }
        public string CompanyShortName { get; init; }
        public string CompanyType { get; init; }
    }
}
