"""Generator for cross-reactivity JSON data."""

import json
import sys
from datetime import datetime
from typing import Dict
from pathlib import Path

# Add parent to path for imports
sys.path.insert(0, str(Path(__file__).parent.parent))

from models import CrossReactivity


def generate_cross_reactivity_json(version: str = "1.0.0") -> Dict:
    """
    Generate cross-reactivity JSON data compatible with MAUI app.
    
    Args:
        version: Semantic version string (MAJOR.MINOR.PATCH)
        
    Returns:
        Dictionary ready for JSON serialization
    """
    
    # Define cross-reactivity relationships
    # IDs must match MAUI IngredientTrigger IDs
    relationships = [
        # Peanuts (1) cross-reacts with Tree Nuts (2)
        CrossReactivity(
            1, 2,
            "Peanut allergy may cross-react with tree nuts in some individuals"
        ),
        
        # Tree Nuts (2) cross-react with Peanuts (1)
        CrossReactivity(
            2, 1,
            "Tree nut allergy may cross-react with peanuts"
        ),
        
        # Milk (3) cross-reacts with Soy (5)
        CrossReactivity(
            3, 5,
            "Some individuals with milk allergy may react to soy protein"
        ),
        
        # Eggs (4) cross-react with Chicken
        # Note: Chicken not in base triggers, but documented for future
        
        # Wheat (6) cross-reacts with Gluten (15)
        CrossReactivity(
            6, 15,
            "Wheat contains gluten; both should be avoided for celiac/gluten sensitivity"
        ),
        
        # Gluten (15) cross-reacts with Wheat (6)
        CrossReactivity(
            15, 6,
            "Gluten sensitivity requires avoiding wheat and other gluten-containing grains"
        ),
        
        # Fish (7) may cross-react between species
        # Shellfish (8) may cross-react between species
        CrossReactivity(
            7, 8,
            "Fish and shellfish are different allergen classes but may cause cross-contamination"
        ),
        
        # Shellfish (8) cross-reacts within class
        CrossReactivity(
            8, 7,
            "Shellfish allergy typically requires avoiding all shellfish species"
        ),
        
        # Corn (12) may cross-react with other grains
        CrossReactivity(
            12, 6,
            "Some individuals sensitive to corn may also react to wheat"
        ),
        
        # Sulfites (11) commonly found with wine/dried fruits
        # May trigger reactions in sensitive individuals
        
        # MSG (10) sensitivity may extend to other glutamates
        CrossReactivity(
            10, 5,
            "MSG sensitivity may be aggravated by naturally occurring glutamates in soy"
        ),
    ]
    
    # Build output structure
    output = {
        "version": version,
        "generated_at": datetime.utcnow().isoformat() + "Z",
        "data": [
            {
                "primary_trigger_id": rel.primary_trigger_id,
                "related_trigger_id": rel.related_trigger_id,
                "description": rel.description
            }
            for rel in relationships
        ]
    }
    
    return output


if __name__ == "__main__":
    # Test generation
    result = generate_cross_reactivity_json()
    print(json.dumps(result, indent=2))
