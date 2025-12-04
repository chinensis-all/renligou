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
using System.Net;

namespace Renligou.Contracts.Exceptions
{
    /// <summary>
    /// UnprocessableEntityException: 无法处理的实体异常(业务验证失败，非参数失败)
    /// </summary>
    public class UnprocessableEntityException : BaseException
    {
        public UnprocessableEntityException (string message = "数据验证失败，请检查输入内容是否正确")
           : base(message, HttpStatusCode.UnprocessableEntity)
        {
        }
    }
}
