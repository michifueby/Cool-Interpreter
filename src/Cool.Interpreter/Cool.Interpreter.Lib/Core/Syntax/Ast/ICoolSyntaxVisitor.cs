//-----------------------------------------------------------------------
// <copyright file="ICoolSyntaxVisitor.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Michael Füby, Armin Zimmerling, Mahmoud Ibrahim</author>
// <summary>ICoolSyntaxVisitor</summary>
//-----------------------------------------------------------------------

namespace Cool.Interpreter.Lib.Core.Syntax.Ast;

using Expressions;
using Features;

/// <summary>
/// Defines a visitor for traversing and processing nodes of the Cool Abstract Syntax Tree (AST).
/// The visitor implements the "visitor" design pattern, enabling externalized operations on
/// the AST node hierarchy without modifying the node classes themselves.
/// </summary>
/// <typeparam name="T">The type of value returned by the visitor after processing AST nodes.</typeparam>
public interface ICoolSyntaxVisitor<out T>
{
    /// <summary>
    /// Visits a node in the abstract syntax tree and performs an operation depending on its type.
    /// This method is a core part of the visitor design pattern implementation for processing
    /// the Cool language AST.
    /// </summary>
    /// <param name="node">The ProgramNode representing the root of the Cool AST to be visited.</param>
    /// <returns>A value of type <typeparamref name="T"/> that represents the result of processing the node.</returns>
    T Visit(ProgramNode node);

    /// <summary>
    /// Visits a ClassNode in the Cool Abstract Syntax Tree (AST) and processes it according to the rules
    /// defined in the visitor implementation.
    /// This method is part of the visitor design pattern, enabling operations on ClassNode instances
    /// without modifying their structure or behavior.
    /// </summary>
    /// <param name="node">The ClassNode to be visited, representing a Cool class in the AST.</param>
    /// <returns>A value of type <typeparamref name="T"/> that is derived from processing the ClassNode.</returns>
    T Visit(ClassNode node);

    /// <summary>
    /// Visits an AttributeNode in the abstract syntax tree (AST) and performs an operation
    /// depending on its type.
    /// This method is part of the implementation of the visitor design pattern for processing
    /// nodes of the Cool language AST.
    /// </summary>
    /// <param name="node">The AttributeNode representing an attribute declaration in the Cool AST to be visited.</param>
    /// <returns>A value of type <typeparamref name="T"/> that represents the result of processing the attribute node.</returns>
    T Visit(AttributeNode node);

    /// <summary>
    /// Visits a method node in the abstract syntax tree, enabling processing and analysis
    /// of method declarations, including their parameters, return types, and body expressions.
    /// This method facilitates operations defined by the visitor design pattern
    /// for <see cref="MethodNode"/> objects.
    /// </summary>
    /// <param name="node">The <see cref="MethodNode"/> representing the method declaration in the AST.</param>
    /// <returns>A value of type <typeparamref name="T"/> that is the result of processing the method node.</returns>
    T Visit(MethodNode node);

    /// <summary>
    /// Visits a FormalNode in the abstract syntax tree (AST) of the Cool programming language.
    /// This method processes a formal parameter node, which defines the name and type of a parameter
    /// within a method or a function.
    /// </summary>
    /// <param name="node">The FormalNode to be visited and processed.</param>
    /// <returns>A value of type <typeparamref name="T"/> representing the result of processing the formal parameter node.</returns>
    T Visit(FormalNode node);

    /// <summary>
    /// Visits a feature node in the Cool abstract syntax tree (AST) and processes it based on its type.
    /// This method supports operations on features such as attributes or methods in a class definition.
    /// </summary>
    /// <param name="node">The FeatureNode instance representing the specific feature to be processed.</param>
    /// <returns>A value of type <typeparamref name="T"/> that represents the outcome of processing the feature node.</returns>
    T Visit(FeatureNode node);

    /// <summary>
    /// Visits an AssignNode in the abstract syntax tree, representing an assignment expression.
    /// This method processes the assignment operation, which involves assigning the result of an
    /// evaluated expression to a specific identifier.
    /// </summary>
    /// <param name="node">The AssignNode representing the assignment expression in the Cool AST.</param>
    /// <returns>A value of type <typeparamref name="T"/> that represents the result of processing the assignment node.</returns>
    T Visit(AssignNode node);

