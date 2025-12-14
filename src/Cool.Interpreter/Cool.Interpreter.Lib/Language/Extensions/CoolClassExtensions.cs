using Cool.Interpreter.Lib.Core.Syntax.Ast;
using Cool.Interpreter.Lib.Core.Syntax.Ast.Features;
using Cool.Interpreter.Lib.Language.Classes;

namespace Cool.Interpreter.Lib.Language.Extensions;

using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Cool language extensions for CoolClass.
/// </summary>
public static class CoolClassExtensions
{
    /// <summary>
    /// Returns all attributes (inherited + declared) in the exact order they must be initialized.
    /// 
    /// Cool specification §5:
    /// "Attributes are initialized in the order of inheritance (parent before child)
    /// and within each class in the order they appear in the source text.
    /// If an attribute is redefined in a subclass, only the most-derived declaration is used."
    /// </summary>
    /// <param name="coolClass">The class to get attributes for.</param>
    /// <returns>
    /// Attributes in initialization order: root class first → leaf class last,
    /// within each class: source order, with shadowing applied correctly.
    /// </returns>
    public static IEnumerable<AttributeNode> GetAllAttributesInOrder(this CoolClass coolClass)
    {
        if (coolClass is null) 
            yield break;

        var seen = new HashSet<string>(StringComparer.Ordinal);
        var stack = new Stack<List<AttributeNode>>();

        // Walk up the inheritance chain: this class → parent → ... → Object
        var current = coolClass;
        while (current != null)
        {
            // Get attributes declared in this class, in source order
            var localAttrs = current.Attributes
                .Values
                .Where(attr => seen.Add(attr.Name)) // first declaration wins (shadowing)
                .OrderBy(attr => attr.SourceOrder)  // preserve source order
                .ToList();

            if (localAttrs.Count > 0)
                stack.Push(localAttrs);

            current = current.Parent;
        }

        // Emit from root → leaf (correct initialization order)
        while (stack.Count > 0)
        {
            foreach (var attr in stack.Pop())
                yield return attr;
        }
    }

    /// <summary>
    /// Returns all attributes including inherited ones, with correct shadowing.
    /// Same as GetAllAttributesInOrder but returns a list for convenience.
    /// </summary>
    public static IReadOnlyList<AttributeNode> GetAllAttributes(this CoolClass coolClass) =>
        coolClass.GetAllAttributesInOrder().ToList().AsReadOnly();
}