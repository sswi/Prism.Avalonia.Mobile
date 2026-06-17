# Prism.Avalonia.Mobile

MAUI-aligned page navigation framework for **Avalonia 12+**, designed for mobile-first, AOT-compatible cross-platform applications.

---

## 架构概述

```
┌──────────────────────────────────────────────────────┐
│  INavigationService         页面级导航               │
│  NavigateAsync / GoBack / GoBackTo / SelectTab       │
│                                                      │
│  ┌────────────────────────────────────────────────┐  │
│  │  NavigationPage (Avalonia 12)                  │  │
│  │  ┌──────┐  ┌──────┐  ┌──────┐               │  │
│  │  │ Page │  │ Page │  │ Page │   Push/Pop     │  │
│  │  └──────┘  └──────┘  └──────┘               │  │
│  │  ┌──────────────────────────────────────────┐│  │
│  │  │  IRegionManager  区域导航                 ││  │
│  │  │  ContentControl / ItemsControl Regions   ││  │
│  │  └──────────────────────────────────────────┘│  │
│  └────────────────────────────────────────────────┘  │
└──────────────────────────────────────────────────────┘
```

双导航系统：
- **`INavigationService`** — 页面级导航 (Push/Pop/Modal)，用于页面跳转
- **`IRegionManager`** — 区域导航，用于页面内部的组合视图切换

---

## 快速开始

### 1. 安装

```bash
dotnet add package Prism.Avalonia.Mobile
dotnet add package Prism.DryIoc.Avalonia.Mobile
```

或直接引用项目：

```xml
<ProjectReference Include="..\..\..\src\Prism.Avalonia.Mobile\Prism.Avalonia.Mobile.csproj" />
<ProjectReference Include="..\..\..\src\Prism.DryIoc.Avalonia.Mobile\Prism.DryIoc.Avalonia.Mobile.csproj" />
```

### 2. 创建 App

```csharp
// App.axaml.cs
using Avalonia.Controls;
using Prism.DryIoc;
using Prism.Ioc;

public partial class App : PrismApplication
{
    protected override AvaloniaObject CreateShell()
    {
        return new NavigationPage
        {
            Content = Container.Resolve<MainPage>()
        };
    }

    protected override void RegisterTypes(IContainerRegistry cr)
    {
        // 方式 A: 属性标注，一行注册所有 [PrismView] 视图
        cr.RegisterPrismViews();

        // 方式 B: 代码注册（与 nameof 配合，编译期安全）
        // cr.RegisterForNavigation<MainPage, MainViewModel>();       // → "MainPage"
        // cr.RegisterForNavigation<DetailPage, DetailViewModel>();   // → "DetailPage"
    }
}
```

### 3. 导航

```csharp
public class MainViewModel
{
    private readonly INavigationService _nav;
    
    public MainViewModel(INavigationService navigationService)
    {
        _nav = navigationService;
    }

    public async Task GoToDetail()
    {
        // URI 导航
        await _nav.NavigateAsync("DetailPage?id=5&name=hello");

        // nameof + 参数 (编译期安全，推荐)
        await _nav.NavigateAsync(nameof(DetailPage),
            new NavigationParameters { { "id", 5 }, { "name", "hello" } });

        // Builder API
        await _nav.CreateBuilder()
            .AddSegment(nameof(DetailPage), s => s.AddParameter("id", 5))
            .NavigateAsync();
    }
}
```

---

## 功能一览

