// See https://aka.ms/new-console-template for more information

using Cool.Interpreter.Lib.Language.Interpretation;

var interpreter = new CoolInterpreter();
var result = interpreter.Run("class Main {\nf(): Int { 1 };\n};");

foreach (var diagnostic in result.Diagnostics)
{
    Console.WriteLine(diagnostic.Message);
    Console.WriteLine(result.Output);
}