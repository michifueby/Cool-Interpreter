//-----------------------------------------------------------------------
// <copyright file="CoolParserEngine.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Michael Füby, Armin Zimmerling, Mahmoud Ibrahim</author>
// <summary>CoolParserEngine</summary>
//-----------------------------------------------------------------------

namespace Cool.Interpreter.Lib.Language.Parsing;

using System.Collections.Immutable;
using Antlr4.Runtime;
using Cool.Interpreter.Lib.Core.Diagnostics;
using Cool.Interpreter.Lib.Core.Syntax;
using Cool.Interpreter.Lib.Core.Syntax.Ast;
using Cool.Interpreter.Lib.Core.Syntax.Ast.Expressions;
using Cool.Interpreter.Lib.Core.Syntax.Ast.Features;
using Cool.Interpreter.Lib.Core.Syntax.Operators;

/// <summary>
/// Converts the raw ANTLR parse tree into our clean, hand-written, type-safe AST.
/// This is the only class that knows about ANTLR — perfect separation of concerns.
/// </summary>
public class AstBuilderVisitor : CoolBaseVisitor<object?>
{
    /// <summary>
    /// Stores the path of the file being processed. This value is used to provide
    /// contextual information about the source of the parsed data, such as its location
    /// and origin. It may affect diagnostics and error reporting during the parsing process.
    /// </summary>
    private readonly string? _filePath;

    public AstBuilderVisitor(string? filePath)
    {
        _filePath = filePath;
    }

    /// <summary>
    /// Visits the root node of a program in the COOL abstract syntax tree and constructs
    /// a <see cref="ProgramNode"/> by aggregating all class definitions.
    /// </summary>
    /// <param name="context">The parser context representing the program node in the syntax tree.</param>
    /// <returns>A <see cref="ProgramNode"/> containing an immutable collection of all class nodes in the program.</returns>
    public override object? VisitProgram(CoolParser.ProgramContext context)
    {
        var classes = context.classDefine()
            .Select(c => Visit(c) as ClassNode)
            .Where(c => c != null)
            .ToImmutableArray();

        return new ProgramNode(classes);
    }

    /// <summary>
    /// Visits a class definition node in the COOL abstract syntax tree and constructs
    /// a <see cref="ClassNode"/> representing the class with its name, inheritance information, features, and source location.
    /// </summary>
    /// <param name="context">The parser context representing the class definition node in the syntax tree.</param>
    /// <returns>A <see cref="ClassNode"/> containing the class name, optional inheritance information, a list of its features, and its source position.</returns>
    public override object? VisitClassDefine(CoolParser.ClassDefineContext context)
    {
        var nameToken = context.TYPE(0);
        var name = nameToken.GetText();

        string? inheritsFrom = context.INHERITS() is not null
            ? context.TYPE(1).GetText()
            : null;

        var features = context.feature()
            .Select((f, index) => Visit(f, index) as FeatureNode)
            .Where(f => f != null)
            .ToImmutableArray();

        var location = ToSourcePosition(nameToken.Symbol);
        return new ClassNode(name, inheritsFrom, features, location);
    }

    /// <summary>
    /// Visits a feature node in the COOL abstract syntax tree, delegating the processing
    /// to either the method or property visitor based on the type of the feature.
    /// </summary>
    /// <param name="context">The parser context representing a feature node, which could be a method or a property.</param>
    /// <returns>An object representing the result of visiting the method or property, or null if the feature is invalid.</returns>
    public override object? VisitFeature(CoolParser.FeatureContext context)
        => context.method() is { } m
            ? Visit(m)
            : Visit(context.property(), -1); // sourceOrder passed via overload

    /// <summary>
    /// Visits a node in the COOL abstract syntax tree and processes it to produce the corresponding object representation.
    /// </summary>
    /// <param name="context">The parser context representing the current node in the syntax tree.</param>
    /// <returns>An object that represents the processed result of the visited node, or null if no specific processing is applied.</returns>
    private object? Visit(CoolParser.FeatureContext context, int sourceOrder)
        => context.method() is { } m
            ? Visit(m)
            : Visit(context.property(), sourceOrder);

