# Food Inspector

A comprehensive food safety system consisting of:
1. **MAUI Mobile App** - Consumer-facing mobile application for scanning food products and detecting allergens
2. **Data Generation Tools** - Python-based tools for generating versioned reference data

**Note:** The MAUI V1 app is the primary deliverable. Python tooling lives under `/tools` and is build-time only; it is not part of the MAUI app build.

## Components

### 1. MAUI Mobile App (Consumer Allergen Scanner)

A .NET 8 MAUI Blazor Hybrid mobile application that helps users identify potential food allergens and triggers.

**Features:**
- Barcode scanning and OCR ingredient extraction
- Allergen detection with synonym matching
- Cross-reactivity warnings
- Flare Mode for heightened sensitivity
- Encrypted local database
- Offline-first with online enrichment

**Technology:** .NET 8, MAUI, Blazor, SQLite

### 2. Data Generation Tools (Build-Time Only)

Python tools that generate versioned JSON reference data consumed by the MAUI app. **No runtime Python dependency.**

**Generated Files:**
- `synonyms.v1.json` - Ingredient synonym mappings
- `cross-reactivity.v1.json` - Cross-reactivity relationships
- `scoring-policy.v1.json` - Scoring thresholds and flare-mode config

## Usage

### Generate Data Files

```bash
cd tools/data-generator
python generate_data.py --version 1 --output ../output --pretty
```

See [tools/README.md](tools/README.md) for detailed documentation.

## Why Separate Tools?

- ✅ No runtime Python dependency in mobile app
- ✅ Version-controlled reference data
- ✅ Clear separation between build-time and runtime
- ✅ Easy to update and regenerate data
- ✅ Simple deployment (just JSON files)

## Integration Workflow

1. Update data in Python generators
2. Run generation script to create JSON files
3. Review generated files
4. Copy JSON files to MAUI project's data directory
5. Commit both generator changes and JSON files
6. MAUI app loads JSON files at startup

## Versioning

- **Data files:** Major version in filename (e.g., `synonyms.v1.json`)
- **MAUI app:** Checks compatibility with data file versions

## License

MIT License
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
# Food Inspector - Food Safety Scanner v1

A .NET 8 MAUI Blazor Hybrid mobile application that helps users identify potential food allergens and triggers in their food products.

## Features

### Core Functionality
- **Barcode Scanning**: Scan product barcodes to automatically retrieve product information
- **OCR Ingredient Scanning**: Take photos of ingredient labels and extract text using on-device OCR
- **Offline Mode**: Fully functional ingredient analysis without internet connection
- **Online Enrichment**: Fetch detailed product information from Open Food Facts API when online
- **Smart Analysis**: Match ingredients against a comprehensive database of triggers, synonyms, and cross-reactivities
- **Safety Ratings**: Get clear SAFE/CAUTION/AVOID ratings with detailed explanations

### Flare Mode
- **Heightened Sensitivity**: Tighten detection thresholds for periods of increased sensitivity
- **Customizable Threshold**: Adjust severity threshold (1-10) to match your needs
- **Visual Indicators**: Clear marking of flare mode scans in history

### Data Management
- **Scan History**: Keep track of all scanned products
- **Secure Storage**: Encrypted SQLite database using SQLCipher with keys stored in platform SecureStorage
- **Export Options**: Export scan history to CSV or JSON format
- **No Login Required**: Privacy-focused design with no account requirements

### Ingredient Database
The app includes a pre-seeded database with:
- **15+ Common Allergens**: Peanuts, tree nuts, milk, eggs, soy, wheat, fish, shellfish, sesame, and more
- **50+ Synonyms**: Alternative names and common ingredient variations
- **Cross-Reactivity Mapping**: Alerts for potential cross-reactions between allergens
- **Severity Scores**: Each trigger rated 1-10 for Flare Mode sensitivity

## Technical Stack

### Framework & Technologies
- **.NET 8**: Latest .NET framework
- **MAUI Blazor Hybrid**: Cross-platform UI with web technologies
- **Entity Framework Core**: Database ORM
- **SQLite with SQLCipher**: Encrypted local database
- **ZXing.Net.Maui**: Barcode scanning
- **Tesseract**: OCR for ingredient text extraction
- **CsvHelper**: CSV export functionality
- **Open Food Facts API**: Product information enrichment

### Architecture
- **Service-based architecture**: Clean separation of concerns
- **Dependency Injection**: Built-in .NET DI container
- **Repository pattern**: DatabaseService abstracts data access
- **Blazor Components**: Reactive UI with component-based design

## Project Structure

```
FoodInspector/
├── Components/
│   ├── Layout/
│   │   └── MainLayout.razor      # Main app layout with navigation
│   ├── Pages/
│   │   ├── Home.razor            # Scanning interface
│   │   ├── History.razor         # Scan history view
│   │   └── Settings.razor        # App settings and info
│   └── Routes.razor              # Routing configuration
├── Data/
│   └── FoodInspectorDbContext.cs # EF Core database context with seed data
├── Models/
│   └── Models.cs                 # Data models (triggers, history, settings)
├── Services/
│   ├── SecureStorageService.cs   # Encryption key management
│   ├── DatabaseService.cs        # Database operations
│   ├── ScanningServices.cs       # Barcode and OCR services
│   ├── IngredientAnalysisService.cs # Ingredient matching and analysis
│   └── ExportService.cs          # CSV/JSON export functionality
├── Resources/                     # App resources (icons, fonts, styles)
├── wwwroot/                       # Web assets (CSS, HTML)
├── MauiProgram.cs                # App configuration and DI setup
└── App.xaml / MainPage.xaml      # MAUI entry points

```

