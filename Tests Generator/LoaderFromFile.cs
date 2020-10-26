﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeDom.Providers.DotNetCompilerPlatform;

namespace Tests_Generator
{
    internal class LoaderFromFile
    {
        private int fLimit;
        private List<string> fFilenames;
 
        internal LoaderFromFile(int limit, List<string> filenames)
        {
            fLimit = limit;
            fFilenames = filenames;
        }

        internal void Start()
        {
            foreach(string filename in fFilenames)
            {
                string text;
                using (StreamReader streamReader = new StreamReader(filename))
                {
                    text = streamReader.ReadToEnd();
                }

                var tree = SyntaxFactory.ParseSyntaxTree(text);
                var root = tree.GetCompilationUnitRoot();

                var syntaxFactory = SyntaxFactory.CompilationUnit();
                syntaxFactory = syntaxFactory.AddUsings(SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("System")),
                                                        SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("System.Threading")),
                                                        SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("Microsoft.VisualStudio.TestTools.UnitTesting"))
                );
                syntaxFactory = AddNode(root, syntaxFactory);
                var code = syntaxFactory.NormalizeWhitespace().ToFullString();
                string writePath = @"D:\test.cs";
                using (StreamWriter sw = new StreamWriter(writePath, false, System.Text.Encoding.Default))
                {
                    sw.WriteLine(code);
                }
            }
        }

        private dynamic AddNode(dynamic originalMember, dynamic generatedMember)
        {
            foreach (var member in originalMember.Members)
            {
                if (member.GetType() == typeof(NamespaceDeclarationSyntax))
                {
                    NamespaceDeclarationSyntax namMember = (NamespaceDeclarationSyntax)member;
                    var myNamespace = SyntaxFactory.NamespaceDeclaration(SyntaxFactory.ParseName(namMember.Name + ".Tests")).NormalizeWhitespace();
                    myNamespace = AddNode(member, myNamespace);
                    generatedMember = generatedMember.AddMembers(myNamespace);
                }
                else if (member.GetType() == typeof(ClassDeclarationSyntax))
                {
                    ClassDeclarationSyntax clsMember = (ClassDeclarationSyntax)member;
                    var myClass = SyntaxFactory.ClassDeclaration(clsMember.Identifier + "Tests");
                    myClass = myClass.AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword));
                    myClass = myClass.AddAttributeLists(SyntaxFactory.AttributeList(SyntaxFactory.SingletonSeparatedList(
                                SyntaxFactory.Attribute(SyntaxFactory.IdentifierName("TestClass")))));
                    myClass = AddNode(member, myClass);
                    generatedMember = generatedMember.AddMembers(myClass);
                }
                else if (member.GetType() == typeof(MethodDeclarationSyntax))
                {
                    MethodDeclarationSyntax mthMember = (MethodDeclarationSyntax)member;
                    var syntax = SyntaxFactory.ParseStatement("Assert.Fail(\"autogenerated\");");
                    var myMethod = SyntaxFactory.MethodDeclaration(SyntaxFactory.ParseTypeName("void"), "Test" + mthMember.Identifier)
                             .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
                             .WithBody(SyntaxFactory.Block(syntax));
                    myMethod = myMethod.AddAttributeLists(SyntaxFactory.AttributeList(SyntaxFactory.SingletonSeparatedList(
                                SyntaxFactory.Attribute(SyntaxFactory.IdentifierName("TestMethod")))));
                    generatedMember = generatedMember.AddMembers(myMethod);
                }
            }
            return generatedMember;
        }

        private void CreateFileClass()
        {
            var syntaxFactory = SyntaxFactory.CompilationUnit();
            syntaxFactory = syntaxFactory.AddUsings(SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("System")),
                                                    SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("System.Threading")),
                                                    SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("Microsoft.VisualStudio.TestTools.UnitTesting"))
            );

            var @namespace = SyntaxFactory.NamespaceDeclaration(SyntaxFactory.ParseName("CodeGenerationSample.Tests")).NormalizeWhitespace();
            var classDeclaration = SyntaxFactory.ClassDeclaration("Order");

            classDeclaration = classDeclaration.AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword));
            classDeclaration = classDeclaration.AddAttributeLists(SyntaxFactory.AttributeList(SyntaxFactory.SingletonSeparatedList(
                        SyntaxFactory.Attribute(SyntaxFactory.IdentifierName("TestClass")))));

            var syntax = SyntaxFactory.ParseStatement("Assert.Fail(\"autogenerated\");");

            var methodDeclaration = SyntaxFactory.MethodDeclaration(SyntaxFactory.ParseTypeName("void"), "MarkAsCanceledTest")
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
                .WithBody(SyntaxFactory.Block(syntax));
            methodDeclaration = methodDeclaration.AddAttributeLists(SyntaxFactory.AttributeList(SyntaxFactory.SingletonSeparatedList(
                        SyntaxFactory.Attribute(SyntaxFactory.IdentifierName("TestMethod")))));

            classDeclaration = classDeclaration.AddMembers(methodDeclaration);

            @namespace = @namespace.AddMembers(classDeclaration);
            syntaxFactory = syntaxFactory.AddMembers(@namespace);
            var code = syntaxFactory.NormalizeWhitespace().ToFullString();


            string writePath = @"D:\test.cs";
            using (StreamWriter sw = new StreamWriter(writePath, false, System.Text.Encoding.Default))
            {
                sw.WriteLine(code);
            }
        }
    }
}