    /// <summary>
    /// Visits a method node in the COOL abstract syntax tree and constructs
    /// a <see cref="MethodNode"/> representing the method's definition, including its name,
    /// return type, parameters, body, and source location.
    /// </summary>
    /// <param name="context">The parser context representing the method node in the syntax tree.</param>
    /// <returns>A <see cref="MethodNode"/> containing an immutable representation of the method's components.</returns>
    public override object? VisitMethod(CoolParser.MethodContext context)
    {
        var id = context.ID().GetText();
        var returnType = context.TYPE().GetText();

        var formals = context.formal()
            .Select(Visit)
            .Cast<FormalNode>()
            .ToImmutableArray();

        var body = Visit(context.expression()) as ExpressionNode
                   ?? new NoExpressionNode(ToSourcePosition(context.expression().Start));

        var location = ToSourcePosition(context.Start);
        return new MethodNode(id, formals, returnType, body, location);
    }

    /// <summary>
    /// Visits a formal node in the COOL abstract syntax tree and creates a <see cref="FormalNode"/>.
    /// </summary>
    /// <param name="context">The parser context representing the formal node in the syntax tree.</param>
    /// <returns>A <see cref="FormalNode"/> containing the identifier and type information of the formal.</returns>
    public override object? VisitFormal(CoolParser.FormalContext context)
    {
        var name = context.ID().GetText();
        var typeName = context.TYPE().GetText();
        return new FormalNode(name, typeName, ToSourcePosition(context.Start));
    }

    /// <summary>
    /// Visits an explicit dispatch expression in the COOL abstract syntax tree and constructs
    /// a <see cref="DispatchNode"/> representing a method call on a specified object or type.
    /// </summary>
    /// <param name="context">The parser context representing the explicit dispatch node in the syntax tree.</param>
    /// <returns>A <see cref="DispatchNode"/> containing the caller expression, optional static type, method name,
    /// method arguments, and source position information.</returns>
    public override object? VisitDispatchExplicit(CoolParser.DispatchExplicitContext context)
    {
        var caller = Visit(context.expression(0)) as ExpressionNode;
        var staticType = context.TYPE()?.GetText();
        var method = context.ID().GetText();

        var args = context.expression().Skip(1)
            .Select(e => Visit(e) as ExpressionNode!)
            .ToImmutableArray();

        return new DispatchNode(caller, staticType, method, args, ToSourcePosition(context.Start));
    }

    /// <summary>
    /// Visits an implicit dispatch node in the COOL abstract syntax tree. This method constructs
    /// a <see cref="DispatchNode"/> that represents a method call on the current object without specifying a receiver.
    /// </summary>
    /// <param name="context">The parser context representing the implicit dispatch node in the syntax tree.</param>
    /// <returns>A <see cref="DispatchNode"/> containing the details of the method being called, the arguments passed,
    /// and the source position of the call.</returns>
    public override object? VisitDispatchImplicit(CoolParser.DispatchImplicitContext context)
    {
        var method = context.ID().GetText();
        var args = context.expression()
            .Select(e => Visit(e) as ExpressionNode!)
            .ToImmutableArray();

        var self = new SelfNode(ToSourcePosition(context.Start));
        return new DispatchNode(self, null, method, args, ToSourcePosition(context.Start));
    }

    /// <summary>
    /// Visits an 'if' expression in the COOL abstract syntax tree and constructs
    /// an <see cref="IfNode"/> representing the conditional expression.
    /// </summary>
    /// <param name="context">The parser context for the 'if' expression, containing the predicate,
    /// 'then' branch, and optional 'else' branch.</param>
    /// <returns>An <see cref="IfNode"/> representing the conditional branching expression in the syntax tree.</returns>
    public override object? VisitIf(CoolParser.IfContext context)
    {
        var pred = Visit(context.expression(0)) as ExpressionNode;
        var thenExpr = Visit(context.expression(1)) as ExpressionNode;
        var elseExpr = Visit(context.expression(2)) as ExpressionNode;
        return new IfNode(pred!, thenExpr, elseExpr, ToSourcePosition(context.Start));
    }

    /// <summary>
    /// Visits a 'while' loop node in the COOL abstract syntax tree, constructing
    /// a <see cref="WhileNode"/> with the condition and body expressions.
    /// </summary>
    /// <param name="context">The parser context representing the 'while' loop node in the syntax tree.</param>
    /// <returns>A <see cref="WhileNode"/> containing the predicate and body expressions of the 'while' loop.</returns>
    public override object? VisitWhile(CoolParser.WhileContext context)
    {
        var pred = Visit(context.expression(0)) as ExpressionNode;
        var body = Visit(context.expression(1)) as ExpressionNode;
        return new WhileNode(pred, body, ToSourcePosition(context.Start));
    }

