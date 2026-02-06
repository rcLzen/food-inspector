"""
Ingredient Matcher with Synonym Dictionary and Word Boundary Matching
"""

import re
import yaml
import os
from typing import Dict, List, Tuple, Optional
from functools import lru_cache


class IngredientMatcher:
    """
    Matches ingredients using a synonym dictionary with word-boundary-safe matching.
    
    Prevents false matches like "malt" matching "maltodextrin" unless explicitly allowed.
    """
    
    def __init__(self, synonyms_file: Optional[str] = None, exceptions: Optional[Dict[str, List[str]]] = None):
        """
        Initialize the ingredient matcher.
        
        Args:
            synonyms_file: Path to YAML file with ingredient synonyms
            exceptions: Optional dictionary reserved for future use to configure
                        term-specific exceptions. Currently this parameter does not
                        override word-boundary matching or enable partial-word
                        matches (for example, {"malt": ["maltodextrin"]} will NOT
                        cause "maltodextrin" to match "malt").
        """
        self.synonyms: Dict[str, List[str]] = {}
        self.reverse_map: Dict[str, str] = {}  # Maps synonym to allergen category
        self.exceptions: Dict[str, List[str]] = exceptions or {}
        
        # Load synonyms from file
        if synonyms_file is None:
            # Use default data file
            data_dir = os.path.join(os.path.dirname(__file__), '..', '..', 'data')
            synonyms_file = os.path.join(data_dir, 'ingredient_synonyms.yaml')
        
        self._load_synonyms(synonyms_file)
    
    def _load_synonyms(self, synonyms_file: str):
        """Load synonyms from YAML file."""
        try:
            with open(synonyms_file, 'r') as f:
                data = yaml.safe_load(f)
        except FileNotFoundError:
            raise FileNotFoundError(
                f"Ingredient synonyms file not found: '{synonyms_file}'. "
                f"Please ensure the file exists."
            )
        except PermissionError:
            raise PermissionError(
                f"Permission denied when reading ingredient synonyms file: '{synonyms_file}'."
            )
        except yaml.YAMLError as e:
            raise ValueError(
                f"Invalid YAML format in ingredient synonyms file '{synonyms_file}': {e}"
            )
        
        # Validate data structure
        if data is None:
            raise ValueError(
                f"Ingredient synonyms file '{synonyms_file}' is empty or contains no data."
            )
        
        if not isinstance(data, dict):
            raise ValueError(
                f"Invalid data structure in '{synonyms_file}': expected a dictionary "
                f"mapping allergen categories to lists of synonyms, but got {type(data).__name__}."
            )
        
        # Validate each category
        for category, synonyms in data.items():
            if not isinstance(synonyms, list):
                raise ValueError(
                    f"Invalid synonyms for category '{category}' in '{synonyms_file}': "
                    f"expected a list, but got {type(synonyms).__name__}."
                )
            if not all(isinstance(s, str) for s in synonyms):
                raise ValueError(
                    f"Invalid synonyms for category '{category}' in '{synonyms_file}': "
                    f"all synonyms must be strings."
                )
        
        self.synonyms = data
        
        # Build reverse mapping
        for category, synonyms in self.synonyms.items():
            for synonym in synonyms:
                self.reverse_map[synonym.lower()] = category
    
    @lru_cache(maxsize=256)
    def _create_word_boundary_pattern(self, term: str) -> re.Pattern:
        """
        Create a regex pattern that matches the term with word boundaries.
        
        Uses LRU cache to avoid recompiling patterns for repeated terms.
        
        Args:
            term: The ingredient term to match
            
        Returns:
            Compiled regex pattern with word boundaries
        """
        # Escape special regex characters
        escaped_term = re.escape(term)
        
        # Use word boundaries to match whole words only
        # \b matches word boundaries (transition between \w and \W)
        pattern = r'\b' + escaped_term + r'\b'
        
        return re.compile(pattern, re.IGNORECASE)
    
    def find_ingredient(self, text: str, ingredient: str) -> List[Tuple[str, int, int]]:
        """
        Find all occurrences of an ingredient in text using word-boundary matching.
        
        Args:
            text: The text to search (e.g., ingredient list)
            ingredient: The ingredient term to find
            
        Returns:
            List of tuples (matched_text, start_pos, end_pos)
        """
        pattern = self._create_word_boundary_pattern(ingredient)
        matches = []
        
        for match in pattern.finditer(text):
            matched_text = match.group(0)
            matches.append((matched_text, match.start(), match.end()))
        
        return matches
    
    def find_allergen_category(self, text: str, category: str) -> Dict[str, List[Tuple[str, int, int]]]:
        """
        Find all ingredients from a specific allergen category in the text.
        
        Args:
            text: The text to search
            category: The allergen category (e.g., 'dairy', 'gluten', 'soy')
            
        Returns:
            Dictionary mapping found synonyms to their match positions
        """
        if category not in self.synonyms:
            return {}
        
        results = {}
        
        for synonym in self.synonyms[category]:
            matches = self.find_ingredient(text, synonym)
            if matches:
                results[synonym] = matches
        
        return results
    
    def scan_text(self, text: str) -> Dict[str, Dict[str, List[Tuple[str, int, int]]]]:
        """
        Scan text for all known allergen categories and their synonyms.
        
        Args:
            text: The text to scan (e.g., full ingredient list)
            
        Returns:
            Dictionary mapping categories to found ingredients and their positions
        """
        results = {}
        
        for category in self.synonyms.keys():
            category_matches = self.find_allergen_category(text, category)
            if category_matches:
                results[category] = category_matches
        
        return results
    
    def get_allergen_for_ingredient(self, ingredient: str) -> Optional[str]:
        """
        Get the allergen category for a specific ingredient.
        
        Args:
            ingredient: The ingredient name
            
        Returns:
            The allergen category or None if not found
        """
        return self.reverse_map.get(ingredient.lower())
    
    def get_all_synonyms(self, category: str) -> List[str]:
        """
        Get all synonyms for an allergen category.
        
        Args:
            category: The allergen category
            
        Returns:
            List of all synonyms for that category
        """
        return self.synonyms.get(category, [])
