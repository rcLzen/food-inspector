"""Generator for ingredient synonym JSON data."""

import json
import sys
from datetime import datetime
from typing import List, Dict
from pathlib import Path

# Add parent to path for imports
sys.path.insert(0, str(Path(__file__).parent.parent))

from models import IngredientTrigger, Synonym


def generate_synonyms_json(version: str = "1.0.0") -> Dict:
    """
    Generate synonyms JSON data compatible with MAUI app.
    
    Args:
        version: Semantic version string (MAJOR.MINOR.PATCH)
        
    Returns:
        Dictionary ready for JSON serialization
    """
    
    # Define ingredient triggers (must match MAUI IngredientTrigger IDs)
    triggers = [
        IngredientTrigger(1, "Peanuts", "Avoid", "Common allergen", 10, True),
        IngredientTrigger(2, "Tree Nuts", "Avoid", "Common allergen", 10, True),
        IngredientTrigger(3, "Milk", "Avoid", "Common allergen", 8, True),
        IngredientTrigger(4, "Eggs", "Avoid", "Common allergen", 8, True),
        IngredientTrigger(5, "Soy", "Caution", "Common allergen", 7, True),
        IngredientTrigger(6, "Wheat", "Avoid", "Contains gluten", 9, True),
        IngredientTrigger(7, "Fish", "Avoid", "Common allergen", 9, True),
        IngredientTrigger(8, "Shellfish", "Avoid", "Common allergen", 10, True),
        IngredientTrigger(9, "Sesame", "Caution", "Common allergen", 7, True),
        IngredientTrigger(10, "MSG", "Caution", "May cause sensitivity", 5, False),
        IngredientTrigger(11, "Sulfites", "Caution", "Preservative", 6, False),
        IngredientTrigger(12, "Corn", "Caution", "Common allergen", 6, True),
        IngredientTrigger(13, "Nitrates", "Caution", "Preservative", 5, False),
        IngredientTrigger(14, "Artificial Colors", "Caution", "May cause reactions", 4, False),
        IngredientTrigger(15, "Gluten", "Avoid", "Protein in wheat", 9, True),
    ]
    
    # Define synonyms for each trigger
    synonyms_data = [
        # Peanuts (ID: 1)
        Synonym(1, "Peanuts", "groundnut"),
        Synonym(1, "Peanuts", "arachis oil"),
        Synonym(1, "Peanuts", "goober"),
        Synonym(1, "Peanuts", "monkey nut"),
        Synonym(1, "Peanuts", "peanut butter"),
        Synonym(1, "Peanuts", "peanut flour"),
        
        # Tree Nuts (ID: 2)
        Synonym(2, "Tree Nuts", "almond"),
        Synonym(2, "Tree Nuts", "cashew"),
        Synonym(2, "Tree Nuts", "walnut"),
        Synonym(2, "Tree Nuts", "pecan"),
        Synonym(2, "Tree Nuts", "pistachio"),
        Synonym(2, "Tree Nuts", "macadamia"),
        Synonym(2, "Tree Nuts", "hazelnut"),
        Synonym(2, "Tree Nuts", "brazil nut"),
        Synonym(2, "Tree Nuts", "pine nut"),
        Synonym(2, "Tree Nuts", "nut butter"),
        
        # Milk (ID: 3)
        Synonym(3, "Milk", "dairy"),
        Synonym(3, "Milk", "lactose"),
        Synonym(3, "Milk", "casein"),
        Synonym(3, "Milk", "whey"),
        Synonym(3, "Milk", "butter"),
        Synonym(3, "Milk", "cheese"),
        Synonym(3, "Milk", "cream"),
        Synonym(3, "Milk", "yogurt"),
        Synonym(3, "Milk", "milk powder"),
        Synonym(3, "Milk", "milk solids"),
        Synonym(3, "Milk", "ghee"),
        
        # Eggs (ID: 4)
        Synonym(4, "Eggs", "albumin"),
        Synonym(4, "Eggs", "egg white"),
        Synonym(4, "Eggs", "egg yolk"),
        Synonym(4, "Eggs", "mayonnaise"),
        Synonym(4, "Eggs", "egg protein"),
        Synonym(4, "Eggs", "ovomucoid"),
        Synonym(4, "Eggs", "ovalbumin"),
        
        # Soy (ID: 5)
        Synonym(5, "Soy", "soya"),
        Synonym(5, "Soy", "soybean"),
        Synonym(5, "Soy", "tofu"),
        Synonym(5, "Soy", "edamame"),
        Synonym(5, "Soy", "soy sauce"),
        Synonym(5, "Soy", "soy protein"),
        Synonym(5, "Soy", "soy lecithin"),
        Synonym(5, "Soy", "tempeh"),
        Synonym(5, "Soy", "miso"),
        
        # Wheat (ID: 6)
        Synonym(6, "Wheat", "wheat flour"),
        Synonym(6, "Wheat", "wheat starch"),
        Synonym(6, "Wheat", "wheat protein"),
        Synonym(6, "Wheat", "whole wheat"),
        Synonym(6, "Wheat", "durum"),
        Synonym(6, "Wheat", "semolina"),
        Synonym(6, "Wheat", "spelt"),
        Synonym(6, "Wheat", "farro"),
        Synonym(6, "Wheat", "bread flour"),
        
        # Fish (ID: 7)
        Synonym(7, "Fish", "salmon"),
        Synonym(7, "Fish", "tuna"),
        Synonym(7, "Fish", "cod"),
        Synonym(7, "Fish", "anchovy"),
        Synonym(7, "Fish", "fish sauce"),
        Synonym(7, "Fish", "fish oil"),
        Synonym(7, "Fish", "seafood"),
        
        # Shellfish (ID: 8)
        Synonym(8, "Shellfish", "shrimp"),
        Synonym(8, "Shellfish", "prawn"),
        Synonym(8, "Shellfish", "crab"),
        Synonym(8, "Shellfish", "lobster"),
        Synonym(8, "Shellfish", "oyster"),
        Synonym(8, "Shellfish", "clam"),
        Synonym(8, "Shellfish", "mussel"),
        Synonym(8, "Shellfish", "scallop"),
        Synonym(8, "Shellfish", "crayfish"),
        
        # Sesame (ID: 9)
        Synonym(9, "Sesame", "tahini"),
        Synonym(9, "Sesame", "sesame oil"),
        Synonym(9, "Sesame", "sesame seed"),
        Synonym(9, "Sesame", "sesamol"),
        
        # MSG (ID: 10)
        Synonym(10, "MSG", "monosodium glutamate"),
        Synonym(10, "MSG", "glutamate"),
        Synonym(10, "MSG", "E621"),
        Synonym(10, "MSG", "yeast extract"),
        Synonym(10, "MSG", "hydrolyzed protein"),
        
        # Sulfites (ID: 11)
        Synonym(11, "Sulfites", "sulfur dioxide"),
        Synonym(11, "Sulfites", "sodium sulfite"),
        Synonym(11, "Sulfites", "E220"),
        Synonym(11, "Sulfites", "E221"),
        Synonym(11, "Sulfites", "metabisulfite"),
        
        # Corn (ID: 12)
        Synonym(12, "Corn", "maize"),
        Synonym(12, "Corn", "corn starch"),
        Synonym(12, "Corn", "corn syrup"),
        Synonym(12, "Corn", "cornmeal"),
        Synonym(12, "Corn", "high fructose corn syrup"),
        Synonym(12, "Corn", "dextrose"),
        Synonym(12, "Corn", "maltodextrin"),
        
        # Nitrates (ID: 13)
        Synonym(13, "Nitrates", "sodium nitrate"),
        Synonym(13, "Nitrates", "sodium nitrite"),
        Synonym(13, "Nitrates", "E250"),
        Synonym(13, "Nitrates", "E251"),
        Synonym(13, "Nitrates", "curing salt"),
        
        # Artificial Colors (ID: 14)
        Synonym(14, "Artificial Colors", "FD&C"),
        Synonym(14, "Artificial Colors", "food dye"),
        Synonym(14, "Artificial Colors", "tartrazine"),
        Synonym(14, "Artificial Colors", "yellow 5"),
        Synonym(14, "Artificial Colors", "red 40"),
        Synonym(14, "Artificial Colors", "blue 1"),
        
        # Gluten (ID: 15)
        Synonym(15, "Gluten", "wheat gluten"),
        Synonym(15, "Gluten", "barley"),
        Synonym(15, "Gluten", "rye"),
        Synonym(15, "Gluten", "malt"),
        Synonym(15, "Gluten", "brewer's yeast"),
    ]
    
    # Group synonyms by trigger
    grouped_synonyms = {}
    for syn in synonyms_data:
        if syn.trigger_id not in grouped_synonyms:
            trigger = next((t for t in triggers if t.id == syn.trigger_id), None)
            grouped_synonyms[syn.trigger_id] = {
                "trigger_id": syn.trigger_id,
                "canonical_name": trigger.name if trigger else "",
                "synonyms": []
            }
        grouped_synonyms[syn.trigger_id]["synonyms"].append(syn.synonym)
    
    # Build output structure
    output = {
        "version": version,
        "generated_at": datetime.utcnow().isoformat() + "Z",
        "data": list(grouped_synonyms.values())
    }
    
    return output


if __name__ == "__main__":
    # Test generation
    result = generate_synonyms_json()
    print(json.dumps(result, indent=2))
