using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml;
using System;
using System.Threading.Tasks;
using Windows.Services.Store;

namespace LifeTimer.Logic
{

    public enum LifeTimerVersionTypes
    {
        Free,
        ProLifetime,
        ProSubscription,
        Plus
    }


   public class WindowsStoreHelper
    {

        private const bool TestModeOverrideStore = false;
        private readonly ILogger<WindowsStoreHelper> _logger;

        public const string PRO_LIFE_VERSION_PRODUCT_ID = "9N1WWH32P6TX";

        public const string PRO_SUB_VERSION_PRODUCT_ID = "9N22J7L5H53P";

        private LifeTimerVersionTypes _productVersion = LifeTimerVersionTypes.Free;
        private bool _useCachedProductValues = false;

        private bool _proLifeIsAvailable = false;
        private string _proLifeFormattedPrice = string.Empty;

        private bool _proSubIsAvailable = false;
        private string _proSubFormattedPrice = string.Empty;
        private string _proSubRenewalPeriod = string.Empty;
        private string _proSubRenewalUnits = string.Empty;


        private StoreContext? _storeContext;

        public WindowsStoreHelper(ILogger<WindowsStoreHelper> logger)
        {
            _logger = logger;
            _logger.LogInformation("WindowStore Helper initialized");
            _storeContext = StoreContext.GetDefault();
        }




        public async Task CheckAndCacheProductAvailability()
        {

            if (IsPlusVersion)
                return;


            //            var renewalString=ResourceHelper.GetString("WindowsStoreHelper_RenewalText");

            if (_storeContext == null)
                throw new InvalidOperationException("_storeContext is not initialized");

            try
            {
                _logger.LogInformation("WindowsHelper: CheckAndCacheProductAvailability() Refreshing product availability");

                var productKinds = new[] { "Durable", "Subscription" };
                //                var storeIds = new[] { APP_STORE_ID };

                var storeIds = new[] { PRO_LIFE_VERSION_PRODUCT_ID, PRO_SUB_VERSION_PRODUCT_ID };

                StoreProductQueryResult storeresult = await _storeContext.GetStoreProductsAsync(productKinds, storeIds);

                if (storeresult != null)
                {
                    if (storeresult.Products.TryGetValue(PRO_LIFE_VERSION_PRODUCT_ID, out StoreProduct? proLifeProduct))
                    {
                        if (proLifeProduct != null)
                        {
                            _proLifeIsAvailable = true;
                            _proLifeFormattedPrice = proLifeProduct.Price.FormattedPrice;

                            _logger.LogInformation($"WindowsHelper: Pro Life Version Available {_proLifeFormattedPrice}");
                        }
                    }

                    if (storeresult.Products.TryGetValue(PRO_SUB_VERSION_PRODUCT_ID, out StoreProduct? proSubProduct))
                    {
                        if (proSubProduct != null)
                        {
                            _proSubIsAvailable = true;
                            _proSubFormattedPrice = proSubProduct.Price.FormattedRecurrencePrice;

                            if (proSubProduct.Skus != null)
                                foreach (StoreSku sku in proSubProduct.Skus)
                                {

                                    if (sku.IsSubscription)
                                    {

                                        string billingPeriod = sku.SubscriptionInfo?.BillingPeriod.ToString(); // e.g. 1
                                        string billingUnit = sku.SubscriptionInfo?.BillingPeriodUnit.ToString();   // e.g.

                                        _proSubRenewalPeriod = billingPeriod;
                                        _proSubRenewalUnits = billingUnit;

                                        _logger.LogInformation($"WindowsHelper: Pro Sub Version Available {_proSubFormattedPrice} period: {_proSubRenewalPeriod} units: {_proSubRenewalUnits}");
                                    }
                                }

                        }

                        _proSubIsAvailable = proSubProduct != null;
                    }
                }

            }
            catch (Exception ex)
            {
                _logger.LogError("Error checking add on availability " + ex.Message);
            }

        }




