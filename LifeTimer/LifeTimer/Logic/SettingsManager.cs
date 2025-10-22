using LifeTimer.Logic.Models;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading;
using Windows.Storage;
using Newtonsoft.Json;
using System;

namespace LifeTimer.Logic;

public class SettingsManager
{

    private static bool ForceNewSettings = false;

    private static int SaveIntervalMS = 250;
    private static int DebounceIntervalMS = 500;
    private Timer _saveTimer;
    private Timer _debounceTimer;
    private bool _isSaveRequired = false;
    private bool _isSaving = false;


    private readonly ILogger<SettingsManager> _logger;
    private readonly ApplicationDataContainer _localSettings = ApplicationData.Current.LocalSettings;
    private ApplicationController _applicationController;

    public SettingsManager(ILogger<SettingsManager> logger)
    {
        _logger = logger;
        _logger.LogInformation("SettingsManager initialized");
    }

    private const string SETTINGS_KEY = "LifeTimerSettings";
    private const string RELEASE_NOTES_KEY = "ReleaseNoteStoredVersion";


    public void InitializeAutoSave(ApplicationController appController)
    {
        _applicationController = appController;
        _saveTimer = new Timer(CheckSaveRequired, null, TimeSpan.FromSeconds(5), TimeSpan.FromMilliseconds(SaveIntervalMS));
        _logger.LogInformation("SettingsManager initialized");
    }


    public void RequestSaveAllWithDebounce()
    {
        // Cancel any existing timer
        _debounceTimer?.Dispose();

        // Start a new timer
        _debounceTimer = new Timer(OnDebounceTimerElapsed, null, DebounceIntervalMS, Timeout.Infinite);

    }

    private void OnDebounceTimerElapsed(object? state)
    {
        ScheduleSave();
    }


    public void Shutdown()
    {
        _saveTimer?.Dispose();
        _debounceTimer?.Dispose();
    }


    private void ScheduleSave()
    {
        if (_applicationController == null)
            throw new InvalidOperationException("Settings manager not initialized");

        _isSaveRequired = true;
    }



    private void CheckSaveRequired(object? state)
    {
        if (_isSaving)
            return;

        if (!_isSaveRequired)
            return;

        SaveSettings();
    }





    private void SaveSettings()
    {

        try
        {
            if (_isSaving)
                return;

            if (!_isSaveRequired)
                return;

            _isSaving = true;

            _applicationController.RequestSaveStatusChanged("Saving");

            _logger.LogInformation("SaveSettings() - performing save operation");

            var settingsModel = _applicationController.CurrentSettings;

            var serialized = JsonConvert.SerializeObject(settingsModel);
            _localSettings.Values[SETTINGS_KEY] = serialized;
            _logger.LogInformation("SaveSettings() - preferences saved");


            _applicationController.RequestSaveStatusChanged("Settings Saved " + DateTime.Now.ToLocalTime());
            //_applicationController.RequestNotifySettingsSaved();
        }
        catch (Exception ex)
        {
            _logger.LogError("SaveSettings() - error saving settings " + ex.Message);
        }
        finally
        {
            _isSaving = false;
            _isSaveRequired = false;
        }
    }