| 功能 | API | 说明 |
|------|-----|------|
| 页面导航 | `NavigateAsync("PageName?param=value")` | URI 分段导航，支持参数 |
| GoBack | `GoBackAsync()` | 回退上一页 |
| GoBackTo | `GoBackToAsync("PageName")` | 回退到指定页面 |
| GoBackToRoot | `GoBackToRootAsync()` | 回退到导航栈根 |
| Tab 选择 | `SelectTabAsync("TabName")` | 编程切换 Tab |
| Builder | `CreateBuilder().AddSegment().NavigateAsync()` | 流式 API 构建导航 |
| 生命周期 | `INavigatedAware` / `IInitialize` / `IConfirmNavigation` | 页面导航生命周期 |
| 页面行为 | `IPageBehaviorFactory` | 页面作用域/激活感知/生命周期 |
| 区域导航 | `IRegionManager.RequestNavigate("RegionName", uri)` | ContentControl/ItemsControl 切换 |
| 对话框 | `IDialogService.ShowDialog` / `ShowInline` / `ShowWindow` | 内嵌模态 / 外部窗口 |
| 模块化 | `IModuleManager` / `IModuleCatalog` | 模块化加载 |
| AOT | 显式注册 + 源生成器 | 支持 Native AOT 编译 |

---

## 对话框

```csharp
// 内嵌弹出 (NavigationPage.PushModalAsync) — 移动端推荐
_dialogService.ShowInline("DemoDialog", parameters,
    new DialogCallback().OnClose(result =>
    {
        if (result.Result == ButtonResult.OK)
        {
            var input = result.Parameters.GetValue<string>("userInput");
        }
    }));

// 外部窗口 — 仅桌面端
_dialogService.ShowWindow("DemoDialog", parameters, callback);

// 自动选择 (移动端=内嵌, 桌面端=窗口)
_dialogService.ShowDialog("DemoDialog", parameters, callback);

// 配置遮罩点击关闭
var p = new DialogParameters
{
    { KnownDialogParameters.CloseOnBackdropTap, false }
};
```

### 对话框 ViewModel

```csharp
public class DemoViewModel : BindableBase, IDialogAware
{
    private IDialogCloser? _closer;
    public DelegateCommand CloseCommand { get; }
    public DelegateCommand CancelCommand { get; }

    public DemoViewModel()
    {
        CloseCommand = new(() => _closer?.Close(new DialogResult
        {
            Result = ButtonResult.OK,
            Parameters = new DialogParameters { { "userInput", Input } }
        }));
        CancelCommand = new(() => _closer?.Close(new DialogResult
        {
            Result = ButtonResult.Cancel
        }));
    }

    public void OnDialogOpened(IDialogParameters p)
    {
        p.TryGetValue<IDialogCloser>(KnownDialogParameters.DialogCloser, out _closer);
    }
}
```

---

## Region 区域导航

```xml
<!-- XAML -->
<ContentControl prism:RegionManager.RegionName="ContentRegion" />
```

```csharp
// 注册区域视图
cr.RegisterForNavigation<DashboardView, DashboardViewModel>("DashboardView");

// 导航到区域
_regionManager.RequestNavigate("ContentRegion", 
    new Uri("DashboardView", UriKind.Relative));
```

---

## AOT 配置

### 1. 推荐方式：`[PrismView]` 属性标注 + `RegisterPrismViews()`

**标注视图：**

```csharp
using Prism;
using Prism.Mvvm;

// Page — 默认 ViewType.Page
[PrismView("MainPage", ViewModel = typeof(MainViewModel))]
public partial class MainPage : ContentPage { }

// Region
[PrismView("DashboardView", ViewModel = typeof(DashboardViewModel), ViewType = ViewType.Region)]
public partial class DashboardView : UserControl { }

// Dialog
[PrismView("DemoDialog", ViewModel = typeof(DemoViewModel), ViewType = ViewType.Dialog)]
public partial class DemoDialog : UserControl, IDialogAware { }
```

**一行注册：**

```csharp
protected override void RegisterTypes(IContainerRegistry cr)
{
    cr.RegisterPrismViews(); // SourceGen 生成，自动注册所有 [PrismView] 视图
}
```

### 2. 备选方式：`RegisterForNavigation` 代码注册

偏好显式控制的用户可以用传统方式，与 `nameof` 配合实现编译期安全：

```csharp
protected override void RegisterTypes(IContainerRegistry cr)
{
    cr.RegisterForNavigation<MainPage, MainViewModel>();       // name: "MainPage"
    cr.RegisterForNavigation<DetailPage, DetailViewModel>();   // name: "DetailPage"
}

// 导航时
_nav.NavigateAsync(nameof(DetailPage), parameters); // 编译期安全
```

