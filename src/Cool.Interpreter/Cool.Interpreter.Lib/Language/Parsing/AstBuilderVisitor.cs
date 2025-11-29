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

/// <summary>
/// Converts the raw ANTLR parse tree into our clean, hand-written, type-safe AST.
/// This is the only class that knows about ANTLR — perfect separation of concerns.
/// </summary>
public class AstBuilderVisitor : CoolBaseVisitor<object?>
{
    /// <summary>
    /// A private field representing a diagnostic collection used to store and track
    /// diagnostic messages (e.g., errors, warnings, informational messages) that
    /// occur during the parsing and building of the AST.
    /// This data is subsequently used for reporting issues to the user.
    /// </summary>
    private readonly DiagnosticBag _diagnostics;

    /// <summary>
    /// A private field representing the file path of the source code being parsed.
    /// This field is used to associate diagnostic messages and source positions with the
    /// specific file from which the code originates, providing context for error reporting
    /// and debugging.
    /// </summary>
    private readonly string? _filePath;

    /// <summary>
    /// Initializes a new instance of the <see cref="AstBuilderVisitor"/> class.
    /// </summary>
    /// <param name="diagnostics"></param>
    /// <param name="filePath"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public AstBuilderVisitor(DiagnosticBag diagnostics, string? filePath)
    {
        _diagnostics = diagnostics ?? throw new ArgumentNullException(nameof(diagnostics));
        _filePath = filePath;
    }

    /// <summary>
    /// Processes the program node in the syntax tree, visiting each class definition and extracting its details
    /// to construct a program representation.
    /// </summary>
    /// <param name="context">
    /// The parsing context representing the root of the program. This contains a collection of class definitions
    /// to be processed.
    /// </param>
    /// <returns>
    /// A <see cref="ProgramNode"/> instance containing a collection of class nodes representing the structure of the program.
    /// </returns>
    public override object? VisitProgram(CoolParser.ProgramContext context)
    {
        var classes = context.classDefine()
            .Select(c => Visit(c) as ClassNode)
            .Where(c => c != null)
            .ToImmutableArray()!;

        return new ProgramNode(classes!);
    }

    /// <summary>
    /// Visits a class definition node in the syntax tree, extracting its identifier,
    /// inheritance details, features, and location in the source code.
    /// </summary>
    /// <param name="context">
    /// The parsing context representing a class definition. This includes the class name,
    /// inheritance information (if any), and a collection of features.
    /// </param>
    /// <returns>
    /// A <see cref="ClassNode"/> instance containing the class name, optional parent class name,
    /// a collection of features, and its location in the source code.
    /// </returns>
    public override object? VisitClassDefine(CoolParser.ClassDefineContext context)
    {
        var nameToken = context.TYPE(0);
        var name = nameToken.GetText();

        string? inheritsFrom = context.INHERITS() is not null
            ? context.TYPE(1).GetText()
            : null;

        var features = context.feature()
            .Select(f => Visit(f) as FeatureNode)
            .Where(f => f != null)
            .ToImmutableArray()!; 

        var location = ToSourcePosition(nameToken.Symbol);

        return new ClassNode(name, inheritsFrom, features, location);
    }
    
    // Implementation missing !!!!

    /// <summary>
    /// Converts an ANTLR token into a <see cref="SourcePosition"/> representation,
    /// encapsulating file path, line, and column details.
    /// </summary>
    /// <param name="token">
    /// The ANTLR token to be converted. It contains positional information such as the line
    /// number and column index in the source code.
    /// </param>
    /// <returns>
    /// A <see cref="SourcePosition"/> instance representing the location of the token
    /// in the source file, including the file path, line number, and column index.
    /// </returns>
    private SourcePosition ToSourcePosition(IToken token)
        => new(_filePath, token.Line, token.Column + 1); // ANTLR columns are 0-based
}