using abcsxl.Models.Entities;
using abcsxl.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace abcsxl.Data
{
    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            // 确保数据库已创建
            // 方法1：使用 EnsureCreated（最直接）
            await context.Database.EnsureCreatedAsync();

            // 方法2：或者使用这个命令
            // await context.Database.MigrateAsync();

            // 检查是否已有数据
            if (await context.Users.AnyAsync())
            {
                return;
            }

            Console.WriteLine("开始生成种子数据...");

            try
            {
                await SeedUsersAsync(context);
                await SeedCategoriesAsync(context);
                await SeedTagsAsync(context);

                await context.SaveChangesAsync();

                await SeedPostsAsync(context);
                await SeedCommentsAsync(context);
                await SeedVisitLogsAsync(context);

                await context.SaveChangesAsync();

                Console.WriteLine("种子数据生成完成！");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"种子数据生成失败: {ex.Message}");
                throw;
            }
        }

        private static async Task SeedUsersAsync(ApplicationDbContext context)
        {
            var users = new[]
            {
                new User
                {
                    Username = "admin",
                    //Email = "admin@example.com",
                    //PhoneNumber = "13800138000",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                    DisplayName = "管理员",
                    Bio = "系统管理员，负责内容管理",
                    //Avatar = "https://example.com/avatars/admin.png",
                    Role = UserRole.Admin,
                    IsActive = true
               },
                new User
                {
                    Username = "zhangsan",
                    //Email = "zhangsan@example.com",
                    //PhoneNumber = "13900139000",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Zhang@123"),
                    DisplayName = "张三",
                    Bio = "技术爱好者，喜欢分享编程经验",
                    //Avatar = "https://example.com/avatars/zhangsan.png",
                    Role = UserRole.Author,
                    IsActive = true
               },
                new User
                {
                    Username = "lisi",
                    //Email = "lisi@example.com",
                    //PhoneNumber = "13700137000",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Li@123456"),
                    DisplayName = "李四",
                    Bio = "前端开发工程师，专注于用户体验",
                    //Avatar = "https://example.com/avatars/lisi.png",
                    Role = UserRole.User,
                    IsActive = false
                }
            };

            await context.Users.AddRangeAsync(users);
        }

        private static async Task SeedCategoriesAsync(ApplicationDbContext context)
        {
            var categories = new[]
            {
                new Category
                {
                    Name = "技术",
                    Description = "各种计算机技术的学习和讨论"
                },
                              new Category
                {
                    Name = "生活",
                    Description = "生活感悟和随想"
                }
            };

            await context.Categories.AddRangeAsync(categories);

            await context.SaveChangesAsync();

            var parent = await context.Categories.FirstOrDefaultAsync(c => c.Name.Contains("技术"));
            if (parent != null)
            {
                var children = new[]
                {
                    new Category
                    {
                        Name = "编程语言",
                        Description = "各种编程语言的学习和讨论",
                        ParentId=parent.Id
                    },
                    new Category
                    {
                        Name = "Web开发",
                        Description = "前端和后端Web开发技术",
                        ParentId=parent.Id
                    },
                    new Category
                    {
                        Name = "数据库",
                        Description = "数据库设计、优化和管理",
                        ParentId=parent.Id
                    },
                    new Category
                    {
                        Name = "DevOps",
                        Description = "开发运维一体化",
                        ParentId=parent.Id
                    }
                };

                await context.Categories.AddRangeAsync(children);
            }
        }

        private static async Task SeedTagsAsync(ApplicationDbContext context)
        {
            var tags = new[]
            {
                new Tag
                {
                    Name = "C#",
                    Description = "C# 编程语言"
                },
                new Tag
                {
                    Name = "ASP.NET Core",
                    Description = "ASP.NET Core 框架"
                }
            };

            await context.Tags.AddRangeAsync(tags);
        }

        private static async Task SeedPostsAsync(ApplicationDbContext context)
        {
            var users = await context.Users.ToListAsync();
            var categories = await context.Categories.ToListAsync();
            var tags = await context.Tags.ToListAsync();
            if (users != null && users.Count > 0)
            {
                var posts = new[]
                {
                    new Post
                    {
                        Title = "EF Core 入门教程",
                        Content = "Entity Framework Core (EF Core) 是微软推出的轻量级、可扩展、跨平台的对象关系映射(ORM)框架...",
                        AuthorId = users[0].Id,
                        //CoverImage = "https://example.com/images/efcore.jpg",
                        Categories= { categories[0] },
                        Tags= { tags[0] },
                        Status = PostStatus.Published
                    },
                    new Post
                    {
                        Title = "ASP.NET Core Web API 开发指南",
                        Content = "ASP.NET Core 是微软推出的跨平台、高性能的Web框架...",
                        AuthorId = users.Count>1?users[1].Id:users[0].Id,
                        //CoverImage = "https://example.com/images/webapi.jpg",
                        Status = PostStatus.Published
                    },
                     new Post
                    {
                        Title = "EF Core 入门教程",
                        Content = "Entity Framework Core (EF Core) 是微软推出的轻量级、可扩展、跨平台的对象关系映射(ORM)框架...",
                        AuthorId = users[0].Id,
                        //CoverImage = "https://example.com/images/efcore.jpg",
                        Status = PostStatus.Published
                     },
                    new Post
                    {
                        Title = "EF Core 入门教程",
                        Content = "Entity Framework Core (EF Core) 是微软推出的轻量级、可扩展、跨平台的对象关系映射(ORM)框架...",
                        AuthorId = users[0].Id,
                        //CoverImage = "https://example.com/images/efcore.jpg",
                        Status = PostStatus.Published
                    },
                    new Post
                    {
                        Title = "EF Core 入门教程",
                        Content = "Entity Framework Core (EF Core) 是微软推出的轻量级、可扩展、跨平台的对象关系映射(ORM)框架...",
                        AuthorId = users[0].Id,
                        //CoverImage = "https://example.com/images/efcore.jpg",
                        Status = PostStatus.Published
                    },
                    new Post
                    {
                        Title = "EF Core 入门教程",
                        Content = "Entity Framework Core (EF Core) 是微软推出的轻量级、可扩展、跨平台的对象关系映射(ORM)框架...",
                        AuthorId = users[0].Id,
                        //CoverImage = "https://example.com/images/efcore.jpg",
                        Status = PostStatus.Published
                    }, new Post
                    {
                        Title = "EF Core 入门教程",
                        Content = "Entity Framework Core (EF Core) 是微软推出的轻量级、可扩展、跨平台的对象关系映射(ORM)框架...",
                        AuthorId = users[0].Id,
                        //CoverImage = "https://example.com/images/efcore.jpg",
                        Status = PostStatus.Published
                    }, new Post
                    {
                        Title = "EF Core 入门教程",
                        Content = "Entity Framework Core (EF Core) 是微软推出的轻量级、可扩展、跨平台的对象关系映射(ORM)框架...",
                        AuthorId = users[0].Id,
                        //CoverImage = "https://example.com/images/efcore.jpg",
                        Status = PostStatus.Published
                    }, new Post
                    {
                        Title = "EF Core 入门教程",
                        Content = "Entity Framework Core (EF Core) 是微软推出的轻量级、可扩展、跨平台的对象关系映射(ORM)框架...",
                        AuthorId = users[0].Id,
                        //CoverImage = "https://example.com/images/efcore.jpg",
                        Status = PostStatus.Published
                    }, new Post
                    {
                        Title = "EF Core 入门教程",
                        Content = "Entity Framework Core (EF Core) 是微软推出的轻量级、可扩展、跨平台的对象关系映射(ORM)框架...",
                        AuthorId = users[0].Id,
                        //CoverImage = "https://example.com/images/efcore.jpg",
                        Status = PostStatus.Published
                    }, new Post
                    {
                        Title = "EF Core 入门教程",
                        Content = "Entity Framework Core (EF Core) 是微软推出的轻量级、可扩展、跨平台的对象关系映射(ORM)框架...",
                        AuthorId = users[0].Id,
                        //CoverImage = "https://example.com/images/efcore.jpg",
                        Status = PostStatus.Published
                    }, new Post
                    {
                        Title = "EF Core 入门教程",
                        Content = "Entity Framework Core (EF Core) 是微软推出的轻量级、可扩展、跨平台的对象关系映射(ORM)框架...",
                        AuthorId = users[0].Id,
                        //CoverImage = "https://example.com/images/efcore.jpg",
                        Status = PostStatus.Published
                    }, new Post
                    {
                        Title = "EF Core 入门教程",
                        Content = "Entity Framework Core (EF Core) 是微软推出的轻量级、可扩展、跨平台的对象关系映射(ORM)框架...",
                        AuthorId = users[0].Id,
                        //CoverImage = "https://example.com/images/efcore.jpg",
                        Status = PostStatus.Published
                    }, new Post
                    {
                        Title = "EF Core 入门教程",
                        Content = "Entity Framework Core (EF Core) 是微软推出的轻量级、可扩展、跨平台的对象关系映射(ORM)框架...",
                        AuthorId = users[0].Id,
                        //CoverImage = "https://example.com/images/efcore.jpg",
                        Status = PostStatus.Published
                    }, new Post
                    {
                        Title = "EF Core 入门教程",
                        Content = "Entity Framework Core (EF Core) 是微软推出的轻量级、可扩展、跨平台的对象关系映射(ORM)框架...",
                        AuthorId = users[0].Id,
                        //CoverImage = "https://example.com/images/efcore.jpg",
                        Status = PostStatus.Published
                    }, new Post
                    {
                        Title = "EF Core 入门教程",
                        Content = "Entity Framework Core (EF Core) 是微软推出的轻量级、可扩展、跨平台的对象关系映射(ORM)框架...",
                        AuthorId = users[0].Id,
                        //CoverImage = "https://example.com/images/efcore.jpg",
                        Status = PostStatus.Published
                    }, new Post
                    {
                        Title = "EF Core 入门教程",
                        Content = "Entity Framework Core (EF Core) 是微软推出的轻量级、可扩展、跨平台的对象关系映射(ORM)框架...",
                        AuthorId = users[0].Id,
                        //CoverImage = "https://example.com/images/efcore.jpg",
                        Status = PostStatus.Published
                    }, new Post
                    {
                        Title = "EF Core 入门教程",
                        Content = "Entity Framework Core (EF Core) 是微软推出的轻量级、可扩展、跨平台的对象关系映射(ORM)框架...",
                        AuthorId = users[0].Id,
                        //CoverImage = "https://example.com/images/efcore.jpg",
                        Status = PostStatus.Published
                    }, new Post
                    {
                        Title = "EF Core 入门教程",
                        Content = "Entity Framework Core (EF Core) 是微软推出的轻量级、可扩展、跨平台的对象关系映射(ORM)框架...",
                        AuthorId = users[0].Id,
                        //CoverImage = "https://example.com/images/efcore.jpg",
                        Status = PostStatus.Published
                    }, new Post
                    {
                        Title = "EF Core 入门教程",
                        Content = "Entity Framework Core (EF Core) 是微软推出的轻量级、可扩展、跨平台的对象关系映射(ORM)框架...",
                        AuthorId = users[0].Id,
                        //CoverImage = "https://example.com/images/efcore.jpg",
                        Status = PostStatus.Published
                    }, new Post
                    {
                        Title = "EF Core 入门教程",
                        Content = "Entity Framework Core (EF Core) 是微软推出的轻量级、可扩展、跨平台的对象关系映射(ORM)框架...",
                        AuthorId = users[0].Id,
                        //CoverImage = "https://example.com/images/efcore.jpg",
                        Status = PostStatus.Published
                    }, new Post
                    {
                        Title = "EF Core 入门教程",
                        Content = "Entity Framework Core (EF Core) 是微软推出的轻量级、可扩展、跨平台的对象关系映射(ORM)框架...",
                        AuthorId = users[0].Id,
                        //CoverImage = "https://example.com/images/efcore.jpg",
                        Status = PostStatus.Published
                    }, new Post
                    {
                        Title = "EF Core 入门教程",
                        Content = "Entity Framework Core (EF Core) 是微软推出的轻量级、可扩展、跨平台的对象关系映射(ORM)框架...",
                        AuthorId = users[0].Id,
                        //CoverImage = "https://example.com/images/efcore.jpg",
                        Status = PostStatus.Published
                    }, new Post
                    {
                        Title = "EF Core 入门教程",
                        Content = "Entity Framework Core (EF Core) 是微软推出的轻量级、可扩展、跨平台的对象关系映射(ORM)框架...",
                        AuthorId = users[0].Id,
                        //CoverImage = "https://example.com/images/efcore.jpg",
                        Status = PostStatus.Published
                    }, new Post
                    {
                        Title = "EF Core 入门教程",
                        Content = "Entity Framework Core (EF Core) 是微软推出的轻量级、可扩展、跨平台的对象关系映射(ORM)框架...",
                        AuthorId = users[0].Id,
                        //CoverImage = "https://example.com/images/efcore.jpg",
                        Status = PostStatus.Published
                    }, new Post
                    {
                        Title = "EF Core 入门教程",
                        Content = "Entity Framework Core (EF Core) 是微软推出的轻量级、可扩展、跨平台的对象关系映射(ORM)框架...",
                        AuthorId = users[0].Id,
                        //CoverImage = "https://example.com/images/efcore.jpg",
                        Status = PostStatus.Published
                    }, new Post
                    {
                        Title = "EF Core 入门教程",
                        Content = "Entity Framework Core (EF Core) 是微软推出的轻量级、可扩展、跨平台的对象关系映射(ORM)框架...",
                        AuthorId = users[0].Id,
                        //CoverImage = "https://example.com/images/efcore.jpg",
                        Status = PostStatus.Published
                    }, new Post
                    {
                        Title = "EF Core 入门教程",
                        Content = "Entity Framework Core (EF Core) 是微软推出的轻量级、可扩展、跨平台的对象关系映射(ORM)框架...",
                        AuthorId = users[0].Id,
                        //CoverImage = "https://example.com/images/efcore.jpg",
                        Status = PostStatus.Published
                    }, new Post
                    {
                        Title = "EF Core 入门教程",
                        Content = "Entity Framework Core (EF Core) 是微软推出的轻量级、可扩展、跨平台的对象关系映射(ORM)框架...",
                        AuthorId = users[0].Id,
                        //CoverImage = "https://example.com/images/efcore.jpg",
                        Status = PostStatus.Published
                    }, new Post
                    {
                        Title = "EF Core 入门教程",
                        Content = "Entity Framework Core (EF Core) 是微软推出的轻量级、可扩展、跨平台的对象关系映射(ORM)框架...",
                        AuthorId = users[0].Id,
                        //CoverImage = "https://example.com/images/efcore.jpg",
                        Status = PostStatus.Published
                    }, new Post
                    {
                        Title = "EF Core 入门教程",
                        Content = "Entity Framework Core (EF Core) 是微软推出的轻量级、可扩展、跨平台的对象关系映射(ORM)框架...",
                        AuthorId = users[0].Id,
                        //CoverImage = "https://example.com/images/efcore.jpg",
                        Status = PostStatus.Published
                    }, new Post
                    {
                        Title = "EF Core 入门教程",
                        Content = "Entity Framework Core (EF Core) 是微软推出的轻量级、可扩展、跨平台的对象关系映射(ORM)框架...",
                        AuthorId = users[0].Id,
                        //CoverImage = "https://example.com/images/efcore.jpg",
                        Status = PostStatus.Published
                    }, new Post
                    {
                        Title = "EF Core 入门教程",
                        Content = "Entity Framework Core (EF Core) 是微软推出的轻量级、可扩展、跨平台的对象关系映射(ORM)框架...",
                        AuthorId = users[0].Id,
                        //CoverImage = "https://example.com/images/efcore.jpg",
                        Status = PostStatus.Published
                    }, new Post
                    {
                        Title = "EF Core 入门教程",
                        Content = "Entity Framework Core (EF Core) 是微软推出的轻量级、可扩展、跨平台的对象关系映射(ORM)框架...",
                        AuthorId = users[0].Id,
                        //CoverImage = "https://example.com/images/efcore.jpg",
                        Status = PostStatus.Published
                    }, new Post
                    {
                        Title = "EF Core 入门教程",
                        Content = "Entity Framework Core (EF Core) 是微软推出的轻量级、可扩展、跨平台的对象关系映射(ORM)框架...",
                        AuthorId = users[0].Id,
                        //CoverImage = "https://example.com/images/efcore.jpg",
                        Status = PostStatus.Published
                    }, new Post
                    {
                        Title = "EF Core 入门教程",
                        Content = "Entity Framework Core (EF Core) 是微软推出的轻量级、可扩展、跨平台的对象关系映射(ORM)框架...",
                        AuthorId = users[0].Id,
                        //CoverImage = "https://example.com/images/efcore.jpg",
                        Status = PostStatus.Published
                    }, new Post
                    {
                        Title = "EF Core 入门教程",
                        Content = "Entity Framework Core (EF Core) 是微软推出的轻量级、可扩展、跨平台的对象关系映射(ORM)框架...",
                        AuthorId = users[0].Id,
                        //CoverImage = "https://example.com/images/efcore.jpg",
                        Status = PostStatus.Published
                    }, new Post
                    {
                        Title = "EF Core 入门教程",
                        Content = "Entity Framework Core (EF Core) 是微软推出的轻量级、可扩展、跨平台的对象关系映射(ORM)框架...",
                        AuthorId = users[0].Id,
                        //CoverImage = "https://example.com/images/efcore.jpg",
                        Status = PostStatus.Published
                    }, new Post
                    {
                        Title = "EF Core 入门教程",
                        Content = "Entity Framework Core (EF Core) 是微软推出的轻量级、可扩展、跨平台的对象关系映射(ORM)框架...",
                        AuthorId = users[0].Id,
                        //CoverImage = "https://example.com/images/efcore.jpg",
                        Status = PostStatus.Published
                    }, new Post
                    {
                        Title = "EF Core 入门教程",
                        Content = "Entity Framework Core (EF Core) 是微软推出的轻量级、可扩展、跨平台的对象关系映射(ORM)框架...",
                        AuthorId = users[0].Id,
                        //CoverImage = "https://example.com/images/efcore.jpg",
                        Status = PostStatus.Published
                    },
                    new Post
                    {
                        Title = "EF Core 入门教程",
                        Content = "Entity Framework Core (EF Core) 是微软推出的轻量级、可扩展、跨平台的对象关系映射(ORM)框架...",
                        AuthorId = users[0].Id,
                        //CoverImage = "https://example.com/images/efcore.jpg",
                        Status = PostStatus.Published
                    }
                };

                await context.Posts.AddRangeAsync(posts);
            }
        }

        private static async Task SeedCommentsAsync(ApplicationDbContext context)
        {
            var posts = await context.Posts.ToListAsync();
            var users = await context.Users.ToListAsync();
            if ((posts != null && posts.Count > 0) && (users != null && users.Count > 0))
            {
                var comments = new[]
                {
                    new Comment
                    {
                        Content = "这篇文章对EF Core的介绍很全面，对我帮助很大！",
                        PostId = posts[0].Id,
                        UserId = users[0].Id,
                        Status = CommentStatus.Approved
                    }
                };

                await context.Comments.AddRangeAsync(comments);
            }
        }

        private static async Task SeedVisitLogsAsync(ApplicationDbContext context)
        {
            var posts = await context.Posts.ToListAsync();
            if (posts != null && posts.Count > 0)
            {
                var visitLogs = new[]
                {
                    new VisitLog
                    {
                        PostId = posts[0].Id,
                        //AccessTime = SeedPublishedAt.AddDays(20).AddHours(1),
                        IPAddress = "192.168.1.100",
                        UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36",
                        Referrer = "https://www.google.com",
                        Country = "中国",
                        City = "北京"
                    }
                };

                await context.VisitLogs.AddRangeAsync(visitLogs);
            }
        }
    }
}
