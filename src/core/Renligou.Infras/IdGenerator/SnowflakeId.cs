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
using Renligou.Contracts.EFCore;

namespace Renligou.Infras.IdGenerator
{
    /// <summary>
    /// SnowflakeId: 雪花算法唯一标识生成器
    /// </summary>
    public class SnowflakeId : IIdGenerator
    {
        private static readonly object _lock = new();

        private const long Epoch = 1764028800000L;   // 原始epoch, 目前选择2025-11-25 00:00:00 UTC

        private const int WorkerIdBits = 8;          // 机器ID，最大 0~255
        private const int SequenceBits = 14;         // 序列号，最大 0~16383, 每毫秒生成16384个ID

        private const long MaxWorkerId = -1L ^ (-1L << WorkerIdBits);

        private const int WorkerIdShift = SequenceBits;
        private const int TimestampLeftShift = SequenceBits + WorkerIdBits;
        private const long SequenceMask = -1L ^ (-1L << SequenceBits);

        private long _lastTimestamp = -1L;
        private long _sequence = 0L;

        public long WorkerId { get; }

        public SnowflakeId(long workerId)
        {
            if (workerId > MaxWorkerId || workerId < 0)
                throw new ArgumentException($"workerId can't be greater than {MaxWorkerId} or less than 0");

            WorkerId = workerId;
        }

        public long NextId()
        {
            lock (_lock)
            {
                long timestamp = TimeGen();

                if (timestamp < _lastTimestamp)
                    throw new Exception(
                        $"Clock moved backwards. Refusing for {_lastTimestamp - timestamp} ms");

                if (_lastTimestamp == timestamp)
                {
                    _sequence = (_sequence + 1) & SequenceMask;
                    if (_sequence == 0)
                    {
                        timestamp = TilNextMillis(_lastTimestamp);
                    }
                }
                else
                {
                    _sequence = 0;
                }

                _lastTimestamp = timestamp;

                return ((timestamp - Epoch) << TimestampLeftShift)
                     | (WorkerId << WorkerIdShift)
                     | _sequence;
            }
        }

        private static long TilNextMillis(long lastTimestamp)
        {
            long timestamp = TimeGen();
            while (timestamp <= lastTimestamp)
            {
                timestamp = TimeGen();
            }
            return timestamp;
        }

        private static long TimeGen() =>
            DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    }
}

