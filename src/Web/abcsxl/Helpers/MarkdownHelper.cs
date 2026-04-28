using System.Text.RegularExpressions;

namespace abcsxl.Helpers
{
    public static class MarkdownHelper
    {
        /// <summary>
        /// 从 Markdown 生成纯文本摘要（移除 HTML 标签、截断）
        /// </summary>
        public static string GenerateExcerptFromMarkdown(string markdown, int maxLength = 20)
        {
            if (string.IsNullOrWhiteSpace(markdown)) return string.Empty;

            // 简单移除 Markdown 语法符号（粗略处理，足够用于摘要）
            // 更严谨可用 Markdig 解析后取文本，但这里为轻量先用正则
            //var plain = Regex.Replace(markdown, @"[*_~`#\\[\]()]|\!\[.*?\]\(.*?\)|\[.*?\]\(.*?\)", " ");
            var plain = Markdig.Markdown.ToPlainText(markdown);

            plain = Regex.Replace(plain, @"\s+", " ").Trim(); // 合并空白

            if (plain.Length <= maxLength) return plain;

            var excerpt = plain[..maxLength];
            var lastSpace = excerpt.LastIndexOf(' ');
            if (lastSpace > maxLength / 2)
                excerpt = excerpt[..lastSpace];

            return excerpt + "...";
        }

        //public static string GenerateExcerptFromMarkdown(string markdown, int maxLength = 200)
        //{
        //    if (string.IsNullOrWhiteSpace(markdown))
        //        return string.Empty;

        //    // 1. 将 Markdown 转为 HTML
        //    var html = Markdown.ToHtml(markdown);

        //    // 2. 移除 HTML 标签，只保留文本
        //    var plainText = Regex.Replace(html, @"<[^>]+>", string.Empty);

        //    // 3. 清理多余空白
        //    plainText = Regex.Replace(plainText, @"\s+", " ").Trim();

        //    // 4. 截断并加省略号
        //    if (plainText.Length <= maxLength)
        //        return plainText;

        //    return plainText.Substring(0, maxLength) + "...";
        //}
    }
}
