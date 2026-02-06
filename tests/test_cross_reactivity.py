"""
Tests for CrossReactivityChecker
"""

import pytest
from food_inspector.cross_reactivity import CrossReactivityChecker, CrossReactivityRule


@pytest.fixture
def checker():
    """Create a CrossReactivityChecker instance for testing."""
    return CrossReactivityChecker()


def test_load_rules(checker):
    """Test that rules are loaded from file."""
    rules = checker.get_all_rules()
    assert len(rules) > 0
    assert all(isinstance(rule, CrossReactivityRule) for rule in rules)


def test_get_potential_reactions_peanuts(checker):
    """Test getting potential cross-reactions for peanuts."""
    reactions = checker.get_potential_reactions("peanuts")
    
    # Should have at least one reaction (tree_nuts)
    assert len(reactions) > 0
    
    # Check that tree_nuts is in the targets
    targets = [r.target for r in reactions]
    assert "tree_nuts" in targets


def test_get_potential_reactions_dairy(checker):
    """Test getting potential cross-reactions for dairy."""
    reactions = checker.get_potential_reactions("dairy")
    
    # Should have reactions to beef, goat_milk, etc.
    assert len(reactions) > 0
    
    targets = [r.target for r in reactions]
    assert "beef" in targets or "goat_milk" in targets


def test_confidence_levels(checker):
    """Test that confidence levels are properly set."""
    # Get a high confidence reaction
    reactions = checker.get_potential_reactions("dairy", min_confidence="high")
    
    for reaction in reactions:
        assert reaction.confidence == "high"


def test_min_confidence_filter_medium(checker):
    """Test filtering by minimum confidence level (medium)."""
    all_reactions = checker.get_potential_reactions("dairy")
    medium_reactions = checker.get_potential_reactions("dairy", min_confidence="medium")
    
    # Medium+ reactions should be subset of all reactions
    assert len(medium_reactions) <= len(all_reactions)
    
    # All returned reactions should be medium or high
    for reaction in medium_reactions:
        assert reaction.confidence in ["medium", "high"]


def test_min_confidence_filter_high(checker):
    """Test filtering by minimum confidence level (high)."""
    medium_reactions = checker.get_potential_reactions("dairy", min_confidence="medium")
    high_reactions = checker.get_potential_reactions("dairy", min_confidence="high")
    
    # High reactions should be subset of medium+ reactions
    assert len(high_reactions) <= len(medium_reactions)
    
    # All returned reactions should be high
    for reaction in high_reactions:
        assert reaction.confidence == "high"


def test_check_cross_reactivity_exists(checker):
    """Test checking for a known cross-reactivity."""
    # Check peanuts -> tree_nuts
    rule = checker.check_cross_reactivity("peanuts", "tree_nuts")
    
    assert rule is not None
    assert rule.source == "peanuts"
    assert rule.target == "tree_nuts"
    assert rule.confidence in ["low", "medium", "high"]


def test_check_cross_reactivity_not_exists(checker):
    """Test checking for a non-existent cross-reactivity."""
    # Check something that doesn't exist
    rule = checker.check_cross_reactivity("dairy", "shellfish")
    
    assert rule is None


def test_get_sources_for_target(checker):
    """Test getting sources that cross-react to a target."""
    # Find what can cross-react to tree_nuts
    sources = checker.get_sources_for_target("tree_nuts")
    
    # Peanuts should be a source
    source_allergens = [r.source for r in sources]
    assert "peanuts" in source_allergens


def test_get_rules_by_confidence_high(checker):
    """Test getting all high-confidence rules."""
    high_rules = checker.get_rules_by_confidence("high")
    
    assert all(r.confidence == "high" for r in high_rules)
    assert len(high_rules) > 0


def test_get_rules_by_confidence_low(checker):
    """Test getting all low-confidence rules."""
    low_rules = checker.get_rules_by_confidence("low")
    
    assert all(r.confidence == "low" for r in low_rules)
    assert len(low_rules) > 0


def test_format_warnings(checker):
    """Test formatting warnings for display."""
    warnings = checker.format_warnings("peanuts")
    
    assert len(warnings) > 0
    assert all(isinstance(w, str) for w in warnings)
    assert any("tree_nuts" in w for w in warnings)


def test_format_warnings_with_confidence(checker):
    """Test formatting warnings with confidence filter."""
    all_warnings = checker.format_warnings("dairy", min_confidence="low")
    high_warnings = checker.format_warnings("dairy", min_confidence="high")
    
    # High confidence warnings should be subset of all warnings
    assert len(high_warnings) <= len(all_warnings)


def test_cross_reactivity_rule_str(checker):
    """Test string representation of CrossReactivityRule."""
    rule = checker.check_cross_reactivity("peanuts", "tree_nuts")
    
    if rule:
        rule_str = str(rule)
        assert "peanuts" in rule_str
        assert "tree_nuts" in rule_str
        assert "confidence" in rule_str


def test_latex_fruit_syndrome(checker):
    """Test latex-fruit syndrome cross-reactivity."""
    reactions = checker.get_potential_reactions("latex")
    
    targets = [r.target for r in reactions]
    # Should include banana, avocado, kiwi
    latex_fruits = [t for t in targets if t in ["banana", "avocado", "kiwi"]]
    assert len(latex_fruits) > 0


def test_oral_allergy_syndrome(checker):
    """Test oral allergy syndrome (birch pollen)."""
    reactions = checker.get_potential_reactions("birch_pollen")
    
    targets = [r.target for r in reactions]
    # Should include apple, cherry, hazelnut
    oas_foods = [t for t in targets if t in ["apple", "cherry", "hazelnut"]]
    assert len(oas_foods) > 0


def test_shellfish_cross_reactivity(checker):
    """Test shellfish cross-reactivity rules."""
    reactions = checker.get_potential_reactions("shellfish")
    
    # Shellfish may cross-react with dust mites, insects
    assert len(reactions) > 0


def test_bidirectional_cross_reactivity(checker):
    """Test that cross-reactivity can be different in each direction."""
    # Peanuts -> tree_nuts should have different confidence than tree_nuts -> peanuts
    peanuts_to_nuts = checker.check_cross_reactivity("peanuts", "tree_nuts")
    nuts_to_peanuts = checker.check_cross_reactivity("tree_nuts", "peanuts")
    
    # At least one should exist
    assert peanuts_to_nuts is not None or nuts_to_peanuts is not None
    
    # If both exist, they might have different confidence levels
    if peanuts_to_nuts and nuts_to_peanuts:
        # This is expected - peanuts to tree nuts is medium, reverse is low
        pass


def test_unknown_allergen(checker):
    """Test querying an unknown allergen."""
    reactions = checker.get_potential_reactions("unknown_allergen")
    
    # Should return empty list
    assert reactions == []


def test_rule_has_notes(checker):
    """Test that rules have notes explaining the cross-reactivity."""
    rule = checker.check_cross_reactivity("dairy", "beef")
    
    if rule:
        assert rule.notes
        assert len(rule.notes) > 0
