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
    /// KeysetPageRequest: 主键偏移分页请求(用其它字段请走正常流程, 不要用偷懒方法)
    /// </summary>
    public class KeysetPageRequest<TCursor>
    {
        /// <summary>
        /// 分页数量
        /// </summary>
        public int Limit { get; set; } = 20;

        /// <summary>
        /// True=下一页，False=上一页
        /// </summary>
        public bool IsNext { get; set; } = true;

        /// <summary>
        /// 排序方式（true=升序 ASC，false=降序 DESC）
        /// </summary>
        public bool Ascending { get; set; } = true;

        /// <summary>
        /// 游标值（上一页最后一条记录的字段值）
        /// 下一页：查询 > Cursor 或 < Cursor
        /// 上一页：反向
        /// </summary>
        public TCursor? Cursor { get; set; } = default!;

        /// <summary>
        /// 过滤条件
        /// </summary>
        public Dictionary<string, object?> Criteria { get; set; } = new();   

    }
}
