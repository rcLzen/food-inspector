using System.Text.RegularExpressions;

namespace FoodInspector.Services;

public interface IIngredientNormalizationService
{
    /// <summary>
    /// Tokenizes a raw ingredients string into individual ingredient tokens.
    /// Multi-word ingredients (e.g., "milk solids", "yeast extract") are preserved as single tokens.
    /// Splits on commas, parentheses, and semicolons, then normalizes whitespace.
    /// </summary>
    List<string> Tokenize(string rawIngredientsText);
}

public class IngredientNormalizationService : IIngredientNormalizationService
{
    public List<string> Tokenize(string rawIngredientsText)
    {
        if (string.IsNullOrWhiteSpace(rawIngredientsText))
        {
            return new List<string>();
        }

        var normalized = rawIngredientsText.ToLowerInvariant();
        normalized = normalized.Replace("(", ",").Replace(")", ",").Replace(";", ",");
        normalized = Regex.Replace(normalized, "\\s+", " ");

        return normalized
            .Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
            .Select(x => x.Trim())
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .ToList();
    }
}
