# Food Inspector

A comprehensive food inspection scoring system with configurable thresholds, flare-mode policy, and data packaging for V1 ingestion.

## Features

### 🎯 Scoring Model
- **Configurable Thresholds**: Customize severity levels (critical, high, medium, low)
- **Weighted Scoring**: Multi-criteria evaluation with configurable weights
  - Food Safety (40%)
  - Cleanliness (30%)
  - Temperature Control (20%)
  - Employee Hygiene (10%)
- **Flexible Configuration**: JSON-based configuration system

### 🚨 Flare-Mode Policy
- **Automatic Trigger**: Activates when scores exceed defined thresholds
- **Priority Escalation**: Applies configurable multipliers to base priority
- **Alert System**: Immediate notifications for critical situations
- **Configurable Parameters**:
  - Trigger score threshold
  - Escalation multiplier
  - Alert threshold

### 📦 Data Package for V1 Ingestion
- **JSON Export Format**: Structured data export for easy integration
- **SQLite Database**: Complete schema with relationships
- **Migration Scripts**: Database initialization and version management
- **Seed Scripts**: Sample data for testing and development

## Installation

```bash
# Clone the repository
git clone https://github.com/rcLzen/food-inspector.git
cd food-inspector

# Install in development mode
pip install -e .
```

## Quick Start

### 1. Run the Demo

```bash
python examples/demo.py
```

This will:
- Initialize the scoring model with configuration
- Evaluate sample inspections
- Create and seed a SQLite database
- Export data to JSON format

### 2. Use the Scoring Model

```python
from food_inspector.scoring import ScoringModel, FlareMode

# Initialize with config
scoring_model = ScoringModel("config.json")
flare_mode = FlareMode("config.json")

# Evaluate an inspection
inspection_data = {
    "food_safety": 85,
    "cleanliness": 78,
    "temperature_control": 82,
    "employee_hygiene": 75
}

evaluation = scoring_model.evaluate_inspection(inspection_data)
print(f"Score: {evaluation['score']}")
print(f"Severity: {evaluation['severity']}")

# Check flare mode
flare_eval = flare_mode.evaluate_flare(evaluation['score'])
if flare_eval['flare_triggered']:
    print("⚠️ FLARE MODE TRIGGERED!")
    print(f"Priority escalated: {flare_eval['escalated_priority']}")
```

### 3. Database Operations

```python
from food_inspector.database import init_database, seed_database
from food_inspector.export import export_to_json

# Initialize database
init_database("food_inspection.db")

# Add sample data
seed_database("food_inspection.db")

# Export to JSON
export_to_json("food_inspection.db", "export.json")
```

## Configuration

Edit `config.json` to customize thresholds and policies:

```json
{
  "scoring_thresholds": {
    "critical": 90,
    "high": 70,
    "medium": 50,
    "low": 30
  },
  "flare_mode": {
    "enabled": true,
    "trigger_score": 80,
    "escalation_multiplier": 1.5,
    "alert_threshold": 85
  },
  "inspection_weights": {
    "food_safety": 0.40,
    "cleanliness": 0.30,
    "temperature_control": 0.20,
    "employee_hygiene": 0.10
  }
}
```

## Database Schema

### Tables

- **establishments**: Food service establishments
- **inspections**: Inspection records with scores
- **violations**: Specific violations found during inspections
- **configuration**: System configuration storage

### JSON Export Format

```json
{
  "version": "1.0.0",
  "export_date": "2024-01-01T12:00:00",
  "data": {
    "establishments": [...],
    "inspections": [...],
    "violations": [...],
    "configuration": [...]
  },
  "metadata": {
    "total_establishments": 5,
    "total_inspections": 10,
    "total_violations": 3
  }
}
```

## API Reference

### ScoringModel

```python
class ScoringModel:
    def __init__(config_path: Optional[str] = None)
    def calculate_score(inspection_data: Dict[str, float]) -> float
    def get_severity(score: float) -> str
    def evaluate_inspection(inspection_data: Dict[str, float]) -> Dict[str, Any]
```

### FlareMode

```python
class FlareMode:
    def __init__(config_path: Optional[str] = None)
    def is_enabled() -> bool
    def should_trigger(score: float) -> bool
    def should_alert(score: float) -> bool
    def apply_escalation(base_priority: int) -> int
    def evaluate_flare(score: float, base_priority: int = 5) -> Dict[str, Any]
```

## Project Structure

```
food-inspector/
├── config.json                 # Configuration file
├── pyproject.toml             # Package configuration
├── README.md                  # This file
├── food_inspector/            # Main package
│   ├── __init__.py
│   ├── scoring.py             # Scoring model & flare mode
│   ├── export.py              # JSON export/import
│   └── database/              # Database modules
│       ├── __init__.py
│       ├── schema.py          # Database schema
│       ├── migrate.py         # Migration scripts
│       └── seed.py            # Seed data
├── examples/                  # Example scripts
│   └── demo.py                # Complete demonstration
└── data/                      # Generated data files
    ├── food_inspection.db     # SQLite database (generated)
    └── export.json            # JSON export (generated)
```

## License

MIT License