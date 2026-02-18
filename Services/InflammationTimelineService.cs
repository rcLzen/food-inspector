using FoodInspector.Models;

namespace FoodInspector.Services;

public class TimelineFlags
{
    public bool Acute { get; set; }
    public bool Subacute { get; set; }
    public bool Chronic { get; set; }
}

public class TimelineBucketDetail
{
    public string Label { get; set; } = string.Empty;
    public bool IsApplicable { get; set; }
    public string Why { get; set; } = string.Empty;
    public List<string> EvidenceCitations { get; set; } = new();
}

public class InflammationTimelineResult
{
    public TimelineFlags Flags { get; set; } = new();
    public string Explanation { get; set; } = string.Empty;
    public List<string> EvidenceReferences { get; set; } = new();
    public List<TimelineBucketDetail> Buckets { get; set; } = new();
}

public interface IInflammationTimelineService
{
    Task<InflammationTimelineResult> BuildTimelineAsync(IEnumerable<MatchDetail> matches);
}

public class InflammationTimelineService : IInflammationTimelineService
{
    private readonly IDatabaseService _databaseService;

    public InflammationTimelineService(IDatabaseService databaseService)
    {
        _databaseService = databaseService;
    }

    public async Task<InflammationTimelineResult> BuildTimelineAsync(IEnumerable<MatchDetail> matches)
    {
        var matchedList = matches.ToList();
        if (!matchedList.Any())
        {
            return CreateEmptyResult();
        }

        var matchedTriggerIds = matchedList
            .Select(x => x.TriggerId)
            .ToHashSet();

        var matchedCategories = matchedList
            .Select(x => x.TriggerCategory)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Cast<string>()
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var profiles = await _databaseService.GetEvidenceTimingProfilesAsync();
        var applicableProfiles = profiles
            .Where(x =>
                (x.TriggerId.HasValue && matchedTriggerIds.Contains(x.TriggerId.Value)) ||
                (!string.IsNullOrWhiteSpace(x.TriggerCategory) && matchedCategories.Contains(x.TriggerCategory)))
            .ToList();

        if (!applicableProfiles.Any())
        {
            return CreateEmptyResult();
        }

        var flags = new TimelineFlags
        {
            Acute = applicableProfiles.Any(x => x.Acute),
            Subacute = applicableProfiles.Any(x => x.Subacute),
            Chronic = applicableProfiles.Any(x => x.Chronic)
        };

        var explanation = string.Join(" ", applicableProfiles
            .Select(x => x.Why)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct());

        var references = applicableProfiles
            .Select(x => x.EvidenceSource?.CitationShort)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Cast<string>()
            .Distinct()
            .ToList();

        return new InflammationTimelineResult
        {
            Flags = flags,
            Explanation = explanation,
            EvidenceReferences = references,
            Buckets = BuildBuckets(applicableProfiles)
        };
    }

    private static List<TimelineBucketDetail> BuildBuckets(List<EvidenceTimingProfile> applicableProfiles)
    {
        return new List<TimelineBucketDetail>
        {
            BuildBucket("Acute (0–24h)", x => x.Acute, applicableProfiles),
            BuildBucket("Subacute (1–7 days)", x => x.Subacute, applicableProfiles),
            BuildBucket("Chronic/Cumulative (weeks+)", x => x.Chronic, applicableProfiles)
        };
    }

    private static TimelineBucketDetail BuildBucket(string label, Func<EvidenceTimingProfile, bool> selector, List<EvidenceTimingProfile> profiles)
    {
        var bucketProfiles = profiles.Where(selector).ToList();
        return new TimelineBucketDetail
        {
            Label = label,
            IsApplicable = bucketProfiles.Any(),
            Why = string.Join(" ", bucketProfiles.Select(x => x.Why).Where(x => !string.IsNullOrWhiteSpace(x)).Distinct()),
            EvidenceCitations = bucketProfiles
                .Select(x => x.EvidenceSource?.CitationShort)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Cast<string>()
                .Distinct()
                .ToList()
        };
    }

    private static InflammationTimelineResult CreateEmptyResult() => new()
    {
        Buckets = new List<TimelineBucketDetail>
        {
            new() { Label = "Acute (0–24h)" },
            new() { Label = "Subacute (1–7 days)" },
            new() { Label = "Chronic/Cumulative (weeks+)" }
        }
    };
}
