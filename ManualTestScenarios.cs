using FoodInspector.Models;

namespace FoodInspector;

public static class ManualTestScenarios
{
    public static readonly (string Ingredients, int FlareModeThreshold, SafetyLevel ExpectedFinalStatus)[] FlareModeThresholdScenarios =
    {
        ("Ingredients: oats, salt", 0, SafetyLevel.Avoid),          // Strict: Moderate -> AVOID
        ("Ingredients: lactose, water", 1, SafetyLevel.Caution),    // Default: Moderate -> CAUTION
        ("Ingredients: mustard, black pepper", 2, SafetyLevel.Safe) // Lenient: Moderate -> SAFE
    };
}
