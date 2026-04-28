namespace abcsxl.Helpers
{
    public static class PaginationHelper
    {
        public static PaginationModel GetPaginationModel(int currentPage, int totalPages)
        {
            var model = new PaginationModel
            {
                CurrentPage = currentPage,
                TotalPages = totalPages,
                ShowFirstPage = true,
                ShowLastPage = true,
                ShowLeftEllipsis = false,
                ShowRightEllipsis = false,
                MiddlePages = []
            };

            // 总页数 <= 5 时，显示所有页码
            if (totalPages <= 5)
            {
                for (int i = 2; i <= totalPages - 1; i++)
                {
                    model.MiddlePages.Add(i);
                }
                model.ShowFirstPage = totalPages > 1;
                model.ShowLastPage = totalPages > 1;
                return model;
            }

            // 总页数 > 5 时，只显示3个中间页码

            // 情况1: 靠近开头 (当前页 <= 3)
            if (currentPage <= 3)
            {
                model.MiddlePages = new List<int> { 2, 3, 4 };
                model.ShowLeftEllipsis = false;
                model.ShowRightEllipsis = true;
            }
            // 情况2: 靠近结尾 (当前页 >= totalPages - 2)
            else if (currentPage >= totalPages - 2)
            {
                model.MiddlePages = [totalPages - 3, totalPages - 2, totalPages - 1];
                model.ShowLeftEllipsis = true;
                model.ShowRightEllipsis = false;
            }
            // 情况3: 中间位置
            else
            {
                model.MiddlePages = [currentPage - 1, currentPage, currentPage + 1];
                model.ShowLeftEllipsis = true;
                model.ShowRightEllipsis = true;
            }

            return model;
        }
    }

    public class PaginationModel
    {
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public List<int> MiddlePages { get; set; } = [];
        public bool ShowLeftEllipsis { get; set; }
        public bool ShowRightEllipsis { get; set; }
        public bool ShowFirstPage { get; set; }
        public bool ShowLastPage { get; set; }
    }
}
