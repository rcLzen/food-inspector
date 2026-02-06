# Food Inspector

A comprehensive food safety system consisting of:
1. **MAUI Mobile App** - Consumer-facing mobile application for scanning food products and detecting allergens
2. **Data Generation Tools** - Python-based tools for generating versioned reference data

## Components

### 1. MAUI Mobile App (Consumer Allergen Scanner)

A .NET 8 MAUI Blazor Hybrid mobile application that helps users identify potential food allergens and triggers.

**Features:**
- Barcode scanning and OCR ingredient extraction
- Allergen detection with synonym matching
- Cross-reactivity warnings
- Flare Mode for heightened sensitivity
- Encrypted local database
- Offline-first with online enrichment

**Technology:** .NET 8, MAUI, Blazor, SQLite

### 2. Data Generation Tools (Build-Time Only)

Python tools that generate versioned JSON reference data consumed by the MAUI app. **No runtime Python dependency.**

**Generated Files:**
- `synonyms.v1.json` - Ingredient synonym mappings
- `cross-reactivity.v1.json` - Cross-reactivity relationships
- `scoring-policy.v1.json` - Scoring thresholds and flare-mode config

## Usage

### Generate Data Files

```bash
cd tools/data-generator
python generate_data.py --version 1 --output ../output --pretty
```

See [tools/README.md](tools/README.md) for detailed documentation.

## Why Separate Tools?

- ✅ No runtime Python dependency in mobile app
- ✅ Version-controlled reference data
- ✅ Clear separation between build-time and runtime
- ✅ Easy to update and regenerate data
- ✅ Simple deployment (just JSON files)

## Integration Workflow

1. Update data in Python generators
2. Run generation script to create JSON files
3. Review generated files
4. Copy JSON files to MAUI project's data directory
5. Commit both generator changes and JSON files
6. MAUI app loads JSON files at startup

## Versioning

- **Data files:** Major version in filename (e.g., `synonyms.v1.json`)
- **MAUI app:** Checks compatibility with data file versions

## License

MIT License
# Food Inspector - Food Safety Scanner v1

A .NET 8 MAUI Blazor Hybrid mobile application that helps users identify potential food allergens and triggers in their food products.

## Features

### Core Functionality
- **Barcode Scanning**: Scan product barcodes to automatically retrieve product information
- **OCR Ingredient Scanning**: Take photos of ingredient labels and extract text using on-device OCR
- **Offline Mode**: Fully functional ingredient analysis without internet connection
- **Online Enrichment**: Fetch detailed product information from Open Food Facts API when online
- **Smart Analysis**: Match ingredients against a comprehensive database of triggers, synonyms, and cross-reactivities
- **Safety Ratings**: Get clear SAFE/CAUTION/AVOID ratings with detailed explanations

### Flare Mode
- **Heightened Sensitivity**: Tighten detection thresholds for periods of increased sensitivity
- **Customizable Threshold**: Adjust severity threshold (1-10) to match your needs
- **Visual Indicators**: Clear marking of flare mode scans in history

### Data Management
- **Scan History**: Keep track of all scanned products
- **Secure Storage**: Encrypted SQLite database using SQLCipher with keys stored in platform SecureStorage
- **Export Options**: Export scan history to CSV or JSON format
- **No Login Required**: Privacy-focused design with no account requirements

### Ingredient Database
The app includes a pre-seeded database with:
- **15+ Common Allergens**: Peanuts, tree nuts, milk, eggs, soy, wheat, fish, shellfish, sesame, and more
- **50+ Synonyms**: Alternative names and common ingredient variations
- **Cross-Reactivity Mapping**: Alerts for potential cross-reactions between allergens
- **Severity Scores**: Each trigger rated 1-10 for Flare Mode sensitivity

## Technical Stack

### Framework & Technologies
- **.NET 8**: Latest .NET framework
- **MAUI Blazor Hybrid**: Cross-platform UI with web technologies
- **Entity Framework Core**: Database ORM
- **SQLite with SQLCipher**: Encrypted local database
- **ZXing.Net.Maui**: Barcode scanning
- **Tesseract**: OCR for ingredient text extraction
- **CsvHelper**: CSV export functionality
- **Open Food Facts API**: Product information enrichment

### Architecture
- **Service-based architecture**: Clean separation of concerns
- **Dependency Injection**: Built-in .NET DI container
- **Repository pattern**: DatabaseService abstracts data access
- **Blazor Components**: Reactive UI with component-based design

## Project Structure

```
FoodInspector/
├── Components/
│   ├── Layout/
│   │   └── MainLayout.razor      # Main app layout with navigation
│   ├── Pages/
│   │   ├── Home.razor            # Scanning interface
│   │   ├── History.razor         # Scan history view
│   │   └── Settings.razor        # App settings and info
│   └── Routes.razor              # Routing configuration
├── Data/
│   └── FoodInspectorDbContext.cs # EF Core database context with seed data
├── Models/
│   └── Models.cs                 # Data models (triggers, history, settings)
├── Services/
│   ├── SecureStorageService.cs   # Encryption key management
│   ├── DatabaseService.cs        # Database operations
│   ├── ScanningServices.cs       # Barcode and OCR services
│   ├── IngredientAnalysisService.cs # Ingredient matching and analysis
│   └── ExportService.cs          # CSV/JSON export functionality
├── Resources/                     # App resources (icons, fonts, styles)
├── wwwroot/                       # Web assets (CSS, HTML)
├── MauiProgram.cs                # App configuration and DI setup
└── App.xaml / MainPage.xaml      # MAUI entry points

```

