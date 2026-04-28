using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using abcsxl.Models.Entities;
using abcsxl.Models.Enums;

namespace abcsxl.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext (DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Post> Posts { get; set; } = default!;
        public DbSet<Category> Categories { get; set; } = default!;
        public DbSet<Tag> Tags { get; set; } = default!;
        public DbSet<User> Users { get; set; } = default!;
        public DbSet<Comment> Comments { get; set; } = default!;
        public DbSet<VisitLog> VisitLogs { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                // 配置表名
                entity.ToTable("User");

                // 配置主键
                entity.HasKey(e => e.Id);

                // 配置备用key
                entity.HasAlternateKey(e => e.Username);

                // 配置索引及部分唯一约束条件
                entity.HasIndex(e => e.Username)
                      .IsUnique();
                entity.HasIndex(e => e.PhoneNumber)
                      .IsUnique();
                entity.HasIndex(e => e.NationalId)
                      .IsUnique();
                entity.HasIndex(e => e.Email)
                      .IsUnique();

                // 配置和post的一对多，不级联删除（删除用户归为匿名或已删除用户）
                entity.HasMany(e => e.Posts)
                      .WithOne(e => e.Author)
                      .HasForeignKey(e => e.AuthorId)
                      .IsRequired(false)
                      .OnDelete(DeleteBehavior.Restrict);

                // 配置和comment的一对多，不级联删除（删除用户归为匿名或已删除用户）
                entity.HasMany(e => e.Comments)
                      .WithOne(e => e.User)
                      .HasForeignKey(e => e.UserId)
                      .IsRequired(false)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Post>(entity =>
            {
                // 配置表名
                entity.ToTable("Post");

                // 配置主键
                entity.HasKey(e => e.Id);

                //// 软删除
                //entity.HasQueryFilter(p => p.Status != PostStatus.Deleted);

                // 配置索引
                entity.HasIndex(e => e.Title);
                entity.HasIndex(e => e.CreatedAt);

                // 配置和Category的多对多
                entity.HasMany(e => e.Categories)
                      .WithMany(e => e.Posts)
                      .UsingEntity<PostCategory>(
                          j => j.Property(e => e.CreatedAt)
                                .ValueGeneratedOnAdd()
                                .HasDefaultValue(DateTime.UtcNow));

                // 配置和tag的多对多
                entity.HasMany(e => e.Tags)
                      .WithMany(e => e.Posts)
                      .UsingEntity<PostTag>(
                          j => j.Property(e => e.CreatedAt)
                                .ValueGeneratedOnAdd()
                                .HasDefaultValue(DateTime.UtcNow));

                // 配置和comment的一对多，级联删除（删除文章删除所有相关评论）
                entity.HasMany(e => e.Comments)
                      .WithOne(e => e.Post)
                      .HasForeignKey(e => e.PostId)
                      .IsRequired(false)
                      .OnDelete(DeleteBehavior.Cascade);

                // 配置和VisitLog的一对多，级联删除（删除文章删除所有访问记录）
                entity.HasMany(e=>e.VisitLogs)
                      .WithOne(e=>e.Post)
                      .HasForeignKey(e=>e.PostId)
                      .IsRequired(false)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Category>(entity =>
            {
                // 配置表名
                entity.ToTable("Category");

                // 配置主键
                entity.HasKey(e => e.Id);

                // 软删除
                entity.HasQueryFilter(c => !c.IsDeleted);

                // 配置索引
                entity.HasIndex(e => e.Name)
                      .IsUnique();

                // 配置自引用（层级分类）
                entity.HasOne(c => c.Parent)
                      .WithMany(c => c.Children)
                      .HasForeignKey(c => c.ParentId)
                      .IsRequired(false)
                      .OnDelete(DeleteBehavior.Restrict);// 防止级联删除
            });

            modelBuilder.Entity<Tag>(entity=>
            {
                // 配置表名
                entity.ToTable("Tag");

                // 配置主键
                entity.HasKey(e => e.Id);

                // 软删除
                entity.HasQueryFilter(t => !t.IsDeleted);

                // 配置索引
                entity.HasIndex(c => c.Name)
                      .IsUnique();
            });          

            modelBuilder.Entity<Comment>(entity =>
            {
                // 配置表名
                entity.ToTable("Comment");

                // 配置主键
                entity.HasKey(e => e.Id);

                // 配置评论内容的限制
                entity.Property(c => c.Content)
                      .HasMaxLength(1000)
                      .IsRequired();

                // 配置索引
                entity.HasIndex(c => c.PostId);       // 按文章查询评论
                entity.HasIndex(c => c.UserId);       // 按用户查询评论
                entity.HasIndex(c => c.CreatedAt);    // 按时间排序

                entity.HasQueryFilter(c => (c.PostId == null) || (c.Post != null && c.Post.Status != PostStatus.Deleted));
            });

            modelBuilder.Entity<PostCategory>(entity =>
            {
                // 配置表名
                entity.ToTable("PostCategory");

                // 配置主键
                entity.HasKey(pc => new { pc.PostId, pc.CategoryId });
            });

            modelBuilder.Entity<PostTag>(entity =>
            {
                // 配置表名
                entity.ToTable("PostTag");

                // 配置主键
                entity.HasKey(pt => new { pt.PostId, pt.TagId }); ;
            });

            modelBuilder.Entity<VisitLog>(entity =>
            {
                // 配置表名
                entity.ToTable("VisitLog");

                // 配置主键
                entity.HasKey(e => e.Id);

                // 配置索引
                entity.HasIndex(e => e.AccessTime);
                entity.HasIndex(e => e.IPAddress);

                // 配置默认值
                entity.Property(e => e.AccessTime)
                      .ValueGeneratedOnAdd()
                      .HasDefaultValue(DateTime.UtcNow);
            });
        }
    }
}