    /// <summary>
    /// Visits a DispatchNode in the abstract syntax tree (AST) and performs an operation specific to the
    /// method dispatch (method call) represented by this node. This method is part of the visitor design pattern
    /// implementation for processing Cool language AST.
    /// </summary>
    /// <param name="node">The DispatchNode representing a method dispatch expression in the AST.</param>
    /// <returns>A value of type <typeparamref name="T"/> that represents the result of processing the DispatchNode.</returns>
    T Visit(DispatchNode node);

    /// <summary>
    /// Visits an "if-then-else" conditional expression node in the Cool Abstract Syntax Tree (AST).
    /// This method processes the predicate expression and determines which branch will be evaluated
    /// based on the condition result.
    /// </summary>
    /// <param name="node">The IfNode representing the conditional expression to be visited.</param>
    /// <returns>A value of type <typeparamref name="T"/> resulting from the processing of the conditional expression node.</returns>
    T Visit(IfNode node);

    /// <summary>
    /// Visits a "while" loop node in the abstract syntax tree for the Cool programming language.
    /// This method processes the predicate (condition) and body of the "while" loop construct,
    /// enabling evaluation, transformation, or any other operation required by the visitor.
    /// </summary>
    /// <param name="node">The WhileNode representing the "while" loop construct in the Cool AST.</param>
    /// <returns>A value of type <typeparamref name="T"/> that represents the result of processing the "while" loop node.</returns>
    T Visit(WhileNode node);

    /// <summary>
    /// Visits a BlockNode in the Cool Abstract Syntax Tree (AST) and performs an operation specific to block expressions.
    /// A BlockNode represents a sequence of expressions, where the result is determined by the last expression in the sequence.
    /// </summary>
    /// <param name="node">The BlockNode instance to be visited, containing a sequence of expressions and its source position.</param>
    /// <returns>A value of type <typeparamref name="T"/> that represents the result of processing the BlockNode.</returns>
    T Visit(BlockNode node);

    /// <summary>
    /// Visits a "let" expression node in the Cool Abstract Syntax Tree (AST), allowing for
    /// processing or transformation of the node. A "let" expression defines a set of
    /// local variable bindings and an associated expression that makes use of those bindings.
    /// </summary>
    /// <param name="node">The LetNode representing the "let" expression to be visited.</param>
    /// <returns>A value of type <typeparamref name="T"/> that represents the result of processing the node.</returns>
    T Visit(LetNode node);

    /// <summary>
    /// Visits a LetBindingNode in the Cool Abstract Syntax Tree (AST) and performs an operation
    /// defined by the implementing visitor. This method processes a single binding in a "let"
    /// expression, which includes an identifier, its type, and an optional initializer expression.
    /// </summary>
    /// <param name="node">The LetBindingNode representing the binding to be visited within the AST.</param>
    /// <returns>A value of type <typeparamref name="T"/> that represents the result of processing the node.</returns>
    T Visit(LetBindingNode node);

    /// <summary>
    /// Visits a "case" expression node in the Cool Abstract Syntax Tree (AST) and performs an operation on it.
    /// This method is invoked during the traversal of a "case" construct, which includes evaluating the scrutinee
    /// and processing its associated branches.
    /// </summary>
    /// <param name="node">The CaseNode representing the "case" expression in the Cool AST to be visited.</param>
    /// <returns>A value of type <typeparamref name="T"/> that represents the result of processing the "case" expression.</returns>
    T Visit(CaseNode node);

    /// <summary>
    /// Visits a node of type <see cref="CaseBranchNode"/> in the abstract syntax tree and performs an operation defined by the visitor implementation.
    /// </summary>
    /// <param name="node">The <see cref="CaseBranchNode"/> to be visited and processed.</param>
    /// <returns>A value of type <typeparamref name="T"/> that represents the result of the visitor's operation on the node.</returns>
    T Visit(CaseBranchNode node);

    /// <summary>
    /// Visits a node that represents the creation of a new object in the Cool language abstract syntax tree.
    /// This method processes the "new" construct and retrieves the associated information.
    /// </summary>
    /// <param name="node">The NewNode that specifies the type of the object being instantiated and its location in the source code.</param>
    /// <returns>A value of type <typeparamref name="T"/> that represents the result of processing the new object creation node.</returns>
    T Visit(NewNode node);

