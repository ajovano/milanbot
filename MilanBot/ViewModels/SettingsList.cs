using System.ComponentModel;

namespace MilanBot.ViewModels
{
    internal class UserSettings
    {
        public bool isReady { get; private set; }

        private string token;

        public string Token { get => token; set
            {
                this.token = value;
                if (this.token != null)
                {
                    SecureStorage.Default.SetAsync("ADOToken", value);
                }
            }
        }

        private string gitPath;

        public string GITPath
        {
            get => gitPath; set
            {
                this.gitPath = value;
                if (this.gitPath != null)
                {
                    SecureStorage.Default.SetAsync("GITPath", value);
                }
            }
        }

        public UserSettings()
        {
            this.InitializeSettings();
        }

        private async void InitializeSettings()
        {
            this.token = await SecureStorage.Default.GetAsync("ADOToken");
            this.gitPath = await SecureStorage.Default.GetAsync("GITPath");
            this.isReady = true;
        }
    }
}
