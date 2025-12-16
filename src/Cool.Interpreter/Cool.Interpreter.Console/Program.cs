//-----------------------------------------------------------------------
// <copyright file="Program.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Michael Füby, Armin Zimmerling, Mahmoud Ibrahim</author>
// <summary>Program</summary>
//-----------------------------------------------------------------------

namespace Cool.Interpreter.Console;

using System;
using Cool.Interpreter.Lib.Core.Diagnostics;
using Cool.Interpreter.Lib.Core.Syntax;
using Cool.Interpreter.Lib.Language.Interpretation;

/// <summary>
/// Represents the entry point of the application, providing functionality to execute Cool programs
/// either through a built-in example or by interpreting a provided file.
/// This class cannot be inherited.
/// </summary>
public static class Program
{
    /// <summary>
    /// Serves as the entry point for the program. Executes Cool programs either from a built-in example
    /// or by interpreting a file provided through the command-line arguments.
    /// </summary>
    /// <param name="args">
    /// An array of strings representing the command-line arguments. If no arguments are provided,
    /// a built-in test Cool program is executed. If one argument is provided and it is a file path,
    /// the file is interpreted. Passing "--help" or "-h" as an argument displays help instructions.
    /// </param>
    /// <returns>
    /// Returns 0 if the program executes successfully or with user-friendly exits such as help display.
    /// Returns 1 for general errors or if the program encounters issues such as file not found,
    /// too many arguments, or runtime errors during interpretation.
    /// </returns>
    public static int Main(string[] args)
    {
        var interpreter = new CoolInterpreter();

        InterpretationResult result;

        if (args.Length == 0)
        {
            // No arguments: run a simple built-in test program
            Console.WriteLine("No file provided. Running built-in test program...\n");
            result = interpreter.Run(@"
class Main {
    main(): Int {
        1 + 2 * 3
    };
};
");
            PrintResult(result);
            return !result.HasErrors ? 0 : 1;
        }

        if (args.Length == 1)
        {
            string arg = args[0].Trim();

            if (arg == "--help" || arg == "-h")
            {
                PrintHelp();
                return 0;
            }

            // Assume it's a file path
            string filePath = arg;

            try
            {
                result = interpreter.RunFile(filePath);
                PrintResult(result);
                return !result.HasErrors ? 0 : 1;
            }
            catch (FileNotFoundException ex)
            {
                Console.Error.WriteLine($"Error: {ex.Message}");
                return 1;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error reading file: {ex.Message}");
                return 1;
            }
        }

        Console.Error.WriteLine("Error: Too many arguments.");
        PrintHelp();
        return 1;
    }

    /// <summary>
    /// Prints the results of an interpretation to the console, including output,
    /// returned value, diagnostics, and overall interpretation status.
    /// </summary>
    /// <param name="result">
    /// The <see cref="InterpretationResult"/> object containing the details of the interpretation
    /// result, such as success status, output, returned value, and diagnostics information.
    /// </param>
    private static void PrintResult(InterpretationResult result)
    {
        if (!string.IsNullOrEmpty(result.Output))
            Console.WriteLine(result.Output);

        if (result.ReturnedValue != null)
            Console.WriteLine($"--> {result.ReturnedValue}");

        if (result.Diagnostics.Length > 0)
        {
            Console.WriteLine("\nDiagnostics:");
            foreach (var d in result.Diagnostics)
            {
                var color = d.Severity == DiagnosticSeverity.Error ? ConsoleColor.Red : ConsoleColor.Yellow;
                Console.ForegroundColor = color;
                Console.WriteLine($"[{ (d.Severity == DiagnosticSeverity.Error ? "Error" : "Warning") }] {d.Message}");
                Console.ResetColor();

                if (d.Location != SourcePosition.None)
                    Console.WriteLine($"    at {d.Location}");
            }
        }

        var statusColor = result.IsSuccess ? ConsoleColor.Green : ConsoleColor.Red;
        Console.ForegroundColor = statusColor;
        Console.WriteLine($"\nStatus: {(result.IsSuccess ? "Success" : "Failed")}");
        Console.ResetColor();
    }

    /// <summary>
    /// Displays a help message in the console, providing details on how to use the Cool interpreter,
    /// including command-line options and usage examples.
    /// </summary>
    private static void PrintHelp()
    {
        Console.WriteLine(@"Cool Interpreter

Usage:
  cool.exe                     - Run built-in test program
  cool.exe <file.cool>         - Run Cool program from file
  cool.exe --help | -h         - Show this help

Examples:
  cool.exe hello.cool
  cool.exe -h
");
    }
}