## Setup & Installation

### Prerequisites
- .NET 8 SDK or later
- Visual Studio 2022 or Visual Studio Code with MAUI workload
- Windows, macOS, or Linux for development
- Target platforms: Android, iOS, macOS, Windows

### Installation Steps

1. **Clone the repository**
   ```bash
   git clone https://github.com/rcLzen/food-inspector.git
   cd food-inspector
   ```

2. **Install MAUI workload** (if not already installed)
   ```bash
   dotnet workload install maui
   ```

3. **Restore NuGet packages**
   ```bash
   dotnet restore
   ```

4. **Build the project**
   ```bash
   dotnet build
   ```

5. **Run on your target platform**
   ```bash
   # Android
   dotnet build -t:Run -f net8.0-android
   
   # iOS (macOS only)
   dotnet build -t:Run -f net8.0-ios
   
   # Windows
   dotnet build -t:Run -f net8.0-windows10.0.19041.0
   
   # macOS
   dotnet build -t:Run -f net8.0-maccatalyst
   ```

## Usage Guide

### Scanning Products

1. **Manual Entry**: Type or paste ingredients directly into the text box
2. **Barcode Scan**: Tap "Scan Barcode" to scan product barcode and fetch info from Open Food Facts
3. **Ingredient Photo**: Tap "Scan Ingredients (OCR)" to photograph and extract ingredient text
4. **Analyze**: Tap "Analyze Ingredients" to get safety assessment

### Understanding Results

- **✅ SAFE**: No known triggers detected
- **⚠️ CAUTION**: Minor triggers or sensitivities found
- **🚫 AVOID**: Major allergens or high-severity triggers detected

Each result includes:
- List of detected triggers
- Detailed warnings with explanations
- Cross-reactivity alerts when applicable

### Using Flare Mode

1. Go to Settings
2. Toggle "Flare Mode" on
3. Adjust severity threshold (1-10)
   - Lower values = more lenient (only high severity triggers)
   - Higher values = more strict (flags more potential issues)
4. Return to scanning - Flare Mode indicator will appear

### Managing History

- View all past scans in the History tab
- Each entry shows date, product, safety level, and ingredients
- Flare Mode scans are marked with 🔥 badge
- Delete individual entries
- Export all history to CSV or JSON

## Database Schema

### Tables

**FoodScanHistory**: User scan records
- Id, ScanDate, Barcode, ProductName, Ingredients
- SafetyLevel, Analysis, IsFlareMode, ImagePath, OpenFoodFactsData

**IngredientTrigger**: Known allergens and triggers
- Id, Name, SafetyLevel, Description, SeverityScore, IsCommonAllergen

**IngredientSynonym**: Alternative names for triggers
- Id, IngredientTriggerId, Synonym

**CrossReactivity**: Cross-reaction relationships
- Id, PrimaryTriggerId, RelatedTriggerId, Description

**AppSettings**: User preferences
- Id, IsFlareMode, FlareModeThreshold

## Security & Privacy

- **No Login**: No personal data collection or accounts required
- **Local Storage**: All data stored locally on device
- **Encrypted Database**: SQLite database encrypted with SQLCipher
- **Secure Keys**: Encryption keys managed by platform SecureStorage
- **Offline First**: Core functionality works without internet

## Customization

### Adding New Triggers

Edit `Data/FoodInspectorDbContext.cs` in the `SeedData()` method:

```csharp
new IngredientTrigger 
{ 
    Id = 16, 
    Name = "Coconut", 
    SafetyLevel = SafetyLevel.Caution, 
    Description = "Tree nut family",
    SeverityScore = 7,
    IsCommonAllergen = true 
}
```

### Adding Synonyms

```csharp
new IngredientSynonym 
{ 
    Id = 57, 
    IngredientTriggerId = 16, 
    Synonym = "coconut oil" 
}
```

### Modifying Safety Thresholds

Adjust severity scores (1-10) for each trigger to customize Flare Mode behavior.

## API Integration

### Open Food Facts

The app integrates with the Open Food Facts API (https://world.openfoodfacts.org/) to fetch product information by barcode.

**Endpoint**: `GET /api/v0/product/{barcode}.json`

**Features**:
- Product name and brand
- Ingredient list
- Product images
- Nutritional information
- Automatically falls back to offline mode if unavailable

## Troubleshooting

### Database Issues
- Delete app data/cache to reset database
- Database will be recreated with seed data on next launch

### Barcode Scanner Not Working
- Ensure camera permissions are granted
- Check device has functional camera
- Try manual barcode entry

### OCR Not Detecting Text
- Ensure good lighting
- Hold camera steady
- Try manual ingredient entry
- Make sure ingredient text is clearly visible

## Future Enhancements

Potential features for future versions:
- Custom user trigger lists
- Sharing scan results
- Nutrition analysis
- Recipe scanner
- Multiple user profiles
- Cloud backup
- Barcode history
- Food diary integration

## Contributing

Contributions welcome! Please:
1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Submit a pull request

## License

This project is open source and available under the MIT License.

## Credits

- **Open Food Facts**: Product database
- **ZXing**: Barcode scanning library
- **Tesseract**: OCR engine
- **SQLCipher**: Database encryption
- **CommunityToolkit.Maui**: MAUI extensions

## Support

For issues, questions, or feature requests, please open an issue on GitHub.

---

**Version**: 1.0.0  
**Last Updated**: 2026-02-06
