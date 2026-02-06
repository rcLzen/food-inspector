"""Database migration and initialization scripts."""

import sqlite3
from pathlib import Path
from typing import Optional
from .schema import SCHEMA_SQL, TRIGGERS_SQL, SCHEMA_VERSION


def init_database(db_path: str) -> None:
    """
    Initialize a new SQLite database with the food inspection schema.
    
    Args:
        db_path: Path to the SQLite database file.
    """
    conn = sqlite3.connect(db_path)
    cursor = conn.cursor()
    
    try:
        # Create schema
        cursor.executescript(SCHEMA_SQL)
        
        # Create triggers
        cursor.executescript(TRIGGERS_SQL)
        
        # Insert schema version
        cursor.execute(
            "INSERT OR REPLACE INTO configuration (config_key, config_value, description) VALUES (?, ?, ?)",
            ("schema_version", SCHEMA_VERSION, "Database schema version")
        )
        
        conn.commit()
        print(f"Database initialized successfully at {db_path}")
        print(f"Schema version: {SCHEMA_VERSION}")
        
    except Exception as e:
        conn.rollback()
        print(f"Error initializing database: {e}")
        raise
    finally:
        conn.close()


def migrate_database(db_path: str, target_version: Optional[str] = None) -> None:
    """
    Migrate database to a specific version.
    
    Args:
        db_path: Path to the SQLite database file.
        target_version: Target schema version. If None, migrates to latest.
    """
    conn = sqlite3.connect(db_path)
    cursor = conn.cursor()
    
    try:
        # Get current version
        cursor.execute(
            "SELECT config_value FROM configuration WHERE config_key = 'schema_version'"
        )
        result = cursor.fetchone()
        current_version = result[0] if result else "0.0.0"
        
        target = target_version or SCHEMA_VERSION
        
        if current_version == target:
            print(f"Database already at version {target}")
            return
        
        print(f"Migrating database from {current_version} to {target}")
        
        # For now, we only have version 1.0.0
        # Future migrations would be added here
        
        # Update schema version
        cursor.execute(
            "UPDATE configuration SET config_value = ? WHERE config_key = 'schema_version'",
            (target,)
        )
        
        conn.commit()
        print(f"Migration completed successfully")
        
    except Exception as e:
        conn.rollback()
        print(f"Error migrating database: {e}")
        raise
    finally:
        conn.close()


def get_database_version(db_path: str) -> str:
    """
    Get the current schema version of the database.
    
    Args:
        db_path: Path to the SQLite database file.
    
    Returns:
        Schema version string.
    """
    conn = sqlite3.connect(db_path)
    cursor = conn.cursor()
    
    try:
        cursor.execute(
            "SELECT config_value FROM configuration WHERE config_key = 'schema_version'"
        )
        result = cursor.fetchone()
        return result[0] if result else "unknown"
    finally:
        conn.close()