    public SettingsViewModel LoadSettings()
    {
          var settingsString = _localSettings.Values[SETTINGS_KEY]?.ToString() ?? string.Empty;

        //var settingsString = String.Empty;

        if (!string.IsNullOrEmpty(settingsString))
        {
            try
            {
                var model = JsonConvert.DeserializeObject<SettingsViewModel>(settingsString);
                if (model != null)
                {
                    _logger.LogInformation("LoadSettings() settings loaded");
                    return model;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("LoadSettings() Error deserializing saved settings "+ex.Message);
            }
        }

        _logger.LogWarning("Could not load saved settings - returning defaults");
        var defaultSettings = SettingsViewModel.CreateDefaultSettings();
        return defaultSettings;

    }



    public int? GetReleaseNotesStoredVersion()
    {
        var value = _localSettings.Values[RELEASE_NOTES_KEY];

        if (value == null)
            return null;

        else
            return Convert.ToInt32(value);
    }


    public void SetReleaseNotesStoredVersion(int? value)
    {
        _localSettings.Values[RELEASE_NOTES_KEY] = value;
    }


    /*
    private const string URL_LIST_KEY = "UrlList";
    private const string ROTATE_LINKS_KEY = "RotateLinks";
    private const string LINK_ROTATION_DELAY_KEY = "LinkRotationDelaySecs";
    private const string INTERACTIVE_STARTUP_KEY = "InteractiveStartup";
    private const string SETTINGS_STARTUP_KEY = "SettingsStartup";
    private const string ALLOW_BACKGROUND_INPUT_KEY = "AllowBackgroundInput";
    private const string WINDOW_HEIGHT_KEY = "WindowHeight";
    private const string WINDOW_WIDTH_KEY = "WindowWidth";
    private const string WINDOW_POS_X_KEY = "WindowPosX";
    private const string WINDOW_POS_Y_KEY = "WindowPosY";
    private const string WINDOW_OPACITY_KEY = "WindowOpacity";
    private const string WINDOW_MAXIMIZED_KEY = "WindowMaximized";

    private const string CURRENT_LINK_KEY = "CurrentLink";
    private const string CURRENT_ROTATION_INDEX = "CurrentRotationIndex";


    public void SaveSettings(SettingsViewModel settings)
    {
        _localSettings.Values[URL_LIST_KEY] = string.Join(";", settings.UrlList);
        _localSettings.Values[ROTATE_LINKS_KEY] = settings.RotateLinks;
        _localSettings.Values[LINK_ROTATION_DELAY_KEY] = settings.LinkRotationDelaySecs;
        _localSettings.Values[INTERACTIVE_STARTUP_KEY] = settings.InteractiveStartup;
        _localSettings.Values[SETTINGS_STARTUP_KEY] = settings.ShowSettingsOnStartup;
        _localSettings.Values[ALLOW_BACKGROUND_INPUT_KEY] = settings.AllowBackgroundInput;
        _localSettings.Values[WINDOW_HEIGHT_KEY] = settings.WindowHeight;
        _localSettings.Values[WINDOW_WIDTH_KEY] = settings.WindowWidth;
        _localSettings.Values[WINDOW_POS_X_KEY] = settings.WindowPosX;
        _localSettings.Values[WINDOW_POS_Y_KEY] = settings.WindowPosY;
        _localSettings.Values[WINDOW_OPACITY_KEY] = settings.WindowOpacity;
        _localSettings.Values[WINDOW_MAXIMIZED_KEY] = settings.WindowMaximized;
        _localSettings.Values[CURRENT_LINK_KEY] = settings.CurrentUrl;
        _localSettings.Values[CURRENT_ROTATION_INDEX] = settings.CurrentRotationIndex;


    }

    public SettingsViewModel LoadSettings()
    {
        var settings = new SettingsViewModel();

        var urlListString = GetStringSetting(URL_LIST_KEY, "");
        if (!string.IsNullOrEmpty(urlListString))
        {
            settings.UrlList = new List<string>(urlListString.Split(';'));
        }

        settings.CurrentUrl = GetStringSetting(CURRENT_LINK_KEY, "");
        settings.CurrentRotationIndex = GetIntSetting(CURRENT_ROTATION_INDEX, 0);
        settings.RotateLinks = GetBoolSetting(ROTATE_LINKS_KEY, false);
        settings.LinkRotationDelaySecs = GetIntSetting(LINK_ROTATION_DELAY_KEY, 30);
        settings.InteractiveStartup = GetBoolSetting(INTERACTIVE_STARTUP_KEY, true);
        settings.ShowSettingsOnStartup = GetBoolSetting(SETTINGS_STARTUP_KEY, true);
        settings.AllowBackgroundInput = GetBoolSetting(ALLOW_BACKGROUND_INPUT_KEY, false);
        settings.WindowHeight = GetIntSetting(WINDOW_HEIGHT_KEY, -1); //default to -1 to enable first run intercept
        settings.WindowWidth = GetIntSetting(WINDOW_WIDTH_KEY, -1);
        settings.WindowPosX = GetIntSetting(WINDOW_POS_X_KEY, 100);
        settings.WindowPosY = GetIntSetting(WINDOW_POS_Y_KEY, 100);
        settings.WindowOpacity = GetIntSetting(WINDOW_OPACITY_KEY, 255);
        settings.WindowMaximized = GetBoolSetting(WINDOW_MAXIMIZED_KEY, false);

        return settings;
    }

    private string GetStringSetting(string key, string defaultValue = "")
    {
        return _localSettings.Values[key]?.ToString() ?? defaultValue;
    }

    private bool GetBoolSetting(string key, bool defaultValue = false)
    {
        return _localSettings.Values[key] as bool? ?? defaultValue;
    }

    private int GetIntSetting(string key, int defaultValue = 0)
    {
        return _localSettings.Values[key] as int? ?? defaultValue;
    }

    */

}