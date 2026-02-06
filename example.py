"""
Example usage of the Food Inspector package
"""

from food_inspector import IngredientMatcher, CrossReactivityChecker


def main():
    print("=" * 70)
    print("Food Inspector - Example Usage")
    print("=" * 70)
    print()
    
    # Initialize matchers
    matcher = IngredientMatcher()
    checker = CrossReactivityChecker()
    
    # Example 1: Scan an ingredient list
    print("Example 1: Scanning an ingredient list")
    print("-" * 70)
    ingredient_list = """
    Ingredients: Enriched wheat flour (wheat flour, niacin, iron, 
    thiamine mononitrate, riboflavin, folic acid), sugar, vegetable oil 
    (contains one or more of the following: canola oil, corn oil, 
    palm oil, soybean oil), whey, eggs, soy lecithin, salt, 
    artificial flavor, malt extract.
    """
    
    print(f"Ingredient list: {ingredient_list.strip()}")
    print()
    
    results = matcher.scan_text(ingredient_list)
    
    print("Detected allergens:")
    for category, ingredients in results.items():
        print(f"\n  {category.upper()}:")
        for ingredient, matches in ingredients.items():
            print(f"    - {ingredient} (found {len(matches)} time(s))")
    
    print()
    print()
    
    # Example 2: Word boundary matching
    print("Example 2: Word boundary matching (malt vs maltodextrin)")
    print("-" * 70)
    
    text1 = "Contains malt extract and corn syrup"
    text2 = "Contains maltodextrin and corn syrup"
    
    print(f"Text 1: '{text1}'")
    matches1 = matcher.find_ingredient(text1, "malt")
    print(f"  Searching for 'malt': {len(matches1)} match(es) found")
    if matches1:
        for match in matches1:
            print(f"    - '{match[0]}' at position {match[1]}")
    
    print()
    print(f"Text 2: '{text2}'")
    matches2 = matcher.find_ingredient(text2, "malt")
    print(f"  Searching for 'malt': {len(matches2)} match(es) found")
    print(f"  (Correctly avoids matching 'malt' in 'maltodextrin')")
    
    print()
    print()
    
    # Example 3: Cross-reactivity checking
    print("Example 3: Cross-reactivity warnings")
    print("-" * 70)
    
    allergen = "peanuts"
    print(f"Checking cross-reactivity for: {allergen}")
    print()
    
    warnings = checker.format_warnings(allergen)
    for warning in warnings:
        print(f"  {warning}")
    
    print()
    print()
    
    # Example 4: Synonym lookup
    print("Example 4: Synonym lookup")
    print("-" * 70)
    
    categories_to_check = ["dairy", "gluten", "soy"]
    
    for category in categories_to_check:
        synonyms = matcher.get_all_synonyms(category)
        print(f"\n{category.upper()} synonyms ({len(synonyms)} total):")
        print(f"  {', '.join(synonyms[:10])}")
        if len(synonyms) > 10:
            print(f"  ... and {len(synonyms) - 10} more")
    
    print()
    print()
    
    # Example 5: Check specific cross-reactivity
    print("Example 5: Specific cross-reactivity check")
    print("-" * 70)
    
    source = "dairy"
    target = "goat_milk"
    
    rule = checker.check_cross_reactivity(source, target)
    if rule:
        print(f"Cross-reactivity found: {source} → {target}")
        print(f"  Confidence: {rule.confidence}")
        print(f"  Notes: {rule.notes}")
    else:
        print(f"No cross-reactivity found between {source} and {target}")
    
    print()
    print("=" * 70)


if __name__ == "__main__":
    main()
