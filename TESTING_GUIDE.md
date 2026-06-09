# 测试编写指南

> 本指南说明如何在 Coast.Api 项目中编写测试用例。

---

## 1. 测试框架

| 组件 | 框架 | 用途 |
|------|------|------|
| 测试运行器 | xUnit | 测试框架 |
| 断言库 | Shouldly | 断言验证 |
| 验证库 | FluentValidation | 输入验证 |
| Mock | Moq / NSubstitute | 模拟对象 |
| 测试基类 | `TestBase` | 测试基础设施 |

---

## 2. 测试项目结构

```
src.tests/
├── Coast.Api.UnitTests/           # 单元测试
│   ├── Handlers/
│   │   └── LoginHandlerTests.cs
│   ├── TestBase.cs              # 测试基类
│   └── TestEnvironmentCache.cs # 环境缓存
├── Coast.Api.IntegrationTests/    # 集成测试
│   ├── Handlers/
│   ├── TestBase.cs
│   └── TestEnvironmentCache.cs
└── Coast.Api.UnitTests.csproj
```

---

## 3. Handler 内置测试

### 3.1 测试结构

每个 Handler 都应包含 `Test` 方法，定义内置测试用例：

```csharp
public void Test(TestContext<LoginCommand, LoginResponse> context)
{
    // 标记是否需要数据库
    context.NoDatabase = true; // 或 false
    
    // 创建测试用例
    var testCase = context.CreateTestCase();
    
    // 配置 Mock（可选）
    testCase.Build = builder =>
    {
        // 注册 Mock 服务
        builder.RegisterType<MockUserService>().As<IUserService>();
    };
    
    // 设置测试数据
    testCase.Message = new LoginCommand
    {
        Username = "admin",
        Password = "Admin@123"
    };
    
    // Arrange：准备测试数据
    testCase.Arrange = async () =>
    {
        //准备数据库数据等
        await Task.CompletedTask;
    };
    
    // Act：执行测试
    testCase.Act = async (mediator, ct) =>
    {
        return await mediator.SendAsync<LoginCommand, LoginResponse>(
            testCase.Message, ct);
    };
    
    // Assert：验证结果
    testCase.Assert = result =>
    {
        result.Exception.ShouldBeNull();
        result.Response.AccessToken.ShouldNotBeNullOrWhiteSpace();
        result.Response.RefreshToken.ShouldNotBeNullOrWhiteSpace();
        return Task.CompletedTask;
    };
}
```

### 3.2 测试类型

#### 3.2.1 无数据库测试
```csharp
context.NoDatabase = true;
```

#### 3.2.2 有数据库测试
```csharp
context.NoDatabase = false;
// 需要在 Arrange 中准备数据
```

#### 3.2.3 异常测试
```csharp
var errorTestCase = context.CreateTestCase();
errorTestCase.Message = new LoginCommand
{
    Username = "invalid"
};
errorTestCase.Assert = result =>
{
    result.Exception.ShouldNotBeNull();
    return Task.CompletedTask;
};
```

---

## 4. 单元测试文件

### 4.1 单元测试模板

```csharp
using Coast.Api.UnitTests;
using Shouldly;
using Xunit;

namespace Coast.Api.UnitTests.Handlers;

public class LoginHandlerTests : TestBase
{
    [Fact]
    public async Task Login_WithValidCredentials_ShouldReturnTokens()
    {
        // Arrange
        var command = new LoginCommand
        {
            Username = "admin",
            Password = "Admin@123"
        };
        
        // Act
        var response = await Mediator.SendAsync<LoginCommand, LoginResponse>(command);
        
        // Assert
        response.AccessToken.ShouldNotBeNullOrWhiteSpace();
        response.RefreshToken.ShouldNotBeNullOrWhiteSpace();
    }
    
    [Fact]
    public async Task Login_WithInvalidUsername_ShouldThrowException()
    {
        // Arrange
        var command = new LoginCommand
        {
            Username = "",
            Password = "password"
        };
        
        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() => 
            Mediator.SendAsync<LoginCommand, LoginResponse>(command));
    }
}
```

### 4.2 测试命名规范

| 模式 | 示例 | 说明 |
|------|------|------|
| `{Method}_With_{Condition}_Should_{Expected}` | `Login_WithValidCredentials_ShouldReturnTokens` | 标准命名 |
| `{Method}_Should_{Expected}` | `GetUser_ShouldReturnUser` | 简单场景 |
| `{Method}_WithInvalid_{Field}_Should_{Expected}` | `Login_WithInvalidUsername_ShouldThrow` | 异常场景 |

---

## 5. 断言示例（Shouldly）

```csharp
// 基本断言
result.ShouldNotBeNull();
result.ShouldBeNull();
result.ShouldBe(expected);
result.ShouldNotBe(expected);

// 数值断言
count.ShouldBeGreaterThan(0);
count.ShouldBeLessThanOrEqualTo(100);
progress.ShouldBeInRange(0, 100);

// 字符串断言
name.ShouldNotBeNullOrWhiteSpace();
email.ShouldContain("@");
code.ShouldStartWith("REQ-");
code.ShouldEndWith(".md");

// 集合断言
items.ShouldContain(x => x.Name == "Test");
items.ShouldHaveSingleItem();
items.ShouldBeEmpty();

// 异常断言
await action.ShouldThrowAsync<NotFoundException>();
await action.ShouldNotThrowAsync<Exception>();

// 类型断言
obj.ShouldBeOfType<User>();
result.ShouldBeAssignableFrom<IEnumerable<User>>();
```

