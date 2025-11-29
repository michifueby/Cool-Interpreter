//-----------------------------------------------------------------------
// <copyright file="SourcePosition.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Michael Füby, Armin Zimmerling, Mahmoud Ibrahim</author>
// <summary>SourcePosition</summary>
//-----------------------------------------------------------------------

namespace Cool.Interpreter.Lib.Core.Syntax;

/// <summary>
/// Represents an exact position in a source file using 1-based line and column numbers.
/// This is the standard type for pointing at "where something happened" in source code.
/// </summary>
public sealed class SourcePosition : IEquatable<SourcePosition>, IComparable<SourcePosition>
{
    /// <summary>
    /// Gets the path or name of the source file (e.g., "examples/Hello.cool", "REPL", "&lt;stdin&gt;").
    /// May be null for synthetic or in-memory sources.
    /// </summary>
    public string? FilePath { get; }

    /// <summary>
    /// Gets the line number (1-based). The first line in a file is line 1.
    /// </summary>
    public int Line { get; }

    /// <summary>
    /// Gets the column number (1-based). The first character on a line is column 1.
    /// </summary>
    public int Column { get; }

    /// <summary>
    /// Gets the zero-based line index (used internally for text processing).
    /// </summary>
    public int LineIndex => Line - 1;

    /// <summary>
    /// Gets the zero-based column index (used internally for string indexing).
    /// </summary>
    public int ColumnIndex => Column - 1;

    /// <summary>
    /// Initializes a new instance of the <see cref="SourcePosition"/> class.
    /// </summary>
    /// <param name="filePath">The source file path or identifier. Can be null.</param>
    /// <param name="line">1-based line number.</param>
    /// <param name="column">1-based column number.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="line"/> or <paramref name="column"/> is less than 1.
    /// </exception>
    public SourcePosition(string? filePath, int line, int column)
    {
        if (line < 1) throw new ArgumentOutOfRangeException(nameof(line), "Line must be >= 1");
        if (column < 1) throw new ArgumentOutOfRangeException(nameof(column), "Column must be >= 1");

        FilePath = filePath;
        Line = line;
        Column = column;
    }

    /// <summary>
    /// Creates a <see cref="SourcePosition"/> without a file path (e.g., for tests).
    /// </summary>
    public static SourcePosition None => new(null, 1, 1);

    /// <summary>
    /// Returns a formatted string suitable for error messages and IDE display.
    /// </summary>
    /// <example>Hello.cool(12,8)</example>
    public override string ToString()
    {
        var path = string.IsNullOrEmpty(FilePath) ? "<unknown>" : FilePath;
        return $"{path}({Line},{Column})";
    }

    public bool Equals(SourcePosition? other)
        => other is not null &&
           FilePath == other.FilePath &&
           Line == other.Line &&
           Column == other.Column;

    public override bool Equals(object? obj) => Equals(obj as SourcePosition);

    public override int GetHashCode() => HashCode.Combine(FilePath, Line, Column);

    public int CompareTo(SourcePosition? other)
    {
        if (other is null) return 1;

        if (FilePath != other.FilePath)
            return string.Compare(FilePath, other.FilePath, StringComparison.Ordinal);

        var lineComp = Line.CompareTo(other.Line);
        return lineComp != 0 ? lineComp : Column.CompareTo(other.Column);
    }

    public static bool operator ==(SourcePosition? left, SourcePosition? right)
        => Equals(left, right);
    public static bool operator !=(SourcePosition? left, SourcePosition? right)
        => !Equals(left, right);
}