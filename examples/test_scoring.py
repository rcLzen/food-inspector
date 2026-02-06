#!/usr/bin/env python3
"""
Simple unit tests for the scoring model and flare mode.
"""

import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).parent.parent))

from food_inspector.scoring import ScoringModel, FlareMode


def test_scoring_model():
    """Test scoring model calculations."""
    print("Testing Scoring Model...")
    model = ScoringModel()
    
    # Test 1: Perfect score
    data = {
        "food_safety": 100,
        "cleanliness": 100,
        "temperature_control": 100,
        "employee_hygiene": 100
    }
    result = model.evaluate_inspection(data)
    assert result["score"] == 100.0, f"Expected 100.0, got {result['score']}"
    assert result["severity"] == "critical", f"Expected 'critical', got {result['severity']}"
    print("  ✓ Perfect score test passed")
    
    # Test 2: Weighted calculation
    data = {
        "food_safety": 80,  # 40% weight
        "cleanliness": 60,  # 30% weight
        "temperature_control": 70,  # 20% weight
        "employee_hygiene": 90  # 10% weight
    }
    # Expected: 80*0.4 + 60*0.3 + 70*0.2 + 90*0.1 = 32 + 18 + 14 + 9 = 73
    result = model.evaluate_inspection(data)
    assert abs(result["score"] - 73.0) < 0.1, f"Expected 73.0, got {result['score']}"
    assert result["severity"] == "high", f"Expected 'high', got {result['severity']}"
    print("  ✓ Weighted calculation test passed")
    
    # Test 3: Severity thresholds
    test_cases = [
        (95, "critical"),
        (75, "high"),
        (55, "medium"),
        (35, "low"),
        (15, "minimal")
    ]
    for score, expected_severity in test_cases:
        severity = model.get_severity(score)
        assert severity == expected_severity, f"Score {score}: expected '{expected_severity}', got '{severity}'"
    print("  ✓ Severity threshold tests passed")


def test_flare_mode():
    """Test flare mode functionality."""
    print("\nTesting Flare Mode...")
    flare = FlareMode()
    
    # Test 1: Trigger threshold
    assert flare.should_trigger(85), "Score 85 should trigger flare mode"
    assert not flare.should_trigger(75), "Score 75 should not trigger flare mode"
    print("  ✓ Trigger threshold test passed")
    
    # Test 2: Alert threshold
    assert flare.should_alert(90), "Score 90 should trigger alert"
    assert not flare.should_alert(82), "Score 82 should not trigger alert"
    print("  ✓ Alert threshold test passed")
    
    # Test 3: Priority escalation
    escalated = flare.apply_escalation(5)
    assert escalated == 7, f"Expected priority 7, got {escalated}"  # 5 * 1.5 = 7.5 -> 7
    
    max_escalated = flare.apply_escalation(8)
    assert max_escalated == 10, f"Expected max priority 10, got {max_escalated}"  # Should cap at 10
    print("  ✓ Priority escalation test passed")
    
    # Test 4: Complete evaluation
    result = flare.evaluate_flare(88, base_priority=5)
    assert result["flare_triggered"] == True, "Flare should be triggered"
    assert result["alert_required"] == True, "Alert should be required"
    assert result["escalated_priority"] == 7, f"Expected priority 7, got {result['escalated_priority']}"
    print("  ✓ Complete evaluation test passed")


def test_configuration():
    """Test custom configuration loading."""
    print("\nTesting Configuration...")
    
    # Test with default config
    model = ScoringModel()
    assert "critical" in model.thresholds, "Critical threshold should exist"
    assert model.thresholds["critical"] == 90, "Default critical threshold should be 90"
    print("  ✓ Default configuration test passed")
    
    # Test with custom config file
    config_path = Path(__file__).parent.parent / "config.json"
    if config_path.exists():
        model = ScoringModel(str(config_path))
        flare = FlareMode(str(config_path))
        assert flare.is_enabled(), "Flare mode should be enabled in config"
        print("  ✓ Custom configuration test passed")


def main():
    """Run all tests."""
    print("=" * 70)
    print("Running Food Inspector Tests")
    print("=" * 70)
    print()
    
    try:
        test_scoring_model()
        test_flare_mode()
        test_configuration()
        
        print()
        print("=" * 70)
        print("All tests passed! ✓")
        print("=" * 70)
        return 0
        
    except AssertionError as e:
        print()
        print("=" * 70)
        print(f"Test failed: {e}")
        print("=" * 70)
        return 1
    except Exception as e:
        print()
        print("=" * 70)
        print(f"Error running tests: {e}")
        print("=" * 70)
        return 1


if __name__ == "__main__":
    sys.exit(main())
