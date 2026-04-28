using abcsxl.Models.Entities;

namespace abcsxl.Extensions
{
    public static class TagExtensions
    {
        /// <summary>
        /// 增加使用次数
        /// </summary>
        public static void IncrementUsage(this Tag tag)
        {
            tag.UsageCount++;
            tag.LastUsedAt = DateTime.UtcNow;
            tag.UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// 减少使用次数
        /// </summary>
        public static void DecrementUsage(this Tag tag)
        {
            if (tag.UsageCount > 0)
            {
                tag.UsageCount--;
                tag.UpdatedAt = DateTime.UtcNow;
            }
        }

        /// <summary>
        /// 软删除
        /// </summary>
        public static void SoftDelete(this Tag tag)
        {
            tag.IsDeleted = true;
            tag.DeletedAt = DateTime.UtcNow;
            tag.UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// 恢复删除
        /// </summary>
        public static void Restore(this Tag tag)
        {
            tag.IsDeleted = false;
            tag.DeletedAt = null;
            tag.UpdatedAt = DateTime.UtcNow;
        }
    }
}
