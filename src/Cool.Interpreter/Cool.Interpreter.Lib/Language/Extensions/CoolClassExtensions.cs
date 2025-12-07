using Cool.Interpreter.Lib.Core.Syntax.Ast;
using Cool.Interpreter.Lib.Core.Syntax.Ast.Features;
using Cool.Interpreter.Lib.Language.Classes;

namespace Cool.Interpreter.Lib.Language.Extensions;

public static class CoolClassExtensions
{
    /// <summary>
    /// Returns all attributes (inherited + local) in the exact order they must be initialized.
    /// Cool spec §5: "Inherited attributes are initialized first in inheritance order...
    /// Within a given class, attributes are initialized in the order they appear in the source text."
    /// </summary>
    public static IEnumerable<AttributeNode> GetAllAttributesInInheritanceOrder(this CoolClass @class)
    {
        var seen = new HashSet<string>(StringComparer.Ordinal);

        // Walk from root to this class
        var current = @class;
        var reversed = new Stack<List<AttributeNode>>();

        while (current is not null)
        {
            var localAttrs = current.Attributes.Values
                .Where(a => seen.Add(a.Name)) // only first declaration wins
                .OrderBy(a => a.SourceOrder) // source order within class
                .ToList();

            if (localAttrs.Count > 0)
                reversed.Push(localAttrs);

            current = current.Parent;
        }

        // Reverse to get root → leaf order
        while (reversed.Count > 0)
        {
            foreach (var attr in reversed.Pop())
                yield return attr;
        }
    }
}