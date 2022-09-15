using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilanBot.ViewModels
{
    internal class AdoItemsList
    {
	    public ObservableCollection<AdoItem> AdoItems { get; set; }
	    
        public AdoItem TrackingItem { get; set; }
        public AdoItem ActiveItem { get; set; }

        private DateTime TimeStarted;
        
        //public bool Paused = false;

        public AdoItemsList()
        {
            this.AdoItems = new ObservableCollection<AdoItem>();
            this.InitializeItems();
            this.TimeStarted = DateTime.Now;
        }

        public void OnItemSelected(AdoItem newItem)
        {
            if (this.TrackingItem != newItem)
            {
                this.UpdateADOItem();
                this.TimeStarted = DateTime.Now;
                this.TrackingItem = newItem;
            }
        }

        private void UpdateADOItem()
        {
            var timeSpent = DateTime.Now - this.TimeStarted;
            if (this.TrackingItem.ID != null)
            {
                // call to ADO to update ADO item.
            }
        }

        private void InitializeItems()
        {
            this.AdoItems.Add(new AdoItem("Not Tracking",null));
            this.AdoItems.Add(new AdoItem("Pass 'Security type' parameter to Marketplace from VM create", "15316439"));
            this.AdoItems.Add(new AdoItem("[Screen Reader-AM64VM-Create an Arm64 VM-Select an Image]: Screen reader is not narrating the Information for the selected image after invoking the 'select' button.", "14626984"));
            this.ActiveItem = this.AdoItems[0];
            this.TrackingItem = this.AdoItems[0];
        }

        //public void PauseTracking()
        //{
        //    if (this.Paused)
        //    {

        //    }
        //    else
        //    {

        //    }
        //    this.Paused = !this.Paused;

        //}
    }
}
