using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Prism.Avalonia.Mobile.SourceGen;

[Generator]
public class NavigationRegistryGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var prismViews = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (node, _) =>
                    node is ClassDeclarationSyntax { AttributeLists.Count: > 0 },
                transform: static (ctx, _) => GetViewInfo(ctx))
            .Where(static info => info is not null);

        var registerCalls = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (node, _) =>
                    node is InvocationExpressionSyntax
                    {
                        Expression: MemberAccessExpressionSyntax
                        {
                            Name.Identifier.Text: "RegisterForNavigation"
                        }
                    },
                transform: static (ctx, _) => GetRegistrationInfo(ctx))
            .Where(static info => info is not null);

        var combined = prismViews.Collect().Combine(registerCalls.Collect());

        // Generate ViewRegistration metadata
        context.RegisterSourceOutput(combined, static (spc, source) =>
        {
            var code = GenerateViewRegistryCode(source.Left, source.Right);
            spc.AddSource("ViewRegistrationSource.g.cs",
                SourceText.From(code, System.Text.Encoding.UTF8));
        });

        // Generate DI registration method from [PrismView] attributes
        context.RegisterSourceOutput(prismViews.Collect(), static (spc, views) =>
        {
            var code = GenerateDiRegistrationCode(views);
            spc.AddSource("PrismViewRegistrar.g.cs",
                SourceText.From(code, System.Text.Encoding.UTF8));
        });
    }

    private static ViewInfo? GetViewInfo(GeneratorSyntaxContext ctx)
    {
        var classDecl = (ClassDeclarationSyntax)ctx.Node;
        var model = ctx.SemanticModel;
        var symbol = model.GetDeclaredSymbol(classDecl);
        if (symbol is null) return null;

        var prismAttr = symbol.GetAttributes().FirstOrDefault(a =>
            a.AttributeClass?.Name.Contains("PrismView") == true);
        if (prismAttr is null) return null;

        var navName = prismAttr.ConstructorArguments.Length > 0
            ? prismAttr.ConstructorArguments[0].Value as string
            : symbol.Name;

        var viewModelType = prismAttr.NamedArguments
            .FirstOrDefault(a => a.Key == "ViewModel").Value.Value as INamedTypeSymbol;

        var viewType = prismAttr.NamedArguments
            .FirstOrDefault(a => a.Key == "ViewType").Value.Value as int? ?? 1;

        return new ViewInfo(
            symbol.ToDisplayString(),
            navName ?? symbol.Name,
            viewModelType?.ToDisplayString(),
            viewType);
    }

    private static RegistrationInfo? GetRegistrationInfo(GeneratorSyntaxContext ctx)
    {
        var invocation = (InvocationExpressionSyntax)ctx.Node;
        var model = ctx.SemanticModel;

        if (invocation.Expression is MemberAccessExpressionSyntax memberAccess)
        {
            var typeArgs = memberAccess.Name is GenericNameSyntax genericName
                ? genericName.TypeArgumentList.Arguments
                : default;

            if (typeArgs.Count >= 1)
            {
                var viewType = model.GetTypeInfo(typeArgs[0]).Type;
                var vmType = typeArgs.Count >= 2
                    ? model.GetTypeInfo(typeArgs[1]).Type
                    : null;

                string? name = null;
                if (invocation.ArgumentList.Arguments.Count > 0)
                {
                    var firstArg = invocation.ArgumentList.Arguments[0].Expression;
                    if (firstArg is LiteralExpressionSyntax literal)
                        name = literal.Token.ValueText;
                }

                return new RegistrationInfo(
                    viewType?.ToDisplayString() ?? string.Empty,
                    vmType?.ToDisplayString(),
                    name ?? viewType?.Name ?? string.Empty);
            }
        }

        return null;
    }

    private static string GenerateViewRegistryCode(
        System.Collections.Immutable.ImmutableArray<ViewInfo?> views,
        System.Collections.Immutable.ImmutableArray<RegistrationInfo?> registrations)
    {
        var allViews = new List<ViewInfo>();
        foreach (var v in views) { if (v is { } view) allViews.Add(view); }

        foreach (var reg in registrations)
        {
            if (reg is not { } r) continue;
            if (!allViews.Exists(v => v.ViewTypeName == r.ViewTypeName) && !string.IsNullOrEmpty(r.ViewTypeName))
                allViews.Add(new ViewInfo(r.ViewTypeName, r.NavigationName, r.ViewModelTypeName, 1));
        }

        var sb = new System.Text.StringBuilder();
        sb.AppendLine("// <auto-generated />");
        sb.AppendLine("#nullable enable");
        sb.AppendLine("using Prism.Mvvm;");
        sb.AppendLine();
        sb.AppendLine("namespace Prism.Navigation;");
        sb.AppendLine();
        sb.AppendLine("internal static class ViewRegistrationSource");
        sb.AppendLine("{");
        sb.AppendLine("    public static ViewRegistration[] GetRegistrations() =>");
        sb.AppendLine("        new ViewRegistration[] {");

        foreach (var view in allViews)
        {
            var vmRef = view.ViewModelTypeName is { } vmt ? $"typeof({vmt})" : "null";
            var viewTypeEnum = view.ViewType switch
            {
                2 => "ViewType.Region", 3 => "ViewType.Dialog", _ => "ViewType.Page"
            };
            sb.AppendLine($"            new ViewRegistration {{ Type = {viewTypeEnum}, View = typeof({view.ViewTypeName}), ViewModel = {vmRef}, Name = \"{view.NavigationName}\" }},");
        }

        sb.AppendLine("        };");
        sb.AppendLine("}");
        return sb.ToString();
    }

    /// <summary>
    /// Generates RegisterPrismViews(this IContainerRegistry) that performs
    /// full DI registration for all [PrismView]-attributed classes.
    /// </summary>
    private static string GenerateDiRegistrationCode(
        System.Collections.Immutable.ImmutableArray<ViewInfo?> views)
    {
        var sb = new System.Text.StringBuilder();
        sb.AppendLine("// <auto-generated />");
        sb.AppendLine("#nullable enable");
        sb.AppendLine("using Prism.Ioc;");
        sb.AppendLine("using Prism.Mvvm;");
        sb.AppendLine("using Prism.Navigation;");
        sb.AppendLine();
        sb.AppendLine("namespace Prism;");
        sb.AppendLine();
        sb.AppendLine("/// <summary>");
        sb.AppendLine("/// Registers all [PrismView]-attributed views with the DI container.");
        sb.AppendLine("/// Call in RegisterTypes(). Works together with manual cr.RegisterForNavigation calls.");
        sb.AppendLine("/// </summary>");
        sb.AppendLine("public static class PrismViewRegistrar");
        sb.AppendLine("{");
        sb.AppendLine("    public static void RegisterPrismViews(this IContainerRegistry cr)");
        sb.AppendLine("    {");

        foreach (var item in views)
        {
            if (item is not { } v) continue;
            var name = v.NavigationName;
            var viewType = v.ViewTypeName;
            var vmType = v.ViewModelTypeName;

            sb.AppendLine($"        // {name}");
            sb.AppendLine($"        cr.Register(typeof(object), typeof({viewType}), \"{name}\");");
            sb.AppendLine($"        cr.Register(typeof({viewType}), typeof({viewType}));");

            if (vmType is not null)
            {
                sb.AppendLine($"        cr.Register(typeof({vmType}), typeof({vmType}));");
                sb.AppendLine($"        ViewModelLocationProvider.Register(typeof({viewType}).ToString(), typeof({vmType}));");
            }

            // Also add to PendingRegistrations for NavigationRegistry
            var viewTypeEnum = v.ViewType switch
            {
                2 => "ViewType.Region", 3 => "ViewType.Dialog", _ => "ViewType.Page"
            };
            sb.AppendLine($"        IContainerRegistryExtensions.PendingRegistrations.Add(new ViewRegistration {{ Type = {viewTypeEnum}, View = typeof({viewType}), ViewModel = {(vmType is not null ? $"typeof({vmType})" : "null")}, Name = \"{name}\" }});");
            sb.AppendLine();
        }

        sb.AppendLine("    }");
        sb.AppendLine("}");
        return sb.ToString();
    }

    private sealed class ViewInfo
    {
        public string ViewTypeName { get; }
        public string NavigationName { get; }
        public string? ViewModelTypeName { get; }
        public int ViewType { get; }

        public ViewInfo(string viewTypeName, string navigationName, string? viewModelTypeName, int viewType)
        {
            ViewTypeName = viewTypeName; NavigationName = navigationName;
            ViewModelTypeName = viewModelTypeName; ViewType = viewType;
        }
    }

    private sealed class RegistrationInfo
    {
        public string ViewTypeName { get; }
        public string? ViewModelTypeName { get; }
        public string NavigationName { get; }

        public RegistrationInfo(string viewTypeName, string? viewModelTypeName, string navigationName)
        {
            ViewTypeName = viewTypeName; ViewModelTypeName = viewModelTypeName;
            NavigationName = navigationName;
        }
    }
}
