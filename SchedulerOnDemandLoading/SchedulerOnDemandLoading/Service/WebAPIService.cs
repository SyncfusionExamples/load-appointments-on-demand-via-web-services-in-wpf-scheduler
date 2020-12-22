using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SchedulerOnDemandLoading
{
    public class WebAPIService
    {
        private HttpClient client;

        public WebAPIService()
        {
            client = new HttpClient();
        }

        /// <summary>
        /// Asynchronously fetching the data from the web API service.
        /// </summary>
        /// <returns></returns>
        public async Task<ObservableCollection<SchedulerAppointment>> GetAppointmentsAsync()
        {
            var uri = new Uri("https://js.syncfusion.com/demos/ejservices/api/Schedule/LoadData");
            try
            {
                var response = await client.GetAsync(uri);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<ObservableCollection<SchedulerAppointment>>(content);
                }
            }
            catch (Exception ex)
            {
            }
            return null;
        }
    }
}
