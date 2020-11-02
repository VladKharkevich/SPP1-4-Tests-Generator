using System;
using System.Collections.Generic;
using System.IO.Enumeration;
using System.Text;
using GeneratorTestClassesLib;
using Tests_Generator;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;

namespace Tests_Generator.Tests
{
    [TestClass]
    public class TestGeneratorTests
    {
        string filename;
        List<string> codes;
        List<CompilationUnitSyntax> roots;

        [TestInitialize]
        public void Initialize()
        {
            roots = new List<CompilationUnitSyntax>();
            filename = "SuperPuperMegaModule.cs";
            Reader fileReader = new Reader();
            string data = fileReader.ReadFromFile(filename).Result;
            codes = GeneratorTestClasses.Start(data);
            foreach (string code in codes)
            {
                SyntaxTree tree = SyntaxFactory.ParseSyntaxTree(code);
                roots.Add(tree.GetCompilationUnitRoot());
            }
        }

        [TestMethod]
        public void TestCountOfFilesAndClasses()
        {
            codes.Count.Should().Be(3);
        }

        [TestMethod]
        public void TestMSTestUsing()
        {
            foreach (var root in roots)
            {
                bool isConnect = false;
                foreach (var mstestUsing in root.Usings)
                {
                    if (mstestUsing.ToString().Contains("Microsoft.VisualStudio.TestTools.UnitTesting"))
                        isConnect = true;
                }
                isConnect.Should().BeTrue();
            }
        }

        [TestMethod]
        public void TestNamespaces()
        {
            foreach (var root in roots)
            {
                NamespaceDeclarationSyntax namespaceDeclarationSyntax = (NamespaceDeclarationSyntax)root.Members[0];
                ClassDeclarationSyntax classDeclarationSyntax = (ClassDeclarationSyntax)namespaceDeclarationSyntax.Members[0];
                if (classDeclarationSyntax.Identifier.ValueText.Contains("SuperPuperMegaModule"))
                {
                    namespaceDeclarationSyntax.Name.ToString().Should().Be("Shit.Tests");
                }
                else if (classDeclarationSyntax.Identifier.ValueText.Contains("AnotherOne") ||
                         classDeclarationSyntax.Identifier.ValueText.Contains("AnotherSecond"))
                {
                    namespaceDeclarationSyntax.Name.ToString().Should().Be("Tests_Generator.Tests");
                }
            }
        }

        [TestMethod]
        public void TestClassModificator()
        {
            foreach (var root in roots)
            {
                NamespaceDeclarationSyntax namespaceDeclarationSyntax = (NamespaceDeclarationSyntax)root.Members[0];
                ClassDeclarationSyntax classDeclarationSyntax = (ClassDeclarationSyntax)namespaceDeclarationSyntax.Members[0];
                classDeclarationSyntax.Modifiers.Any((modifier) => modifier.IsKind(SyntaxKind.PublicKeyword)).Should().BeTrue();
            }
        }

        [TestMethod]
        public void TestClassAttribute()
        {
            foreach (var root in roots)
            {
                NamespaceDeclarationSyntax namespaceDeclarationSyntax = (NamespaceDeclarationSyntax)root.Members[0];
                ClassDeclarationSyntax classDeclarationSyntax = (ClassDeclarationSyntax)namespaceDeclarationSyntax.Members[0];
                classDeclarationSyntax.AttributeLists.Count.Should().Be(1);
                AttributeListSyntax attributeArgumentSyntax = classDeclarationSyntax.AttributeLists[0];
                attributeArgumentSyntax.ToFullString().Trim().Should().Be("[TestClass]");
            }
        }

        [TestMethod]
        public void TestMethodModificator()
        {
            foreach (var root in roots)
            {
                NamespaceDeclarationSyntax namespaceDeclarationSyntax = (NamespaceDeclarationSyntax)root.Members[0];
                ClassDeclarationSyntax classDeclarationSyntax = (ClassDeclarationSyntax)namespaceDeclarationSyntax.Members[0];
                MethodDeclarationSyntax methodDeclarationSyntax = (MethodDeclarationSyntax)classDeclarationSyntax.Members[0];
                methodDeclarationSyntax.Modifiers.Any((modifier) => modifier.IsKind(SyntaxKind.PublicKeyword)).Should().BeTrue();
            }
        }

        [TestMethod]
        public void TestMethodAttribute()
        {
            foreach (var root in roots)
            {
                NamespaceDeclarationSyntax namespaceDeclarationSyntax = (NamespaceDeclarationSyntax)root.Members[0];
                ClassDeclarationSyntax classDeclarationSyntax = (ClassDeclarationSyntax)namespaceDeclarationSyntax.Members[0];
                MethodDeclarationSyntax methodDeclarationSyntax = (MethodDeclarationSyntax)classDeclarationSyntax.Members[0];
                methodDeclarationSyntax.AttributeLists.Count.Should().Be(1);
                AttributeListSyntax attributeArgumentSyntax = methodDeclarationSyntax.AttributeLists[0];
                attributeArgumentSyntax.ToFullString().Trim().Should().Be("[TestMethod]");
            }
        }

        [TestMethod]
        public void TestMethodReturnType()
        {
            foreach (var root in roots)
            {
                NamespaceDeclarationSyntax namespaceDeclarationSyntax = (NamespaceDeclarationSyntax)root.Members[0];
                ClassDeclarationSyntax classDeclarationSyntax = (ClassDeclarationSyntax)namespaceDeclarationSyntax.Members[0];
                MethodDeclarationSyntax methodDeclarationSyntax = (MethodDeclarationSyntax)classDeclarationSyntax.Members[0];
                methodDeclarationSyntax.ReturnType.ToString().Should().Be("void");
            }
        }

        [TestMethod]
        public void TestMethodCode()
        {
            foreach (var root in roots)
            {
                NamespaceDeclarationSyntax namespaceDeclarationSyntax = (NamespaceDeclarationSyntax)root.Members[0];
                ClassDeclarationSyntax classDeclarationSyntax = (ClassDeclarationSyntax)namespaceDeclarationSyntax.Members[0];
                MethodDeclarationSyntax methodDeclarationSyntax = (MethodDeclarationSyntax)classDeclarationSyntax.Members[0];
                string body = methodDeclarationSyntax.Body.ToString().Trim();
                body = body.Substring(1, body.Length - 2).Trim();
                body.Should().Be("Assert.Fail(\"autogenerated\");");
            }
        }
    }
}
