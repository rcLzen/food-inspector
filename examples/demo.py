#!/usr/bin/env python3
"""
Example script demonstrating the food inspection scoring system.
"""

import sys
from pathlib import Path

# Add parent directory to path for imports
sys.path.insert(0, str(Path(__file__).parent.parent))

from food_inspector.scoring import ScoringModel, FlareMode
from food_inspector.database import init_database, seed_database, get_database_version
from food_inspector.export import export_to_json


def main():
    """Run example demonstration."""
    print("=" * 70)
    print("Food Inspector - Scoring Model & Flare Mode Demo")
    print("=" * 70)
    print()
    
    # 1. Initialize scoring model
    print("1. Initializing Scoring Model")
    print("-" * 70)
    
    # Load configuration
    config_path = Path(__file__).parent.parent / "config.json"
    scoring_model = ScoringModel(str(config_path))
    flare_mode = FlareMode(str(config_path))
    
    print(f"   Thresholds: {scoring_model.thresholds}")
    print(f"   Flare mode enabled: {flare_mode.is_enabled()}")
    print()
    
    # 2. Example inspections with different scores
    print("2. Evaluating Sample Inspections")
    print("-" * 70)
    
    sample_inspections = [
        {
            "name": "Joe's Pizza (Excellent)",
            "data": {
                "food_safety": 95,
                "cleanliness": 92,
                "temperature_control": 98,
                "employee_hygiene": 90
            }
        },
        {
            "name": "Main Street Deli (High Risk - Flare)",
            "data": {
                "food_safety": 85,
                "cleanliness": 78,
                "temperature_control": 82,
                "employee_hygiene": 75
            }
        },
        {
            "name": "Corner Store (Medium Risk)",
            "data": {
                "food_safety": 65,
                "cleanliness": 70,
                "temperature_control": 60,
                "employee_hygiene": 75
            }
        },
        {
            "name": "Quick Mart (Low Risk)",
            "data": {
                "food_safety": 40,
                "cleanliness": 45,
                "temperature_control": 35,
                "employee_hygiene": 50
            }
        }
    ]
    
    for inspection in sample_inspections:
        print(f"\n   {inspection['name']}")
        evaluation = scoring_model.evaluate_inspection(inspection['data'])
        flare_eval = flare_mode.evaluate_flare(evaluation['score'])
        
        print(f"   Score: {evaluation['score']:.2f}")
        print(f"   Severity: {evaluation['severity']}")
        print(f"   Flare Triggered: {'YES' if flare_eval['flare_triggered'] else 'NO'}")
        print(f"   Alert Required: {'YES' if flare_eval['alert_required'] else 'NO'}")
        if flare_eval['flare_triggered']:
            print(f"   Priority: {flare_eval['base_priority']} → {flare_eval['escalated_priority']}")
    
    print()
    
    # 3. Database setup
    print("3. Setting Up Database")
    print("-" * 70)
    
    db_path = Path(__file__).parent.parent / "data" / "food_inspection.db"
    db_path.parent.mkdir(exist_ok=True)
    
    # Remove old database if exists
    if db_path.exists():
        db_path.unlink()
    
    print(f"   Database path: {db_path}")
    init_database(str(db_path))
    
    version = get_database_version(str(db_path))
    print(f"   Schema version: {version}")
    print()
    
    # 4. Seed with sample data
    print("4. Seeding Database with Sample Data")
    print("-" * 70)
    seed_database(str(db_path))
    print()
    
    # 5. Export to JSON
    print("5. Exporting to JSON Format")
    print("-" * 70)
    
    json_path = Path(__file__).parent.parent / "data" / "export.json"
    export_to_json(str(db_path), str(json_path))
    print()
    
    print("=" * 70)
    print("Demo completed successfully!")
    print("=" * 70)
    print()
    print(f"Database: {db_path}")
    print(f"JSON Export: {json_path}")
    print()


if __name__ == "__main__":
    main()
