"""Seed data for food inspection database."""

import sqlite3
import json
from datetime import datetime, timedelta
from pathlib import Path


# Sample establishments
SAMPLE_ESTABLISHMENTS = [
    {
        "name": "Joe's Pizza Palace",
        "address": "123 Main Street",
        "city": "Springfield",
        "state": "IL",
        "zip_code": "62701",
        "establishment_type": "Restaurant",
        "license_number": "REST-2024-001"
    },
    {
        "name": "Fresh Market Grocery",
        "address": "456 Oak Avenue",
        "city": "Springfield",
        "state": "IL",
        "zip_code": "62702",
        "establishment_type": "Grocery",
        "license_number": "GROC-2024-002"
    },
    {
        "name": "Downtown Deli",
        "address": "789 Elm Street",
        "city": "Springfield",
        "state": "IL",
        "zip_code": "62703",
        "establishment_type": "Restaurant",
        "license_number": "REST-2024-003"
    },
    {
        "name": "Sunrise Bakery",
        "address": "321 Maple Drive",
        "city": "Springfield",
        "state": "IL",
        "zip_code": "62704",
        "establishment_type": "Bakery",
        "license_number": "BAKE-2024-004"
    },
    {
        "name": "Harbor Seafood Restaurant",
        "address": "555 Water Street",
        "city": "Springfield",
        "state": "IL",
        "zip_code": "62705",
        "establishment_type": "Restaurant",
        "license_number": "REST-2024-005"
    }
]

# Sample inspections with varying scores
SAMPLE_INSPECTIONS = [
    # Good inspection
    {
        "establishment_index": 0,
        "days_ago": 5,
        "inspector_name": "Jane Smith",
        "food_safety_score": 95,
        "cleanliness_score": 92,
        "temperature_control_score": 98,
        "employee_hygiene_score": 90,
        "notes": "Excellent compliance with all standards."
    },
    # Critical score - should trigger flare mode
    {
        "establishment_index": 1,
        "days_ago": 3,
        "inspector_name": "John Doe",
        "food_safety_score": 92,
        "cleanliness_score": 88,
        "temperature_control_score": 95,
        "employee_hygiene_score": 85,
        "notes": "Some minor issues found but overall good."
    },
    # Medium severity
    {
        "establishment_index": 2,
        "days_ago": 10,
        "inspector_name": "Sarah Johnson",
        "food_safety_score": 65,
        "cleanliness_score": 70,
        "temperature_control_score": 60,
        "employee_hygiene_score": 75,
        "notes": "Multiple violations found. Re-inspection required."
    },
    # Low score
    {
        "establishment_index": 3,
        "days_ago": 7,
        "inspector_name": "Mike Williams",
        "food_safety_score": 88,
        "cleanliness_score": 85,
        "temperature_control_score": 90,
        "employee_hygiene_score": 82,
        "notes": "Good practices observed."
    },
    # High severity - triggers flare
    {
        "establishment_index": 4,
        "days_ago": 2,
        "inspector_name": "Jane Smith",
        "food_safety_score": 85,
        "cleanliness_score": 75,
        "temperature_control_score": 82,
        "employee_hygiene_score": 78,
        "notes": "Temperature control issues noted."
    }
]

# Sample violations
SAMPLE_VIOLATIONS = [
    {
        "inspection_index": 2,  # Downtown Deli
        "violation_code": "FS-101",
        "description": "Food stored at incorrect temperature",
        "severity": "high",
        "corrected": False
    },
    {
        "inspection_index": 2,
        "violation_code": "CL-205",
        "description": "Kitchen floor needs deep cleaning",
        "severity": "medium",
        "corrected": False
    },
    {
        "inspection_index": 4,  # Harbor Seafood
        "violation_code": "TC-302",
        "description": "Refrigerator temperature above safe level",
        "severity": "high",
        "corrected": True,
        "days_ago_corrected": 1
    }
]