    /// <summary>
    /// Visits a block node in the COOL abstract syntax tree and constructs a
    /// <see cref="BlockNode"/> that aggregates all expressions within the block.
    /// </summary>
    /// <param name="context">The parser context representing the block node in the syntax tree.</param>
    /// <returns>A <see cref="BlockNode"/> containing an immutable collection of all expression nodes in the block.</returns>
    public override object? VisitBlock(CoolParser.BlockContext context)
    {
        var exprs = context.expression()
            .Select(Visit)
            .Cast<ExpressionNode>()
            .ToImmutableArray();

        return new BlockNode(exprs, ToSourcePosition(context.Start));
    }

    /// <summary>
    /// Processes a case expression in the COOL abstract syntax tree, constructing
    /// a <see cref="CaseNode"/> representation containing the scrutinee and its associated case branches.
    /// </summary>
    /// <param name="context">The parser context representing the case expression in the syntax tree.</param>
    /// <returns>A <see cref="CaseNode"/> representing the case expression, which includes the scrutinee,
    /// the collection of case branches, and their source positions.</returns>
    public override object? VisitCase(CoolParser.CaseContext context)
    {
        var scrutineeExpr = context.expression(0);
        var scrutinee = Visit(scrutineeExpr) as ExpressionNode;

        var branchExpressions = context.expression().Skip(1).ToArray();

        var branches = context.formal()
            .Zip(branchExpressions, (formalCtx, bodyCtx) =>
            {
                var id = formalCtx.ID().GetText();
                var typeName = formalCtx.TYPE().GetText();
                var body = Visit(bodyCtx) as ExpressionNode;
                var location = ToSourcePosition(formalCtx.Start);
                return new CaseBranchNode(id, typeName, body, location);
            })
            .ToImmutableArray();

        var location = ToSourcePosition(context.CASE().Symbol);
        return new CaseNode(scrutinee, branches, location);
    }

    /// <summary>
    /// Visits the "new" expression in the COOL abstract syntax tree and constructs
    /// a <see cref="NewNode"/> representing the creation of an instance of a specified type.
    /// </summary>
    /// <param name="context">The parser context representing the "new" expression in the abstract syntax tree.</param>
    /// <returns>A <see cref="NewNode"/> describing the instantiation of the specified type, including its source position.</returns>
    public override object? VisitNew(CoolParser.NewContext context)
        => new NewNode(context.TYPE().GetText(), ToSourcePosition(context.Start));

    /// <summary>
    /// Visits a negative expression node in the COOL abstract syntax tree and constructs
    /// a <see cref="UnaryOperationNode"/> representing a negation operation.
    /// </summary>
    /// <param name="context">The parser context for the negative expression node.</param>
    /// <returns>A <see cref="UnaryOperationNode"/> encapsulating the negation operator and the operand expression node.</returns>
    public override object? VisitNegative(CoolParser.NegativeContext context)
        => new UnaryOperationNode(UnaryOperator.Negate,
            (Visit(context.expression()) as ExpressionNode)!, ToSourcePosition(context.Start));

    /// <summary>
    /// Visits a node representing an isvoid expression in the COOL abstract syntax tree
    /// and constructs an <see cref="IsVoidNode"/> by processing its child expression node.
    /// </summary>
    /// <param name="context">The parser context that encapsulates the isvoid expression in the syntax tree.</param>
    /// <returns>An <see cref="IsVoidNode"/> representing the isvoid expression,
    /// containing its child expression and source position.</returns>
    public override object? VisitIsvoid(CoolParser.IsvoidContext context)
        => new IsVoidNode((Visit(context.expression()) as ExpressionNode)!, ToSourcePosition(context.Start));

    /// <summary>
    /// Visits the arithmetic expression node in the COOL abstract syntax tree, evaluates its components,
    /// and constructs a <see cref="BinaryOperationNode"/> representing the operation.
    /// </summary>
    /// <param name="context">The parser context representing the arithmetic expression node.</param>
    /// <returns>A <see cref="BinaryOperationNode"/> containing the left and right operands, the binary operator,
    /// and source position data for the arithmetic expression.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the arithmetic operator is unrecognized or unsupported.</exception>
    public override object? VisitArithmetic(CoolParser.ArithmeticContext context)
    {
        var left = Visit(context.expression(0)) as ExpressionNode;
        var right = Visit(context.expression(1)) as ExpressionNode;

        BinaryOperator op = context.op.Text switch
        {
            "+" =>  BinaryOperator.Plus,       // '+'
            
            "-" => BinaryOperator.Minus,      // '-'
            
            "*" => BinaryOperator.Multiply,   // '*'
            
            "/" => BinaryOperator.Divide,     // '/'
            
            _ => throw new InvalidOperationException($"Unknown arithmetic operator {context.op.Text}")
        };

        return new BinaryOperationNode(left, op, right, ToSourcePosition(context.op));
    }

