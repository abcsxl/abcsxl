using abcsxl.Models.Entities;
using abcsxl.Models.Enums;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Linq;

namespace abcsxl.Extensions
{
    public static class CategoryExtensions
    {
        public static string GetFullPath(this Category category, string separator = " / ")
        {
            var path = new List<string> { category.Name };
            var current = category.Parent;
            while (current != null)
            {
                path.Insert(0, current.Name);
                current = current.Parent;
            }
            return string.Join(separator, path);
        }

        public static int GetLevel(this Category category)
        {
            int level = 1;
            var current = category.Parent;
            while (current != null)
            {
                level++;
                current = current.Parent;
            }
            return level;
        }

        /// <summary>
        /// 获取所有子分类ID（包括嵌套子分类）
        /// </summary>
        public static List<Guid> GetAllDescendantIds(this Category category, bool includeSelf = false)
        {
            var ids = new List<Guid>();
            if (includeSelf) ids.Add(category.Id);
            AddDescendantIds(category, ids);
            return ids;
        }

        private static void AddDescendantIds(Category category, List<Guid> ids)
        {
            foreach (var child in category.Children.Where(c => !c.IsDeleted))
            {
                ids.Add(child.Id);
                AddDescendantIds(child, ids);
            }
        }

        /// <summary>
        /// 更新统计信息
        /// </summary>
        public static void UpdateStatistics(this Category category)
        {
            category.PostCount = category.PostCategories
                .Count(pc => pc.Post != null && pc.Post.Status != PostStatus.Deleted && pc.Post.Status == PostStatus.Published);
        }

        /// <summary>
        /// 软删除
        /// </summary>
        public static void SoftDelete(this Category category)
        {
            category.IsDeleted = true;
            category.DeletedAt = DateTime.UtcNow;
            category.UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// 恢复删除
        /// </summary>
        public static void Restore(this Category category)
        {
            category.IsDeleted = false;
            category.DeletedAt = null;
            category.UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// 移动分类（更改父级）
        /// </summary>
        public static void MoveTo(this Category category, Category? newParent)
        {
            if (newParent?.Id == category.Id)
                throw new InvalidOperationException("不能将分类移动到自身");

            // 检查循环引用
            if (newParent != null && IsDescendantOf(category, newParent))
                throw new InvalidOperationException("不能将分类移动到其子分类下");

            category.ParentId = newParent?.Id;
            category.UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// 检查是否为指定分类的后代
        /// </summary>
        private static bool IsDescendantOf(this Category category, Category item)
        {
            var current = category.Parent;
            while (current != null)
            {
                if (current.Id == item.Id)
                    return true;
                current = current.Parent;
            }
            return false;
        }
    }
}
