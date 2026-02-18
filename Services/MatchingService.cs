using System.Text.RegularExpressions;
using FoodInspector.Models;

namespace FoodInspector.Services;

public class MatchDetail
{
    public int TriggerId { get; set; }
    public string TriggerName { get; set; } = string.Empty;
    public string? TriggerCategory { get; set; }
    public string MatchedText { get; set; } = string.Empty;
    public ScanMatchReason Reason { get; set; }
    public TriggerSeverity Severity { get; set; }
    public int? EvidenceSourceId { get; set; }
    public string? EvidenceCitationShort { get; set; }
    public string? EvidenceCitationFull { get; set; }
    public string? EvidenceSummary { get; set; }
    public string? SourceName { get; set; }
    public string? Strength { get; set; }
    public bool IsExpanded { get; set; } = false;
}

public class MatchResult
{
    public List<MatchDetail> DirectMatches { get; set; } = new();
    public List<MatchDetail> CrossReactiveMatches { get; set; } = new();
    public SafetyLevel FinalStatus { get; set; }
}

public interface IMatchingService
{
    Task<MatchResult> MatchAsync(string ingredients, bool flareModeOn);
}

public class MatchingService : IMatchingService
{
    private readonly IDatabaseService _databaseService;
    private readonly IIngredientNormalizationService _normalizationService;

    public MatchingService(IDatabaseService databaseService, IIngredientNormalizationService normalizationService)
    {
        _databaseService = databaseService;
        _normalizationService = normalizationService;
    }

    public async Task<MatchResult> MatchAsync(string ingredients, bool flareModeOn)
    {
        var result = new MatchResult();
        var tokens = _normalizationService.Tokenize(ingredients);
        var triggers = await _databaseService.GetAllTriggersAsync();
        var synonyms = await _databaseService.GetAllSynonymsAsync();
        var rules = await _databaseService.GetCrossReactivityRulesAsync();

        var directTriggerIds = new HashSet<int>();

        foreach (var trigger in triggers.Where(x => x.Enabled))
        {
            if (tokens.Any(token => string.Equals(token, trigger.Name.ToLowerInvariant(), StringComparison.OrdinalIgnoreCase)))
            {
                directTriggerIds.Add(trigger.Id);
                result.DirectMatches.Add(new MatchDetail
                {
                    TriggerId = trigger.Id,
                    TriggerName = trigger.Name,
                    TriggerCategory = trigger.Category,
                    MatchedText = trigger.Name,
                    Reason = ScanMatchReason.Direct,
                    Severity = trigger.Severity
                });
            }
        }

        foreach (var synonym in synonyms.Where(x => x.Trigger != null && x.Trigger.Enabled))
        {
            var matched = tokens.FirstOrDefault(token => IsMatch(token, synonym.SynonymText, synonym.MatchType));
            if (matched == null)
            {
                continue;
            }

            if (directTriggerIds.Add(synonym.TriggerId))
            {
                result.DirectMatches.Add(new MatchDetail
                {
                    TriggerId = synonym.TriggerId,
                    TriggerName = synonym.Trigger!.Name,
                    TriggerCategory = synonym.Trigger.Category,
                    MatchedText = matched,
                    Reason = ScanMatchReason.Synonym,
                    Severity = synonym.Trigger.Severity
                });
            }
        }

        var directByCategory = triggers
            .Where(x => directTriggerIds.Contains(x.Id))
            .Select(x => x.Category)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        foreach (var rule in rules.Where(x => x.Enabled))
        {
            var sourceMatch =
                (rule.SourceTriggerId.HasValue && directTriggerIds.Contains(rule.SourceTriggerId.Value)) ||
                (!string.IsNullOrWhiteSpace(rule.SourceCategory) && directByCategory.Contains(rule.SourceCategory));

            if (!sourceMatch)
            {
                continue;
            }

            var targetTriggers = triggers.Where(t => t.Enabled &&
                ((rule.TargetTriggerId.HasValue && t.Id == rule.TargetTriggerId.Value) ||
                 (!string.IsNullOrWhiteSpace(rule.TargetCategory) && string.Equals(t.Category, rule.TargetCategory, StringComparison.OrdinalIgnoreCase))));

            foreach (var target in targetTriggers)
            {
                if (directTriggerIds.Contains(target.Id) || result.CrossReactiveMatches.Any(x => x.TriggerId == target.Id))
                {
                    continue;
                }

                result.CrossReactiveMatches.Add(new MatchDetail
                {
                    TriggerId = target.Id,
                    TriggerName = target.Name,
                    TriggerCategory = target.Category,
                    MatchedText = target.Name,
                    Reason = ScanMatchReason.CrossReact,
                    Severity = target.Severity,
                    EvidenceSourceId = rule.EvidenceSourceId,
                    EvidenceCitationShort = rule.EvidenceSource?.CitationShort,
                    EvidenceCitationFull = rule.EvidenceSource?.CitationFull,
                    EvidenceSummary = rule.EvidenceSource?.Summary,
                    SourceName = rule.SourceTrigger?.Name ?? rule.SourceCategory,
                    Strength = rule.Strength.ToString()
                });
            }
        }

        var allSeverities = result.DirectMatches.Select(x => x.Severity)
            .Concat(result.CrossReactiveMatches.Select(x => x.Severity));

        result.FinalStatus = ComputeFinalStatus(allSeverities, flareModeOn);
        return result;
    }

    private static bool IsMatch(string token, string pattern, TriggerMatchType matchType)
    {
        return matchType switch
        {
            TriggerMatchType.Exact => string.Equals(token, pattern, StringComparison.OrdinalIgnoreCase),
            TriggerMatchType.Contains => token.Contains(pattern, StringComparison.OrdinalIgnoreCase),
            _ => Regex.IsMatch(token, $"\\b{Regex.Escape(pattern)}\\b", RegexOptions.IgnoreCase)
        };
    }

    private static SafetyLevel ComputeFinalStatus(IEnumerable<TriggerSeverity> severities, bool flareModeOn)
    {
        if (!severities.Any())
        {
            return SafetyLevel.NotFound;
        }

        if (severities.Any(x => x == TriggerSeverity.High))
        {
            return SafetyLevel.Avoid;
        }

        if (severities.Any(x => x == TriggerSeverity.Moderate))
        {
            return flareModeOn ? SafetyLevel.Avoid : SafetyLevel.Caution;
        }

        return SafetyLevel.Safe;
    }
}