    /// <summary>
    /// Visits a comparison operation node in the COOL abstract syntax tree
    /// and constructs a <see cref="BinaryOperationNode"/> by combining
    /// the left and right expressions with the corresponding binary operator.
    /// </summary>
    /// <param name="context">The parser context representing the comparison operation node in the syntax tree.</param>
    /// <returns>A <see cref="BinaryOperationNode"/> representing the comparison operation, combining left and right expressions.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the comparison operator is not recognized.
    /// </exception>
    public override object? VisitComparisson(CoolParser.ComparissonContext context)
    {
        var left = Visit(context.expression(0)) as ExpressionNode;
        var right = Visit(context.expression(1)) as ExpressionNode;

        var op = context.op.Text switch
        {
            "<=" => BinaryOperator.LessThanOrEqual,   // '<='
            
            "<" => BinaryOperator.LessThan,          // '<'
            
            "=" => BinaryOperator.Equal,             // '='
            
            _ => throw new InvalidOperationException($"Unknown comparison operator {context.op.Text}")
        };

        return new BinaryOperationNode(left, op, right, ToSourcePosition(context.op));
    }

    /// <summary>
    /// Visits a Boolean "not" operation node in the COOL abstract syntax tree,
    /// creating a <see cref="UnaryOperationNode"/> that represents the logical negation operation.
    /// </summary>
    /// <param name="context">The parser context representing the "not" expression in the syntax tree.</param>
    /// <returns>A <see cref="UnaryOperationNode"/> representing the logical negation,
    /// containing the operand and its source position.</returns>
    public override object? VisitBoolNot(CoolParser.BoolNotContext context)
        => new UnaryOperationNode(UnaryOperator.Not,
            Visit(context.expression()) as ExpressionNode, ToSourcePosition(context.Start));

    /// <summary>
    /// Visits an assignment node in the COOL abstract syntax tree and generates
    /// an <see cref="AssignNode"/> containing the identifier, the expression being assigned,
    /// and the source position of the node in the code.
    /// </summary>
    /// <param name="context">The parser context representing the assignment node in the syntax tree.</param>
    /// <returns>An <see cref="AssignNode"/> representing an assignment operation in the syntax tree.</returns>
    public override object? VisitAssignment(CoolParser.AssignmentContext context)
        => new AssignNode(context.ID().GetText(),
            Visit(context.expression()) as ExpressionNode, ToSourcePosition(context.Start));

    /// <summary>
    /// Visits a let-in expression node in the COOL abstract syntax tree, constructing a <see cref="LetNode"/>
    /// that aggregates variable bindings and the body of the expression.
    /// </summary>
    /// <param name="context">The parser context representing the let-in expression node in the syntax tree.</param>
    /// <returns>A <see cref="LetNode"/> containing an immutable collection of variable bindings and the body of the let-in expression.</returns>
    public override object? VisitLetIn(CoolParser.LetInContext context)
    {
        var bindings = context.property()
            .Select((p, i) => Visit(p, i) as LetBindingNode)
            .ToImmutableArray();

        var body = Visit(context.expression()) as ExpressionNode;
        return new LetNode(bindings, body, ToSourcePosition(context.Start));
    }

    /// <summary>
    /// Visits a parentheses expression in the COOL abstract syntax tree and processes
    /// the nested expression by delegating the visit operation to it.
    /// </summary>
    /// <param name="context">The parser context representing a parentheses expression in the syntax tree.</param>
    /// <returns>The result of the visit operation on the nested expression, as an object.</returns>
    public override object? VisitParentheses(CoolParser.ParenthesesContext context)
        => Visit(context.expression());

    /// <summary>
    /// Visits an identifier in the COOL abstract syntax tree and creates an <see cref="IdentifierExpressionNode"/>
    /// representing the identifier and its position in the source code.
    /// </summary>
    /// <param name="context">The parser context representing the identifier node in the syntax tree.</param>
    /// <returns>An <see cref="IdentifierExpressionNode"/> containing the identifier's name and its source position.</returns>
    public override object? VisitId(CoolParser.IdContext context)
        => new IdentifierExpressionNode(context.ID().GetText(), ToSourcePosition(context.Start));

