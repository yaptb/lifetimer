# LifeTimer - WinUI3 Application

## Project Overview
LifeTimer is a Windows Desktop application that displays rotating countdown timers on the user's desktop. It's designed to run as a background overlay showing multiple timers in sequence, with both a main display window and a settings window for configuration.

## Technology Stack
- **Platform**: WinUI3 (.NET 8.0 targeting Windows 10.0.19041.0+)
- **Target OS**: Windows 11 Desktop
- **Deployment**: Microsoft Store (MSIX packaging)
- **Architecture**: MVVM pattern with centralized state management

## Project Structure

### Core Components
- **MainWindow.xaml/cs**: Primary timer display window with system tray integration
- **SettingsWindow.xaml/cs**: Configuration interface with tabbed layout
- **ApplicationController.cs**: Central state manager and coordinator for all application logic

### User Controls Architecture
The UI is decomposed into reusable UserControl components:

#### Layout Controls
- `SidenavTabLayout.xaml` - Main navigation structure
- `SidenavItemControl.xaml` - Individual navigation items
- `SettingsPageLayout.xaml` - Settings page container
- `SettingsGroupControl.xaml` - Settings section grouping

#### Settings Controls
- `SettingsToggleControl.xaml` - Toggle switches
- `SettingsSliderControl.xaml` - Slider controls
- `SettingsNumericFieldControl.xaml` - Numeric input fields
- `SettingsPositionControl.xaml` - Window positioning
- `SettingsStatusControl.xaml` - Status display
- `SettingsSwitchesControl.xaml` - Switch collections
- `SettingsURLListControl.xaml` - URL management
- `SettingsUrlRotationControl.xaml` - Timer rotation settings

#### Tab Controls
- `CommandTabUserControl.xaml` - Command interface
- `HelpTabUserControl.xaml` - Help content
- `SettingsTabUserControl.xaml` - Settings tab
- `WebTabUserControl.xaml` - Web content display
- `DummyUserControl.xaml` - Placeholder control

### Business Logic (`Logic/` folder)
- **ApplicationController.cs**: Central application coordinator and state manager
- **SettingsManager.cs**: Settings persistence and management
- **LinkRotator.cs**: Timer rotation logic
- **NagTimer.cs & FreemiumNagTimer.cs**: Freemium version prompts
- **WindowsStoreHelper.cs**: Microsoft Store integration
- **Logger.cs**: Application logging

### Models (`Logic/Models/` folder)
- **SettingsViewModel.cs**: Application settings data model with INotifyPropertyChanged
- **TimerViewModel.cs**: Timer definition model (TimerDefinition class)

### Dependencies
Key NuGet packages:
- `Microsoft.WindowsAppSDK` (1.7.x) - WinUI3 framework
- `H.NotifyIcon.WinUI` (2.3.0) - System tray functionality
- `Microsoft.Extensions.Logging` (9.0.x) - Logging framework
- `Microsoft.Extensions.DependencyInjection` (9.0.x) - DI container
- `Serilog` - Logging implementation
- `Newtonsoft.Json` (13.0.4) - JSON serialization

## Application Features

### Core Functionality
- **Timer Display**: Shows countdown timers with customizable titles and target dates
- **Timer Rotation**: Automatically cycles through multiple timer definitions
- **Desktop Overlay**: Runs as background window overlay on desktop
- **System Tray**: System tray integration with context menu
- **Settings Management**: Comprehensive settings UI with persistence

### Window Modes
- **Interactive Mode**: Full window interaction enabled
- **Background Mode**: Overlay mode with optional input suppression
- **Opacity Control**: Configurable window transparency

### Store Integration
- **Freemium Model**: Free version with limited features (3 timers max)
- **Pro Version**: Unlocked version (15 timers max)
- **In-App Purchase**: Upgrade capability through Windows Store

## Development Guidelines

### Code Standards
- Follow WinUI3 best practices and patterns
- Use MVVM architecture with proper data binding
- Implement INotifyPropertyChanged for data models
- Centralize state management through ApplicationController
- Use dependency injection for service management

### UI/UX Guidelines
- Maintain consistency with Windows 11 design system
- Use proper WinUI3 controls and styling
- Implement responsive layouts
- Support proper keyboard navigation and accessibility

### Store Compliance
- Follow Microsoft Store certification requirements
- Implement proper MSIX packaging
- Handle startup tasks and background execution properly
- Use appropriate app manifest capabilities (runFullTrust)

### Architecture Patterns
- **Central Controller**: All major application logic flows through ApplicationController
- **Event-Driven**: Components communicate via events and callbacks
- **UI Thread Marshalling**: Background operations properly marshal to UI thread
- **Service Locator**: Use App.Services for dependency resolution

## Key Constraints
- **WinUI3 Only**: Use only WinUI3-compatible APIs and controls
- **Windows 11 Target**: Optimize for Windows 11 desktop experience
- **Store Deployment**: Must comply with Microsoft Store policies
- **Desktop Focused**: Not a mobile or web application
- **Background Operation**: Must handle background/foreground modes properly

## Settings Model
The application uses `SettingsViewModel` with these key properties:
- `Timers`: List of TimerDefinition objects
- `RotateTimers`: Enable/disable timer rotation
- `TimerRotationDelaySecs`: Rotation interval
- `WindowOpacity`: Transparency level
- `InteractiveStartup`: Launch mode
- `WindowPosX/Y/Width/Height`: Window positioning
- `CurrentTimerName`: Active timer identifier

## Build Configuration
- Supports x86, x64, and ARM64 platforms
- MSIX packaging enabled (`EnableMsixTooling`)
- Nullable reference types enabled
- Targets Windows 10.0.19041.0+ with minimum 10.0.17763.0
- Build output directory: `E:\_dev\_github\lifetimer\Builds\`