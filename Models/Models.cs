namespace FoodInspector.Models;

public enum SafetyLevel
{
    Safe,
    Caution,
    Avoid,
    NotFound
}

public enum TriggerSeverity
{
    Low,
    Moderate,
    High
}

public enum TriggerMatchType
{
    Exact,
    WordBoundaryContains,
    Contains
}

public enum CrossReactivityStrength
{
    Low,
    Medium,
    High
}

public enum ScanMatchReason
{
    Direct,
    Synonym,
    CrossReact
}

public enum ImpactType
{
    SymptomLikely,
    InflammationLikely,
    Unknown
}

public class Trigger
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public TriggerSeverity Severity { get; set; }
    public bool Enabled { get; set; } = true;
}

public class TriggerSynonym
{
    public int Id { get; set; }
    public int TriggerId { get; set; }
    public string SynonymText { get; set; } = string.Empty;
    public TriggerMatchType MatchType { get; set; } = TriggerMatchType.WordBoundaryContains;
    public Trigger? Trigger { get; set; }
}

public class EvidenceSource
{
    public int Id { get; set; }
    public string CitationShort { get; set; } = string.Empty;
    public string CitationFull { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public string ScopeTag { get; set; } = string.Empty;
}

public class EvidenceTimingProfile
{
    public int Id { get; set; }
    public int? TriggerId { get; set; }
    public string? TriggerCategory { get; set; }
    public bool Acute { get; set; }
    public bool Subacute { get; set; }
    public bool Chronic { get; set; }
    public string Why { get; set; } = string.Empty;
    public int EvidenceSourceId { get; set; }
    public bool Enabled { get; set; } = true;

    public Trigger? Trigger { get; set; }
    public EvidenceSource? EvidenceSource { get; set; }
}

public class CrossReactivityRule
{
    public int Id { get; set; }
    public int? SourceTriggerId { get; set; }
    public string? SourceCategory { get; set; }
    public int? TargetTriggerId { get; set; }
    public string? TargetCategory { get; set; }
    public CrossReactivityStrength Strength { get; set; }
    public int EvidenceSourceId { get; set; }
    public string Notes { get; set; } = string.Empty;
    public bool Enabled { get; set; } = true;

    public Trigger? SourceTrigger { get; set; }
    public Trigger? TargetTrigger { get; set; }
    public EvidenceSource? EvidenceSource { get; set; }
}

public class ScanRecord
{
    public int Id { get; set; }
    public DateTime CreatedUtc { get; set; }
    public string RawIngredientsText { get; set; } = string.Empty;
    public string? Barcode { get; set; }
    public string? ProductName { get; set; }
    public SafetyLevel FinalStatus { get; set; }
    public bool FlareModeOn { get; set; }
    public string AnalysisSummary { get; set; } = string.Empty;
    public ImpactType ImpactType { get; set; } = ImpactType.Unknown;
    public string ImpactExplanation { get; set; } = string.Empty;
    public string? ImpactCitationShort { get; set; }
    public List<ScanMatch> Matches { get; set; } = new();
}

public class ScanMatch
{
    public int Id { get; set; }
    public int ScanRecordId { get; set; }
    public int TriggerId { get; set; }
    public string MatchedText { get; set; } = string.Empty;
    public ScanMatchReason Reason { get; set; }
    public int? EvidenceSourceId { get; set; }

    public ScanRecord? ScanRecord { get; set; }
    public Trigger? Trigger { get; set; }
    public EvidenceSource? EvidenceSource { get; set; }
}

public class AppSettings
{
    public int Id { get; set; }
    public bool IsFlareMode { get; set; }
    public int FlareModeThreshold { get; set; } = 5;
}
