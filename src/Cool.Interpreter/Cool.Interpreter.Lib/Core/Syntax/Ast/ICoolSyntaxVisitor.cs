//-----------------------------------------------------------------------
// <copyright file="ICoolSyntaxVisitor.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Michael Füby, Armin Zimmerling, Mahmoud Ibrahim</author>
// <summary>ICoolSyntaxVisitor</summary>
//-----------------------------------------------------------------------

namespace Cool.Interpreter.Lib.Core.Syntax.Ast;

using Cool.Interpreter.Lib.Core.Syntax.Ast.Expressions;
using Cool.Interpreter.Lib.Core.Syntax.Ast.Features;

/// <summary>
/// Defines a visitor for traversing and processing nodes of the Cool Abstract Syntax Tree (AST).
/// The visitor implements the "visitor" design pattern, enabling externalized operations on
/// the AST node hierarchy without modifying the node classes themselves.
/// </summary>
/// <typeparam name="T">The type of value returned by the visitor after processing AST nodes.</typeparam>
public interface ICoolSyntaxVisitor<T>
{
    T Visit(ProgramNode node);
    T Visit(ClassNode node);
    T Visit(AttributeNode node);
    T Visit(MethodNode node);
    T Visit(FormalNode node);
    T Visit(FeatureNode node);

    T Visit(AssignNode node);
    T Visit(DispatchNode node);
    T Visit(IfNode node);
    T Visit(WhileNode node);
    T Visit(BlockNode node);
    T Visit(LetNode node);
    T Visit(LetBindingNode node);
    T Visit(CaseNode node);
    T Visit(CaseBranchNode node);
    T Visit(NewNode node);
    T Visit(IsVoidNode node);
    T Visit(BinaryOperationNode node);
    T Visit(UnaryOperationNode node);
    T Visit(IntegerLiteralNode node);
    T Visit(StringLiteralNode node);
    T Visit(BoolLiteralNode node);
    T Visit(IdentifierExpressionNode node);
    T Visit(SelfNode node);
    T Visit(NoExpressionNode node);
    T Visit(BuiltinExpressionNode node);
}