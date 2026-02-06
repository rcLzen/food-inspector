"""Database schema for food inspection system."""

SCHEMA_VERSION = "1.0.0"

# SQLite schema for food inspection data
SCHEMA_SQL = """
-- Food Inspection Database Schema v1.0.0

-- Establishments table
CREATE TABLE IF NOT EXISTS establishments (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    name TEXT NOT NULL,
    address TEXT NOT NULL,
    city TEXT NOT NULL,
    state TEXT NOT NULL,
    zip_code TEXT NOT NULL,
    establishment_type TEXT NOT NULL,
    license_number TEXT UNIQUE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Inspections table
CREATE TABLE IF NOT EXISTS inspections (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    establishment_id INTEGER NOT NULL,
    inspection_date TIMESTAMP NOT NULL,
    inspector_name TEXT NOT NULL,
    food_safety_score REAL NOT NULL CHECK(food_safety_score >= 0 AND food_safety_score <= 100),
    cleanliness_score REAL NOT NULL CHECK(cleanliness_score >= 0 AND cleanliness_score <= 100),
    temperature_control_score REAL NOT NULL CHECK(temperature_control_score >= 0 AND temperature_control_score <= 100),
    employee_hygiene_score REAL NOT NULL CHECK(employee_hygiene_score >= 0 AND employee_hygiene_score <= 100),
    total_score REAL NOT NULL,
    severity TEXT NOT NULL,
    flare_triggered BOOLEAN DEFAULT 0,
    alert_required BOOLEAN DEFAULT 0,
    notes TEXT,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (establishment_id) REFERENCES establishments(id) ON DELETE CASCADE
);

-- Violations table
CREATE TABLE IF NOT EXISTS violations (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    inspection_id INTEGER NOT NULL,
    violation_code TEXT NOT NULL,
    description TEXT NOT NULL,
    severity TEXT NOT NULL,
    corrected BOOLEAN DEFAULT 0,
    correction_date TIMESTAMP,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (inspection_id) REFERENCES inspections(id) ON DELETE CASCADE
);

-- Configuration table for thresholds
CREATE TABLE IF NOT EXISTS configuration (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    config_key TEXT UNIQUE NOT NULL,
    config_value TEXT NOT NULL,
    description TEXT,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Indexes for performance
CREATE INDEX IF NOT EXISTS idx_inspections_establishment ON inspections(establishment_id);
CREATE INDEX IF NOT EXISTS idx_inspections_date ON inspections(inspection_date);
CREATE INDEX IF NOT EXISTS idx_inspections_severity ON inspections(severity);
CREATE INDEX IF NOT EXISTS idx_violations_inspection ON violations(inspection_id);
CREATE INDEX IF NOT EXISTS idx_establishments_license ON establishments(license_number);
"""

# Trigger to update timestamps
TRIGGERS_SQL = """
-- Trigger to update updated_at on establishments
CREATE TRIGGER IF NOT EXISTS update_establishments_timestamp 
AFTER UPDATE ON establishments
BEGIN
    UPDATE establishments SET updated_at = CURRENT_TIMESTAMP WHERE id = NEW.id;
END;

-- Trigger to update updated_at on configuration
CREATE TRIGGER IF NOT EXISTS update_configuration_timestamp 
AFTER UPDATE ON configuration
BEGIN
    UPDATE configuration SET updated_at = CURRENT_TIMESTAMP WHERE id = NEW.id;
END;
"""
