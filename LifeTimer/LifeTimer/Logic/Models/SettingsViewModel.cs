using Microsoft.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.UI;

namespace LifeTimer.Logic.Models;


public enum OperatingMode
{
    Timer =0,
    Pomodoro=1
}


public class SettingsViewModel : INotifyPropertyChanged
{
    public static int InitialWindowWidth = 350;
    public static int InitialWindowHeight = 100;

    private List<TimerDefinition> _timers = new List<TimerDefinition>() { };
    private AppearanceViewModel _appearance = new AppearanceViewModel();
    private PomodoroViewModel _pomodoroModel = new PomodoroViewModel();

    private bool _operationHints = true;
    private bool _rotateTimers = false;
    private int _timerRotationDelaySecs = 30;
    private Color _windowColor = Colors.Black;
    private int _windowOpacity = 128;
    private bool _showSettingsOnStartup = false;
    private int _windowPosX = 100;
    private int _windowPosY = 100;
    private int _windowWidth = -1; //we want this calculated for current dpi on first run
    private int _windowHeight = -1; // ditto
    private bool _windowMaximized = false;

    private string? _currentTimerId= null;
    private int? _currentRotationIndex = null;

    private OperatingMode _operatingMode = OperatingMode.Timer;

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

    public PomodoroViewModel Pomodoro
    {
        get => _pomodoroModel;
        set => SetProperty(ref _pomodoroModel, value);
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

    public bool ShowOperationHints
    {
        get => _operationHints;
        set => SetProperty(ref _operationHints, value);
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

    public string? CurrentTimerId
    {
        get => _currentTimerId;
        set => SetProperty(ref _currentTimerId, value);
    }


    public int? CurrentRotationIndex
    {
        get => _currentRotationIndex;
        set => SetProperty(ref _currentRotationIndex, value);
    }

    public OperatingMode OperatingMode
    {
        get => _operatingMode;
        set => SetProperty(ref _operatingMode, value);
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

    protected static TimerDefinition CreateDefaultCurrentTimer()
    {
        return new TimerDefinition()
        {
            Id = Guid.NewGuid(),
            Title = "Current Time",
            IsCurrentTime = true,
            DisplayHours = true,
            DisplayMinutes = true,
            DisplaySeconds = true,
         };
    }

    protected static TimerDefinition CreateDefaultTargetTimer()
    {
        return new TimerDefinition()
        {
            Id = Guid.NewGuid(),
            Title = "Time Since Unix Epoch",
            TargetDateTime = new DateTime(1970,1,1,0,0,0,0),
            IsCurrentTime = false,
            DisplayHours = true,
            DisplayMinutes = true,
            DisplaySeconds = true,
        };
    }


    public static SettingsViewModel CreateDefaultSettings()
    {
        var model = new SettingsViewModel();
        model.Timers.Add(CreateDefaultCurrentTimer());
        model.Appearance = AppearanceViewModel.CreateDefaultAppearance();
        model.CurrentTimerId = model.Timers[0].Id.ToString();
        return model;
    }
}