    /// <summary>
    /// Visits a node representing the "isvoid" operation in the Cool Abstract Syntax Tree (AST) and performs
    /// an operation defined by the visitor.
    /// </summary>
    /// <param name="node">The IsVoidNode representing the "isvoid" operation in the Cool AST.</param>
    /// <returns>A value of type <typeparamref name="T"/> that represents the result of processing the "isvoid" node.</returns>
    T Visit(IsVoidNode node);

    /// <summary>
    /// Visits a BinaryOperationNode within the abstract syntax tree and performs an operation
    /// depending on its type and context.
    /// </summary>
    /// <param name="node">The BinaryOperationNode representing a binary operation expression to be processed.</param>
    /// <returns>A value of type <typeparamref name="T"/> that represents the result of processing the binary operation node.</returns>
    T Visit(BinaryOperationNode node);

    /// <summary>
    /// Visits a UnaryOperationNode in the abstract syntax tree, enabling processing of a unary operation
    /// consisting of a single operand and a unary operator.
    /// </summary>
    /// <param name="node">The UnaryOperationNode representing the unary operation to be processed.</param>
    /// <returns>A value of type <typeparamref name="T"/> that represents the result of processing the unary operation.</returns>
    T Visit(UnaryOperationNode node);

    /// <summary>
    /// Visits an IntegerLiteralNode in the abstract syntax tree and performs an operation
    /// based on its specific type. This method allows processing of integer literal expressions
    /// within the Cool language.
    /// </summary>
    /// <param name="node">The IntegerLiteralNode representing an integer value in the AST.</param>
    /// <returns>A value of type <typeparamref name="T"/> that represents the result of processing the node.</returns>
    T Visit(IntegerLiteralNode node);

    /// <summary>
    /// Visits a string literal node in the Cool Abstract Syntax Tree (AST) and performs an operation on it.
    /// This method is part of the visitor design pattern implementation for processing string literal nodes specifically.
    /// </summary>
    /// <param name="node">The StringLiteralNode representing a string literal in the Cool AST to be visited.</param>
    /// <returns>A value of type <typeparamref name="T"/> that represents the result of processing the string literal node.</returns>
    T Visit(StringLiteralNode node);

    /// <summary>
    /// Visits a boolean literal node in the Cool Abstract Syntax Tree (AST) and processes its value.
    /// This operation is part of the visitor design pattern for handling AST traversal and evaluation.
    /// </summary>
    /// <param name="node">The BoolLiteralNode representing a literal boolean value in the Cool AST.</param>
    /// <returns>A value of type <typeparamref name="T"/> resulting from the processing of the BoolLiteralNode.</returns>
    T Visit(BoolLiteralNode node);

    /// <summary>
    /// Visits an identifier expression node in the abstract syntax tree and performs
    /// operations specific to the node's role within a Cool program.
    /// This method supports externalized behavior through the visitor pattern.
    /// </summary>
    /// <param name="node">The IdentifierExpressionNode representing an identifier in the Cool AST.</param>
    /// <returns>A value of type <typeparamref name="T"/> that describes the result of processing the identifier node.</returns>
    T Visit(IdentifierExpressionNode node);

    /// <summary>
    /// Visits a SelfNode in the Cool Abstract Syntax Tree (AST) and performs an operation specific
    /// to references of the current object (`self`) within the Cool programming language.
    /// This method is invoked as part of the visitor pattern for processing AST nodes.
    /// </summary>
    /// <param name="node">The SelfNode representing a reference to the current object (`self`).</param>
    /// <returns>A value of type <typeparamref name="T"/> that represents the result of processing the node.</returns>
    T Visit(SelfNode node);

    /// <summary>
    /// Visits a NoExpressionNode in the abstract syntax tree and performs an operation associated
    /// with this type of node. This method is part of the visitor design pattern implementation
    /// for processing nodes in the Cool language AST.
    /// </summary>
    /// <param name="node">The NoExpressionNode representing a placeholder for an absent expression in the AST.</param>
    /// <returns>A value of type <typeparamref name="T"/> that represents the result of processing the node.</returns>
    T Visit(NoExpressionNode node);

    /// <summary>
    /// Visits a node in the Cool Abstract Syntax Tree (AST) that represents a built-in expression.
    /// This method processes built-in constructs in the language which have pre-defined semantics
    /// or evaluation rules.
    /// </summary>
    /// <param name="node">The BuiltinExpressionNode to be visited and processed.</param>
    /// <returns>A value of type <typeparamref name="T"/> that represents the result of processing the built-in expression node.</returns>
    T Visit(BuiltinExpressionNode node);
}