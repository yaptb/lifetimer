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

            // create ResourceManager
            var rm = new ResourceManager();

            // resource map name is usually the library/assembly name (see note below)
            string resourceMapName = "LifeTimerResources"; // replace with your library's resource map name

            // full path: <ResourceMapName>/Resources/<Key>
            string result = rm.MainResourceMap.GetValue($"{resourceMapName}/Resources/{key}").ValueAsString;

            return result;
        }
    }
}