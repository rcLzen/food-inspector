"""
Food Inspector Package
Provides ingredient analysis with synonym matching and cross-reactivity detection.
"""

from .matcher import IngredientMatcher
from .cross_reactivity import CrossReactivityChecker

__version__ = "0.1.0"
__all__ = ["IngredientMatcher", "CrossReactivityChecker"]
