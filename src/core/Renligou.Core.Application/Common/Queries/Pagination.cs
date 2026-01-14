namespace Renligou.Core.Application.Common.Queries
{
    public sealed record Pagination<T>
    {
        public int Page { get; init; } 

        public int PageSize { get; init; }

        public long Total { get; init; }

        public List<T> Items { get; init; }

        public long TotalPages => (long)Math.Ceiling((double)Total / PageSize);

        public bool HasPrevious => Page > 1;

        public bool HasNext => Page < TotalPages;

        public int Count => Items.Count;
    }
}
