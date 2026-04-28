using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace abcsxl.Models.Entities
{
    /// <summary>
    /// 文章-标签关联表（多对多关系）
    /// </summary>
    public class PostTag
    {
        public Guid PostId { get; set; }
        public Guid TagId { get; set; }
        public DateTime CreatedAt { get; set; }

    }
}
