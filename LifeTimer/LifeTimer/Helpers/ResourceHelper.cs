using Microsoft.Windows.ApplicationModel.Resources;



namespace LifeTimer.Helpers
{
    public static class ResourceHelper
    {
        private static ResourceManager _resourceManager;

        static ResourceHelper()
        {
            _resourceManager = new ResourceManager();

        }

        public static string GetString(string key)
        {
            var result = _resourceManager.MainResourceMap.GetValue("Resources/"+key).ValueAsString;
            return result;
        }
    }
}