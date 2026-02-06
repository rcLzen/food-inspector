"""Scoring model for food inspection with configurable thresholds."""

import json
from typing import Dict, Any, Optional
from pathlib import Path


class ScoringModel:
    """
    A configurable scoring model for food inspections.
    
    The model calculates weighted scores based on multiple inspection criteria
    and provides severity classification based on configurable thresholds.
    """
    
    def __init__(self, config_path: Optional[str] = None):
        """
        Initialize the scoring model with configuration.
        
        Args:
            config_path: Path to JSON configuration file. If None, uses default config.
        """
        if config_path:
            with open(config_path, 'r') as f:
                self.config = json.load(f)
        else:
            # Default configuration
            self.config = {
                "scoring_thresholds": {
                    "critical": 90,
                    "high": 70,
                    "medium": 50,
                    "low": 30
                },
                "inspection_weights": {
                    "food_safety": 0.40,
                    "cleanliness": 0.30,
                    "temperature_control": 0.20,
                    "employee_hygiene": 0.10
                }
            }
        
        self.thresholds = self.config["scoring_thresholds"]
        self.weights = self.config["inspection_weights"]
    
    def calculate_score(self, inspection_data: Dict[str, float]) -> float:
        """
        Calculate weighted inspection score.
        
        Args:
            inspection_data: Dictionary with inspection criteria scores (0-100).
                            Keys should match weight keys.
        
        Returns:
            Weighted total score (0-100).
        """
        total_score = 0.0
        total_weight = 0.0
        
        for criterion, weight in self.weights.items():
            if criterion in inspection_data:
                total_score += inspection_data[criterion] * weight
                total_weight += weight
        
        # Normalize if not all criteria provided
        if total_weight > 0:
            return total_score / total_weight if total_weight != 1.0 else total_score
        
        return 0.0
    
    def get_severity(self, score: float) -> str:
        """
        Determine severity level based on score and thresholds.
        
        Args:
            score: Calculated inspection score (0-100).
        
        Returns:
            Severity level: 'critical', 'high', 'medium', 'low', or 'minimal'.
        """
        if score >= self.thresholds["critical"]:
            return "critical"
        elif score >= self.thresholds["high"]:
            return "high"
        elif score >= self.thresholds["medium"]:
            return "medium"
        elif score >= self.thresholds["low"]:
            return "low"
        else:
            return "minimal"
    
    def evaluate_inspection(self, inspection_data: Dict[str, float]) -> Dict[str, Any]:
        """
        Complete evaluation of inspection data.
        
        Args:
            inspection_data: Dictionary with inspection criteria scores.
        
        Returns:
            Dictionary with score, severity, and details.
        """
        score = self.calculate_score(inspection_data)
        severity = self.get_severity(score)
        
        return {
            "score": round(score, 2),
            "severity": severity,
            "criteria": inspection_data,
            "thresholds": self.thresholds
        }


class FlareMode:
    """
    Flare-mode policy for elevated inspection responses.
    
    When triggered, applies escalation multipliers and alerts for high-risk situations.
    """
    
    def __init__(self, config_path: Optional[str] = None):
        """
        Initialize flare mode with configuration.
        
        Args:
            config_path: Path to JSON configuration file. If None, uses default config.
        """
        if config_path:
            with open(config_path, 'r') as f:
                self.config = json.load(f)
        else:
            # Default configuration
            self.config = {
                "flare_mode": {
                    "enabled": True,
                    "trigger_score": 80,
                    "escalation_multiplier": 1.5,
                    "alert_threshold": 85
                }
            }
        
        self.flare_config = self.config["flare_mode"]
    
    def is_enabled(self) -> bool:
        """Check if flare mode is enabled."""
        return self.flare_config.get("enabled", False)
    
    def should_trigger(self, score: float) -> bool:
        """
        Determine if flare mode should be triggered.
        
        Args:
            score: Inspection score (0-100).
        
        Returns:
            True if score exceeds trigger threshold.
        """
        if not self.is_enabled():
            return False
        
        return score >= self.flare_config["trigger_score"]
    
    def should_alert(self, score: float) -> bool:
        """
        Determine if immediate alert should be sent.
        
        Args:
            score: Inspection score (0-100).
        
        Returns:
            True if score exceeds alert threshold.
        """
        if not self.is_enabled():
            return False
        
        return score >= self.flare_config["alert_threshold"]
    
    def apply_escalation(self, base_priority: int) -> int:
        """
        Apply escalation multiplier to priority level.
        
        Args:
            base_priority: Base priority level (1-10).
        
        Returns:
            Escalated priority level (capped at 10).
        """
        if not self.is_enabled():
            return base_priority
        
        multiplier = self.flare_config["escalation_multiplier"]
        escalated = int(base_priority * multiplier)
        
        return min(escalated, 10)
    
    def evaluate_flare(self, score: float, base_priority: int = 5) -> Dict[str, Any]:
        """
        Complete flare mode evaluation.
        
        Args:
            score: Inspection score (0-100).
            base_priority: Base priority level (1-10).
        
        Returns:
            Dictionary with flare status and actions.
        """
        triggered = self.should_trigger(score)
        alert_required = self.should_alert(score)
        escalated_priority = self.apply_escalation(base_priority) if triggered else base_priority
        
        return {
            "flare_triggered": triggered,
            "alert_required": alert_required,
            "base_priority": base_priority,
            "escalated_priority": escalated_priority,
            "score": score,
            "thresholds": {
                "trigger": self.flare_config["trigger_score"],
                "alert": self.flare_config["alert_threshold"]
            }
        }
