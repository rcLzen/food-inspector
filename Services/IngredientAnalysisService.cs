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
}

public class IngredientAnalysisService : IIngredientAnalysisService
{
    private readonly IDatabaseService _databaseService;

    public IngredientAnalysisService(IDatabaseService databaseService)
    {
        _databaseService = databaseService;
    }

    public async Task<AnalysisResult> AnalyzeIngredientsAsync(string ingredients, bool isFlareMode, int flareModeThreshold)
    {
        var result = new AnalysisResult();
        
        if (string.IsNullOrWhiteSpace(ingredients))
        {
            result.SafetyLevel = SafetyLevel.Safe;
            result.Summary = "No ingredients detected.";
            return result;
        }

        // Get triggers and synonyms from database
        var triggers = await _databaseService.GetAllTriggersAsync();
        var synonyms = await _databaseService.GetAllSynonymsAsync();
        var crossReactivities = await _databaseService.GetAllCrossReactivitiesAsync();

        // Normalize ingredients text
        var ingredientsLower = ingredients.ToLower();
        var detectedTriggerIds = new HashSet<int>();
        var maxSafetyLevel = SafetyLevel.Safe;

        // Check for direct trigger matches
        foreach (var trigger in triggers)
        {
            if (ingredientsLower.Contains(trigger.Name.ToLower()))
            {
                // In Flare Mode, only flag triggers that meet or exceed the severity threshold
                // This makes the app MORE sensitive by focusing on higher-severity items
                // In Normal Mode, flag all detected triggers regardless of severity
                if (isFlareMode && trigger.SeverityScore >= flareModeThreshold)
                {
                    detectedTriggerIds.Add(trigger.Id);
                    result.DetectedTriggers.Add(trigger.Name);
                    result.Warnings.Add($"{trigger.Name}: {trigger.Description} (Severity: {trigger.SeverityScore})");
                    
                    if (trigger.SafetyLevel > maxSafetyLevel)
                    {
                        maxSafetyLevel = trigger.SafetyLevel;
                    }
                }
                else if (!isFlareMode)
                {
                    detectedTriggerIds.Add(trigger.Id);
                    result.DetectedTriggers.Add(trigger.Name);
                    result.Warnings.Add($"{trigger.Name}: {trigger.Description}");
                    
                    if (trigger.SafetyLevel > maxSafetyLevel)
                    {
                        maxSafetyLevel = trigger.SafetyLevel;
                    }
                }
            }
        }

        // Check for synonym matches
        foreach (var synonym in synonyms)
        {
            if (ingredientsLower.Contains(synonym.Synonym.ToLower()) && synonym.IngredientTrigger != null)
            {
                var trigger = synonym.IngredientTrigger;
                
                // Apply same Flare Mode logic for synonyms
                if (isFlareMode && trigger.SeverityScore >= flareModeThreshold)
                {
                    if (detectedTriggerIds.Add(trigger.Id))
                    {
                        result.DetectedTriggers.Add($"{trigger.Name} (as {synonym.Synonym})");
                        result.Warnings.Add($"{trigger.Name} detected as '{synonym.Synonym}': {trigger.Description} (Severity: {trigger.SeverityScore})");
                        
                        if (trigger.SafetyLevel > maxSafetyLevel)
                        {
                            maxSafetyLevel = trigger.SafetyLevel;
                        }
                    }
                }
                else if (!isFlareMode)
                {
                    if (detectedTriggerIds.Add(trigger.Id))
                    {
                        result.DetectedTriggers.Add($"{trigger.Name} (as {synonym.Synonym})");
                        result.Warnings.Add($"{trigger.Name} detected as '{synonym.Synonym}': {trigger.Description}");
                        
                        if (trigger.SafetyLevel > maxSafetyLevel)
                        {
                            maxSafetyLevel = trigger.SafetyLevel;
                        }
                    }
                }
            }
        }

        // Check for cross-reactivities
        foreach (var cross in crossReactivities)
        {
            if (detectedTriggerIds.Contains(cross.PrimaryTriggerId) && cross.RelatedTrigger != null)
            {
                result.Warnings.Add($"⚠️ Cross-reactivity warning: {cross.Description}");
            }
        }

        result.SafetyLevel = maxSafetyLevel;
        
        // Generate summary
        if (result.DetectedTriggers.Count == 0)
        {
            result.Summary = isFlareMode 
                ? $"SAFE in Flare Mode - No triggers detected with severity >= {flareModeThreshold}." 
                : "SAFE - No known triggers detected.";
        }
        else
        {
            var levelText = maxSafetyLevel switch
            {
                SafetyLevel.Avoid => "AVOID",
                SafetyLevel.Caution => "CAUTION",
                _ => "SAFE"
            };
            
            var modeText = isFlareMode ? " (Flare Mode)" : "";
            result.Summary = $"{levelText}{modeText} - Detected {result.DetectedTriggers.Count} potential trigger(s).";
        }

        return result;
    }
}
