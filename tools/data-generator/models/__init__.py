"""Data models for JSON generation."""

from dataclasses import dataclass, field
from typing import List, Optional
from datetime import datetime


@dataclass
class IngredientTrigger:
    """Represents an ingredient trigger/allergen."""
    id: int
    name: str
    safety_level: str  # "Safe", "Caution", "Avoid"
    description: str
    severity_score: int  # 1-10
    is_common_allergen: bool


@dataclass
class Synonym:
    """Synonym mapping for an ingredient trigger."""
    trigger_id: int
    canonical_name: str
    synonym: str


@dataclass
class CrossReactivity:
    """Cross-reactivity relationship between allergens."""
    primary_trigger_id: int
    related_trigger_id: int
    description: str


@dataclass
class ScoringThresholds:
    """Scoring thresholds for severity classification."""
    critical: int = 90
    high: int = 70
    medium: int = 50
    low: int = 30


@dataclass
class FlareMode:
    """Flare mode configuration."""
    default_threshold: int = 5
    escalation_multiplier: float = 1.5
    min_threshold: int = 1
    max_threshold: int = 10


@dataclass
class ScoringPolicy:
    """Complete scoring policy configuration."""
    thresholds: ScoringThresholds
    flare_mode: FlareMode


@dataclass
class VersionedOutput:
    """Base class for versioned JSON outputs."""
    version: str
    generated_at: str
    
    def __post_init__(self):
        if not self.generated_at:
            self.generated_at = datetime.utcnow().isoformat() + "Z"
