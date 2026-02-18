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
            _ => "SAFE - No mapped triggers found."
        };

        var timeline = await _timelineService.BuildTimelineAsync(result.DirectMatches.Concat(result.CrossReactiveMatches));
        result.TimelineFlags = timeline.Flags;
        result.TimelineExplanation = timeline.Explanation;
        result.TimelineEvidenceReferences = timeline.EvidenceReferences;
        result.TimelineBuckets = timeline.Buckets;

        return result;
    }
}
