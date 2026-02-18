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
    public TimelineFlags TimelineFlags { get; set; } = new();
    public string TimelineExplanation { get; set; } = string.Empty;
    public List<string> TimelineEvidenceReferences { get; set; } = new();
    public List<TimelineBucketDetail> TimelineBuckets { get; set; } = new();
    public ImpactType ImpactType { get; set; } = ImpactType.Unknown;
    public string ImpactExplanation { get; set; } = string.Empty;
    public string? ImpactCitationShort { get; set; }
}

public class IngredientAnalysisService : IIngredientAnalysisService
{
    private readonly IMatchingService _matchingService;
    private readonly IInflammationTimelineService _timelineService;

    public IngredientAnalysisService(IMatchingService matchingService, IInflammationTimelineService timelineService)
    {
        _matchingService = matchingService;
        _timelineService = timelineService;
    }

    public async Task<AnalysisResult> AnalyzeIngredientsAsync(string ingredients, bool isFlareMode, int flareModeThreshold)
    {
        if (string.IsNullOrWhiteSpace(ingredients))
        {
            return new AnalysisResult
            {
                SafetyLevel = SafetyLevel.NotFound,
                Summary = "No ingredients provided.",
                ImpactType = ImpactType.Unknown,
                ImpactExplanation = "Unknown: no matched trigger had enough evidence to classify symptom vs inflammation risk."
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
            SafetyLevel.Safe => "SAFE - All ingredients recognized with no triggers found.",
            SafetyLevel.NotFound => "NOT FOUND - No recognized ingredients matched our database.",
            _ => "NOT FOUND - No recognized ingredients matched our database."
        };

        var timeline = await _timelineService.BuildTimelineAsync(result.DirectMatches.Concat(result.CrossReactiveMatches));
        result.TimelineFlags = timeline.Flags;
        result.TimelineExplanation = timeline.Explanation;
        result.TimelineEvidenceReferences = timeline.EvidenceReferences;
        result.TimelineBuckets = timeline.Buckets;
        var impact = BuildImpactInsight(result);
        result.ImpactType = impact.Type;
        result.ImpactExplanation = impact.Explanation;
        result.ImpactCitationShort = impact.CitationShort;

        return result;
    }

    private static (ImpactType Type, string Explanation, string? CitationShort) BuildImpactInsight(AnalysisResult result)
    {
        if (!result.DirectMatches.Any() && !result.CrossReactiveMatches.Any())
        {
            return (ImpactType.Unknown, "Unknown: no matched trigger had enough evidence to classify symptom vs inflammation risk.", null);
        }

        var matches = result.DirectMatches.Concat(result.CrossReactiveMatches).ToList();
        var hasCrossReactiveEvidence = result.CrossReactiveMatches.Any(x => !string.IsNullOrWhiteSpace(x.EvidenceCitationShort));
        var hasImmuneCategory = matches.Any(x =>
            string.Equals(x.TriggerCategory, "Cereal", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(x.TriggerCategory, "Legume", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(x.TriggerCategory, "Nut", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(x.TriggerCategory, "Seed", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(x.TriggerCategory, "Egg", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(x.TriggerCategory, "Seafood", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(x.TriggerCategory, "Other", StringComparison.OrdinalIgnoreCase));
        var hasHighSeverityNonLactose = matches.Any(x =>
            x.Severity == TriggerSeverity.High &&
            !string.Equals(x.TriggerName, "Lactose", StringComparison.OrdinalIgnoreCase));

        if (hasCrossReactiveEvidence || hasImmuneCategory || hasHighSeverityNonLactose)
        {
            var citation = result.CrossReactiveMatches.Select(x => x.EvidenceCitationShort)
                .Concat(result.TimelineEvidenceReferences)
                .FirstOrDefault(x => !string.IsNullOrWhiteSpace(x));
            return (ImpactType.InflammationLikely, "Possible inflammatory trigger: matched patterns are more consistent with immune activation or barrier stress.", citation);
        }

        var hasSymptomPattern = matches.Any(x => string.Equals(x.TriggerName, "Lactose", StringComparison.OrdinalIgnoreCase)) ||
            matches.All(x =>
                string.Equals(x.TriggerCategory, "Additive", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(x.TriggerCategory, "Nightshade", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(x.TriggerCategory, "Fungi/Yeast", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(x.TriggerCategory, "Fruit", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(x.TriggerCategory, "Vegetable", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(x.TriggerCategory, "Seed Oil", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(x.TriggerCategory, "Dairy", StringComparison.OrdinalIgnoreCase));

        if (hasSymptomPattern)
        {
            return (ImpactType.SymptomLikely, "Likely symptom trigger: pattern is more consistent with irritation, intolerance, osmotic load, or motility effects.", null);
        }

        return (ImpactType.Unknown, "Unknown: evidence is mixed or limited, so this result cannot be confidently classified.", null);
    }
}