## Setup & Installation

### Prerequisites
- .NET 8 SDK or later
- Visual Studio 2022 or Visual Studio Code with MAUI workload
- Windows, macOS, or Linux for development
- Target platforms: Android, iOS, macOS, Windows

### Installation Steps

1. **Clone the repository**
   ```bash
   git clone https://github.com/rcLzen/food-inspector.git
   cd food-inspector
   ```

2. **Install MAUI workload** (if not already installed)
   ```bash
   dotnet workload install maui
   ```

3. **Restore NuGet packages**
   ```bash
   dotnet restore
   ```

4. **Build the project**
   ```bash
   dotnet build
   ```

5. **Run on your target platform**
   ```bash
   # Android
   dotnet build -t:Run -f net8.0-android
   
   # iOS (macOS only)
   dotnet build -t:Run -f net8.0-ios
   
   # Windows
   dotnet build -t:Run -f net8.0-windows10.0.19041.0
   
   # macOS
   dotnet build -t:Run -f net8.0-maccatalyst
   ```

## Usage Guide

### Scanning Products

1. **Manual Entry**: Type or paste ingredients directly into the text box
2. **Barcode Scan**: Tap "Scan Barcode" to scan product barcode and fetch info from Open Food Facts
3. **Ingredient Photo**: Tap "Scan Ingredients (OCR)" to photograph and extract ingredient text
4. **Analyze**: Tap "Analyze Ingredients" to get safety assessment

### Understanding Results

- **✅ SAFE**: No known triggers detected
- **⚠️ CAUTION**: Minor triggers or sensitivities found
- **🚫 AVOID**: Major allergens or high-severity triggers detected

Each result includes:
- List of detected triggers
- Detailed warnings with explanations
- Cross-reactivity alerts when applicable

### Using Flare Mode

1. Go to Settings
2. Toggle "Flare Mode" on
3. Adjust severity threshold (1-10)
   - Lower values = more lenient (only high severity triggers)
   - Higher values = more strict (flags more potential issues)
4. Return to scanning - Flare Mode indicator will appear

### Managing History

- View all past scans in the History tab
- Each entry shows date, product, safety level, and ingredients
- Flare Mode scans are marked with 🔥 badge
- Delete individual entries
- Export all history to CSV or JSON

## Database Schema

### Tables

**FoodScanHistory**: User scan records
- Id, ScanDate, Barcode, ProductName, Ingredients
- SafetyLevel, Analysis, IsFlareMode, ImagePath, OpenFoodFactsData

**IngredientTrigger**: Known allergens and triggers
- Id, Name, SafetyLevel, Description, SeverityScore, IsCommonAllergen

**IngredientSynonym**: Alternative names for triggers
- Id, IngredientTriggerId, Synonym

**CrossReactivity**: Cross-reaction relationships
- Id, PrimaryTriggerId, RelatedTriggerId, Description

**AppSettings**: User preferences
- Id, IsFlareMode, FlareModeThreshold

## Security & Privacy

- **No Login**: No personal data collection or accounts required
- **Local Storage**: All data stored locally on device
- **Encrypted Database**: SQLite database encrypted with SQLCipher
- **Secure Keys**: Encryption keys managed by platform SecureStorage
- **Offline First**: Core functionality works without internet

## Customization

### Adding New Triggers

Edit `Data/FoodInspectorDbContext.cs` in the `SeedData()` method:

```csharp
new IngredientTrigger 
{ 
    Id = 16, 
    Name = "Coconut", 
    SafetyLevel = SafetyLevel.Caution, 
    Description = "Tree nut family",
    SeverityScore = 7,
    IsCommonAllergen = true 
}
```

### Adding Synonyms

```csharp
new IngredientSynonym 
{ 
    Id = 57, 
    IngredientTriggerId = 16, 
    Synonym = "coconut oil" 
}
```

### Modifying Safety Thresholds

Adjust severity scores (1-10) for each trigger to customize Flare Mode behavior.

## API Integration

### Open Food Facts

The app integrates with the Open Food Facts API (https://world.openfoodfacts.org/) to fetch product information by barcode.

**Endpoint**: `GET /api/v0/product/{barcode}.json`

**Features**:
- Product name and brand
- Ingredient list
- Product images
- Nutritional information
- Automatically falls back to offline mode if unavailable

## Troubleshooting

### Database Issues
- Delete app data/cache to reset database
- Database will be recreated with seed data on next launch

### Barcode Scanner Not Working
- Ensure camera permissions are granted
- Check device has functional camera
- Try manual barcode entry

### OCR Not Detecting Text
- Ensure good lighting
- Hold camera steady
- Try manual ingredient entry
- Make sure ingredient text is clearly visible

## Future Enhancements

Potential features for future versions:
- Custom user trigger lists
- Sharing scan results
- Nutrition analysis
- Recipe scanner
- Multiple user profiles
- Cloud backup
- Barcode history
- Food diary integration

## Contributing

Contributions welcome! Please:
1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Submit a pull request

## License

This project is open source and available under the MIT License.

## Credits

- **Open Food Facts**: Product database
- **ZXing**: Barcode scanning library
- **Tesseract**: OCR engine
- **SQLCipher**: Database encryption
- **CommunityToolkit.Maui**: MAUI extensions

## Support

For issues, questions, or feature requests, please open an issue on GitHub.

---

**Version**: 1.0.0  
**Last Updated**: 2026-02-06
