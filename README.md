# Food Inspector

A comprehensive food safety system consisting of:
1. **MAUI Mobile App** - Consumer-facing mobile application for scanning food products and detecting allergens
2. **Data Generation Tools** - Python-based tools for generating versioned reference data

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
