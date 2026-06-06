using System.Text.RegularExpressions;

namespace abcsxl.Helpers
{
    public static class MarkdownHelper
    {
        /// <summary>
        /// 从 Markdown 生成纯文本摘要（移除 Markdown 元素、表格、图片等）
        /// </summary>
        public static string GenerateExcerptFromMarkdown(string markdown, int maxLength = 200)
        {
            if (string.IsNullOrWhiteSpace(markdown)) return string.Empty;

            // 先移除表格语法 (| ... |)
            var text = Regex.Replace(markdown, @"^\|.+\|$", "", RegexOptions.Multiline);
            
            // 移除图片语法 ![alt](url)
            text = Regex.Replace(text, @"!\[.*?\]\(.*?\)", "");
            
            // 移除链接 [text](url) 保留文字
            text = Regex.Replace(text, @"\[([^\]]+)\]\([^)]+\)", "$1");
            
            // 移除 HTML 标签
            text = Regex.Replace(text, @"<[^>]+>", "");
            
            // 移除代码块
            text = Regex.Replace(text, @"```[\s\S]*?```", "");
            text = Regex.Replace(text, @"`[^`]+`", "");
            
            // 移除 Markdown 符号，保留中文、英文、数字
            text = Regex.Replace(text, @"[*_~`#\\>()\[\]]", "");
            
            // 多空格合并
            text = Regex.Replace(text, @"\s+", " ").Trim();
            
            // 截断
            if (text.Length <= maxLength) return text;
            
            var excerpt = text[..maxLength];
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
