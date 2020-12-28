using Newtonsoft.Json;
using Syncfusion.UI.Xaml.Scheduler;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SchedulerOnDemandLoading
{
    public static class WebAPIService
    {
        /// <summary>
        /// Asynchronously fetching the data from the web API service.
        /// </summary>
        /// <returns></returns>
        public static async Task<ObservableCollection<Event>> GetAppointmentsAsync()
        {
            var uri = new Uri("https://js.syncfusion.com/demos/ejservices/api/Schedule/LoadData");
            try
            {
                var client = new HttpClient();
                var response = await client.GetAsync(uri);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return (JsonConvert.DeserializeObject<ObservableCollection<Event>>(content));
                }
            }
            catch (Exception ex)
            {
            }
            return null;
        }
    }
}
