# Food Inspector

A comprehensive food safety toolkit with implementations for multiple platforms.

## Repository Contents

This repository contains **two complementary implementations** of the Food Inspector system:

### 1. 📱 .NET MAUI Mobile App (main branch)
A complete cross-platform mobile application for iOS, Android, Windows, and macOS with:
- Barcode scanning and OCR for ingredient labels
- Offline-first architecture with encrypted SQLite database
- Online enrichment via Open Food Facts API
- Flare Mode for heightened sensitivity periods
- Scan history and export functionality

**[See the .NET MAUI app documentation →](https://github.com/rcLzen/food-inspector/tree/main)**

### 2. 🐍 Python Package (this branch)
A Python library for programmatic ingredient analysis with:
- Curated ingredient synonym dictionary (150+ synonyms across 9 allergen categories)
- Structured cross-reactivity rules with confidence levels
- Word-boundary-safe pattern matching to prevent false positives
- Comprehensive test suite and API

---

## Python Package Documentation

A Python package for analyzing food ingredients with advanced allergen detection, synonym matching, and cross-reactivity checking.

## Features

### 1. 🥛 Curated Ingredient Synonym Dictionary

Comprehensive mapping of allergen categories to their various forms and synonyms:

- **Dairy**: whey, casein, milk solids, lactose, etc.
- **Soy**: lecithin, TVP, tofu, tempeh, etc.
- **Gluten**: malt extract, semolina, wheat flour, etc.
- **Tree Nuts**: almonds, cashews, hazelnuts, etc.
- **Peanuts**: groundnut, goober, etc.
- **Eggs**: albumin, ovalbumin, lysozyme, etc.
- **Fish**: various fish types and fish-derived ingredients
- **Shellfish**: shrimp, crab, lobster, etc.
- **Sesame**: tahini, sesame oil, benne, etc.

### 2. 🔗 Structured Cross-Reactivity Rules

A comprehensive database of known allergen cross-reactivities with:

- **Source → Target mapping**: Clear relationship between allergens
- **Confidence levels**: Low, medium, or high
- **Clinical notes**: Explanations of the cross-reactivity mechanism

Examples:
- Peanuts ↔ Tree nuts
- Dairy → Beef (bovine serum albumin)
- Latex → Banana, Avocado, Kiwi (latex-fruit syndrome)
- Birch pollen → Apple, Cherry (oral allergy syndrome)

### 3. ✅ Word-Boundary-Safe Matching

Intelligent pattern matching that prevents false positives:

- ✅ "malt" matches "malt extract"
- ❌ "malt" does NOT match "maltodextrin"
- ✅ "milk" matches "milk solids"
- ❌ "milk" does NOT match "milkshake"

Uses regex word boundaries (`\b`) to ensure accurate matching while supporting:
- Hyphenated words (e.g., "half-and-half")
- Apostrophes (e.g., "brewer's yeast")
- Multi-word ingredients (e.g., "malt extract")

## Installation

```bash
# Clone the repository
git clone https://github.com/rcLzen/food-inspector.git
cd food-inspector

# Install dependencies
pip install -r requirements.txt

# Install the package in development mode
pip install -e .
```

## Quick Start

```python
from food_inspector import IngredientMatcher, CrossReactivityChecker

# Initialize the matchers
matcher = IngredientMatcher()
checker = CrossReactivityChecker()

# Scan an ingredient list
ingredient_list = """
Ingredients: wheat flour, sugar, soy lecithin, 
whey protein, eggs, malt extract
"""

results = matcher.scan_text(ingredient_list)
print("Detected allergens:", list(results.keys()))
# Output: ['gluten', 'soy', 'dairy', 'eggs']

# Check for cross-reactivity
warnings = checker.format_warnings("peanuts")
for warning in warnings:
    print(warning)
# Output: ⚠️  May cross-react with tree_nuts (confidence: medium)
```

## Usage Examples

### Example 1: Basic Allergen Detection

```python
from food_inspector import IngredientMatcher

matcher = IngredientMatcher()

# Scan ingredient text
text = "Contains: milk, soy lecithin, wheat flour"
results = matcher.scan_text(text)

for category, ingredients in results.items():
    print(f"{category}: {list(ingredients.keys())}")
# Output:
# dairy: ['milk']
# soy: ['lecithin']
# gluten: ['wheat']
```

### Example 2: Word Boundary Matching

```python
# Demonstrates that 'malt' doesn't match 'maltodextrin'
text1 = "Contains malt extract"
text2 = "Contains maltodextrin"

matches1 = matcher.find_ingredient(text1, "malt")
print(f"Found 'malt' in text1: {len(matches1) > 0}")  # True

matches2 = matcher.find_ingredient(text2, "malt")
print(f"Found 'malt' in text2: {len(matches2) > 0}")  # False
```

### Example 3: Cross-Reactivity Checking

```python
from food_inspector import CrossReactivityChecker

checker = CrossReactivityChecker()

# Get all potential reactions for peanuts
reactions = checker.get_potential_reactions("peanuts")
for reaction in reactions:
    print(f"{reaction.source} → {reaction.target} ({reaction.confidence})")
    print(f"  Note: {reaction.notes}")

# Check specific cross-reactivity
rule = checker.check_cross_reactivity("dairy", "goat_milk")
if rule:
    print(f"Cross-reactivity exists with {rule.confidence} confidence")
```

### Example 4: Synonym Lookup

```python
# Get all synonyms for a category
dairy_synonyms = matcher.get_all_synonyms("dairy")
print(f"Dairy synonyms: {', '.join(dairy_synonyms[:5])}...")

# Reverse lookup: ingredient → category
category = matcher.get_allergen_for_ingredient("whey")
print(f"'whey' belongs to category: {category}")  # dairy
```

### Example 5: Confidence-Based Filtering

```python
# Get only high-confidence cross-reactions
high_conf = checker.get_potential_reactions("dairy", min_confidence="high")
print(f"High-confidence reactions: {len(high_conf)}")

# Get formatted warnings
warnings = checker.format_warnings("latex", min_confidence="medium")
for warning in warnings:
    print(warning)
```

## Running Tests

```bash
# Install test dependencies
pip install -r requirements-dev.txt

# Run all tests
pytest

# Run with coverage
pytest --cov=food_inspector --cov-report=html

# Run specific test file
pytest tests/test_matcher.py
pytest tests/test_cross_reactivity.py
```

## Project Structure

```
food-inspector/
├── src/
│   └── food_inspector/
│       ├── __init__.py
│       ├── matcher.py              # Ingredient matching with word boundaries
│       └── cross_reactivity.py     # Cross-reactivity rule management
├── data/
│   ├── ingredient_synonyms.yaml    # Curated synonym dictionary
│   └── cross_reactivity.yaml       # Cross-reactivity rules
├── tests/
│   ├── test_matcher.py
│   └── test_cross_reactivity.py
├── example.py                      # Usage examples
├── setup.py
├── requirements.txt
└── README.md
```

## Data Files

### ingredient_synonyms.yaml

Contains allergen categories mapped to their synonyms:

```yaml
dairy:
  - milk
  - whey
  - casein
  - lactose
  # ... more synonyms

soy:
  - soybean
  - lecithin
  - tofu
  # ... more synonyms
```

### cross_reactivity.yaml

Contains structured cross-reactivity rules:

```yaml
cross_reactivity_rules:
  - source: peanuts
    target: tree_nuts
    confidence: medium
    notes: "About 25-40% of peanut-allergic individuals also react to tree nuts"
  
  - source: latex
    target: banana
    confidence: medium
    notes: "Latex-fruit syndrome; shared proteins cause cross-reactivity"
```

## Integration with .NET MAUI App

The Python package and .NET MAUI mobile app can work together:

### Shared Data Format
Both implementations use similar data structures for allergen synonyms and cross-reactivity rules. The Python YAML files (`data/ingredient_synonyms.yaml` and `data/cross_reactivity.yaml`) can serve as:
- **Data source** for the .NET app's database seeding
- **Reference implementation** for validation logic
- **Shared documentation** of allergen relationships

### Potential Integration Patterns
1. **Backend API**: Deploy Python package as a REST API that the mobile app can call
2. **Data synchronization**: Use Python package to generate/update the .NET app's SQLite database
3. **Testing**: Use Python tests to validate .NET implementation behavior
4. **Analytics**: Process .NET app export data (CSV/JSON) using Python package

### Example: Converting Python Data to .NET
```python
# Generate C# seed data from Python YAML files
from food_inspector import IngredientMatcher
matcher = IngredientMatcher()

for category, synonyms in matcher.synonyms.items():
    print(f"// {category}")
    for synonym in synonyms:
        print(f'new IngredientSynonym {{ Synonym = "{synonym}" }}')
```

## API Reference

### IngredientMatcher

- `find_ingredient(text, ingredient)`: Find specific ingredient with word boundaries
- `find_allergen_category(text, category)`: Find all ingredients from a category
- `scan_text(text)`: Scan for all known allergen categories
- `get_allergen_for_ingredient(ingredient)`: Reverse lookup ingredient → category
- `get_all_synonyms(category)`: Get all synonyms for a category

### CrossReactivityChecker

- `get_potential_reactions(allergen, min_confidence=None)`: Get cross-reactions for an allergen
- `get_sources_for_target(target, min_confidence=None)`: Get sources that react to target
- `check_cross_reactivity(source, target)`: Check specific cross-reaction
- `get_all_rules()`: Get all cross-reactivity rules
- `get_rules_by_confidence(confidence)`: Filter rules by confidence level
- `format_warnings(allergen, min_confidence='low')`: Get formatted warning messages

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## License

This project is open source and available under the MIT License.

## Acknowledgments

- Allergen data compiled from FDA allergen guidelines
- Cross-reactivity information based on clinical allergy research
- Word boundary matching inspired by common food labeling challenges