---

## 6. 集成测试

### 6.1 集成测试模板

```csharp
using Coast.Api.IntegrationTests;
using Shouldly;
using Xunit;

namespace Coast.Api.IntegrationTests.Api;

public class UserControllerTests : TestBase
{
    [Fact]
    public async Task Login_WithValidCredentials_ShouldReturn200()
    {
        // Arrange
        var command = new LoginCommand
        {
            Username = "admin",
            Password = "Admin@123"
        };
        
        // Act
        var response = await Mediator.SendAsync<LoginCommand, LoginResponse>(command);
        
        // Assert
        response.AccessToken.ShouldNotBeNullOrWhiteSpace();
    }
}
```

### 6.2 测试环境配置

```csharp
// TestEnvironmentCache.cs
public static class TestEnvironmentCache
{
    public static ILifetimeScope? LifetimeScope { get; set; }
}
```

---

## 7. 测试数据准备

### 7.1 使用 Fixture

```csharp
public class TestDataFixture
{
    public ApplicationUser AdminUser { get; } = new ApplicationUser
    {
        Id = Guid.NewGuid(),
        Username = "admin",
        Name = "管理员"
    };
    
    public Requirement SampleRequirement { get; } = new Requirement
    {
        Id = 1,
        Name = "测试需求",
        Code = "REQ-TEST-001",
        Status = RequirementStatus.Pending
    };
}
```

### 7.2 实现 IClassFixture

```csharp
public class UserHandlerTests : IClassFixture<TestDataFixture>
{
    private readonly TestDataFixture _fixture;
    
    public UserHandlerTests(TestDataFixture fixture)
    {
        _fixture = fixture;
    }
}
```

---

## 8. Mock 使用

### 8.1 Moq 示例

```csharp
using Moq;

public void Test_WithMockService()
{
    // Arrange
    var mockUserService = new Mock<IUserService>();
    mockUserService.Setup(x => x.GetByUsernameAsync(It.IsAny<string>()))
        .ReturnsAsync(new ApplicationUser { Username = "test" });
    
    var testCase = Context.CreateTestCase();
    testCase.Build = builder =>
    {
        builder.RegisterInstance(mockUserService.Object);
    };
    
    // Act & Assert
    // ...
}
```

### 8.2 NSubstitute 示例

```csharp
using NSubstitute;

public void Test_WithSubstitute()
{
    // Arrange
    var userService = Substitute.For<IUserService>();
    userService.GetByUsernameAsync("test").Returns(
        new ApplicationUser { Username = "test" });
    
    var testCase = Context.CreateTestCase();
    testCase.Build = builder =>
    {
        builder.RegisterInstance(userService);
    };
    
    // Act & Assert
    // ...
}
```

---

## 9. 测试覆盖率目标

| 类型 | 目标覆盖率 |
|------|-----------|
| Handler | ≥80% |
| 验证器 | ≥ 90% |
| 控制器 | ≥ 70% |
| 整体 | ≥ 60% |

---

## 10. 常见测试场景

### 10.1 正常流程测试
```csharp
[Fact]
public async Task CreateRequirement_WithValidData_ShouldCreateSuccessfully()
{
    // Arrange
    var command = new CreateRequirementCommand { /* 有效数据 */ };
    
    // Act
    var result = await Mediator.SendAsync<CreateRequirementCommand, CreateRequirementResponse>(command);
    
    // Assert
    result.Id.ShouldBeGreaterThan(0);
}
```

### 10.2 验证失败测试
```csharp
[Fact]
public async Task CreateRequirement_WithEmptyName_ShouldFailValidation()
{
    // Arrange
    var command = new CreateRequirementCommand { Name = "" };
    
    // Act & Assert
    var ex = await Should.ThrowAsync<ValidationException>(
        () => Mediator.SendAsync<CreateRequirementCommand>(command));
    
    ex.Errors.ShouldContain(e => e.PropertyName == "Name");
}
```

### 10.3 并发测试
```csharp
[Fact]
public async Task UpdateRequirement_WithConcurrentEdit_ShouldHandleConflict()
{
    // Arrange
    var requirement = await CreateTestRequirement();
    var v1 = await GetRequirement(requirement.Id);
    var v2 = await GetRequirement(requirement.Id);
    
    // Act
    v1.Progress = 50;
    await Mediator.SendAsync(new UpdateRequirementCommand(v1));
    
    v2.Progress = 80;
    var ex = await Should.ThrowAsync<ConcurrencyException>(
        () => Mediator.SendAsync(new UpdateRequirementCommand(v2)));
    
    // Assert
    ex.Message.ShouldContain("已被他人修改");
}
```

---

## 11. 运行测试

```bash
# 运行所有测试
dotnet test

# 运行特定项目
dotnet test src.tests/Coast.Api.UnitTests

# 运行特定测试类
dotnet test --filter "FullyQualifiedName~LoginHandlerTests"

# 带覆盖率
dotnet test --collect:"XPlat Code Coverage"
```

---

**维护者**: AI Coding Team  
**版本**: v1.0  
**最后更新**: 2026-06-10