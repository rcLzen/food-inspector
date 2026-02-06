using System.Text.RegularExpressions;

namespace FoodInspector.Services;

public interface IIngredientNormalizationService
{
    /// <summary>
    /// Tokenizes raw ingredient text into normalized tokens for matching.
    /// </summary>
    /// <param name="rawIngredientsText">The raw ingredient list text to tokenize.</param>
    /// <returns>A list of normalized, trimmed tokens ready for matching.</returns>
    /// <remarks>
    /// Tokenization behavior:
    /// - Splits on commas as the primary delimiter
    /// - Treats parentheses and semicolons as commas
    /// - Preserves spaces within tokens to support multi-word ingredients (e.g., "milk solids", "yeast extract")
    /// - Performs case-insensitive normalization (converts to lowercase)
    /// - Removes leading/trailing whitespace from each token
    /// - Filters out empty tokens
    /// </remarks>
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
