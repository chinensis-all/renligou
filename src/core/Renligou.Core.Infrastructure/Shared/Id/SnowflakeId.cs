using Renligou.Core.Shared.Ddd;

namespace Renligou.Core.Infrastructure.Shared.Id
{
    public class SnowflakeId : IIdGenerator
    {
        private readonly object _lock = new();

        private const long Epoch = 1764028800000L;
        private const int WorkerIdBits = 10;     // 支持的最大机器 ID 数量为 2^10 = 1024 0-1023
        private const int SequenceBits = 12;     // 每毫秒内生成的 ID 数量为 2^12 = 4096 0-4095

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
                throw new ArgumentException($"workerId 必须在 0 到 {MaxWorkerId} 之间");

            WorkerId = workerId;
        }

        public long NextId()
        {
            lock (_lock)
            {
                long timestamp = TimeGen();

                if (timestamp < _lastTimestamp)
                {
                    long offset = _lastTimestamp - timestamp;
                    if (offset < 5) // 容忍 5ms 以内的时钟回拨
                    {
                        Thread.Sleep((int)offset);
                        timestamp = TimeGen();
                    }
                    else
                    {
                        throw new Exception($"时钟回拨过大，拒绝生成 ID，差值: {offset}ms");
                    }
                }

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

        private static long TimeGen() => DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    }
}
