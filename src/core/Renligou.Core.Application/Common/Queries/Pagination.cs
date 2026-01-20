using System.ComponentModel;

namespace Renligou.Core.Application.Common.Queries
{
    public sealed record Pagination<T>
    {
        [Description("当前页码")]
        public int Page { get; init; }

        [Description("每页数量")]
        public int PageSize { get; init; }

        [Description("总数量")]
        public long Total { get; init; }

        [Description("数据列表")]
        public List<T> Items { get; init; } = new List<T>();

        [Description("总页数")]
        public long TotalPages => (long)Math.Ceiling((double)Total / PageSize);

        [Description("是否有上一页")]
        public bool HasPrevious => Page > 1;

        [Description("是否有下一页")]
        public bool HasNext => Page < TotalPages;

        [Description("当前页数量")]
        public int Count => Items.Count;
    }
}
