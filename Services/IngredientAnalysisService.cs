using FoodInspector.Models;

namespace FoodInspector.Services;

public interface IIngredientAnalysisService
{
    Task<AnalysisResult> AnalyzeIngredientsAsync(string ingredients, bool isFlareMode, int flareModeThreshold);
}

public class AnalysisResult
{
    public SafetyLevel SafetyLevel { get; set; }
    public List<string> DetectedTriggers { get; set; } = new();
    public List<string> Warnings { get; set; } = new();
    public string Summary { get; set; } = string.Empty;
    public List<MatchDetail> DirectMatches { get; set; } = new();
    public List<MatchDetail> CrossReactiveMatches { get; set; } = new();
}

public class IngredientAnalysisService : IIngredientAnalysisService
{
    private readonly IMatchingService _matchingService;

    public IngredientAnalysisService(IMatchingService matchingService)
    {
        _matchingService = matchingService;
    }

    /// <summary>
    /// Analyzes ingredients text to detect triggers and cross-reactive matches.
    /// </summary>
    /// <param name="ingredients">The ingredient list text to analyze.</param>
    /// <param name="isFlareMode">If true, escalates Moderate severity findings to AVOID status.</param>
    /// <param name="flareModeThreshold">DEPRECATED: This parameter is no longer used. Flare mode now uses an all-or-nothing escalation approach (Moderate → AVOID) rather than a threshold-based system. Retained for backward compatibility.</param>
    /// <returns>Analysis result with safety level, detected triggers, and warnings.</returns>
    public async Task<AnalysisResult> AnalyzeIngredientsAsync(string ingredients, bool isFlareMode, int flareModeThreshold)
    {
        if (string.IsNullOrWhiteSpace(ingredients))
        {
            return new AnalysisResult
            {
                SafetyLevel = SafetyLevel.Safe,
                Summary = "No ingredients detected."
            };
        }

        var matched = await _matchingService.MatchAsync(ingredients, isFlareMode);
        var result = new AnalysisResult
        {
            SafetyLevel = matched.FinalStatus,
            DirectMatches = matched.DirectMatches,
            CrossReactiveMatches = matched.CrossReactiveMatches,
            DetectedTriggers = matched.DirectMatches.Select(x => x.TriggerName).Distinct().ToList()
        };

        result.Warnings = matched.DirectMatches
            .Select(x => $"{x.TriggerName} ({x.Reason})")
            .Concat(matched.CrossReactiveMatches.Select(x =>
                $"Cross-reactive: {x.SourceName} → {x.TriggerName} [{x.Strength}] - {x.EvidenceCitationShort}"))
            .ToList();

        result.Summary = matched.FinalStatus switch
        {
            SafetyLevel.Avoid => isFlareMode ? "AVOID - Flare mode escalation active." : "AVOID - High-risk triggers found.",
            SafetyLevel.Caution => "CAUTION - Moderate-risk triggers found.",
            _ => matched.DirectMatches.Any() || matched.CrossReactiveMatches.Any() 
                ? "SAFE - Only low-severity triggers detected." 
                : "SAFE - No mapped triggers found."
        };

        return result;
    }
}
