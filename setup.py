from setuptools import setup, find_packages

setup(
    name="food-inspector",
    version="0.1.0",
    description="Food ingredient analysis with synonym matching and cross-reactivity detection",
    author="Food Inspector Team",
    packages=find_packages(where="src"),
    package_dir={"": "src"},
    python_requires=">=3.7",
    install_requires=[
        "pyyaml>=5.4.0",
    ],
    extras_require={
        "dev": [
            "pytest>=6.2.0",
            "pytest-cov>=2.12.0",
        ],
    },
    package_data={
        "": ["data/*.yaml"],
    },
)
