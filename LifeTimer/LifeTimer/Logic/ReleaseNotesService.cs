using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;

namespace LifeTimer.Logic
{
    public class ReleaseNotesService
    {
        //DEBUG ONLY!
        public bool ForceShowReleaseNotes = false;

        //set to false to disable release notes feature
        public bool EnableReleaseNotes = true;

        public const string ReleaseNotesFolder = "ReleaseNotes";
        public const string ReleaseNotesFile = "ReleaseNotes.txt";
        public const string ReleaseNotesVersionKey = "RELEASE_VERSION";

        public const int ReleaseNotesVersion = 1010;

        private SettingsManager _settingsManager;
        private ILogger<ReleaseNotesService> _logger;

        public ReleaseNotesService(SettingsManager settingsManager, ILogger<ReleaseNotesService> logger)
        {
            _settingsManager = settingsManager;
            _logger = logger;
        }


        public bool CheckShowReleaseNotes()
        {

#if DEBUG
            if (ForceShowReleaseNotes)
                _settingsManager.SetReleaseNotesStoredVersion(null);
#endif


            if (!EnableReleaseNotes)
                return false;


            var releaseNotesVersion = _settingsManager.GetReleaseNotesStoredVersion();

            if (releaseNotesVersion == null)
                return true;

            int storedVersionNo = releaseNotesVersion.Value;

            if (ReleaseNotesVersion > storedVersionNo) return true;

            return false;
        }

        public async Task<string?> GetReleaseNotesContent()
        {
            try
            {
                string installPath = Windows.ApplicationModel.Package.Current.InstalledLocation.Path;

                var path = System.IO.Path.Combine(installPath, ReleaseNotesFolder, ReleaseNotesFile);
                var text = await File.ReadAllTextAsync(path);
                return text;
            }
            catch (Exception ex)
            {
                _logger.LogError("ReleaseNotesService - unable to load release notes document");
            }

            return null;
        }


        public void UpdateReleaseNotesStoredVersion()
        {
            _settingsManager.SetReleaseNotesStoredVersion(ReleaseNotesVersion);
        }


    }
}
