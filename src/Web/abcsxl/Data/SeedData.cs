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

            await context.Database.EnsureCreatedAsync();

            if (await context.Users.AnyAsync())
            {
                return;
            }

            Console.WriteLine("开始生成种子数据...");

            try
            {
                await SeedUsersAsync(context);
                await context.SaveChangesAsync();

                await SeedCategoriesAsync(context);
                await SeedTagsAsync(context);
                await context.SaveChangesAsync();

                await SeedPostsAsync(context);
                await context.SaveChangesAsync();

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
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                    DisplayName = "管理员",
                    Bio = "全栈开发者，专注.NET技术栈，偶尔写点博客记录踩坑经历。",
                    Role = UserRole.Admin,
                    IsActive = true
                },
                new User
                {
                    Username = "author1",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Author@123"),
                    DisplayName = "代码流浪者",
                    Bio = "热爱开源，喜欢折腾各种新技术，分享即学习。",
                    Role = UserRole.Author,
                    IsActive = true
                },
                new User
                {
                    Username = "author2",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Author@456"),
                    DisplayName = "数据库小学生",
                    Bio = "DBA一枚，专注于数据库优化和数据架构设计。",
                    Role = UserRole.Author,
                    IsActive = true
                }
            };

            await context.Users.AddRangeAsync(users);
        }

        private static async Task SeedCategoriesAsync(ApplicationDbContext context)
        {
            var tech = new Category
            {
                Name = "技术",
                Description = "编程、开发、运维相关的技术文章",
                Order = 1
            };

            var life = new Category
            {
                Name = "生活",
                Description = "生活感悟、读书笔记、旅行见闻",
                Order = 2
            };

            var share = new Category
            {
                Name = "分享",
                Description = "工具推荐、资源整理、经验分享",
                Order = 3
            };

            await context.Categories.AddRangeAsync(tech, life, share);
            await context.SaveChangesAsync();

            var techChildren = new[]
            {
                new Category { Name = "编程语言", Description = "C#、Python、JavaScript等编程语言", ParentId = tech.Id, Order = 1 },
                new Category { Name = "Web开发", Description = "前端后端Web开发技术", ParentId = tech.Id, Order = 2 },
                new Category { Name = "数据库", Description = "关系型数据库、NoSQL、数据缓存", ParentId = tech.Id, Order = 3 },
                new Category { Name = "DevOps", Description = "Docker、K8s、CI/CD、自动化部署", ParentId = tech.Id, Order = 4 },
                new Category { Name = "架构设计", Description = "微服务、分布式、系统设计", ParentId = tech.Id, Order = 5 }
            };

            var lifeChildren = new[]
            {
                new Category { Name = "读书笔记", Description = "读书感悟、书评推荐", ParentId = life.Id, Order = 1 },
                new Category { Name = "随想录", Description = "生活感悟、碎碎念", ParentId = life.Id, Order = 2 }
            };

            await context.Categories.AddRangeAsync(techChildren);
            await context.Categories.AddRangeAsync(lifeChildren);
        }

        private static async Task SeedTagsAsync(ApplicationDbContext context)
        {
            var tags = new[]
            {
                new Tag { Name = "C#", Color = "#178600", Description = "C#编程语言" },
                new Tag { Name = "ASP.NET Core", Color = "#512BD4", Description = "ASP.NET Core框架" },
                new Tag { Name = "EF Core", Color = "#512BD4", Description = "Entity Framework Core" },
                new Tag { Name = "Docker", Color = "#2496ED", Description = "Docker容器技术" },
                new Tag { Name = "PostgreSQL", Color = "#336791", Description = "PostgreSQL数据库" },
                new Tag { Name = "Redis", Color = "#DC382D", Description = "Redis缓存" },
                new Tag { Name = "Vue.js", Color = "#4FC08D", Description = "Vue.js前端框架" },
                new Tag { Name = "微服务", Color = "#FF6B6B", Description = "微服务架构" },
                new Tag { Name = "API设计", Color = "#FFA500", Description = "API设计与规范" },
                new Tag { Name = "性能优化", Color = "#9B59B6", Description = "性能调优" },
                new Tag { Name = "Linux", Color = "#FCC624", Description = "Linux系统" },
                new Tag { Name = "Git", Color = "#F05032", Description = "Git版本控制" }
            };

            await context.Tags.AddRangeAsync(tags);
        }

        private static async Task SeedPostsAsync(ApplicationDbContext context)
        {
            var users = await context.Users.ToListAsync();
            var categories = await context.Categories.ToListAsync();
            var tags = await context.Tags.ToListAsync();

            var techCategory = categories.First(c => c.Name == "技术");
            var webDevCategory = categories.First(c => c.Name == "Web开发");
            var dbCategory = categories.First(c => c.Name == "数据库");
            var devopsCategory = categories.First(c => c.Name == "DevOps");
            var lifeCategory = categories.First(c => c.Name == "生活");
            var readNoteCategory = categories.First(c => c.Name == "读书笔记");
            var shareCategory = categories.First(c => c.Name == "分享");

            var tagCsharp = tags.First(t => t.Name == "C#");
            var tagAspNet = tags.First(t => t.Name == "ASP.NET Core");
            var tagEfCore = tags.First(t => t.Name == "EF Core");
            var tagDocker = tags.First(t => t.Name == "Docker");
            var tagPostgres = tags.First(t => t.Name == "PostgreSQL");
            var tagRedis = tags.First(t => t.Name == "Redis");
            var tagApi = tags.First(t => t.Name == "API设计");
            var tagLinux = tags.First(t => t.Name == "Linux");

            var baseTime = DateTime.UtcNow.AddDays(-30);

            var post1 = new Post
            {
                Title = "ASP.NET Core 8.0 新特性详解",
                Subtitle = "一起看看.NET 8带来了哪些令人兴奋的新功能",
                Content = GetPost1Content(),
                AuthorId = users[0].Id,
                Excerpt = "ASP.NET Core 8.0 引入了许多令人兴奋的新功能，包括 Native AOT、Blazor 的新组件模型、性能优化等。本文带你深入了解这些新特性。",
                Status = PostStatus.Published,
                PublishedAt = baseTime.AddDays(-20)
            };
            post1.Categories.Add(webDevCategory);
            post1.Categories.Add(techCategory);
            post1.Tags.Add(tagAspNet);
            post1.Tags.Add(tagCsharp);

            var post2 = new Post
            {
                Title = "Entity Framework Core 8 高级查询技巧",
                Subtitle = "掌握EF Core查询的精髓",
                Content = GetPost2Content(),
                AuthorId = users[0].Id,
                Excerpt = "本文介绍EF Core中的高级查询技巧，包括分页、预加载、拆分查询等，让你的数据访问层更加高效。",
                Status = PostStatus.Published,
                PublishedAt = baseTime.AddDays(-15)
            };
            post2.Categories.Add(dbCategory);
            post2.Categories.Add(techCategory);
            post2.Tags.Add(tagEfCore);
            post2.Tags.Add(tagCsharp);

            var post3 = new Post
            {
                Title = "Docker Compose 部署 ASP.NET Core 应用",
                Subtitle = "从零搭建生产可用的容器化部署方案",
                Content = GetPost3Content(),
                AuthorId = users[1].Id,
                Excerpt = "详细讲解如何使用Docker Compose部署ASP.NET Core应用，包含Nginx反向代理、HTTPS配置等生产环境必备配置。",
                Status = PostStatus.Published,
                PublishedAt = baseTime.AddDays(-10)
            };
            post3.Categories.Add(devopsCategory);
            post3.Categories.Add(techCategory);
            post3.Tags.Add(tagDocker);
            post3.Tags.Add(tagAspNet);
            post3.Tags.Add(tagLinux);

            var post4 = new Post
            {
                Title = "PostgreSQL 16 新特性一览",
                Subtitle = "数据库又双叒叕更新了",
                Content = GetPost4Content(),
                AuthorId = users[2].Id,
                Excerpt = "PostgreSQL 16带来了许多新特性，包括性能提升、新增函数、对LLVM的支持等，一起来看看吧。",
                Status = PostStatus.Published,
                PublishedAt = baseTime.AddDays(-8)
            };
            post4.Categories.Add(dbCategory);
            post4.Categories.Add(techCategory);
            post4.Tags.Add(tagPostgres);

            var post5 = new Post
            {
                Title = "RESTful API 设计最佳实践",
                Subtitle = "如何设计一套优雅的REST接口",
                Content = GetPost5Content(),
                AuthorId = users[1].Id,
                Excerpt = "API设计是���端开发的重要一环，本文总结了我多年实践中的RESTful API设计经验与最佳实践。",
                Status = PostStatus.Published,
                PublishedAt = baseTime.AddDays(-5)
            };
            post5.Categories.Add(webDevCategory);
            post5.Categories.Add(techCategory);
            post5.Categories.Add(shareCategory);
            post5.Tags.Add(tagApi);
            post5.Tags.Add(tagAspNet);

            var post6 = new Post
            {
                Title = "使用 Redis 实现分布式锁",
                Subtitle = "解决并发场景下的资源竞争问题",
                Content = GetPost6Content(),
                AuthorId = users[2].Id,
                Excerpt = "分布式锁是分布式系统中的重要组件，本文介绍如何使用Redis实现可靠的分布式锁，并讨论常见的坑与最佳实践。",
                Status = PostStatus.Published,
                PublishedAt = baseTime.AddDays(-3)
            };
            post6.Categories.Add(dbCategory);
            post6.Categories.Add(techCategory);
            post6.Tags.Add(tagRedis);
            post6.Tags.Add(tagCsharp);

            var post7 = new Post
            {
                Title = "读《代码整洁之道》有感",
                Subtitle = "代码是写给人看的，顺便给机器执行",
                Content = GetPost7Content(),
                AuthorId = users[0].Id,
                Excerpt = "读完Bob大叔的《代码整洁之道》，对代码质量有了更深的理解，分享一些读书笔记和感悟。",
                Status = PostStatus.Published,
                PublishedAt = baseTime.AddDays(-1)
            };
            post7.Categories.Add(readNoteCategory);
            post7.Categories.Add(lifeCategory);

            var post8 = new Post
            {
                Title = "我的2024年技术回顾",
                Subtitle = "年终总结与新年计划",
                Content = GetPost8Content(),
                AuthorId = users[0].Id,
                Excerpt = "2024年就要过去了，这一年在技术上有了一些成长，也踩了不少坑，来做个总结吧。",
                Status = PostStatus.Published,
                PublishedAt = baseTime
            };
            post8.Categories.Add(lifeCategory);

            var post9 = new Post
            {
                Title = "搭建个人博客的技术选型",
                Subtitle = "为什么我最终选择了这个方案",
                Content = GetPost9Content(),
                AuthorId = users[1].Id,
                Excerpt = "从WordPress到Hexo再到现在的自建方案，分享我搭建博客的技术选型思考。",
                Status = PostStatus.Published,
                PublishedAt = baseTime.AddDays(2)
            };
            post9.Categories.Add(shareCategory);
            post9.Categories.Add(techCategory);
            post9.Tags.Add(tagAspNet);
            post9.Tags.Add(tagDocker);

            var post10 = new Post
            {
                Title = "Git 工作流实践指南",
                Subtitle = "团队协作中的版本控制策略",
                Content = GetPost10Content(),
                AuthorId = users[2].Id,
                Excerpt = "介绍几种常见的Git工作流，以及如何在团队中选择合适的分支策略。",
                Status = PostStatus.Published,
                PublishedAt = baseTime.AddDays(4)
            };
            post10.Categories.Add(shareCategory);
            post10.Categories.Add(techCategory);
            post10.Tags.Add(tagCsharp);

            var postDraft = new Post
            {
                Title = "还在写草稿中的文章...",
                Content = "这篇文章还在起草中，尚未完成。",
                AuthorId = users[0].Id,
                Status = PostStatus.Draft,
                Categories = { techCategory }
            };

            var posts = new[] { post1, post2, post3, post4, post5, post6, post7, post8, post9, post10, postDraft };

            foreach (var post in posts)
            {
                post.CreatedAt = post.PublishedAt ?? DateTime.UtcNow;
                post.UpdatedAt = post.CreatedAt;
            }

            await context.Posts.AddRangeAsync(posts);
        }

        private static async Task SeedCommentsAsync(ApplicationDbContext context)
        {
            var posts = await context.Posts.Where(p => p.Status == PostStatus.Published).ToListAsync();
            var users = await context.Users.ToListAsync();

            if (posts.Count == 0 || users.Count == 0) return;

            var comments = new List<Comment>
            {
                new Comment
                {
                    Content = "写得真好！终于有人把 ASP.NET Core 8 的新特性讲清楚了，之前看官方文档看得云里雾里的。",
                    PostId = posts[0].Id,
                    UserId = users[1].Id,
                    Status = CommentStatus.Approved,
                    CreatedAt = posts[0].PublishedAt.Value.AddHours(2)
                },
                new Comment
                {
                    Content = "支持楼主，希望能多写一些 EF Core 相关的教程，这块资料太少了。",
                    PostId = posts[1].Id,
                    UserId = users[2].Id,
                    Status = CommentStatus.Approved,
                    CreatedAt = posts[1].PublishedAt.Value.AddHours(3)
                },
                new Comment
                {
                    Content = "Docker Compose 那篇文章很实用，我按着步骤操作成功部署了，感谢！",
                    PostId = posts[2].Id,
                    UserId = users[0].Id,
                    Status = CommentStatus.Approved,
                    CreatedAt = posts[2].PublishedAt.Value.AddDays(1)
                },
                new Comment
                {
                    Content = "分布式锁那段讲得不错，不过我感觉 RedLock 可能更可靠一些？",
                    PostId = posts[5].Id,
                    UserId = users[1].Id,
                    Status = CommentStatus.Approved,
                    CreatedAt = posts[5].PublishedAt.Value.AddHours(5)
                },
                new Comment
                {
                    Content = "这本书我也读过，确实收获很大，尤其是关于命名和注释的章节。",
                    PostId = posts[6].Id,
                    UserId = users[2].Id,
                    Status = CommentStatus.Approved,
                    CreatedAt = posts[6].PublishedAt.Value.AddHours(1)
                }
            };

            await context.Comments.AddRangeAsync(comments);
        }

        private static async Task SeedVisitLogsAsync(ApplicationDbContext context)
        {
            var posts = await context.Posts.Where(p => p.Status == PostStatus.Published).ToListAsync();

            var userAgents = new[]
            {
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 Chrome/120.0.0.0 Safari/537.36",
                "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36 Safari/537.36",
                "Mozilla/5.0 (iPhone; CPU iPhone OS 17_2 like Mac OS X) AppleWebKit/605.1.15 Mobile Safari/604.1"
            };

            var referrers = new[] { "https://www.google.com", "https://www.bing.com", "https://www.baidu.com", "" };
            var countries = new[] { ("中国", "北京"), ("中国", "上海"), ("中国", "深圳"), ("美国", "加州") };

            var random = new Random(42);
            var logs = new List<VisitLog>();

            foreach (var post in posts.Take(5))
            {
                var visitCount = random.Next(10, 100);
                for (int i = 0; i < visitCount; i++)
                {
                    logs.Add(new VisitLog
                    {
                        PostId = post.Id,
                        AccessTime = post.PublishedAt.Value.AddMinutes(random.Next(1, 43200)),
                        IPAddress = $"{(random.Next(1, 255))}.{(random.Next(1, 255))}.{(random.Next(1, 255))}.{(random.Next(1, 255))}",
                        UserAgent = userAgents[random.Next(userAgents.Length)],
                        Referrer = referrers[random.Next(referrers.Length)],
                        Country = countries[random.Next(countries.Length)].Item1,
                        City = countries[random.Next(countries.Length)].Item2
                    });
                }
            }

            await context.VisitLogs.AddRangeAsync(logs);
        }

        private static string GetPost1Content() => """
# ASP.NET Core 8.0 新特性详解

ASP.NET Core 8.0 带来了许多令人兴奋的新功能，让我们的开发体验更加顺畅。

## Native AOT 支持

Native AOT (Ahead-of-Time) 编译可以显著提升应用启动速度和减少内存占用。

```csharp
// 项目文件配置
<PropertyGroup>
    <PublishAot>true</PublishAot>
</PropertyGroup>
```

## Blazor 的新组件模型

Blazor 在 8.0 中引入了新的组件模型，支持更灵活的组件组合：

```razor
@* 新的组件使用方式 *@
<MyComponent @bind-Value="myValue" />
```

## 性能优化

8.0 版本对核心框架进行了大量性能优化，包括：

- HTTP/3 支持
- Kestrel 性能提升
- 更好的内存管理

## 总结

ASP.NET Core 8.0 是一个重要的里程碑版本，推荐大家尽快升级体验。
""";

        private static string GetPost2Content() => """
# Entity Framework Core 8 高级查询技巧

EF Core 是 .NET 生态中最流行的 ORM，本文介绍一些高级查询技巧。

## 分页查询

使用 `Skip` 和 `Take` 实现高效分页：

```csharp
var page = await context.Posts
    .Where(p => p.Status == PostStatus.Published)
    .OrderByDescending(p => p.PublishedAt)
    .Skip((pageNumber - 1) * pageSize)
    .Take(pageSize)
    .ToListAsync();
```

## 预加载与筛选

使用 `ThenInclude` 进行多层预加载：

```csharp
var posts = await context.Posts
    .Include(p => p.Author)
    .Include(p => p.Comments.Where(c => c.Status == CommentStatus.Approved))
    .ThenInclude(c => c.User)
    .ToListAsync();
```

## 拆分查询

对于复杂的 JOIN 查询，使用 `AsSplitQuery()` 避免 Cartesian Product：

```csharp
var posts = await context.Posts
    .Include(p => p.Tags)
    .AsSplitQuery()
    .ToListAsync();
```

## 全局查询过滤器

结合软删除实现自动过滤：

```csharp
modelBuilder.Entity<Post>().HasQueryFilter(p => !p.IsDeleted);
```

这些技巧能帮助你构建更高效的数据访问层。
""";

        private static string GetPost3Content() => """
# Docker Compose 部署 ASP.NET Core 应用

本文介绍如何用 Docker Compose 部署一个生产级别的 ASP.NET Core 应用。

## 项目结构

```
project/
├── docker-compose.yml
├── docker-compose.prod.yml
├── nginx/
│   └── nginx.conf
├── app/
│   ├── Dockerfile
│   └── ...
```

## docker-compose.yml

```yaml
version: '3.8'

services:
  web:
    build:
      context: ./app
      dockerfile: Dockerfile
    ports:
      - "5000:5000"
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
    restart: unless-stopped

  nginx:
    image: nginx:alpine
    ports:
      - "80:80"
      - "443:443"
    volumes:
      - ./nginx/nginx.conf:/etc/nginx/nginx.conf:ro
      - ./nginx/certs:/etc/nginx/certs:ro
    depends_on:
      - web
    restart: unless-stopped

  db:
    image: postgres:16
    environment:
      POSTGRES_DB: mydb
      POSTGRES_USER: user
      POSTGRES_PASSWORD: password
    volumes:
      - pgdata:/var/lib/postgresql/data
    restart: unless-stopped

volumes:
  pgdata:
```

## Nginx 配置

```nginx
server {
    listen 80;
    server_name example.com;
    return 301 https://$server_name$request_uri;
}

server {
    listen 443 ssl http2;
    server_name example.com;

    ssl_certificate /etc/nginx/certs/server.crt;
    ssl_certificate_key /etc/nginx/certs/server.key;

    location / {
        proxy_pass http://web:5000;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection keep-alive;
        proxy_set_header Host $host;
        proxy_cache_bypass $http_upgrade;
    }
}
```

## 部署命令

```bash
# 开发环境
docker-compose up -d

# 生产环境
docker-compose -f docker-compose.yml -f docker-compose.prod.yml up -d
```

这样就完成了一个带 Nginx 反向代理和数据库的生产环境部署！
""";

        private static string GetPost4Content() => """
# PostgreSQL 16 新特性一览

PostgreSQL 16 已经发布，带来了一系列令人兴奋的新特性。

## 性能提升

- 基于 LLVM 的 JIT 编译优化
- 更好的并行查询优化
- 索引扫描性能提升

## 新增函数

### 字符串函数增强

```sql
-- 新增的字符串函数
SELECT string_to_array('a,b,c', ',') AS arr;
-- 返回: {a,b,c}
```

### 正则表达式增强

支持更多正则表达式选项，简化复杂模式匹配。

## 对 COPY 的改进

```sql
-- COPY NOW 支持指定格式
COPY my_table FROM '/path/to/file.csv' WITH (FORMAT csv, HEADER true);
```

## 总结

PostgreSQL 16 继续保持其作为最先进开源数据库的地位，值得升级体验。
""";

        private static string GetPost5Content() => """
# RESTful API 设计最佳实践

好的 API 设计能让开发者爱不释手，差的 API 设计则是��梦的开始。

## URL 设计原则

### 使用名词而非动词

```
✅ GET /users
✅ GET /users/123
❌ GET /getUsers
❌ GET /getUserById/123
```

### 层级结构要合理

```
✅ GET /users/123/posts
✅ GET /users/123/posts/456
❌ GET /posts?userId=123
```

## HTTP 状态码

| 状态码 | 含义 |
|--------|------|
| 200 | 成功 |
| 201 | 创建成功 |
| 204 | 无内容成功 |
| 400 | 请求参数错误 |
| 401 | 未认证 |
| 403 | 无权限 |
| 404 | 资源不存在 |
| 500 | 服务器错误 |

## 分页与排序

```json
GET /posts?page=1&pageSize=20&sort=createdAt&order=desc

{
    "data": [...],
    "pagination": {
        "page": 1,
        "pageSize": 20,
        "total": 100,
        "totalPages": 5
    }
}
```

## 错误响应格式

```json
{
    "error": {
        "code": "VALIDATION_ERROR",
        "message": "请求参数验证失败",
        "details": [
            {"field": "email", "message": "邮箱格式不正确"}
        ]
    }
}
```

## 版本控制

推荐使用 URL 路径进行版本控制：

```
/api/v1/users
/api/v2/users
```

遵循这些原则，你就能设计出一套优雅、易用的 RESTful API。
""";

        private static string GetPost6Content() => """
# 使用 Redis 实现分布式锁

分布式锁是分布式系统中解决资源竞争问题的利器。

## 为什么需要分布式锁

在单机环境下，我们可以使用 `lock` 关键字。但在分布式环境下，多个服务实例可能同时竞争同一个资源，这时候就需要分布式锁。

## 基础实现

使用 SETNX (SET if Not eXists)：

```csharp
public class RedisLock : IDisposable
{
    private readonly IDatabase _database;
    private readonly string _key;
    private readonly string _value;
    private bool _disposed;

    public RedisLock(IDatabase database, string key)
    {
        _database = database;
        _key = key;
        _value = Guid.NewGuid().ToString();
    }

    public async Task<bool> AcquireAsync(TimeSpan expiry)
    {
        return await _database.StringSetAsync(
            _key, _value, expiry, When.NotExists);
    }

    public async Task ReleaseAsync()
    {
        var script = @"
            if redis.call('get', KEYS[1]) == ARGV[1] then
                return redis.call('del', KEYS[1])
            else
                return 0
            end";

        await _database.ScriptEvaluateAsync(script, new[] { _key }, new[] { _value });
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            ReleaseAsync().GetAwaiter().GetResult();
            _disposed = true;
        }
    }
}
```

## 使用示例

```csharp
using (var lock = new RedisLock(database, $"product:{productId}"))
{
    if (await lock.AcquireAsync(TimeSpan.FromSeconds(10)))
    {
        // 执行业务逻辑
        await ProcessOrderAsync(orderId);
    }
    else
    {
        throw new Exception("获取锁失败，请重试");
    }
}
```

## 注意事项

1. **锁的过期时间要设置合理**：太短可能导致锁自动释放，业务还未完成
2. **使用唯一值作为锁的值**：确保只有持有锁的进程才能释放
3. **考虑 RedLock 算法**：对于高可用场景，使用 RedLock 更可靠

分布式锁是分布式开发中的重要工具，熟练掌握能帮你解决很多棘手问题。
""";

        private static string GetPost7Content() => """
# 读《代码整洁之道》有感

> "代码是写给人看的，顺便给机器执行。" —— Martin Fowler

最近读完了 Bob 大叔的《代码整洁之道》，收获颇丰。

## 关于命名

好的名称应该：
- 揭示意图
- 避免误导
- 有意义的区分
- 可读的

```csharp
// ❌ 糟糕的命名
int d; // 时间，单位天？
int ymd; // 年月日？

// ✅ 好的命名
int elapsedDays;
int registrationDate;
```

## 函数的职责

函数应该：
- 做一件事
- 尽量短小
- 一个抽象层级

函数参数最好少于 3 个，超过 3 个就需要考虑封装了。

## 注释

注释是"无法补救的失败"——当你需要写注释解释代码时，说明代码本身写得不清晰。

好的注释：
- 法律信息
- 解释意图
- 警告

## 格式

代码格式关乎可读性和可维护性：
- 垂直格式：相关代码靠近
- 横向格式：留白表达关联

这本书让我重新审视了自己的代码风格。推荐所有开发者都读一读。
""";

        private static string GetPost8Content() => """
# 我的2024年技术回顾

2024年就要过去了，来做个年终总结吧。

## 技术成长

今年在以下几个方面有了明显进步：

### 微服务架构

从年初对微服务的模糊认知，到能独立设计和实现简单的微服务系统。使用的技术栈包括：
- Docker 容器化
- Consul 服务发现
- Ocelot API 网关

### 数据库优化

通过实际项目的踩坑，对 PostgreSQL 的性能调优有了实战经验：
- 索引设计
- 查询优化
- 分表分库策略

### 团队协作

开始在团队中承担更多的技术方案设计和 code review 工作。

## 踩过的坑

### 第一个坑：缓存穿透

上线初期遇到大量缓存穿透导致数据库被打爆，后来加了布隆过滤器和空值缓存才解决。

### 第二个坑：分布式事务

业务拆分后遇到了分布式事务的问题，最终用 Saga 模式解决了。

## 2025年计划

- 系统学习云原生技术
- 深入研究消息队列
- 输出更多技术文章

技术之路，道阻且长，与君共勉。
""";

        private static string GetPost9Content() => """
# 搭建个人博客的技术选型

从 WordPress 到 Hexo 再到现在，聊聊我的博客技术选型。

## 方案对比

| 方案 | 优点 | 缺点 |
|------|------|------|
| WordPress | 功能完善、生态丰富 | 笨重、需要维护数据库 |
| Hexo | 静态、快速 | 动态内容支持差 |
| 自建博客 | 完全可控 | 需要开发维护 |

## 我的选择

最终选择了 ASP.NET Core + SQLite + 静态页面渲染的方案。

### 为什么是 ASP.NET Core

- 熟悉 .NET 技术栈
- 部署在阿里云函数计算上，成本极低
- 可以随时扩展功能

### 为什么是 SQLite

- 无需额外部署数据库服务
- 对于个人博客来说性能足够
- 方便备份和迁移

## 关键特性

- Markdown 写作，Vditor 编辑器
- 自动生成摘要
- SEO 友好
- 响应式设计

如果你也在考虑搭建个人博客，希望这篇文章能给你一些参考。
""";

        private static string GetPost10Content() => """
# Git 工作流实践指南

团队协作中，选择合适的 Git 工作流至关重要。

## 常见工作流

### 1. Git Flow

适合有固定发布周期的项目：

- `main`: 主分支，只读
- `develop`: 开发分支
- `feature/*`: 功能分支
- `release/*`: 发布分支
- `hotfix/*`: 热修复分支

### 2. GitHub Flow

适合持续发布的项目：

- `main`: 唯一的长期分支
- 通过 PR 合并所有变更
- 合并后立即部署

### 3. Trunk-Based Development

团队技术能力强时推荐：

- 所有开发者频繁提交到 main
- 使用特性开关控制未完成功能
- 强调持续集成

## 我的建议

小团队推荐 GitHub Flow，流程简单清晰。

```bash
# 1. 从 main 创建功能分支
git checkout -b feature/my-feature

# 2. 开发并提交
git commit -m "feat: add new feature"

# 3. 推送并创建 PR
git push -u origin feature/my-feature

# 4. Code Review 后合并
```

选择工作流不是目的，团队协作效率才是。祝大家编码愉快！
""";
    }
}