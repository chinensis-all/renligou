namespace Renligou.Core.Application.Common.Criterias
{
    public abstract record PaginateCriteria(
        int Page = 1,
        int PageSize = 15
    )
    {
        public int Page { get; init; } = Page < 1 ? 1 : Page;
        public int PageSize { get; init; } = PageSize <= 0 ? 15 : PageSize;

        public int Skip => (Page - 1) * PageSize;
    }
}
