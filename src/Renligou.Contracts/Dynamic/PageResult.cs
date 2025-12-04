/**
 * Copyright (C) 2025 zhangxihai<mail@sniu.com>，All rights reserved.
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <https://www.gnu.org/licenses/>.
 *
 * WARNING: This code is licensed under the GPL. Any derivative work or
 * distribution of this code must also be licensed under the GPL. Failure
 * to comply with the terms of the GPL may result in legal action.
 */
namespace Renligou.Contracts.Dynamic
{
    /// <summary>
    /// PageResult: 分页结果
    /// </summary>
    /// <typeparam name="TDto"></typeparam>
    public class PageResult<TDto>
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public long Total { get; set; }
        public long TotalPages { get; set; }
        public int Count => Items?.Count ?? 0;
        public long? PrevPage { get; set; }
        public long? NextPage { get; set; }
        public IReadOnlyList<TDto> Items { get; set; } = Array.Empty<TDto>();
     
        public PageResult(int page, int pageSize, long total, IEnumerable<TDto> items)
        {
            Page = Math.Max(1, page);
            PageSize = Math.Max(1, pageSize);
            Total = Math.Max(0, total);
            Items = items?.ToList() ?? new List<TDto>();
            TotalPages = Total > 0 ? (Total + PageSize - 1) / PageSize : 0;
            PrevPage = Page > 1 ? Page - 1 : null;
            NextPage = Page < TotalPages ? Page + 1 : null;
        }
    }
}
