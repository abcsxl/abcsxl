using abcsxl.Helpers;
using abcsxl.Models.Entities;
using abcsxl.Models.Enums;
using System.Text.RegularExpressions;

namespace abcsxl.Extensions
{
    public static class PostExtensions
    {
        // 内容相关
        public static string GetExcerpt(this Post post, int length = 20)
        {
            if (length > 20 || String.IsNullOrEmpty(post.Excerpt))
            {
                post.Excerpt = MarkdownHelper.GenerateExcerptFromMarkdown(post.Content, length);
            }

            return post.Excerpt;
        }

        /// <summary>
        /// 发布文章
        /// </summary>
        public static void Publish(this Post post)
        {
            post.Status = PostStatus.Published;
            post.PublishedAt = DateTime.UtcNow;
            post.UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// 设为草稿
        /// </summary>
        public static void Draft(this Post post)
        {
            post.Status = PostStatus.Draft;
            post.UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// 设置主要分类
        /// </summary>
        public static void SetPrimaryCategory(this Post post, Guid categoryId)
        {
            // 移除现有的主要分类标记
            foreach (var postCategory in post.PostCategories.Where(pc => pc.IsPrimaryCategory))
            {
                postCategory.IsPrimaryCategory = false;
            }

            // 设置新的主要分类
            var target = post.PostCategories.FirstOrDefault(pc => pc.CategoryId == categoryId);
            if (target != null)
            {
                target.IsPrimaryCategory = true;
                target.UpdatedAt = DateTime.UtcNow;
            }
            else
            {
                throw new InvalidOperationException($"文章未关联分类 ID: {categoryId}");
            }
        }

        /// <summary>
        /// 添加分类（可选设置为主要分类）
        /// </summary>
        public static void AddCategory(this Post post, Category category, bool isPrimary = false, int order = 0)
        {
            if (post.PostCategories.Any(pc => pc.CategoryId == category.Id))
                return; // 已存在

            var postCategory = new PostCategory
            {
                PostId = post.Id,
                CategoryId = category.Id,
                IsPrimaryCategory = isPrimary,
                Order = order,
                CreatedAt = DateTime.UtcNow
            };

            // 如果设置为主要分类，需要移除现有的主要分类
            if (isPrimary)
            {
                foreach (var pc in post.PostCategories.Where(pc => pc.IsPrimaryCategory))
                {
                    pc.IsPrimaryCategory = false;
                }
            }

            post.PostCategories.Add(postCategory);
        }

        /// <summary>
        /// 软删除
        /// </summary>
        public static void SoftDelete(this Post post)
        {
            post.Status = PostStatus.Deleted;
            post.DeletedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// 恢复删除
        /// </summary>
        public static void Restore(this Post post)
        {
            post.DeletedAt = null;
            post.Status = PostStatus.Draft;
        }

        /// <summary>
        /// 增加查看次数
        /// </summary>
        public static void IncrementViewCount(this Post post)
        {
            post.ViewCount++;
            post.UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// 更新阅读时间估算
        /// </summary>
        public static void UpdateReadingMinutes(this Post post, int wordsPerMinute = 200)
        {
            var wordCount = post.Content.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;
            post.ReadingMinutes = (int)Math.Ceiling(wordCount / (double)wordsPerMinute);
        }

        //// DTO 映射
        //public static PostDto ToDto(this Post post)
        //    => new()
        //    {
        //        Id = post.Id,
        //        Title = post.Title,
        //        Excerpt = post.GetExcerpt(),
        //        ReadingMinutes = post.GetReadingMinutes(),
        //        CreatedAt = post.CreatedAt,
        //        AuthorName = post.Author?.Name ?? "匿名",
        //        Tags = post.GetTags().Select(t => t.Name).ToList(),
        //        PrimaryCategory = post.GetPrimaryCategory()?.Name,
        //        CommentCount = post.GetCommentCount(),
        //        Url = $"/blog/{post.GetUrlSlug()}"
        //    };
    }

    //public class PostDto
    //{
    //    public int Id { get; set; }
    //    public string Title { get; set; } = string.Empty;
    //    public string Excerpt { get; set; } = string.Empty;
    //    public double ReadingMinutes { get; set; }
    //    public DateTime CreatedAt { get; set; }
    //    public string AuthorName { get; set; } = string.Empty;
    //    public List<string> Tags { get; set; } = new();
    //    public string? PrimaryCategory { get; set; }
    //    public int CommentCount { get; set; }
    //    public string Url { get; set; } = string.Empty;
    //}
}
