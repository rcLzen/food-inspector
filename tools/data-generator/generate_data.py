#!/usr/bin/env python3
"""
Main script for generating versioned JSON data files.

Usage:
    python generate_data.py --version 1 --output ../output
    python generate_data.py --type synonyms --version 1
"""

import argparse
import json
import sys
from pathlib import Path

# Add parent directory to path for imports
sys.path.insert(0, str(Path(__file__).parent))

from generators.synonyms import generate_synonyms_json
from generators.cross_reactivity import generate_cross_reactivity_json
from generators.scoring_policy import generate_scoring_policy_json


def main():
    parser = argparse.ArgumentParser(
        description='Generate versioned JSON data files for Food Inspector MAUI app'
    )
    parser.add_argument(
        '--type',
        choices=['all', 'synonyms', 'cross-reactivity', 'scoring-policy'],
        default='all',
        help='Type of data to generate (default: all)'
    )
    parser.add_argument(
        '--version',
        type=int,
        default=1,
        help='Major version number (default: 1)'
    )
    parser.add_argument(
        '--output',
        type=str,
        default='../output',
        help='Output directory for generated files (default: ../output)'
    )
    parser.add_argument(
        '--pretty',
        action='store_true',
        help='Pretty-print JSON with indentation'
    )
    
    args = parser.parse_args()
    
    # Create output directory if it doesn't exist
    output_dir = Path(args.output)
    output_dir.mkdir(parents=True, exist_ok=True)
    
    # Determine semantic version string
    version_str = f"{args.version}.0.0"
    
    # Generate requested data
    generators = {
        'synonyms': (generate_synonyms_json, f'synonyms.v{args.version}.json'),
        'cross-reactivity': (generate_cross_reactivity_json, f'cross-reactivity.v{args.version}.json'),
        'scoring-policy': (generate_scoring_policy_json, f'scoring-policy.v{args.version}.json'),
    }
    
    if args.type == 'all':
        types_to_generate = generators.keys()
    else:
        types_to_generate = [args.type]
    
    print(f"Generating data files (version {version_str})...")
    print(f"Output directory: {output_dir.absolute()}")
    print()
    
    for data_type in types_to_generate:
        generator_func, filename = generators[data_type]
        
        print(f"Generating {data_type}...", end=' ')
        
        try:
            # Generate data
            data = generator_func(version=version_str)
            
            # Write to file
            output_path = output_dir / filename
            with open(output_path, 'w') as f:
                if args.pretty:
                    json.dump(data, f, indent=2)
                    f.write('\n')  # Add trailing newline
                else:
                    json.dump(data, f)
            
            print(f"✓ {output_path}")
            
        except Exception as e:
            print(f"✗ Error: {e}")
            sys.exit(1)
    
    print()
    print("Generation complete!")
    print()
    print("Next steps:")
    print("1. Review generated files in", output_dir.absolute())
    print("2. Copy files to MAUI project's Data/reference-data/ directory")
    print("3. Update MAUI code to load and consume the JSON data")


if __name__ == '__main__':
    main()
