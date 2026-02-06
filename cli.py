#!/usr/bin/env python3
"""
Command-line interface for food inspector operations.
"""

import argparse
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).parent.parent))

from food_inspector.database import init_database, seed_database, get_database_version
from food_inspector.export import export_to_json, import_from_json
from food_inspector.scoring import ScoringModel, FlareMode


def cmd_init(args):
    """Initialize a new database."""
    init_database(args.database)
    if args.seed:
        print("Seeding database with sample data...")
        seed_database(args.database)


def cmd_export(args):
    """Export database to JSON."""
    export_to_json(args.database, args.output)


def cmd_import(args):
    """Import JSON data to database."""
    import_from_json(args.input, args.database)


def cmd_score(args):
    """Calculate inspection score."""
    model = ScoringModel(args.config)
    
    data = {
        "food_safety": args.food_safety,
        "cleanliness": args.cleanliness,
        "temperature_control": args.temperature_control,
        "employee_hygiene": args.employee_hygiene
    }
    
    evaluation = model.evaluate_inspection(data)
    
    print(f"Score: {evaluation['score']:.2f}")
    print(f"Severity: {evaluation['severity']}")
    
    if args.check_flare:
        flare = FlareMode(args.config)
        flare_eval = flare.evaluate_flare(evaluation['score'])
        
        print(f"Flare Triggered: {'YES' if flare_eval['flare_triggered'] else 'NO'}")
        print(f"Alert Required: {'YES' if flare_eval['alert_required'] else 'NO'}")
        if flare_eval['flare_triggered']:
            print(f"Priority: {flare_eval['base_priority']} → {flare_eval['escalated_priority']}")


def cmd_version(args):
    """Get database version."""
    version = get_database_version(args.database)
    print(f"Database schema version: {version}")


def main():
    """Main CLI entry point."""
    parser = argparse.ArgumentParser(
        description="Food Inspector CLI - Scoring system and database management"
    )
    
    subparsers = parser.add_subparsers(dest="command", help="Available commands")
    
    # Init command
    init_parser = subparsers.add_parser("init", help="Initialize a new database")
    init_parser.add_argument("database", help="Path to database file")
    init_parser.add_argument("--seed", action="store_true", help="Seed with sample data")
    
    # Export command
    export_parser = subparsers.add_parser("export", help="Export database to JSON")
    export_parser.add_argument("database", help="Path to database file")
    export_parser.add_argument("output", help="Output JSON file path")
    
    # Import command
    import_parser = subparsers.add_parser("import", help="Import JSON to database")
    import_parser.add_argument("input", help="Input JSON file path")
    import_parser.add_argument("database", help="Path to database file")
    
    # Score command
    score_parser = subparsers.add_parser("score", help="Calculate inspection score")
    score_parser.add_argument("--food-safety", type=float, required=True, help="Food safety score (0-100)")
    score_parser.add_argument("--cleanliness", type=float, required=True, help="Cleanliness score (0-100)")
    score_parser.add_argument("--temperature-control", type=float, required=True, help="Temperature control score (0-100)")
    score_parser.add_argument("--employee-hygiene", type=float, required=True, help="Employee hygiene score (0-100)")
    score_parser.add_argument("--config", help="Path to config file")
    score_parser.add_argument("--check-flare", action="store_true", help="Check flare mode status")
    
    # Version command
    version_parser = subparsers.add_parser("version", help="Get database schema version")
    version_parser.add_argument("database", help="Path to database file")
    
    args = parser.parse_args()
    
    if not args.command:
        parser.print_help()
        return 1
    
    try:
        if args.command == "init":
            cmd_init(args)
        elif args.command == "export":
            cmd_export(args)
        elif args.command == "import":
            cmd_import(args)
        elif args.command == "score":
            cmd_score(args)
        elif args.command == "version":
            cmd_version(args)
        
        return 0
        
    except Exception as e:
        print(f"Error: {e}", file=sys.stderr)
        return 1


if __name__ == "__main__":
    sys.exit(main())