        public async Task CheckAndCacheProductVersionAsync()
        {
            if (IsPlusVersion)
                return;

            if (_storeContext == null)
                throw new InvalidOperationException("_storeContext is not initialized");


            try
            {

                if (_useCachedProductValues)
                    return;

                _logger.LogInformation("WindowsHelper: CheckAndCacheProductVersionAsync() Refreshing product add-on licenses");

                var result = await _storeContext.GetAppLicenseAsync();

                if (result == null)
                {
                    _logger.LogInformation("WindowsHelper: Null license result - defaulting to free license");
                    _productVersion = LifeTimerVersionTypes.Free;
                    return;
                }

                foreach(var licenseKVP in result.AddOnLicenses)
                {

                    // Check if the lifetime pro license is owned and active
                    if (licenseKVP.Key.StartsWith(PRO_LIFE_VERSION_PRODUCT_ID))
                    {
                        var license = licenseKVP.Value;

                        _logger.LogInformation("WindowsHelper: CheckAndCacheProductVersionAsync() Found Pro Lifetime license: "+licenseKVP.Key);

                        if (license.IsActive)
                        {
                            _logger.LogInformation("WindowsHelper: CheckAndCacheProductVersionAsync() Lifetime license is ACTIVE");
                            _productVersion = LifeTimerVersionTypes.ProLifetime;
                            return;
                        }
                        else
                        {
                            _logger.LogInformation("WindowsHelper: CheckAndCacheProductVersionAsync() Lifetime license is NOT ACTIVE");
                        }
                    }

                    // Check if the sub pro license is owned and active
                    if (licenseKVP.Key.StartsWith(PRO_SUB_VERSION_PRODUCT_ID))
                    {
                        var license = licenseKVP.Value;

                        _logger.LogInformation("CheckAndCacheProductVersionAsync() Found Pro Subscription license "+licenseKVP.Key);

                        if (license.IsActive)
                        {
                            _logger.LogInformation(" CheckAndCacheProductVersionAsync() Pro Subscription license is ACTIVE");
                            _productVersion = LifeTimerVersionTypes.ProSubscription;
                            return;
                        }
                        else
                        {
                            _logger.LogInformation("CheckAndCacheProductVersionAsync() Pro Subscription license is NOT ACTIVE");
                        }
                    }





                }
                _logger.LogInformation($"WindowsHelper: License Check Completed");


            }
            catch (Exception ex)
            {
                _logger.LogError("Error checking product version: " + ex.Message);
                _logger.LogInformation("CheckAndCacheProductVersionAsync() Error checking license status - defaulting to free version");
                _productVersion = LifeTimerVersionTypes.Free;
            }

            finally
            {
                SetupTestModeStoreOverrides();

            }

        }



        /// <summary>
        /// this method must be called from the context of a UI windows to be able to
        /// display the store purchasing UI
        /// </summary>
        public async Task<StorePurchaseResult?> PeformStorePurchaseInWindow(Window window, string productID)
        {
           


            try
            {
                _logger.LogInformation($"WindowsHelper: PerformStorePurchaseInWindow - attempting purchase of {productID}");

                var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(window);

                //Note - we update the cached store context here
                var _storeContext = StoreContext.GetDefault();

                //This is the magic statement required for WINUI store UI to work
                WinRT.Interop.InitializeWithWindow.Initialize(_storeContext, hWnd);

                // This code will now execute on the UI thread
                StorePurchaseResult result = await _storeContext.RequestPurchaseAsync(productID);


                if (result != null)
                {
                    _logger.LogInformation($"WindowsHelper: PerformStorePurchaseInWindow - got store result {result.Status}");
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError("PeformStorePurchaseInWindow() : Error completing store purchase " + ex.Message);

            }

            return null;
        }


        public void ForceFreeVersion()
        {
            _productVersion = LifeTimerVersionTypes.Free;
        }


        public void ForcePlusVersion()
        {
            _productVersion = LifeTimerVersionTypes.Plus;
        }



        public void InvalidateCache()
        {
            this._useCachedProductValues = false;
        }


        public bool IsFreeVersion { get { return _productVersion == LifeTimerVersionTypes.Free; } }

        public bool IsPlusVersion { get { return _productVersion == LifeTimerVersionTypes.Plus; } }


        public bool IsProSubVersion { get { return _productVersion == LifeTimerVersionTypes.ProSubscription; } }

        public bool IsProLifetimeVersion { get { return _productVersion == LifeTimerVersionTypes.ProLifetime; } }

        public bool IsProLifeAddOnAvailable { get { return _proLifeIsAvailable; } }

        public bool IsProSubAddOnAvailable { get { return _proSubIsAvailable; } }

        public string ProLifetimeVersionFormattedPrice { get { return _proLifeFormattedPrice; } }

        public string ProSubVersionFormattedPrice { get { return _proSubFormattedPrice; } }

        public string ProSubRenewalPeriod { get { return _proSubRenewalPeriod; } }

        public string ProSubRenewalUnits { get { return _proSubRenewalUnits; } }


        private void SetupTestModeStoreOverrides()
        {
            if (!TestModeOverrideStore)
                return;
#if DEBUG


            _logger.LogWarning("***** STORE TEST MODE ENABLED ******");

            _productVersion = LifeTimerVersionTypes.Free;
            
            _proLifeIsAvailable = false;
            _proLifeFormattedPrice = "$14.99";

            _proSubIsAvailable = false;
            _proSubFormattedPrice = "$3.99";
            _proSubRenewalPeriod = "12";
            _proSubRenewalUnits = "months";

#endif

        }


    }
}