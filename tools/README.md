# Food Inspector Data Generation Tools

This directory contains Python tools for generating versioned JSON data files consumed by the Food Inspector MAUI application.

## Overview

The tools in this directory are **build-time only** and not used at runtime in the V1 MAUI app. They generate static JSON files that are checked into the MAUI project.

## Purpose

Generate versioned JSON data for:
- **Ingredient Synonyms**: Alternative names for allergens and triggers
- **Cross-Reactivity Data**: Relationships between different allergens
- **Scoring Policy**: Thresholds and flare-mode configuration

## Directory Structure

```
tools/
├── data-generator/          # Python source code for data generation
│   ├── generators/          # Individual JSON generators
│   ├── models/              # Data models
│   └── config/              # Source configuration files
├── output/                  # Generated JSON files (versioned)
└── README.md               # This file
```

## Usage

### Prerequisites

Python 3.8 or higher is required.

```bash
cd tools/data-generator
pip install -r requirements.txt
```

### Generate All Data Files

```bash
python generate_data.py --version 1 --output ../output
```

This generates:
- `synonyms.v1.json`
- `cross-reactivity.v1.json`
- `scoring-policy.v1.json`

### Generate Individual Files

```bash
# Generate synonyms only
python generate_data.py --type synonyms --version 1

# Generate cross-reactivity only
python generate_data.py --type cross-reactivity --version 1

# Generate scoring policy only
python generate_data.py --type scoring-policy --version 1
```

## JSON Output Formats

### synonyms.v1.json

Maps ingredient synonyms to canonical allergen names for detection.

```json
{
  "version": "1.0.0",
  "generated_at": "2024-01-01T12:00:00Z",
  "synonyms": [
    {
      "trigger_id": 1,
      "canonical_name": "Peanuts",
      "synonyms": ["groundnut", "arachis oil", "goober"]
    }
  ]
}
```

### cross-reactivity.v1.json

Defines cross-reactivity relationships between allergens.

```json
{
  "version": "1.0.0",
  "generated_at": "2024-01-01T12:00:00Z",
  "relationships": [
    {
      "primary_trigger_id": 1,
      "related_trigger_id": 2,
      "description": "Peanut allergy may cross-react with tree nuts"
    }
  ]
}
```

### scoring-policy.v1.json

Contains scoring thresholds and flare-mode configuration.

```json
{
  "version": "1.0.0",
  "generated_at": "2024-01-01T12:00:00Z",
  "scoring_thresholds": {
    "critical": 90,
    "high": 70,
    "medium": 50,
    "low": 30
  },
  "flare_mode": {
    "default_threshold": 5,
    "escalation_multiplier": 1.5,
    "min_threshold": 1,
    "max_threshold": 10
  }
}
```

## Integration with MAUI App

The generated JSON files from `tools/output/` should be copied to the MAUI project's data directory where they can be read at runtime:

```
FoodInspector/
├── Data/
│   └── reference-data/
│       ├── synonyms.v1.json
│       ├── cross-reactivity.v1.json
│       └── scoring-policy.v1.json
```

## Versioning

Data files use semantic versioning (MAJOR.MINOR.PATCH):
- **MAJOR**: Breaking changes to JSON schema
- **MINOR**: Backward-compatible additions
- **PATCH**: Data updates without schema changes

The MAUI app reads the version from each file to ensure compatibility.
