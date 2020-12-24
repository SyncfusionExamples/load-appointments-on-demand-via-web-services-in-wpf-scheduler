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
        public async Task<ObservableCollection<Event>> GetAppointmentsAsync(DateRange visibleDateRange)
        {
            var uri = new Uri("https://js.syncfusion.com/demos/ejservices/api/Schedule/LoadData");
            try
            {
                var response = await client.GetAsync(uri);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var webAppointments = (JsonConvert.DeserializeObject<ObservableCollection<Event>>(content));
                    var events = new ObservableCollection<Event>();
                    foreach (Event appointment in webAppointments)
                    {
                        if ((visibleDateRange.StartDate <= appointment.StartTime.Date && visibleDateRange.EndDate >= appointment.StartTime.Date) ||
                            (visibleDateRange.StartDate <= appointment.EndTime.Date && visibleDateRange.EndDate >= appointment.EndTime.Date))
                        {
                            events.Add(appointment);
                        }
                    }

                    return events;
                }
            }
            catch (Exception ex)
            {
            }
            return null;
        }
    }
}
