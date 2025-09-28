using Microsoft.UI;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.UI;

namespace LifeTimer.Logic;

public class SettingsViewModel : INotifyPropertyChanged
{
    private List<string> _urlList = new() { "https://lifetimer.app/docs" };
    private bool _rotateLinks = false;
    private int _linkRotationDelaySecs = 30;
    private Color _windowColor = Colors.Black;
    private int _windowOpacity = 128;
    private bool _interactiveStartup = true;
    private bool _showSettingsOnStartup = true;
    private bool _allowBackgroundInput = false;
    private int _windowPosX = 100;
    private int _windowPosY = 100;
    private int _windowWidth = -1; //we want this calculated for current dpi on first run
    private int _windowHeight = -1; // ditto
    private bool _windowMaximized = false;

    private string _currentLink = string.Empty;
    private int _currentRotationIndex = 0;


    public List<string> UrlList
    {
        get => _urlList;
        set => SetProperty(ref _urlList, value);
    }

    public bool RotateLinks
    {
        get => _rotateLinks;
        set => SetProperty(ref _rotateLinks, value);
    }

    public int LinkRotationDelaySecs
    {
        get => _linkRotationDelaySecs;
        set => SetProperty(ref _linkRotationDelaySecs, value);
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


    public bool AllowBackgroundInput
    {
        get => _allowBackgroundInput;
        set => SetProperty(ref _allowBackgroundInput, value);
    }

    /*
    public bool PreProcessMediaLinks
    {
        get => _preProcessMediaLinks;
        set => SetProperty(ref _preProcessMediaLinks, value);
    }

    public int MediaWidth
    {
        get => _mediaWidth;
        set => SetProperty(ref _mediaWidth, value);
    }

    public int MediaHeight
    {
        get => _mediaHeight;
        set => SetProperty(ref _mediaHeight, value);
    }
    */

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

    public string CurrentUrl
    {
        get => _currentLink;
        set => SetProperty(ref _currentLink, value);
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
}

public enum VizWindowOperationMode
{
    Interactive = 1,
    Background = 2,
    ScreenSaver = 3
}