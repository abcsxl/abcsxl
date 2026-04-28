using System.Runtime.InteropServices;

namespace abcsxl.Helpers
{
    public static class DateTimeHelper
    {
        // 可在测试中替换
        public static Func<DateTime> UtcNow = () => DateTime.UtcNow;

        // 缓存中国时区（启动时初始化）
        private static readonly TimeZoneInfo _chinaTimeZone =
            RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                ? TimeZoneInfo.FindSystemTimeZoneById("China Standard Time")
                : TimeZoneInfo.FindSystemTimeZoneById("Asia/Shanghai");

        /// <summary>
        /// 获取当前中国标准时间（UTC+8）
        /// </summary>
        public static DateTime BeijingNow =>
            TimeZoneInfo.ConvertTimeFromUtc(UtcNow(), _chinaTimeZone);

        /// <summary>
        /// 将 UTC 时间转换为中国标准时间（UTC+8）
        /// 兼容处理：允许输入 DateTimeKind.Unspecified (常见于 SQLite)，并自动视为 UTC 处理。
        /// </summary>
        public static DateTime ToChinaStandardTime(this DateTime utcTime)
        {
            // 允许 Utc 或 Unspecified。如果是 Local，则抛出异常，因为无法确定其偏移量是否正确。
            if (utcTime.Kind != DateTimeKind.Utc && utcTime.Kind != DateTimeKind.Unspecified)
            {
                throw new ArgumentException(
                    $"Input time Kind must be Utc or Unspecified. Got: {utcTime.Kind}. " +
                    "If coming from SQLite, ensure the value represents UTC time.",
                    nameof(utcTime));
            }

            // 如果是 Unspecified (例如从 SQLite 读取)，强制指定为 Utc 以确保 ConvertTimeFromUtc 行为正确
            DateTime safeUtcTime = utcTime.Kind == DateTimeKind.Utc
                ? utcTime
                : DateTime.SpecifyKind(utcTime, DateTimeKind.Utc);

            return TimeZoneInfo.ConvertTimeFromUtc(safeUtcTime, _chinaTimeZone);
        }

        /// <summary>
        /// 将 UTC 时间转换为指定时区的时间
        /// 兼容处理：允许输入 DateTimeKind.Unspecified (常见于 SQLite)，并自动视为 UTC 处理。
        /// </summary>
        public static DateTime ToTimeZone(this DateTime utcTime, string timeZoneId)
        {
            // 允许 Utc 或 Unspecified
            if (utcTime.Kind != DateTimeKind.Utc && utcTime.Kind != DateTimeKind.Unspecified)
            {
                throw new ArgumentException(
                    $"Input time Kind must be Utc or Unspecified. Got: {utcTime.Kind}.",
                    nameof(utcTime));
            }

            // 强制指定为 Utc 以安全转换
            DateTime safeUtcTime = utcTime.Kind == DateTimeKind.Utc
                ? utcTime
                : DateTime.SpecifyKind(utcTime, DateTimeKind.Utc);

            var timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            return TimeZoneInfo.ConvertTimeFromUtc(safeUtcTime, timeZone);
        }
    }
}
