using LifeTimer.Logic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace LifeTimer.FirstRun;


/// <summary>
/// display a call to action at the end of the onboard wizard for freemium apps
/// for free users - prompt to upgrade
/// for paid users - prompt to review
/// </summary>
public sealed partial class FirstRunCTAControl : UserControl
{


    private readonly ILogger<FirstRunCTAControl> _logger;
    private readonly ApplicationController _applicationController;
    private readonly WindowsStoreHelper _storeHelper;

    private string? _selectedProductId = null;

    public FirstRunCTAControl()
    {
        InitializeComponent();

        _logger = AppManager.Services.GetRequiredService<ILogger<FirstRunCTAControl>>();
        _applicationController = AppManager.Services.GetRequiredService<ApplicationController>();
        _storeHelper = AppManager.Services.GetRequiredService<WindowsStoreHelper>();
        _applicationController.NotifyVersionChange += _applicationController_NotifyVersionChange;

        this.SubOption.Checked += SubOption_Checked;
        this.LifeOption.Checked += LifeOption_Checked;
        this.Loaded += UpgradeControl_Loaded;


    }

    private void UpgradeControl_Loaded(object sender, RoutedEventArgs e)
    {
        UpdateControls();
    }

    private void _applicationController_NotifyVersionChange(object? sender, EventArgs e)
    {
        UpdateControls();
    }


    private void UpdateControls()
    {

        var isFreeVersion = _applicationController.CheckIsFreeVersion();

        //setup panels
        if (isFreeVersion)
        {
            this.ProVersionPanel.Visibility = Visibility.Collapsed;
            SetupUpgradePanel();
        }
        else
        {
            SetupProVersionPanel();
            this.UpgradePanel.Visibility = Visibility.Collapsed;
            this.ProVersionPanel.Visibility = Visibility.Visible;
        }

    }


    private void SetupUpgradePanel()
    {

        if (!(_storeHelper.IsProLifeAddOnAvailable || _storeHelper.IsProSubAddOnAvailable))
        {
            //no add on available - hide panel
            this.UpgradePanel.Visibility = Visibility.Collapsed;
            return;
        }

        //at least one upgrade option is available
        this.UpgradePanel.Visibility = Visibility.Visible;
        this.LifeOption.Visibility = Visibility.Collapsed;
        this.SubOption.Visibility = Visibility.Collapsed;


        if (_storeHelper.IsProLifeAddOnAvailable)
        {

            var proLifeOptionString = $"LifeTimer Pro Perpetual License";
            var proLifePrice = _storeHelper.ProLifetimeVersionFormattedPrice;

            if (!string.IsNullOrEmpty(proLifePrice))
            {
                proLifeOptionString += " (lifetime license) : " + proLifePrice;
            }

            this.LifeOption.Content = proLifeOptionString;
            this.LifeOption.Visibility = Visibility.Visible;
            this.LifeOption.IsChecked = true;
            this._selectedProductId = WindowsStoreHelper.PRO_LIFE_VERSION_PRODUCT_ID;
        }

        if (_storeHelper.IsProSubAddOnAvailable)
        {

            var proSubOptionString = $"LifeTimer Pro Subscription";

            var proSubPrice = _storeHelper.ProSubVersionFormattedPrice;
            var proSubPeriod = _storeHelper.ProSubRenewalPeriod;
            var proSubUnits = _storeHelper.ProSubRenewalUnits;

            if (!(String.IsNullOrEmpty(proSubPrice) || String.IsNullOrEmpty(proSubPeriod) || String.IsNullOrEmpty(proSubUnits)))
            {
                proSubOptionString += $" (renews every {proSubPeriod} {proSubUnits}) : {proSubPrice}";
            }

            this.SubOption.Content = proSubOptionString;
            this.SubOption.Visibility = Visibility.Visible;
            this.LifeOption.IsChecked = false;
            this.SubOption.IsChecked = true;
            this._selectedProductId = WindowsStoreHelper.PRO_SUB_VERSION_PRODUCT_ID;
        }

    }

    private void LifeOption_Checked(object sender, RoutedEventArgs e)
    {
        _logger.LogInformation("Lifetime Checked");
        this._selectedProductId = WindowsStoreHelper.PRO_LIFE_VERSION_PRODUCT_ID;
        this.UpgradeButton.IsEnabled = true;
    }

    private void SubOption_Checked(object sender, RoutedEventArgs e)
    {
        _logger.LogInformation("Subscription Checked");
        this._selectedProductId = WindowsStoreHelper.PRO_SUB_VERSION_PRODUCT_ID;
        this.UpgradeButton.IsEnabled = true;
    }


    private void SetupProVersionPanel()
    {
        var versionString = "LifeTimer Pro ";

        if (_storeHelper.IsProLifetimeVersion)
            versionString += "Perpetual License";

        if (_storeHelper.IsProSubVersion)
            versionString += "Subscription License";

        ProVersionTitle.Text = versionString;
    }


    private void UpgradeButton_Click(object sender, RoutedEventArgs e)
    {
        if (this._selectedProductId == null)
            return;

        // this.UpgradeButton.IsEnabled = false;
        _applicationController.RequestVersionUpgrade(_selectedProductId);
    }


}
