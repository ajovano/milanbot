namespace MilanBot.ViewModels
{
    internal class UserSettings
    {
        public bool isReady { get; private set; }

        private string token;
	    public string Token { get => token; set
            {
                this.token = value;
                this.PushSettings();
            }
        }

        public UserSettings()
        {
            this.InitializeSettings();
        }

        private async void InitializeSettings()
        {
            Token = await SecureStorage.Default.GetAsync("ADOToken");
            this.isReady = true;
        }
        private void PushSettings()
        {
            if (this.token != null)
            {
                SecureStorage.Default.SetAsync("ADOToken", this.token);
            }
        }
    }
}
