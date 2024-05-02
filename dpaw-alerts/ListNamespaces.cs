using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;

public class Program
{
    public static void Main()
    {
        var workspace = MSBuildWorkspace.Create();
        var projectPath = "~/dpaw-alerts/dpaw-alerts.csproj"; // replace with your project path
        var project = workspace.OpenProjectAsync(projectPath).Result;
        var compilation = project.GetCompilationAsync().Result;

        var namespaces = compilation.SyntaxTrees
            .SelectMany(syntaxTree => syntaxTree.GetRoot().DescendantNodes()
            .OfType<Microsoft.CodeAnalysis.CSharp.Syntax.NamespaceDeclarationSyntax>()
            .Select(namespaceSyntax => namespaceSyntax.Name.ToString()))
            .Distinct()
            .OrderBy(namespaceName => namespaceName);

        foreach (var namespaceName in namespaces)
        {
            Console.WriteLine(namespaceName);
        }
    }
}