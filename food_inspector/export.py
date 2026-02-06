"""JSON export and import utilities for food inspection data."""

import json
import sqlite3
from typing import Dict, List, Any
from datetime import datetime


def export_to_json(db_path: str, output_path: str) -> None:
    """
    Export database to JSON format for V1 ingestion.
    
    Args:
        db_path: Path to the SQLite database file.
        output_path: Path for the output JSON file.
    """
    conn = sqlite3.connect(db_path)
    conn.row_factory = sqlite3.Row
    cursor = conn.cursor()
    
    try:
        # Export establishments
        cursor.execute("SELECT * FROM establishments")
        establishments = [dict(row) for row in cursor.fetchall()]
        
        # Export inspections
        cursor.execute("SELECT * FROM inspections")
        inspections = [dict(row) for row in cursor.fetchall()]
        
        # Export violations
        cursor.execute("SELECT * FROM violations")
        violations = [dict(row) for row in cursor.fetchall()]
        
        # Export configuration
        cursor.execute("SELECT * FROM configuration")
        configuration = [dict(row) for row in cursor.fetchall()]
        
        # Build the complete export
        export_data = {
            "version": "1.0.0",
            "export_date": datetime.now().isoformat(),
            "data": {
                "establishments": establishments,
                "inspections": inspections,
                "violations": violations,
                "configuration": configuration
            },
            "metadata": {
                "total_establishments": len(establishments),
                "total_inspections": len(inspections),
                "total_violations": len(violations),
                "configuration_items": len(configuration)
            }
        }
        
        # Write to file
        with open(output_path, 'w') as f:
            json.dump(export_data, f, indent=2)
        
        print(f"Data exported successfully to {output_path}")
        print(f"Exported: {len(establishments)} establishments, {len(inspections)} inspections, {len(violations)} violations")
        
    finally:
        conn.close()


def import_from_json(json_path: str, db_path: str) -> None:
    """
    Import data from JSON format into database.
    
    Args:
        json_path: Path to the JSON file.
        db_path: Path to the SQLite database file.
    """
    with open(json_path, 'r') as f:
        import_data = json.load(f)
    
    conn = sqlite3.connect(db_path)
    cursor = conn.cursor()
    
    try:
        data = import_data["data"]
        
        # Import establishments
        for est in data["establishments"]:
            cursor.execute("""
                INSERT OR REPLACE INTO establishments 
                (id, name, address, city, state, zip_code, establishment_type, license_number, created_at, updated_at)
                VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?)
            """, (est["id"], est["name"], est["address"], est["city"], est["state"],
                  est["zip_code"], est["establishment_type"], est["license_number"],
                  est["created_at"], est["updated_at"]))
        
        # Import inspections
        for insp in data["inspections"]:
            cursor.execute("""
                INSERT OR REPLACE INTO inspections 
                (id, establishment_id, inspection_date, inspector_name,
                 food_safety_score, cleanliness_score, temperature_control_score, employee_hygiene_score,
                 total_score, severity, flare_triggered, alert_required, notes, created_at)
                VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)
            """, (insp["id"], insp["establishment_id"], insp["inspection_date"], insp["inspector_name"],
                  insp["food_safety_score"], insp["cleanliness_score"], 
                  insp["temperature_control_score"], insp["employee_hygiene_score"],
                  insp["total_score"], insp["severity"], insp["flare_triggered"],
                  insp["alert_required"], insp["notes"], insp["created_at"]))
        
        # Import violations
        for viol in data["violations"]:
            cursor.execute("""
                INSERT OR REPLACE INTO violations 
                (id, inspection_id, violation_code, description, severity, corrected, correction_date, created_at)
                VALUES (?, ?, ?, ?, ?, ?, ?, ?)
            """, (viol["id"], viol["inspection_id"], viol["violation_code"], 
                  viol["description"], viol["severity"], viol["corrected"],
                  viol.get("correction_date"), viol["created_at"]))
        
        # Import configuration
        for conf in data["configuration"]:
            cursor.execute("""
                INSERT OR REPLACE INTO configuration 
                (id, config_key, config_value, description, updated_at)
                VALUES (?, ?, ?, ?, ?)
            """, (conf["id"], conf["config_key"], conf["config_value"],
                  conf.get("description"), conf["updated_at"]))
        
        conn.commit()
        print(f"Data imported successfully from {json_path}")
        
    except Exception as e:
        conn.rollback()
        print(f"Error importing data: {e}")
        raise
    finally:
        conn.close()


def create_sample_export() -> Dict[str, Any]:
    """
    Create a sample JSON export structure for documentation.
    
    Returns:
        Dictionary representing the JSON export format.
    """
    return {
        "version": "1.0.0",
        "export_date": "2024-01-01T12:00:00",
        "data": {
            "establishments": [
                {
                    "id": 1,
                    "name": "Example Restaurant",
                    "address": "123 Main St",
                    "city": "Springfield",
                    "state": "IL",
                    "zip_code": "62701",
                    "establishment_type": "Restaurant",
                    "license_number": "REST-2024-001",
                    "created_at": "2024-01-01T10:00:00",
                    "updated_at": "2024-01-01T10:00:00"
                }
            ],
            "inspections": [
                {
                    "id": 1,
                    "establishment_id": 1,
                    "inspection_date": "2024-01-15T14:00:00",
                    "inspector_name": "Jane Smith",
                    "food_safety_score": 95.0,
                    "cleanliness_score": 92.0,
                    "temperature_control_score": 98.0,
                    "employee_hygiene_score": 90.0,
                    "total_score": 94.3,
                    "severity": "critical",
                    "flare_triggered": 1,
                    "alert_required": 1,
                    "notes": "Excellent compliance",
                    "created_at": "2024-01-15T14:30:00"
                }
            ],
            "violations": [],
            "configuration": [
                {
                    "id": 1,
                    "config_key": "schema_version",
                    "config_value": "1.0.0",
                    "description": "Database schema version",
                    "updated_at": "2024-01-01T10:00:00"
                }
            ]
        },
        "metadata": {
            "total_establishments": 1,
            "total_inspections": 1,
            "total_violations": 0,
            "configuration_items": 1
        }
    }