    /// <summary>
    /// Visits an integer literal node in the COOL abstract syntax tree and creates
    /// an <see cref="IntegerLiteralNode"/> representing the parsed integer value.
    /// </summary>
    /// <param name="context">The parser context containing the integer literal token.</param>
    /// <returns>An <see cref="IntegerLiteralNode"/> representing the integer value
    /// along with its source position in the syntax tree.</returns>
    public override object? VisitInt(CoolParser.IntContext context)
        => new IntegerLiteralNode(int.Parse(context.INT().GetText()), ToSourcePosition(context.Start));

    /// <summary>
    /// Visits a string literal node in the COOL abstract syntax tree and creates
    /// a <see cref="StringLiteralNode"/> that represents the unescaped string value and its location.
    /// </summary>
    /// <param name="context">The parser context for the string literal node in the syntax tree.</param>
    /// <returns>A <see cref="StringLiteralNode"/> containing the unescaped string value and its source position.</returns>
    public override object? VisitString(CoolParser.StringContext context)
        => new StringLiteralNode(UnescapeString(context.STRING().GetText()), ToSourcePosition(context.Start));

    /// <summary>
    /// Processes a boolean literal node in the COOL abstract syntax tree and generates
    /// a <see cref="BoolLiteralNode"/> with the parsed value and source position.
    /// </summary>
    /// <param name="context">The parser context representing the boolean literal in the syntax tree.</param>
    /// <returns>A <see cref="BoolLiteralNode"/> representing the boolean value and its source location.</returns>
    public override object? VisitBoolean(CoolParser.BooleanContext context)
        => new BoolLiteralNode(context.TRUE() != null, ToSourcePosition(context.Start));
    
    /// <summary>
    /// Processes a property context in the COOL abstract syntax tree, extracting details about
    /// attribute declaration such as name, type, initializer, and source position, then
    /// constructs an <see cref="AttributeNode"/>.
    /// </summary>
    /// <param name="context">The property context node in the syntax tree that represents an attribute declaration.</param>
    /// <param name="sourceOrder">The order of the attribute within the scope of its containing class or feature.</param>
    /// <returns>An <see cref="AttributeNode"/> representing the attribute with its name, type, optional initializer, order, and location in source code.</returns>
    private object? Visit(CoolParser.PropertyContext context, int sourceOrder)
    {
        var formal = context.formal();
        var name = formal.ID().GetText();
        var typeName = formal.TYPE().GetText();

        ExpressionNode? initializer = context.ASSIGNMENT() is not null
            ? Visit(context.expression()) as ExpressionNode
            : null;

        var location = ToSourcePosition(context.Start);

        return new AttributeNode(name, typeName, initializer, sourceOrder, location);
    }

    /// <summary>
    /// Converts a given token into a <see cref="SourcePosition"/> object, which represents
    /// the location of the token in the source code.
    /// </summary>
    /// <param name="token">The token from which the source position is derived. It provides details like line and column information.</param>
    /// <returns>A <see cref="SourcePosition"/> that encapsulates the file path, line number, and column number of the token.</returns>
    private SourcePosition ToSourcePosition(IToken token)
        => new(_filePath, token.Line, token.Column + 1);

    /// <summary>
    /// Processes a raw string literal by removing surrounding quotes and unescaping special characters.
    /// </summary>
    /// <param name="raw">The raw string literal to be unescaped, including surrounding quotes and escape sequences.</param>
    /// <returns>A string with the surrounding quotes removed and escape sequences replaced by their corresponding characters.</returns>
    private static string UnescapeString(string raw)
    {
        var s = raw.Substring(1, raw.Length - 2);
        var sb = new System.Text.StringBuilder(s.Length);

        for (int i = 0; i < s.Length; i++)
        {
            if (s[i] == '\\')
            {
                if (++i >= s.Length) { sb.Append('\\'); break; }
                sb.Append(s[i] switch
                {
                    'b' => '\b',
                    't' => '\t',
                    'n' => '\n',
                    'f' => '\f',
                    'r' => '\r',
                    '"' => '"',
                    '\\' => '\\',
                    _   => s[i]
                });
            }
            else sb.Append(s[i]);
        }
        return sb.ToString();
    }
}