def seed_database(db_path: str) -> None:
    """
    Seed the database with sample data.
    
    Args:
        db_path: Path to the SQLite database file.
    """
    # Import scoring model to calculate scores
    from food_inspector.scoring import ScoringModel, FlareMode
    
    scoring_model = ScoringModel()
    flare_mode = FlareMode()
    
    conn = sqlite3.connect(db_path)
    cursor = conn.cursor()
    
    try:
        # Insert establishments
        establishment_ids = []
        for est in SAMPLE_ESTABLISHMENTS:
            cursor.execute("""
                INSERT INTO establishments (name, address, city, state, zip_code, establishment_type, license_number)
                VALUES (?, ?, ?, ?, ?, ?, ?)
            """, (est["name"], est["address"], est["city"], est["state"], 
                  est["zip_code"], est["establishment_type"], est["license_number"]))
            establishment_ids.append(cursor.lastrowid)
        
        # Insert inspections
        inspection_ids = []
        for insp in SAMPLE_INSPECTIONS:
            inspection_date = datetime.now() - timedelta(days=insp["days_ago"])
            
            # Calculate scores using the scoring model
            inspection_data = {
                "food_safety": insp["food_safety_score"],
                "cleanliness": insp["cleanliness_score"],
                "temperature_control": insp["temperature_control_score"],
                "employee_hygiene": insp["employee_hygiene_score"]
            }
            
            evaluation = scoring_model.evaluate_inspection(inspection_data)
            flare_eval = flare_mode.evaluate_flare(evaluation["score"])
            
            cursor.execute("""
                INSERT INTO inspections 
                (establishment_id, inspection_date, inspector_name, 
                 food_safety_score, cleanliness_score, temperature_control_score, employee_hygiene_score,
                 total_score, severity, flare_triggered, alert_required, notes)
                VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)
            """, (
                establishment_ids[insp["establishment_index"]],
                inspection_date.isoformat(),
                insp["inspector_name"],
                insp["food_safety_score"],
                insp["cleanliness_score"],
                insp["temperature_control_score"],
                insp["employee_hygiene_score"],
                evaluation["score"],
                evaluation["severity"],
                1 if flare_eval["flare_triggered"] else 0,
                1 if flare_eval["alert_required"] else 0,
                insp["notes"]
            ))
            inspection_ids.append(cursor.lastrowid)
        
        # Insert violations
        for viol in SAMPLE_VIOLATIONS:
            correction_date = None
            if viol.get("corrected") and "days_ago_corrected" in viol:
                correction_date = (datetime.now() - timedelta(days=viol["days_ago_corrected"])).isoformat()
            
            cursor.execute("""
                INSERT INTO violations 
                (inspection_id, violation_code, description, severity, corrected, correction_date)
                VALUES (?, ?, ?, ?, ?, ?)
            """, (
                inspection_ids[viol["inspection_index"]],
                viol["violation_code"],
                viol["description"],
                viol["severity"],
                1 if viol.get("corrected", False) else 0,
                correction_date
            ))
        
        # Insert default configuration
        config_items = [
            ("scoring_threshold_critical", "90", "Critical severity threshold"),
            ("scoring_threshold_high", "70", "High severity threshold"),
            ("scoring_threshold_medium", "50", "Medium severity threshold"),
            ("scoring_threshold_low", "30", "Low severity threshold"),
            ("flare_mode_enabled", "true", "Enable flare mode"),
            ("flare_trigger_score", "80", "Score to trigger flare mode"),
            ("flare_escalation_multiplier", "1.5", "Priority escalation multiplier"),
            ("flare_alert_threshold", "85", "Score requiring immediate alert")
        ]
        
        for key, value, desc in config_items:
            cursor.execute("""
                INSERT OR REPLACE INTO configuration (config_key, config_value, description)
                VALUES (?, ?, ?)
            """, (key, value, desc))
        
        conn.commit()
        print(f"Database seeded successfully with {len(SAMPLE_ESTABLISHMENTS)} establishments")
        print(f"Added {len(SAMPLE_INSPECTIONS)} inspections and {len(SAMPLE_VIOLATIONS)} violations")
        
    except Exception as e:
        conn.rollback()
        print(f"Error seeding database: {e}")
        raise
    finally:
        conn.close()
