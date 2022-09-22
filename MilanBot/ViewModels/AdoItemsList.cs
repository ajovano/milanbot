using Microsoft.VisualStudio.Services.Common;
using MilanBotLib.ADO;
using MilanBotLib.Git;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace MilanBot.ViewModels
{
    internal class AdoItemsList : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<ADOWorkItem> AdoItems { get; set; }

        private ADOWorkItem activeItem;
        public ADOWorkItem ActiveItem { get => this.activeItem; set
            {
                if (value != null)
                {
                    if (this.activeItem?.ID != value?.ID)
                    {
                        this.UpdateADOItem(this.activeItem);
                        this.TimeStarted = DateTime.Now;
                    }
                    this.activeItem = value;
                    this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ActiveItem)));
                }
            }
        }

        private string statusMessage;
        public string StatusMessage { get => this.statusMessage; set
            {
                this.statusMessage = value;
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(StatusMessage)));
            }
        }

        private Timer automaticModeTimer;
        private bool isAutomaticMode;
        public bool IsAutomaticMode { get => this.isAutomaticMode; set
            {
                this.isAutomaticMode = value;
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsAutomaticMode)));
                if (value)
                {
                    this.automaticModeTimer = new Timer(this.AutomaticModeTimerCallback, null, TimeSpan.Zero, TimeSpan.FromSeconds(10));
                    this.AutomaticModeTimerCallback();
                }
                else
                {
                    this.automaticModeTimer?.Dispose();
                }
            }
        }

        private async void AutomaticModeTimerCallback(object state = null)
        {
            var path = await SecureStorage.Default.GetAsync("GITPath");
            if (string.IsNullOrEmpty(path))
            {
                this.StatusMessage = "No GIT Repo configured.";
                this.IsAutomaticMode = false;
                return;
            }

            GitRepository repo = null;
            try
            {
                repo = new GitRepository(new GitConfiguration() { RepositoryPath = path });
            }
            catch (Exception ex)
            {
                StatusMessage = ex.Message;
                return;
            }
            var branch = repo.GetCurrentBranchName();
            var newItem = this.AdoItems.FirstOrDefault(item => item.GitBranchName == branch);
            if (newItem != null)
            {
                this.ActiveItem = newItem;
                this.StatusMessage = String.Format("Now Tracking {0} {1} due to GIT branch {2}.", 
                    newItem.ID, 
                    newItem.Title,
                    branch);
            }
            else
            {
                this.StatusMessage = String.Format("Current GIT Branch {0} has no work item associated.", branch);
                this.ActiveItem = this.AdoItems[0];
            }
        }

        private bool isPaused;
        private DateTime pauseStartTime;
        private TimeSpan totalPauseTime;
        public bool IsPaused { 
            get => isPaused; 
            set
            {
                if (this.isPaused && !value)
                {
                    this.totalPauseTime += DateTime.Now - this.pauseStartTime;
                }
                pauseStartTime = DateTime.Now;
                this.isPaused = value;
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsPaused)));
            }
        }
        public DateTime TimeStarted { get; set; }

        public AdoItemsList()
        {
            this.AdoItems = new ObservableCollection<ADOWorkItem>();
            this.AdoItems.Insert(0, new ADOWorkItem(null, 0, "Not Tracking", null, 0, ADOWorkItemType.PRODUCT_BACKLOG_ITEM));
            this.ActiveItem = this.AdoItems[0];
            this.InitializeItems();
            this.TimeStarted = DateTime.Now;
        }

        private async void UpdateADOItem(ADOWorkItem item)
        {
            if (this.IsPaused)
            {
                this.IsPaused = false;
            }
            var pauseTime = this.totalPauseTime;
            var timeSpent = DateTime.Now - this.TimeStarted - pauseTime;
            this.totalPauseTime = TimeSpan.Zero;
            if (item != null && item?.ID != 0)
            {
                await item.UpdateTotalHoursSpent(item.TotalHoursSpent + timeSpent.TotalHours);
            }
        }

        public async Task<IEnumerable<ADOWorkItem>> RefreshItems()
        {
            this.StatusMessage = null;
            var token = await SecureStorage.Default.GetAsync("ADOToken");
            if (!string.IsNullOrEmpty(token))
            {
                try
                {
                    var adoQueries = new ADOWorkItemQueries(
                                new ADOConfiguration()
                                {
                                    CollectionUri = "https://msazure.visualstudio.com/DefaultCollection",
                                    ProjectName = "One",
                                    AccessToken = token
                                });
                    var items = await adoQueries.GetWorkItemsInSprint();
                    this.AdoItems.Clear();
                    this.AdoItems.Insert(0, new ADOWorkItem(null, 0, "Not Tracking", null, 0, ADOWorkItemType.PRODUCT_BACKLOG_ITEM));
                    this.AdoItems.AddRange(items);
                    if (this.ActiveItem != null)
                    {
                        var newSelectedItem = this.AdoItems.First(item => item.ID == this.ActiveItem?.ID);
                        if (newSelectedItem != null)
                        {
                            this.ActiveItem = newSelectedItem;
                        } 
                    }
                    return items;
                }
                catch (Microsoft.VisualStudio.Services.Common.VssUnauthorizedException ex)
                {
                    this.StatusMessage = ex.Message;
                    return null;
                }
            }
            else
            {
                this.StatusMessage = "Add a token in settings to load work items";
                return null;
            }
        }

        private async void InitializeItems()
        {
            var items = await RefreshItems();
            this.ActiveItem = this.AdoItems[0];
        }
    }
}
