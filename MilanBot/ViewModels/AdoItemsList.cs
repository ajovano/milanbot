using Microsoft.VisualStudio.Services.Common;
using MilanBotLib.ADO;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace MilanBot.ViewModels
{
    internal class AdoItemsList : INotifyPropertyChanged
    {
	    public ObservableCollection<ADOWorkItem> AdoItems { get; set; }
        private ADOWorkItem activeItem;
        public ADOWorkItem ActiveItem { get => this.activeItem; set
            {
                if (value != null)
                {
                    if (this.activeItem?.ID != value?.ID)
                    {
                        this.UpdateADOItem();
                        this.TimeStarted = DateTime.Now;
                    }
                    this.activeItem = value;
                    this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ActiveItem)));
                }
            }
        }
        public string ADOError { get; set; }
        public bool IsAutomaticMode { get; set; }
        private bool isPaused;

        public event PropertyChangedEventHandler PropertyChanged;

        public bool IsPaused { 
            get => isPaused; 
            set
            {
                if (value)
                {
                    this.TimePaused = DateTime.Now;
                }
                else
                {
                    this.TimeStarted += (DateTime.Now - this.TimePaused);
                }
                this.isPaused = value;
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsPaused)));
            }
        }
        public DateTime TimeStarted { get; set; }
        public DateTime TimePaused { get; set; }

        public AdoItemsList()
        {
            this.AdoItems = new ObservableCollection<ADOWorkItem>();
            this.AdoItems.Insert(0, new ADOWorkItem(null, 0, "Not Tracking", null, 0, ADOWorkItemType.PRODUCT_BACKLOG_ITEM));
            this.ActiveItem = this.AdoItems[0];
            this.InitializeItems();
            this.TimeStarted = DateTime.Now;
        }

        public void OnItemSelected(ADOWorkItem newItem)
        {
        }

        private void UpdateADOItem()
        {
            if (this.IsPaused)
            {
                this.TimeStarted += (DateTime.Now - this.TimePaused);
                this.IsPaused = false;
            }
            var timeSpent = DateTime.Now - this.TimeStarted;
            if (this.activeItem != null && this.activeItem?.ID != 0)
            {
                // call to ADO to update ADO item.
            }
        }
        public async Task<IEnumerable<ADOWorkItem>> RefreshItems()
        {
            this.ADOError = null;
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
                    this.ADOError = ex.Message;
                    return null;
                }
            }
            else
            {
                this.ADOError = "Add a token in settings to load work items";
                return null;
            }
        }

        private async void InitializeItems()
        {
            var items = await RefreshItems();
            this.ActiveItem = this.AdoItems[0];
        }

        public void PauseTracking()
        {
            if (this.IsPaused)
            {
                this.TimeStarted += (DateTime.Now - this.TimePaused);
            }
            else
            {
                this.TimePaused = DateTime.Now;
            }
            this.IsPaused = !this.IsPaused;
        }
    }
}
