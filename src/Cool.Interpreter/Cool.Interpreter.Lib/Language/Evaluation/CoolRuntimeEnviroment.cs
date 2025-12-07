using System.Collections.Immutable;
using Cool.Interpreter.Lib.Core.Exeptions;
using Cool.Interpreter.Lib.Core.Syntax.Ast;
using Cool.Interpreter.Lib.Core.Syntax.Ast.Expressions;
using Cool.Interpreter.Lib.Core.Syntax.Ast.Features;
using Cool.Interpreter.Lib.Language.Classes;
using Cool.Interpreter.Lib.Language.Classes.BuiltIn;
using Cool.Interpreter.Lib.Language.Symbols;

namespace Cool.Interpreter.Lib.Language.Evaluation;

public class CoolRuntimeEnvironment
{
    private readonly TextWriter _output;
    private readonly TextReader _input;

    public CoolRuntimeEnvironment(SymbolTable symbolTable, TextWriter? output = null, TextReader? input = null)
    {
        this.SymbolTable = symbolTable ?? throw new ArgumentNullException(nameof(symbolTable));
        _output = output ?? Console.Out;
        _input = input ?? Console.In;
    }

    public SymbolTable SymbolTable { get; set; }


    
    
}