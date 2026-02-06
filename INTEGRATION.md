# Integration Guide: Python Package + .NET MAUI App

This document explains how the Python package and .NET MAUI mobile app implementations can work together.

## Repository Structure

The Food Inspector repository contains two complementary implementations:

### 1. .NET MAUI Mobile App (main branch)
- **Language**: C#
- **Platform**: Cross-platform mobile (iOS, Android, Windows, macOS)
- **Database**: SQLite with SQLCipher encryption
- **Features**: Barcode scanning, OCR, offline mode, scan history, export

### 2. Python Package (this branch)
- **Language**: Python 3.7+
- **Platform**: Library/API
- **Data Format**: YAML files
- **Features**: Synonym matching, cross-reactivity analysis, word-boundary matching

## Why Both Implementations?

The two implementations serve different purposes:

- **Mobile App**: End-user facing tool for scanning products in real-time
- **Python Package**: Backend processing, API services, data analysis, testing

## Integration Patterns

### Pattern 1: Shared Data Format

The Python YAML files serve as the "source of truth" for allergen data:

```
Python YAML Files (authoritative)
    ↓
.NET Database Seed Data (derived)
    ↓
Mobile App (runtime)
```

**Benefits**:
- Single source of truth for allergen synonyms and cross-reactivity
- Python package tests validate the data integrity
- Easy to update and version control

### Pattern 2: Backend API

Deploy the Python package as a REST API:

```
Mobile App → REST API (Python) → Analysis Results
```

**Example**:
```python
# Flask/FastAPI backend
from food_inspector import IngredientMatcher, CrossReactivityChecker

@app.post("/api/analyze")
def analyze_ingredients(ingredients: str):
    matcher = IngredientMatcher()
    results = matcher.scan_text(ingredients)
    return {"allergens": results}
```

**Benefits**:
- Centralized logic updates (no app store submissions)
- Server-side analytics and logging
- Enhanced features without mobile constraints

### Pattern 3: Data Synchronization

Use Python to generate C# code for database seeding:

```python
from food_inspector import IngredientMatcher
import yaml

# Generate C# from Python data
matcher = IngredientMatcher()
print("// Generated from ingredient_synonyms.yaml")
print("var synonyms = new List<IngredientSynonym> {")
for category, synonyms in matcher.synonyms.items():
    for synonym in synonyms:
        print(f'    new IngredientSynonym {{ '
              f'TriggerName = "{category}", '
              f'Synonym = "{synonym}" }},')
print("};")
```

**Benefits**:
- Automated database seeding
- Consistency between platforms
- Reduced manual data entry errors

### Pattern 4: Testing & Validation

Use Python tests to validate .NET implementation:

```python
# Test that .NET app's SQLite DB matches Python YAML
def test_dotnet_database_sync():
    # Load Python data
    matcher = IngredientMatcher()
    python_synonyms = set(matcher.synonyms.keys())
    
    # Load .NET database
    conn = sqlite3.connect("foodinspector.db")
    dotnet_synonyms = set(row[0] for row in 
                          conn.execute("SELECT DISTINCT TriggerName FROM IngredientSynonym"))
    
    # Validate
    assert python_synonyms == dotnet_synonyms
```

**Benefits**:
- Continuous validation of data consistency
- Catch drift between implementations
- Automated regression testing

### Pattern 5: Analytics Processing

Process export data from mobile app using Python:

```python
import pandas as pd
from food_inspector import IngredientMatcher

# Load scan history CSV from mobile app
scans = pd.read_csv("scan_history.csv")

# Analyze trends
matcher = IngredientMatcher()
for _, row in scans.iterrows():
    results = matcher.scan_text(row['Ingredients'])
    # Process and aggregate...
```

**Benefits**:
- Rich data analysis capabilities
- Machine learning on scan patterns
- Generate insights for users

## Recommended Workflow

1. **Development**:
   - Update Python YAML files as source of truth
   - Add/modify synonyms and cross-reactivity rules
   - Test with Python unit tests

2. **Synchronization**:
   - Generate C# database seed code from YAML
   - Update .NET app's database initialization
   - Run integration tests

3. **Deployment**:
   - Deploy Python package as API (optional)
   - Submit mobile app to app stores
   - Monitor both systems

## File Correspondence

| Python | .NET | Purpose |
|--------|------|---------|
| `data/ingredient_synonyms.yaml` | `Data/FoodInspectorDbContext.cs` (seed) | Allergen synonyms |
| `data/cross_reactivity.yaml` | `Data/FoodInspectorDbContext.cs` (seed) | Cross-reactivity rules |
| `src/food_inspector/matcher.py` | `Services/IngredientAnalysisService.cs` | Matching logic |
| `src/food_inspector/cross_reactivity.py` | `Models/CrossReactivity.cs` | Cross-reactivity model |

## Future Enhancements

1. **Automated Sync**: CI/CD pipeline to generate .NET seed data from Python YAML
2. **API Gateway**: Deploy Python package as serverless functions
3. **Machine Learning**: Use Python for ML models, deploy to .NET via ONNX
4. **Shared Tests**: Cross-platform test suite to validate both implementations
5. **Data Updates**: Centralized allergen database with versioning

## Getting Started

### For Mobile Development:
1. Checkout `main` branch for .NET MAUI app
2. Use YAML files as reference for allergen data
3. Run mobile app with `dotnet build` and deploy to device

### For Backend/API Development:
1. Checkout this branch for Python package
2. Install with `pip install -e .`
3. Import and use: `from food_inspector import IngredientMatcher`

### For Data Management:
1. Edit `data/*.yaml` files
2. Run Python tests: `pytest tests/`
3. Generate .NET seed code (if needed)
4. Commit to both branches

## Questions?

See README.md in each branch for detailed documentation on that implementation.
