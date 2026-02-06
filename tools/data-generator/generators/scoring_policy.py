"""Generator for scoring policy JSON data."""

import json
import sys
from datetime import datetime
from typing import Dict
from pathlib import Path

# Add parent to path for imports
sys.path.insert(0, str(Path(__file__).parent.parent))

from models import ScoringThresholds, FlareMode


def generate_scoring_policy_json(version: str = "1.0.0") -> Dict:
    """
    Generate scoring policy JSON data compatible with MAUI app.
    
    Args:
        version: Semantic version string (MAJOR.MINOR.PATCH)
        
    Returns:
        Dictionary ready for JSON serialization
    """
    
    # Define scoring thresholds
    thresholds = ScoringThresholds(
        critical=90,
        high=70,
        medium=50,
        low=30
    )
    
    # Define flare mode configuration
    flare_mode = FlareMode(
        default_threshold=5,
        escalation_multiplier=1.5,
        min_threshold=1,
        max_threshold=10
    )
    
    # Build output structure
    output = {
        "version": version,
        "generated_at": datetime.utcnow().isoformat() + "Z",
        "scoring_thresholds": {
            "critical": thresholds.critical,
            "high": thresholds.high,
            "medium": thresholds.medium,
            "low": thresholds.low
        },
        "flare_mode": {
            "default_threshold": flare_mode.default_threshold,
            "escalation_multiplier": flare_mode.escalation_multiplier,
            "min_threshold": flare_mode.min_threshold,
            "max_threshold": flare_mode.max_threshold,
            "description": "Flare mode heightens sensitivity by only flagging triggers with severity >= threshold"
        },
        "severity_levels": {
            "1": "Minimal - mild sensitivity",
            "2-3": "Low - minor reactions possible",
            "4-6": "Medium - moderate reactions",
            "7-8": "High - significant reactions",
            "9-10": "Critical - severe reactions, common allergens"
        }
    }
    
    return output


if __name__ == "__main__":
    # Test generation
    result = generate_scoring_policy_json()
    print(json.dumps(result, indent=2))
