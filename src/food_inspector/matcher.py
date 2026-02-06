"""
Ingredient Matcher with Synonym Dictionary and Word Boundary Matching
"""

import re
import yaml
import os
from typing import Dict, List, Set, Tuple, Optional


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
            exceptions: Dictionary mapping ingredient terms to allowed exceptions
                       (e.g., {"malt": ["maltodextrin"]} to allow maltodextrin to match malt)
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
        with open(synonyms_file, 'r') as f:
            data = yaml.safe_load(f)
        
        self.synonyms = data
        
        # Build reverse mapping
        for category, synonyms in self.synonyms.items():
            for synonym in synonyms:
                self.reverse_map[synonym.lower()] = category
    
    def _create_word_boundary_pattern(self, term: str) -> re.Pattern:
        """
        Create a regex pattern that matches the term with word boundaries.
        
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
            
            # Check if this match should be excluded
            if self._should_exclude_match(text, matched_text, match.start(), match.end(), ingredient):
                continue
            
            matches.append((matched_text, match.start(), match.end()))
        
        return matches
    
    def _should_exclude_match(self, text: str, matched_text: str, start: int, end: int, 
                              ingredient: str) -> bool:
        """
        Determine if a match should be excluded based on exceptions.
        
        Args:
            text: Full text being searched
            matched_text: The matched text
            start: Start position of match
            end: End position of match
            ingredient: The ingredient being searched for
            
        Returns:
            True if match should be excluded, False otherwise
        """
        # Check if there's a larger word that contains this match
        # Look for alphanumeric characters before and after
        
        # Get a window around the match to check for compound words
        window_start = max(0, start - 20)
        window_end = min(len(text), end + 20)
        window = text[window_start:window_end]
        
        # Extract the word that contains our match
        match_in_window = start - window_start
        
        # Find the boundaries of the containing word
        word_start = match_in_window
        while word_start > 0 and (window[word_start - 1].isalnum() or window[word_start - 1] in ['-', "'"]):
            word_start -= 1
        
        word_end = match_in_window + len(matched_text)
        while word_end < len(window) and (window[word_end].isalnum() or window[word_end] in ['-', "'"]):
            word_end += 1
        
        containing_word = window[word_start:word_end]
        
        # If the containing word is different from matched text, check exceptions
        if containing_word.lower() != matched_text.lower():
            # Check if this compound word is in the allowed exceptions
            exceptions_for_ingredient = self.exceptions.get(ingredient.lower(), [])
            if containing_word.lower() not in [e.lower() for e in exceptions_for_ingredient]:
                return True  # Exclude this match
        
        return False
    
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
