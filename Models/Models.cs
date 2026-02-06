namespace FoodInspector.Models;

public enum SafetyLevel
{
    Safe,
    Caution,
    Avoid
}

public class FoodScanHistory
{
    public int Id { get; set; }
    public DateTime ScanDate { get; set; }
    public string? Barcode { get; set; }
    public string? ProductName { get; set; }
    public string Ingredients { get; set; } = string.Empty;
    public SafetyLevel SafetyLevel { get; set; }
    public string Analysis { get; set; } = string.Empty;
    public bool IsFlareMode { get; set; }
    public string? ImagePath { get; set; }
    public string? OpenFoodFactsData { get; set; }
}

public class IngredientTrigger
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public SafetyLevel SafetyLevel { get; set; }
    public string Description { get; set; } = string.Empty;
    public int SeverityScore { get; set; } // 1-10, used for Flare Mode
    public bool IsCommonAllergen { get; set; }
}

public class IngredientSynonym
{
    public int Id { get; set; }
    public int IngredientTriggerId { get; set; }
    public string Synonym { get; set; } = string.Empty;
    public IngredientTrigger? IngredientTrigger { get; set; }
}

public class CrossReactivity
{
    public int Id { get; set; }
    public int PrimaryTriggerId { get; set; }
    public int RelatedTriggerId { get; set; }
    public string Description { get; set; } = string.Empty;
    public IngredientTrigger? PrimaryTrigger { get; set; }
    public IngredientTrigger? RelatedTrigger { get; set; }
}

public class AppSettings
{
    public int Id { get; set; }
    public bool IsFlareMode { get; set; }
    public int FlareModeThreshold { get; set; } = 5; // Triggers with severity >= this are flagged in Flare Mode
}
