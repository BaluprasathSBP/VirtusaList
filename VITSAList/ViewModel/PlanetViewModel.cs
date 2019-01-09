using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xamarin.Forms;
namespace VITSAList
{
    public class PlanetViewModel:INotifyPropertyChanged
    {
        public ObservableCollection<PlanetDetail> Planets
        {
            get;
            set;
        }
        private APIClient apiClient;

        public event PropertyChangedEventHandler PropertyChanged;


        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public PlanetViewModel()
        {
            var planetDetails = GetPlanets();
            if (planetDetails.Result != null)
            {
                Planets = new ObservableCollection<PlanetDetail>(planetDetails.Result);
            }

            if(Planets== null)
            {
                // Offline
                if (Application.Current.Properties.ContainsKey("Planets"))
                {
                    Planets = Application.Current.Properties["Planets"] as ObservableCollection<PlanetDetail>;
                }
            }

            // Add in application properties to show it in offline, the data will be saved in device cache
            if (Application.Current.Properties.ContainsKey("Planets"))
            {
                Application.Current.Properties["Planets"] = Planets;
            }
            else
            {
                Application.Current.Properties.Add("Planets", Planets);
            }
            Application.Current.SavePropertiesAsync().ConfigureAwait(false);
            NotifyPropertyChanged("Planets");
        }

        /// <summary>
        /// Gets the planets. UNIT Testable
        /// </summary>
        /// <returns>The planets.</returns>
        public async Task<List<PlanetDetail>> GetPlanets()
        {
            try
            {
                apiClient = new APIClient();
                var planetDetail = await apiClient.GetPlanets();
                return planetDetail.results;
            }
            catch(Exception ex)
            {
                return null;
            }
        }

    }
}