### 3. 混合使用

两种方式可共存，不冲突：

```csharp
protected override void RegisterTypes(IContainerRegistry cr)
{
    cr.RegisterPrismViews();                                    // 属性标注的
    cr.RegisterForNavigation<SettingsPage>("SettingsPage");     // 代码注册的
}
```

**等效对比：**

| 注册方式 | 代码量 | AOT 安全 | 编译期检查 |
|---------|--------|---------|-----------|
| `[PrismView]` + `RegisterPrismViews()` | 属性 + 一行调用 | ✅ | ✅ SourceGen |
| `RegisterForNavigation<T>()` | 每个 View 一行 | ✅ | ✅ 泛型 |
| 混合 | 灵活搭配 | ✅ | ✅ |

### 4. `[PrismView]` 属性参数

| 参数 | 类型 | 默认 | 说明 |
|------|------|------|------|
| `navigationName` | `string?` | 类名 | 导航名称，对应 `NavigateAsync` 的 URI |
| `ViewModel` | `Type?` | `null` | ViewModel 类型，SourceGen 自动生成映射 |
| `ViewType` | `ViewType` | `Page` | 视图类型 |

### 5. `ViewType` 枚举

| 值 | 导航方式 | 典型控件 |
|----|---------|---------|
| `Page` | `INavigationService` | `ContentPage`, `NavigationPage` |
| `Region` | `IRegionManager` | `UserControl` → `ContentControl` / `ItemsControl` |
| `Dialog` | `IDialogService` | `UserControl` → `IDialogAware` |

### 6. 源生成器编译流程

```
编译时                                  运行时
───────                                ──────
[PrismView] 属性                        |
    ↓                                  |
NavigationRegistryGenerator 解析        |
    ├── PrismViewRegistrar.g.cs        |   cr.RegisterPrismViews() 调用
    │    (DI 注册代码) ───────────────────→ 注册 View + ViewModel 到容器
    │                                   |
    └── ViewRegistrationSource.g.cs    |   NavigationRegistry 消费
         (元数据工厂) ───────────────────→ CreateView() 零反射查找
```

### 7. DryIoc 容器 AOT 规则

```csharp
protected override Rules CreateContainerRules()
{
    return Rules.Default
        .WithConcreteTypeDynamicRegistrations((_, _) => false)
        .WithAutoConcreteTypeResolution(false);
}
```

### 8. 已被禁用的反射路径

| 反射 API | 替代方案 |
|---------|---------|
| `Type.GetType(name)` | `[PrismView]` + SourceGen |
| `Activator.CreateInstance(type)` | DI 容器 `Resolve(type)` |
| `Assembly.GetTypes()` 扫描 | SourceGen 编译时 |
| 约定命名 `Views.Xxx` → `ViewModels.Xxx` | `ViewModel` 参数显式指定 |

---

## 返回拦截 (Back Navigation Guard)

### 程序化拦截：`IConfirmNavigation`

在 ViewModel 中实现 `IConfirmNavigation`，`CanNavigate` 返回 `false` 即可阻止所有后退操作。

```csharp
public class EditViewModel : BindableBase, IConfirmNavigation
{
    private bool _hasUnsavedChanges;

    /// <summary>
    /// 在任何后退操作之前调用（GoBack/GoBackTo/GoBackToRoot/系统返回键/侧滑）
    /// 返回 false 阻止导航。
    /// </summary>
    public bool CanNavigate(INavigationParameters parameters)
    {
        if (_hasUnsavedChanges)
            return false;  // 阻止后退
        return true;
    }
}
```

**被拦截的后退操作：**

| 操作 | 是否拦截 |
|------|---------|
| `_nav.GoBackAsync()` | ✅ |
| `_nav.GoBackToAsync("Page")` | ✅ |
| `_nav.GoBackToRootAsync()` | ✅ |
| Android 系统返回键 | ✅ |
| 侧滑返回手势 | ⚠️ 框架捕获事件，但物理手势本身无法取消 |

