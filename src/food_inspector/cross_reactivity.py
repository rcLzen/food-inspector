"""
Cross-Reactivity Checker
Provides structured cross-reactivity rules for allergens.
"""

import yaml
import os
from typing import Dict, List, Optional
from dataclasses import dataclass


@dataclass
class CrossReactivityRule:
    """Represents a cross-reactivity rule between allergens."""
    source: str
    target: str
    confidence: str  # 'low', 'medium', 'high'
    notes: str
    
    def __str__(self):
        return f"{self.source} → {self.target} (confidence: {self.confidence})"


class CrossReactivityChecker:
    """
    Manages cross-reactivity rules between allergens.
    
    Provides methods to query potential cross-reactions based on known allergens.
    """
    
    def __init__(self, rules_file: Optional[str] = None):
        """
        Initialize the cross-reactivity checker.
        
        Args:
            rules_file: Path to YAML file with cross-reactivity rules
        """
        self.rules: List[CrossReactivityRule] = []
        self.rules_by_source: Dict[str, List[CrossReactivityRule]] = {}
        self.rules_by_target: Dict[str, List[CrossReactivityRule]] = {}
        
        # Load rules from file
        if rules_file is None:
            # Use default data file
            data_dir = os.path.join(os.path.dirname(__file__), '..', '..', 'data')
            rules_file = os.path.join(data_dir, 'cross_reactivity.yaml')
        
        self._load_rules(rules_file)
    
    def _load_rules(self, rules_file: str):
        """Load cross-reactivity rules from YAML file."""
        with open(rules_file, 'r') as f:
            data = yaml.safe_load(f)
        
        rules_data = data.get('cross_reactivity_rules', [])
        
        for rule_dict in rules_data:
            rule = CrossReactivityRule(
                source=rule_dict['source'],
                target=rule_dict['target'],
                confidence=rule_dict['confidence'],
                notes=rule_dict.get('notes', '')
            )
            self.rules.append(rule)
            
            # Index by source
            if rule.source not in self.rules_by_source:
                self.rules_by_source[rule.source] = []
            self.rules_by_source[rule.source].append(rule)
            
            # Index by target
            if rule.target not in self.rules_by_target:
                self.rules_by_target[rule.target] = []
            self.rules_by_target[rule.target].append(rule)
    
    def get_potential_reactions(self, allergen: str, 
                               min_confidence: Optional[str] = None) -> List[CrossReactivityRule]:
        """
        Get potential cross-reactive allergens for a given source allergen.
        
        Args:
            allergen: The source allergen
            min_confidence: Optional minimum confidence level ('low', 'medium', 'high')
            
        Returns:
            List of cross-reactivity rules for this allergen
        """
        rules = self.rules_by_source.get(allergen, [])
        
        if min_confidence:
            confidence_levels = {'low': 1, 'medium': 2, 'high': 3}
            min_level = confidence_levels.get(min_confidence, 1)
            rules = [r for r in rules if confidence_levels.get(r.confidence, 1) >= min_level]
        
        return rules
    
    def get_sources_for_target(self, target_allergen: str,
                               min_confidence: Optional[str] = None) -> List[CrossReactivityRule]:
        """
        Get allergens that may cross-react to a target allergen.
        
        Args:
            target_allergen: The target allergen
            min_confidence: Optional minimum confidence level ('low', 'medium', 'high')
            
        Returns:
            List of cross-reactivity rules targeting this allergen
        """
        rules = self.rules_by_target.get(target_allergen, [])
        
        if min_confidence:
            confidence_levels = {'low': 1, 'medium': 2, 'high': 3}
            min_level = confidence_levels.get(min_confidence, 1)
            rules = [r for r in rules if confidence_levels.get(r.confidence, 1) >= min_level]
        
        return rules
    
    def check_cross_reactivity(self, source: str, target: str) -> Optional[CrossReactivityRule]:
        """
        Check if there's a known cross-reactivity between two allergens.
        
        Args:
            source: Source allergen
            target: Target allergen
            
        Returns:
            CrossReactivityRule if found, None otherwise
        """
        rules = self.rules_by_source.get(source, [])
        for rule in rules:
            if rule.target == target:
                return rule
        return None
    
    def get_all_rules(self) -> List[CrossReactivityRule]:
        """
        Get all cross-reactivity rules.
        
        Returns:
            List of all rules
        """
        return self.rules
    
    def get_rules_by_confidence(self, confidence: str) -> List[CrossReactivityRule]:
        """
        Get all rules with a specific confidence level.
        
        Args:
            confidence: Confidence level ('low', 'medium', 'high')
            
        Returns:
            List of rules matching the confidence level
        """
        return [r for r in self.rules if r.confidence == confidence]
    
    def format_warnings(self, allergen: str, min_confidence: str = 'low') -> List[str]:
        """
        Format cross-reactivity warnings for display.
        
        Args:
            allergen: The source allergen
            min_confidence: Minimum confidence level to include
            
        Returns:
            List of formatted warning strings
        """
        rules = self.get_potential_reactions(allergen, min_confidence)
        warnings = []
        
        for rule in rules:
            warning = f"⚠️  May cross-react with {rule.target} (confidence: {rule.confidence})"
            if rule.notes:
                warning += f"\n   Note: {rule.notes}"
            warnings.append(warning)
        
        return warnings
