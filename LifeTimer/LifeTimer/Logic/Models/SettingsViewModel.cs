using Microsoft.UI;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.UI;

namespace LifeTimer.Logic.Models;

public class SettingsViewModel : INotifyPropertyChanged
{

    private List<TimerDefinition> _timers = new List<TimerDefinition>() { };
    private AppearanceViewModel _appearance = new AppearanceViewModel();


    private bool _rotateTimers = false;
    private int _timerRotationDelaySecs = 30;
    private Color _windowColor = Colors.Black;
    private int _windowOpacity = 128;
    private bool _interactiveStartup = true;
    private bool _showSettingsOnStartup = true;
    private int _windowPosX = 100;
    private int _windowPosY = 100;
    private int _windowWidth = -1; //we want this calculated for current dpi on first run
    private int _windowHeight = -1; // ditto
    private bool _windowMaximized = false;


    private string _currentTimerName= string.Empty;
    private int _currentRotationIndex = 0;



    public List<TimerDefinition> Timers
    {
        get => _timers;
        set => SetProperty(ref _timers, value);
    }

    public AppearanceViewModel Appearance
    {
        get => _appearance;
        set => SetProperty(ref _appearance, value);
    }

    public bool RotateTimers
    {
        get => _rotateTimers;
        set => SetProperty(ref _rotateTimers, value);
    }

    public int TimerRotationDelaySecs
    {
        get => _timerRotationDelaySecs;
        set => SetProperty(ref _timerRotationDelaySecs, value);
    }

    public int WindowOpacity
    {
        get => _windowOpacity;
        set => SetProperty(ref _windowOpacity, value);
    }

    /*
    public Color WindowColor
    {
        get => _windowColor;
        set => SetProperty(ref _windowColor, value);
    }
    */

    public bool InteractiveStartup
    {
        get => _interactiveStartup;
        set => SetProperty(ref _interactiveStartup, value);
    }

    public bool ShowSettingsOnStartup
    {
        get => _showSettingsOnStartup;
        set => SetProperty(ref _showSettingsOnStartup, value);
    }

    public int WindowPosX
    {
        get => _windowPosX;
        set => SetProperty(ref _windowPosX, value);
    }

    public int WindowPosY
    {
        get => _windowPosY;
        set => SetProperty(ref _windowPosY, value);
    }

    public int WindowWidth
    {
        get => _windowWidth;
        set => SetProperty(ref _windowWidth, value);
    }

    public int WindowHeight
    {
        get => _windowHeight;
        set => SetProperty(ref _windowHeight, value);
    }

    public bool WindowMaximized
    {
        get => _windowMaximized;
        set => SetProperty(ref _windowMaximized, value);
    }

    public string CurrentTimerName
    {
        get => _currentTimerName;
        set => SetProperty(ref _currentTimerName, value);
    }

    public int CurrentRotationIndex
    {
        get => _currentRotationIndex;
        set => SetProperty(ref _currentRotationIndex, value);
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected bool SetProperty<T>(ref T backingStore, T value, [CallerMemberName] string propertyName = "")
    {
        if (EqualityComparer<T>.Default.Equals(backingStore, value))
            return false;

        backingStore = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        return true;
    }

    protected static TimerDefinition CreateDefaultTimer()
    {
        return new TimerDefinition()
        {
            Title = "Unix Epoch",
            TargetDateTime = new System.DateTime(1970,1,1,0,0,0,0,0,System.DateTimeKind.Utc)
        };
    }

    public static SettingsViewModel CreateDefaultSettings()
    {
        var model = new SettingsViewModel();
        model.Timers.Add(CreateDefaultTimer());
        model.Appearance = AppearanceViewModel.CreateDefaultAppearance();
        return model;
    }
}

