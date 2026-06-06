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
                new Tag { Name = "Git", Color = "#F05032", Description = "Git版本控制" },
                new Tag { Name = "架构设计", Color = "#E74C3C", Description = "系统架构与设计模式" },
                new Tag { Name = "DDD", Color = "#8E44AD", Description = "领域驱动设计" },
                new Tag { Name = "领域驱动设计", Color = "#8E44AD", Description = "领域驱动设计理论与实践" },
                new Tag { Name = "opencode", Color = "#00B4D8", Description = "opencode AI编程助手" },
                new Tag { Name = "Kubernetes", Color = "#326CE5", Description = "Kubernetes容器编排" },
                new Tag { Name = "事件驱动", Color = "#E67E22", Description = "事件驱动架构" },
                new Tag { Name = "消息队列", Color = "#3498DB", Description = "消息队列技术" },
                new Tag { Name = "分布式事务", Color = "#C0392B", Description = "分布式事务解决方案" },
                new Tag { Name = "Saga", Color = "#C0392B", Description = "Saga事务模式" },
                new Tag { Name = "开源", Color = "#27AE60", Description = "开源项目与协作" },
                new Tag { Name = "领域建模", Color = "#2C3E50", Description = "领域建模方法" }
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
            var archCategory = categories.First(c => c.Name == "架构设计");
            var progLangCategory = categories.First(c => c.Name == "编程语言");

            var tagCsharp = tags.First(t => t.Name == "C#");
            var tagAspNet = tags.First(t => t.Name == "ASP.NET Core");
            var tagDocker = tags.First(t => t.Name == "Docker");
            var tagPerf = tags.First(t => t.Name == "性能优化");
            var tagGit = tags.First(t => t.Name == "Git");
            var tagMicroSvc = tags.First(t => t.Name == "微服务");
            var tagDDD = tags.First(t => t.Name == "DDD");
            var tagArch = tags.First(t => t.Name == "架构设计");
            var tagDddFull = tags.First(t => t.Name == "领域驱动设计");
            var tagOpencode = tags.First(t => t.Name == "opencode");
            var tagK8s = tags.First(t => t.Name == "Kubernetes");
            var tagEventDriven = tags.First(t => t.Name == "事件驱动");
            var tagMq = tags.First(t => t.Name == "消息队列");
            var tagDistTx = tags.First(t => t.Name == "分布式事务");
            var tagSaga = tags.First(t => t.Name == "Saga");
            var tagOss = tags.First(t => t.Name == "开源");
            var tagDomainModel = tags.First(t => t.Name == "领域建模");

            var baseTime = DateTime.UtcNow.AddDays(-30);

            var post1 = new Post
            {
                Title = "DDD 中聚合根与实体类的区别与划分",
                Subtitle = "深入理解领域驱动设计的核心概念",
                Content = GetPost1Content(),
                AuthorId = users[0].Id,
                Excerpt = "实体、值对象、聚合、聚合根——DDD中这四个概念如何区分？本文从实战角度梳理它们的区别与应用，附订单系统完整示例。",
                Status = PostStatus.Published,
                PublishedAt = baseTime.AddDays(-25)
            };
            post1.Categories.Add(archCategory);
            post1.Categories.Add(techCategory);
            post1.Tags.Add(tagDDD);
            post1.Tags.Add(tagArch);
            post1.Tags.Add(tagDomainModel);

            var post2 = new Post
            {
                Title = "VS Code 安装并使用 opencode 完整指南",
                Subtitle = "从零开始配置你的 AI 编程助手",
                Content = GetPost2Content(),
                AuthorId = users[1].Id,
                Excerpt = "opencode 是强大的开源 AI 编程助手。本文手把手教你完成安装、配置 API、VS Code 集成，以及 agent 模式的使用技巧。",
                Status = PostStatus.Published,
                PublishedAt = baseTime.AddDays(-20)
            };
            post2.Categories.Add(progLangCategory);
            post2.Categories.Add(shareCategory);
            post2.Tags.Add(tagOpencode);

            var post3 = new Post
            {
                Title = "ASP.NET Core 9 / .NET 9 新特性深度解析",
                Subtitle = "全面了解 .NET 9 带来的新变化",
                Content = GetPost3Content(),
                AuthorId = users[0].Id,
                Excerpt = ".NET 9 在性能、ASP.NET Core、Blazor、EF Core 等多个方面带来了重要更新。本文带你全面了解这些新特性与迁移指南。",
                Status = PostStatus.Published,
                PublishedAt = baseTime.AddDays(-16)
            };
            post3.Categories.Add(webDevCategory);
            post3.Categories.Add(techCategory);
            post3.Tags.Add(tagAspNet);
            post3.Tags.Add(tagCsharp);

            var post4 = new Post
            {
                Title = "微服务架构中的事务一致性：Saga vs 事件溯源",
                Subtitle = "两种经典方案，你选哪个？",
                Content = GetPost4Content(),
                AuthorId = users[2].Id,
                Excerpt = "Saga 模式和事件溯源是解决分布式事务的两种主流方案。本文深入比较它们的原理、代码实现和适用场景。",
                Status = PostStatus.Published,
                PublishedAt = baseTime.AddDays(-12)
            };
            post4.Categories.Add(archCategory);
            post4.Categories.Add(techCategory);
            post4.Tags.Add(tagMicroSvc);
            post4.Tags.Add(tagDistTx);
            post4.Tags.Add(tagSaga);

            var post5 = new Post
            {
                Title = "性能优化实战：ASP.NET Core 应用从 0 到 1",
                Subtitle = "一套完整的性能优化方法论",
                Content = GetPost5Content(),
                AuthorId = users[1].Id,
                Excerpt = "从数据库优化到多级缓存、从响应压缩到静态文件策略，本文用实测数据带你一步步优化 ASP.NET Core 应用的响应速度。",
                Status = PostStatus.Published,
                PublishedAt = baseTime.AddDays(-8)
            };
            post5.Categories.Add(webDevCategory);
            post5.Categories.Add(archCategory);
            post5.Tags.Add(tagPerf);
            post5.Tags.Add(tagAspNet);
            post5.Tags.Add(tagCsharp);

            var post6 = new Post
            {
                Title = "事件驱动架构入门与实践：从理论到落地",
                Subtitle = "从餐馆点餐理解事件驱动",
                Content = GetPost6Content(),
                AuthorId = users[2].Id,
                Excerpt = "事件驱动架构是微服务解耦的关键。本文用生活化的例子带你理解核心概念，并用 .NET + RabbitMQ 实现完整示例。",
                Status = PostStatus.Published,
                PublishedAt = baseTime.AddDays(-5)
            };
            post6.Categories.Add(archCategory);
            post6.Categories.Add(techCategory);
            post6.Tags.Add(tagEventDriven);
            post6.Tags.Add(tagMq);

            var post7 = new Post
            {
                Title = "从零开始：Kubernetes 部署 .NET 应用完整指南",
                Subtitle = "告别手动部署，拥抱容器编排",
                Content = GetPost7Content(),
                AuthorId = users[0].Id,
                Excerpt = "从 Dockerfile 到 K8s 清单文件，从健康检查到 HPA 自动伸缩，本文带你完整走一遍 .NET 应用的云原生部署之旅。",
                Status = PostStatus.Published,
                PublishedAt = baseTime.AddDays(-3)
            };
            post7.Categories.Add(devopsCategory);
            post7.Categories.Add(techCategory);
            post7.Tags.Add(tagK8s);
            post7.Tags.Add(tagDocker);
            post7.Tags.Add(tagAspNet);

            var post8 = new Post
            {
                Title = "领域驱动设计实战：从战略设计到战术设计",
                Subtitle = "不只是理论，更是实操指南",
                Content = GetPost8Content(),
                AuthorId = users[0].Id,
                Excerpt = "限界上下文、上下文映射、统一语言、实体、值对象、领域事件、仓储——从战略到战术全链路讲解 DDD 落地。",
                Status = PostStatus.Published,
                PublishedAt = baseTime.AddDays(-1)
            };
            post8.Categories.Add(archCategory);
            post8.Categories.Add(techCategory);
            post8.Tags.Add(tagDddFull);
            post8.Tags.Add(tagArch);
            post8.Tags.Add(tagDomainModel);

            var post9 = new Post
            {
                Title = "读《代码整洁之道》有感",
                Subtitle = "代码是写给人看的，顺便给机器执行",
                Content = GetPost9Content(),
                AuthorId = users[0].Id,
                Excerpt = "读完 Bob 大叔的《代码整洁之道》，分享关于命名、函数职责、注释态度和类设计的读书笔记与行动清单。",
                Status = PostStatus.Published,
                PublishedAt = baseTime.AddDays(1)
            };
            post9.Categories.Add(readNoteCategory);
            post9.Categories.Add(lifeCategory);

            var post10 = new Post
            {
                Title = "开源项目贡献指南：从 Issue 到 PR 的完整流程",
                Subtitle = "第一次给开源项目提 PR 该怎么操作",
                Content = GetPost10Content(),
                AuthorId = users[1].Id,
                Excerpt = "从选择项目、阅读贡献指南、搭建环境、找到 Good First Issue，到提交 PR 并通过 Code Review，一站式开源贡献教程。",
                Status = PostStatus.Published,
                PublishedAt = baseTime.AddDays(3)
            };
            post10.Categories.Add(shareCategory);
            post10.Categories.Add(techCategory);
            post10.Tags.Add(tagGit);
            post10.Tags.Add(tagOss);

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
                    Content = "聚合根的设计一直是我在 DDD 落地中最困惑的地方，文章里的订单示例很清晰，终于搞懂了！",
                    PostId = posts[0].Id,
                    UserId = users[1].Id,
                    Status = CommentStatus.Approved,
                    CreatedAt = posts[0].PublishedAt.Value.AddHours(2)
                },
                new Comment
                {
                    Content = "opencode 的 agent 模式确实好用，配合自定义规则文件可以在团队中统一代码风格。",
                    PostId = posts[1].Id,
                    UserId = users[2].Id,
                    Status = CommentStatus.Approved,
                    CreatedAt = posts[1].PublishedAt.Value.AddHours(3)
                },
                new Comment
                {
                    Content = ".NET 9 的性能提升很明显，我们升级后接口响应时间平均降低了 20%。推荐大家升级！",
                    PostId = posts[2].Id,
                    UserId = users[0].Id,
                    Status = CommentStatus.Approved,
                    CreatedAt = posts[2].PublishedAt.Value.AddDays(1)
                },
                new Comment
                {
                    Content = "事件驱动架构和消息队列结合起来威力很大，不过运维复杂度也上去了，小项目不建议上。",
                    PostId = posts[5].Id,
                    UserId = users[1].Id,
                    Status = CommentStatus.Approved,
                    CreatedAt = posts[5].PublishedAt.Value.AddHours(5)
                },
                new Comment
                {
                    Content = "K8s 的健康检查配置太重要了，之前没配导致滚动更新时服务中断，血泪教训。",
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
# DDD 中聚合根与实体类的区别与划分

领域驱动设计（DDD）中的几个核心概念常常让人困惑：实体、值对象、聚合、聚合根。本文从实战角度梳理它们的区别与应用。

## 实体（Entity）

实体有唯一标识，其属性可变。两个实体如果 ID 相同，即使其他属性不同，也视为同一个对象。

```csharp
public class User
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public Email Email { get; private set; }

    public void ChangeEmail(Email email)
    {
        Email = email;
    }
}
```

## 值对象（Value Object）

值对象没有标识，由属性值共同定义其相等性。不可变，可共享。

```csharp
public record Address
{
    public string Street { get; init; }
    public string City { get; init; }
    public string ZipCode { get; init; }
}
```

## 聚合（Aggregate）与聚合根（Aggregate Root）

聚合是一组相关对象的集合，作为数据修改的一致性边界。聚合根是聚合中唯一的入口，外部只能通过聚合根操作聚合内的对象。

## 典型示例：订单系统

```
Order（聚合根）
├── OrderId: Guid
├── CustomerId: Guid
├── OrderItems: List<OrderItem>（聚合内的实体）
│   ├── ProductId
│   ├── Quantity
│   └── Price
├── ShippingAddress: Address（值对象）
└── Status: OrderStatus（值对象）
```

```csharp
public class Order
{
    private readonly List<OrderItem> _items = new();

    public Guid Id { get; private set; }
    public Guid CustomerId { get; private set; }
    public Address ShippingAddress { get; private set; }
    public IReadOnlyList<OrderItem> Items => _items.AsReadOnly();

    public void AddItem(Product product, int quantity)
    {
        if (quantity <= 0) throw new DomainException("数量必须大于0");
        var item = new OrderItem(product.Id, product.Price, quantity);
        _items.Add(item);
    }

    public decimal GetTotal() => _items.Sum(i => i.Price * i.Quantity);
}
```

## 如何划分聚合根

### 1. 事务边界原则
一个聚合内的修改要么全部成功，要么全部回滚。

### 2. 一致性原则
聚合内部的数据必须满足业务不变量（invariants）。

### 3. 引用原则
聚合根之间通过 ID 引用，不直接持有对象引用。

## 常见误区

**误区一：把数据库表设计等同于聚合划分**  
数据库的关联关系不等于领域模型的聚合关系。

**误区二：聚合太大**  
把所有关联对象放进一个聚合，导致大量锁竞争和性能问题。

**误区三：聚合太小**  
每个实体都是一个聚合根，失去了一致性保护。

## 实践建议

1. 从业务不变量出发，而不是从数据关系出发
2. 小聚合优先，不变量要求时再扩大
3. 聚合根要确保自身的业务规则始终有效
4. 使用领域事件处理跨聚合的最终一致性

掌握聚合和聚合根的设计是 DDD 落地的关键一步。
""";

        private static string GetPost2Content() => """
# VS Code 安装并使用 opencode 完整指南

opencode 是一个强大的 AI 编程助手，支持在终端和 VS Code 中使用。本文手把手教你从零开始配置。

## 什么是 opencode

opencode 是一款开源的 AI 辅助编程工具，类似 GitHub Copilot，但更灵活、支持更多 AI 模型。它能理解代码上下文，生成代码片段、重构代码、解释代码逻辑。

## 安装步骤

### 第一步：安装 opencode CLI

```bash
# Windows（使用 winget）
winget install opencode

# macOS（使用 Homebrew）
brew install opencode

# Linux（使用 curl 安装）
curl -fsSL https://opencode.ai/install.sh | sh
```

### 第二步：配置 API 密钥

opencode 支持多种 AI 模型提供商，包括 OpenAI、Anthropic、Azure OpenAI 等：

```bash
# 设置默认的 API Key
opencode config set OPENAI_API_KEY sk-your-key-here

# 或者使用配置文件 ~/.opencode/config.json
```

### 第三步：安装 VS Code 扩展

打开 VS Code 扩展面板，搜索 "opencode" 并安装。

## 基本使用

### 终端中使用

```bash
# 用 opencode 解释代码
opencode explain "这段代码做了什么？"

# 生成代码
opencode generate "用 C# 写一个 Redis 分布式锁"

# 在聊天中交互
opencode chat
```

### VS Code 中使用

1. 选中代码，右键选择 "Ask opencode"
2. 使用 `Ctrl+I`（或 `Cmd+I`）打开内联对话
3. 在侧边栏打开 opencode Chat 面板
4. 使用 Code Actions 自动修复问题

## agent 模式

opencode 的 agent 模式是其最强大的功能。agent 可以自主执行多步骤操作：

```bash
opencode --agent "帮我优化这个项目的构建流程"
```

agent 会自动分析项目、执行命令、检查结果，直到完成任务。

## 自定义规则

通过 `.opencoderules` 文件为项目设置自定义规则：

```
# .opencoderules
- 使用 C# 10 文件作用域命名空间
- 所有公共方法都要有 XML 文档注释
- 使用 FluentAssertions 编写测试断言
```

## 常用技巧

1. **代码审查**：提供整个文件的 diff，让 opencode 审查代码质量
2. **测试生成**：让 opencode 为现有代码自动生成单元测试
3. **重构建议**：描述你想要的重构方向，opencode 执行具体修改
4. **文档生成**：从代码自动生成 README 和 API 文档

## 总结

opencode 让 AI 辅助编程变得更加可控和高效。结合 agent 模式，你几乎可以自动化任何开发任务。
""";

        private static string GetPost3Content() => """
# ASP.NET Core 9 / .NET 9 新特性深度解析

.NET 9 带来了诸多令人兴奋的新特性，本文带你全面了解这些变化。

## 性能全面提升

### 运行时优化

.NET 9 在 JIT 编译、GC 和运行时层面做了大量优化：

- **PGO（Profile-Guided Optimization）默认开启**：更智能的代码优化
- **ARM64 性能大幅提升**：在 ARM 设备上性能接近 x64
- **GC 改进**：减少暂停时间，提升吞吐量

### 新增类型

```csharp
// 只读的 Span 扩展
ReadOnlySpan<char> span = "hello".AsSpan();

// 新增的集合表达式性能优化
int[] numbers = [1, 2, 3, 4, 5];
```

## ASP.NET Core 9 新特性

### 改进的反向代理支持

```csharp
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));
```

### OpenAPI 增强

ASP.NET Core 9 提供了更完善的 OpenAPI 支持，无需额外包即可生成规范的 API 文档：

```csharp
builder.Services.AddOpenApi();

app.MapOpenApi();
```

### 中间件改进

新的基于约定的中间件注册方式更直观：

```csharp
app.MapGet("/api/health", () => Results.Ok(new { status = "healthy" }))
   .CacheOutput(policy => policy.Expire(TimeSpan.FromSeconds(30)));
```

## Blazor 新特性

Blazor 在 .NET 9 中进一步增强了交互式渲染和静态 SSR 渲染的性能，新增了对表单验证的改进。

## EF Core 9 更新

```csharp
// 新增的 ExecuteDeleteAsync 替代方案
await context.Orders
    .Where(o => o.Status == OrderStatus.Expired)
    .ExecuteDeleteAsync();

// 更高效的批量更新
await context.Orders
    .Where(o => o.Status == OrderStatus.Pending)
    .ExecuteUpdateAsync(s => s.SetProperty(o => o.Total, o => o.Subtotal * 0.9m));
```

## 迁移指南

从 .NET 8 迁移到 .NET 9 通常只需要修改目标框架：

```xml
<TargetFramework>net9.0</TargetFramework>
```

绝大多数 API 保持向后兼容。建议在实际项目开始前先在测试项目进行验证。

## 总结

.NET 9 延续了 .NET 团队在性能上的极致追求，同时也增强了开发者体验。如果你已经在使用 .NET 8，升级到 .NET 9 几乎没有成本但能获得明显的性能提升。
""";

        private static string GetPost4Content() => """
# 微服务架构中的事务一致性：Saga vs 事件溯源

分布式系统中，跨服务的事务一致性是最大的挑战之一。本文将深入比较两种主流方案：Saga 模式和事件溯源（Event Sourcing）。

## 问题背景

在微服务架构中，一个业务操作往往涉及多个服务。传统的本地事务无法跨服务边界，于是我们需要分布式事务方案。

```
下单业务流程：
订单服务 → 扣减库存 → 扣减余额 → 更新订单状态
          ↓            ↓            ↓
    库存服务      账户服务      订单服务
```

如果扣减库存成功但扣减余额失败，该怎么办？

## Saga 模式

Saga 是一种补偿性事务模式，将长事务拆分为一系列本地事务，每个本地事务都有对应的补偿操作。

### 编排式 Saga

```csharp
public class OrderSagaOrchestrator
{
    public async Task<OrderResult> PlaceOrder(OrderRequest request)
    {
        try
        {
            await _inventoryService.ReserveStock(request.ProductId, request.Quantity);
            await _accountService.DeductBalance(request.UserId, request.Total);
            await _orderService.CreateOrder(request);
            return OrderResult.Success;
        }
        catch (Exception)
        {
            // 补偿：逆序回滚
            await _accountService.RefundBalance(request.UserId, request.Total);
            await _inventoryService.ReleaseStock(request.ProductId, request.Quantity);
            return OrderResult.Failed;
        }
    }
}
```

###  choreography（ choreography）Saga

```csharp
// 订单创建后发布事件
await _eventBus.PublishAsync(new OrderCreatedEvent
{
    OrderId = order.Id,
    ProductId = productId,
    Quantity = quantity
});

// 库存服务监听事件
[EventHandler]
public async Task HandleOrderCreated(OrderCreatedEvent @event)
{
    await _inventory.DeductAsync(@event.ProductId, @event.Quantity);
    await _eventBus.PublishAsync(new InventoryDeductedEvent { ... });
}
```

## 事件溯源（Event Sourcing）

事件溯源不存储当前状态，而是存储所有状态变更事件。当前状态由事件流重放（replay）得到。

```csharp
public class BankAccount
{
    private readonly List<IEvent> _changes = new();
    private decimal _balance;

    public void Deposit(decimal amount)
    {
        Apply(new DepositedEvent { Amount = amount, Timestamp = DateTime.UtcNow });
    }

    public void Withdraw(decimal amount)
    {
        if (_balance < amount) throw new InsufficientBalanceException();
        Apply(new WithdrawnEvent { Amount = amount, Timestamp = DateTime.UtcNow });
    }

    private void Apply(IEvent @event)
    {
        switch (@event)
        {
            case DepositedEvent e: _balance += e.Amount; break;
            case WithdrawnEvent e: _balance -= e.Amount; break;
        }
        _changes.Add(@event);
    }
}
```

## 对比

| 维度 | Saga | 事件溯源 |
|------|------|---------|
| 一致性模型 | 最终一致性 | 最终一致性 |
| 数据存储 | 关系型数据库 | 事件存储（Event Store） |
| 复杂度 | 中等 | 较高 |
| 审计能力 | 需额外实现 | 天然具备（所有历史事件可查） |
| 回滚能力 | 补偿操作 | 逆向事件 |
| 学习曲线 | 平缓 | 陡峭 |

## 选择建议

- 团队经验有限、业务逻辑较直接 → **Saga**
- 需要完整审计日志、复杂业务规则 → **事件溯源**
- 两者可结合使用：Saga 协调流程 + 事件溯源记录关键状态

没有银弹，选择最适合当前团队的方案比追求最热门的方案更重要。
""";

        private static string GetPost5Content() => """
# 性能优化实战：ASP.NET Core 应用从 0 到 1

性能优化不是锦上添花，在用户等待 3 秒后就开始流失的今天，响应速度直接决定产品成败。

## 第一步：找到瓶颈

不要猜测性能问题在哪里，用工具说话。

### 使用 MiniProfiler

```csharp
// 添加 MiniProfiler
builder.Services.AddMiniProfiler(options =>
{
    options.RouteBasePath = "/profiler";
}).AddEntityFramework();
```

它会自动记录每个请求的数据库查询、执行时间和调用堆栈。

### 使用 Application Insights

对于生产环境，推荐使用 Application Insights 或 OpenTelemetry：

```csharp
builder.Services.AddOpenTelemetry()
    .WithAspNetCoreInstrumentation()
    .WithHttpClientInstrumentation()
    .WithEntityFrameworkCoreInstrumentation();
```

## 第二步：数据库优化（最见效）

### N+1 查询问题

```csharp
// ❌ N+1：查询 1 次 post + N 次 author
var posts = await context.Posts.ToListAsync();
foreach (var post in posts)
{
    Console.WriteLine(post.Author.Name);
}

// ✅ 使用 Include 预加载
var posts = await context.Posts
    .Include(p => p.Author)
    .ToListAsync();
```

### 索引优化

```csharp
modelBuilder.Entity<Post>(entity =>
{
    // 覆盖索引：包含查询所需的所有列
    entity.HasIndex(p => new { p.Status, p.PublishedAt })
          .IncludeProperties(p => new { p.Title, p.Excerpt });
});
```

### 分页

```csharp
// 使用 Keyset Pagination 替代 Offset 分页（大数据集下更高效）
var page = await context.Posts
    .Where(p => p.PublishedAt < lastSeenPublishedAt)
    .OrderByDescending(p => p.PublishedAt)
    .Take(pageSize)
    .ToListAsync();
```

## 第三步：缓存策略

### 多级缓存架构

```
请求 → 内存缓存（IMemoryCache） → Redis 缓存 → 数据库
```

```csharp
public async Task<Post> GetPostAsync(Guid id)
{
    // 一级缓存：内存
    if (_memoryCache.TryGetValue($"post:{id}", out Post post))
        return post;

    // 二级缓存：Redis
    post = await _cache.GetAsync<Post>($"post:{id}");
    if (post != null)
    {
        _memoryCache.Set($"post:{id}", post, TimeSpan.FromMinutes(5));
        return post;
    }

    // 三级：数据库
    post = await _context.Posts.FindAsync(id);
    _memoryCache.Set($"post:{id}", post, TimeSpan.FromMinutes(5));
    await _cache.SetAsync($"post:{id}", post, TimeSpan.FromHours(1));
    return post;
}
```

## 第四步：响应压缩

```csharp
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<BrotliCompressionProvider>();
    options.Providers.Add<GzipCompressionProvider>();
});
```

## 第五步：静态文件优化

```csharp
app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        // 启用浏览器缓存
        ctx.Context.Response.Headers.CacheControl =
            "public, max-age=31536000, immutable";
    }
});
```

## 优化效果

| 优化措施 | 预期效果 |
|---------|---------|
| 数据库索引 + 预加载 | 查询速度提升 10-100 倍 |
| 多级缓存 | 响应时间从 100ms 降至 1ms |
| 响应压缩 | 传输大小减少 70% |
| 静态文件缓存 | 重复访问零延迟 |

性能优化是一个持续的过程，每次优化后都要用数据验证效果。
""";

        private static string GetPost6Content() => """
# 事件驱动架构入门与实践：从理论到落地

想象一下餐馆的运行方式：顾客点餐（事件）、厨房响应（处理）、服务员上菜（后续事件）。这就是事件驱动架构的日常隐喻。

## 核心概念

### 事件（Event）

事件是"已经发生的事情"，不可变，以过去时态命名：

- `OrderSubmitted`（订单已提交）
- `PaymentCompleted`（支付已完成）
- `InventoryDepleted`（库存已耗尽）

### 命令（Command）vs 事件（Event）

```
命令：PlaceOrder（请下单）—— 期望某事发生
事件：OrderPlaced（已下单）—— 某事已经发生了
```

### 事件总线（Event Bus）

```csharp
// 发布事件
public async Task SubmitOrder(Order order)
{
    // 业务逻辑
    await _eventBus.PublishAsync(new OrderSubmittedEvent
    {
        OrderId = order.Id,
        CustomerId = order.CustomerId,
        Total = order.Total,
        Timestamp = DateTime.UtcNow
    });
}

// 订阅事件 - 库存服务
public class InventoryHandler : IEventHandler<OrderSubmittedEvent>
{
    public async Task HandleAsync(OrderSubmittedEvent @event)
    {
        var stock = await _inventory.GetStockAsync(@event.OrderId);
        if (stock.IsAvailable)
        {
            await _inventory.DeductAsync(@event.OrderId);
            await _eventBus.PublishAsync(new InventoryReservedEvent { ... });
        }
        else
        {
            await _eventBus.PublishAsync(new OutOfStockEvent { ... });
        }
    }
}
```

## 使用 .NET + RabbitMQ 实现

### 发送端

```csharp
public class RabbitMqEventBus : IEventBus
{
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public async Task PublishAsync<T>(T @event) where T : IEvent
    {
        var body = JsonSerializer.SerializeToUtf8Bytes(@event);
        var properties = _channel.CreateBasicProperties();
        properties.Persistent = true;
        properties.Type = typeof(T).Name;
        properties.MessageId = Guid.NewGuid().ToString();

        _channel.BasicPublish(
            exchange: "abcsxl.events",
            routingKey: typeof(T).Name,
            basicProperties: properties,
            body: body);
    }
}
```

### 接收端

```csharp
public class EventConsumer : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.Received += async (_, ea) =>
        {
            var eventType = ea.BasicProperties.Type;
            var body = ea.Body.ToArray();
            var @event = JsonSerializer.Deserialize(body, GetType(eventType));
            await _mediator.Publish(@event);
            _channel.BasicAck(ea.DeliveryTag, false);
        };

        _channel.BasicConsume(queue: "abcsxl.orders", autoAck: false, consumer: consumer);
    }
}
```

## 事件驱动 vs REST 的对比

| 场景 | REST | 事件驱动 |
|------|------|---------|
| 查询数据 | ✅ 直观 | ❌ 需额外实现 CQRS |
| 跨服务通信 | ❌ 紧耦合 | ✅ 松耦合 |
| 异步处理 | ❌ 请求/响应阻塞 | ✅ 天然异步 |
| 可扩展性 | 中等 | 高 |
| 可观测性 | 简单 | 需分布式追踪 |

## 什么时候用事件驱动

**适合**：
- 跨多个服务的业务流程（如订单-支付-物流）
- 需要解耦生产者和消费者
- 异步处理或需要背压的场景

**不适合**：
- 简单的 CRUD 应用
- 需要即时响应读操作
- 团队对消息中间件不熟悉

事件驱动不是银弹，但在正确的场景下威力巨大。
""";

        private static string GetPost7Content() => """
# 从零开始：Kubernetes 部署 .NET 应用完整指南

本文带你从容器化到 K8s 部署，完整走一遍 .NET 应用的云原生之旅。

## 第一步：容器化 .NET 应用

### 多阶段构建 Dockerfile

```dockerfile
# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["abcsxl.csproj", "."]
RUN dotnet restore
COPY . .
RUN dotnet publish -c Release -o /app/publish --no-restore

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
EXPOSE 8080
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "abcsxl.dll"]
```

## 第二步：Kubernetes 清单文件

### 部署（Deployment）

```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: abcsxl-web
spec:
  replicas: 3
  selector:
    matchLabels:
      app: abcsxl-web
  template:
    metadata:
      labels:
        app: abcsxl-web
    spec:
      containers:
      - name: web
        image: abcsxl/abcsxl-web:latest
        ports:
        - containerPort: 8080
        env:
        - name: ASPNETCORE_ENVIRONMENT
          value: "Production"
        - name: ConnectionStrings__DefaultConnection
          valueFrom:
            secretKeyRef:
              name: db-connection
              key: connection-string
        livenessProbe:
          httpGet:
            path: /health
            port: 8080
          initialDelaySeconds: 30
          periodSeconds: 10
        readinessProbe:
          httpGet:
            path: /health/ready
            port: 8080
          initialDelaySeconds: 5
          periodSeconds: 5
        resources:
          requests:
            memory: "128Mi"
            cpu: "250m"
          limits:
            memory: "512Mi"
            cpu: "500m"
```

### 服务（Service）

```yaml
apiVersion: v1
kind: Service
metadata:
  name: abcsxl-service
spec:
  type: ClusterIP
  selector:
    app: abcsxl-web
  ports:
  - port: 80
    targetPort: 8080
```

### 配置映射（ConfigMap）

```yaml
apiVersion: v1
kind: ConfigMap
metadata:
  name: abcsxl-config
data:
  appsettings.K8s.json: |
    {
      "Logging": {
        "LogLevel": { "Default": "Information" }
      },
      "Kestrel": {
        "Endpoints": {
          "Http": { "Url": "http://+:8080" }
        }
      }
    }
```

## 第三步：健康检查

在 Program.cs 中添加健康检查端点：

```csharp
builder.Services.AddHealthChecks()
    .AddDbContextCheck<ApplicationDbContext>();

var app = builder.Build();

app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResultStatusCodes =
    {
        [HealthStatus.Healthy] = StatusCodes.Status200OK,
        [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
    }
});

app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = _ => true
});
```

## 第四步：水平自动伸缩（HPA）

```yaml
apiVersion: autoscaling/v2
kind: HorizontalPodAutoscaler
metadata:
  name: abcsxl-hpa
spec:
  scaleTargetRef:
    apiVersion: apps/v1
    kind: Deployment
    name: abcsxl-web
  minReplicas: 2
  maxReplicas: 10
  metrics:
  - type: Resource
    resource:
      name: cpu
      target:
        type: Utilization
        averageUtilization: 70
  - type: Resource
    resource:
      name: memory
      target:
        type: Utilization
        averageUtilization: 80
```

## 部署命令

```bash
# 部署到集群
kubectl apply -f k8s/configmap.yaml
kubectl apply -f k8s/deployment.yaml
kubectl apply -f k8s/service.yaml
kubectl apply -f k8s/hpa.yaml

# 查看状态
kubectl get pods -l app=abcsxl-web -w
kubectl get hpa

# 滚动更新
kubectl set image deployment/abcsxl-web web=abcsxl/abcsxl-web:v2
```

## 总结

将 .NET 应用部署到 Kubernetes 可以获得弹性伸缩、滚动更新、自愈等云原生能力。虽然初始配置看起来繁琐，但一旦建立标准化流程，后续的部署和运维将变得无比轻松。
""";

        private static string GetPost8Content() => """
# 领域驱动设计实战：从战略设计到战术设计

DDD（领域驱动设计）不是空中楼阁，而是一套系统化的软件设计方法论。本文从战略和战术两个层面讲述如何在实际项目中落地 DDD。

## 战略设计：看清业务全貌

### 限界上下文（Bounded Context）

限界上下文是 DDD 的基石。每个上下文有独立的领域模型、独立的数据库、独立的团队。

```
电商系统的限界上下文划分：

┌─────────────┐  ┌─────────────┐  ┌─────────────┐
│   商品上下文  │  │   订单上下文  │  │   支付上下文  │
│  Product BC  │  │   Order BC  │  │  Payment BC │
│  模型：Product│  │  模型：Order │  │  模型：Payment│
└─────────────┘  └─────────────┘  └─────────────┘
```

### 上下文映射（Context Map）

```csharp
// 防腐层（Anti-Corruption Layer）：保护限界上下文不被外部模型污染
public class ProductCatalogAdapter
{
    private readonly IProductClient _client;

    public async Task<ProductDto> GetProductAsync(Guid productId)
    {
        var externalProduct = await _client.GetProductAsync(productId);
        return new ProductDto
        {
            Id = externalProduct.Id,
            Name = externalProduct.Title,
            Price = externalProduct.Amount
        };
    }
}
```

### 统一语言（Ubiquitous Language）

团队内部统一术语，代码、文档、讨论使用同一个词汇表。例如：

| 术语 | 含义 |
|------|------|
| 订单（Order） | 客户购买商品的记录 |
| 已提交（Submitted） | 订单已创建但未支付 |
| 已确认（Confirmed） | 订单已支付，开始处理 |
| 已发货（Shipped） | 商品已发出 |
| 已取消（Cancelled） | 订单被取消 |

## 战术设计：落地实现

### 实体（Entity）与值对象（Value Object）

```csharp
// 运单作为实体
public class Shipment
{
    public Guid Id { get; private set; }
    public OrderId OrderId { get; private set; }
    public TrackingNumber TrackingNumber { get; private set; }
    public Address DeliveryAddress { get; private set; }
    public ShipmentStatus Status { get; private set; }

    public void MarkAsDelivered(DateTime deliveredAt)
    {
        Status = ShipmentStatus.Delivered;
        AddDomainEvent(new ShipmentDeliveredEvent(Id, deliveredAt));
    }
}

// 地址作为值对象
public record Address
{
    public string Province { get; init; }
    public string City { get; init; }
    public string District { get; init; }
    public string Detail { get; init; }
    public string ZipCode { get; init; }
}
```

### 领域服务（Domain Service）

当业务逻辑不属于任何实体或值对象时，用领域服务表示：

```csharp
public class PricingService
{
    public decimal CalculateOrderTotal(Order order, IEnumerable<Coupon> coupons)
    {
        var subtotal = order.Items.Sum(i => i.UnitPrice * i.Quantity);
        var discount = coupons
            .Where(c => c.IsValidFor(order))
            .Select(c => c.CalculateDiscount(subtotal))
            .Max();
        return subtotal - discount;
    }
}
```

### 领域事件（Domain Event）

```csharp
public record OrderSubmittedEvent : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public Guid OrderId { get; init; }
    public Guid CustomerId { get; init; }
    public decimal Total { get; init; }
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
```

### 仓储（Repository）

```csharp
public interface IOrderRepository
{
    Task<Order> GetByIdAsync(Guid id);
    Task AddAsync(Order order);
    Task SaveAsync(Order order);
}

public class OrderRepository : IOrderRepository
{
    private readonly ApplicationDbContext _context;

    public async Task<Order> GetByIdAsync(Guid id)
    {
        return await _context.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == id);
    }
}
```

## 如何在现有项目中引入 DDD

1. **从核心域开始**：找到业务最复杂的部分，DDD 在那里最有价值
2. **不要推翻重来**：在现有系统外层包裹防腐层，逐步重构
3. **先战略后战术**：先划清业务边界，再精细化领域模型
4. **持续迭代**：领域模型不是一次完成的，随着业务理解加深不断演进

DDD 的核心价值不在于代码模式，而在于让开发者真正理解业务。技术是手段，业务是目的。
""";

        private static string GetPost9Content() => """
# 读《代码整洁之道》有感

> "代码是写给人看的，顺便给机器执行。" —— Martin Fowler

最近读完了 Bob 大叔的《代码整洁之道》，收获颇丰。这不是一本教你怎么写代码的书，而是一本教你**如何思考代码**的书。

## 关于命名

好的名称应该揭示意图。命名是最简单的重构，也是最容易被忽视的。

```csharp
// ❌ 糟糕的命名
int d; // 时间，单位天？
int ymd; // 年月日？

// ✅ 好的命名
int elapsedDays;
int registrationDate;
```

书中举了一个经典案例：一个函数叫 `getThem()`，返回 `list1`。你完全不知道它在干什么。改成 `getFlaggedCells()` 返回 `flaggedCells` 后，代码瞬间自文档化。

## 函数的职责

函数应该做一件事、尽量短小、保持一个抽象层级。**函数参数最好少于 3 个**，超过 3 个就需要考虑封装了。

```csharp
// ❌ 参数过多
public void CreateUser(string name, string email, string phone,
    string address, string city, string country, string zipCode)

// ✅ 封装成对象
public void CreateUser(UserProfile profile)
```

## 注释是无奈的选择

书中说注释是"无法补救的失败"——当你需要写注释解释代码时，说明代码本身写得不清晰。好的注释只有三种：

1. 法律信息（版权声明等）
2. 解释意图（为什么这么做，而不是做了什么）
3. 警告（潜在风险）

```csharp
// ❌ 糟糕的注释：解释"是什么"
// 循环遍历所有用户
foreach (var user in users) { ... }

// ✅ 好的注释：解释"为什么"
// 不能使用批量操作，因为每个用户可能有独立的订阅规则
foreach (var user in users) { ... }
```

## 错误处理

作者推崇使用异常而非返回错误码。错误码会导致调用者不得不写大量的 if-else 嵌套。

```csharp
// ❌ 错误码风格
if (payResult == PaymentResult.Success)
{
    if (shipResult == ShipResult.Success) { ... }
    else { /* 处理发货错误 */ }
}
else { /* 处理支付错误 */ }

// ✅ 异常风格
try
{
    ProcessPayment(order);
    ShipOrder(order);
}
catch (PaymentException ex) { /* 处理支付错误 */ }
catch (ShipmentException ex) { /* 处理发货错误 */ }
```

## 类设计

书中强调类应该短小，遵循单一职责原则。一个类如果有多个职责，就应该拆分。

```csharp
// ❌ 太胖的类
public class ReportService
{
    public Report GenerateReport() { ... }
    public void SendEmail(Report report) { ... }
    public PdfFile ConvertToPdf(Report report) { ... }
    public void ArchiveReport(Report report) { ... }
}

// ✅ 拆分
public class ReportGenerator { ... }
public class EmailService { ... }
public class PdfConverter { ... }
public class ReportArchiver { ... }
```

## 读后行动清单

读完这本书后，我给自己定了几个规则：

1. **童子军规则**：每次提交代码时，让代码比之前更整洁
2. **命名先行**：花 30 秒想一个好名字，而不是随便起一个
3. **单次重构**：每次修改一个文件，顺手做一次小重构
4. **少写注释**：用更好的代码表达意图

这本书值得每年重读一遍。推荐所有开发者都读一读。
""";

        private static string GetPost10Content() => """
# 开源项目贡献指南：从 Issue 到 PR 的完整流程

参与开源项目是提升技术能力的最佳途径之一。本文手把手教你完成第一次开源贡献。

## 第一步：找到合适的项目

### 选择标准

- 你正在使用的技术栈（更容易发现问题）
- 活跃的社区（有维护者 review PR）
- 对新手友好（标注了 good first issue 标签）

### 在 GitHub 上搜索

```
labels:good-first-issue
labels:help-wanted
```

GitHub 会自动推荐基于你兴趣的开源项目。也可以在 Awesome for Beginners 等网站寻找。

## 第二步：阅读贡献指南

每个开源项目都有 CONTRIBUTING.md 或类似的文档，**一定要先看**。通常包括：

- 代码风格要求
- 提交信息格式（如 Conventional Commits）
- PR 模板
- 开发环境搭建步骤

```bash
# fork 项目到自己的账号
# 克隆到本地
git clone https://github.com/YOUR_USERNAME/project.git
cd project

# 添加上游仓库
git remote add upstream https://github.com/ORIGINAL_OWNER/project.git
```

## 第三步：搭建开发环境

```bash
# 创建功能分支
git checkout -b fix/issue-42

# 安装依赖
npm install  # 或 dotnet restore, pip install -e .

# 运行测试确认环境正常
npm test     # 或 dotnet test, pytest
```

## 第四步：找到可以贡献的 Issue

### Good First Issue 标签

大多数项目用这个标签标记适合新手的任务，通常包括：
- 文档改进（最简单）
- 测试补充
- 小 bug 修复
- 代码重构

### 在 Issue 下留言

```
Hi! I'd like to work on this issue. Could you assign it to me?
```

这样可以避免多人同时做同一个任务。

## 第五步：编写代码

### 遵守项目规范

```bash
# 遵循代码风格
# 项目一般有 .editorconfig, .eslintrc 等配置文件
# IDE 会自动读取这些配置

# 编写测试
# 新功能要有对应的测试用例
# 修复 bug 要先写一个复现 bug 的测试
```

### 提交信息规范

```
# Conventional Commits 格式
feat: add user authentication middleware
fix: correct null reference in post details
docs: update README with new API endpoints
refactor: extract email validation to shared utility
test: add unit tests for OrderService
```

## 第六步：提交 PR

```bash
# 推送分支
git push origin fix/issue-42

# 在 GitHub 上创建 Pull Request
# 填写 PR 模板：
# - 关联的 Issue 编号
# - 修改内容描述
# - 测试结果
# - 截图（如果是 UI 变更）
```

## 第七步：Code Review

PR 创建后维护者会 review 代码，可能会要求修改：

```bash
# 处理 review 意见
git commit -m "fix: address review feedback"
git push origin fix/issue-42
```

**关键原则**：不要 rebase 已经公开的 commit，除非维护者要求。用新的 commit 回应 review 反馈。

## 第八步：PR 合并

当维护者批准后，你的代码就会被合并进主分支。恭喜，你正式成为开源贡献者了！

## 持续贡献

- 关注你贡献过的项目的 Release 和 Changelog
- 定期回来看看新的 Good First Issue
- 慢慢地从修复 bug 过渡到实现新功能
- 最终你也可能成为项目的维护者

开源不是单向的索取，而是双向的成长。每一次 PR 都是你技术履历上的一颗星。
""";
    }
}