### 物理手势拦截：禁用侧滑

侧滑手势由 Avalonia 12 原生处理，`IConfirmNavigation` 在 Pop 事件后触发。要阻止物理手势本身，需禁用侧滑：

```xml
<!-- XAML: 单页禁用 -->
<ContentPage NavigationPage.IsGestureEnabled="False" />

<!-- XAML: 全局禁用 -->
<NavigationPage IsGestureEnabled="False" />
```

```csharp
// 代码: 根据状态动态切换
NavigationPage.SetIsGestureEnabled(this, !_hasUnsavedChanges);
```

### 完整示例：编辑页面（Demo 中可运行）

```
Demo 操作：
  主页 → "Back Guard" → "Open Edit Page"
  → 勾选 "I have made changes"
  → 点 "Go Back" / 按返回键 / 侧滑
  → 被阻止，显示 "Back blocked — you have unsaved changes"
  → 取消勾选 → 后退恢复正常
```

```csharp
// View
[PrismView("EditPage", ViewModel = typeof(EditViewModel))]
public partial class EditPage : ContentPage { ... }

// ViewModel
public class EditViewModel : BindableBase, IConfirmNavigation
{
    private bool _hasChanges;

    public bool CanNavigate(INavigationParameters parameters)
        => !_hasChanges; // true = 允许, false = 阻止

    public async Task TryGoBack()
    {
        var result = await _nav.GoBackAsync();
        if (!result.Success)
            Console.WriteLine("Back was blocked!");
    }
}
```

### 两种拦截时机

```
程序化返回 (_nav.GoBackAsync)
  → CanNavigateAsync() 检查 IConfirmNavigation
    → false: 抛出 NavigationException，导航取消
    → true:  正常后退

物理手势 / 系统返回键
  → Avalonia 原生手势识别 → Popped 事件
  → PageSystemBackBehavior 捕获 Popped
  → CanNavigateAsync() 检查
    → false: 返回 NavigationException，但页面已经 Pop 了一半
    → 建议: 在手势场景下配合 IsGestureEnabled=false 使用
```

---

## Demo 项目

```
samples/SampleApp/
├── SampleApp/             共享库 (XAML Views + ViewModels)
├── SampleApp.Desktop/     Windows / macOS / Linux
├── SampleApp.Android/     Android
├── SampleApp.iOS/         iOS
└── SampleApp.Browser/     WebAssembly
```

运行：

```bash
# Desktop
dotnet run --project samples/SampleApp/SampleApp.Desktop

# Android (需 workload)
dotnet build -t:Run -f net10.0-android samples/SampleApp/SampleApp.Android
```

---

## 平台支持

| 平台 | 状态 |
|------|------|
| Windows | ✅ Desktop |
| macOS | ✅ Desktop |
| Linux | ✅ Desktop |
| Android | ✅ |
| iOS | ✅ |
| Browser (WASM) | ✅ |

---

## 依赖

| NuGet | 说明 |
|-------|------|
| [Prism.Core](https://www.nuget.org/packages/Prism.Core) | 核心接口/抽象 |
| [Prism.Container.DryIoc](https://www.nuget.org/packages/Prism.Container.DryIoc) | DryIoc DI 容器 |

不使用 Prism.Core 源码——全部通过 NuGet 引用。

---

## 与 Prism MAUI 的区别

| 特性 | Prism MAUI | Prism.Avalonia.Mobile |
|------|-----------|----------------------|
| Shell | ✅ | ❌ (Avalonia 无对应) |
| FlyoutPage | ✅ | `DrawerPage` (API 不同) |
| NavigationPage | ✅ | ✅ |
| TabbedPage | ✅ | ⚠️ (API 差异) |
| INavigationService | ✅ | ✅ 100% 对齐 |
| Region | ✅ | ✅ |
| Dialog | ✅ | ✅ |
| AOT | ❌ | ✅ |
