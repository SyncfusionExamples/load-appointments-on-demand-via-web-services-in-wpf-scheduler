# Load appointments On-Demand via web services in WPF Scheduler (SfScheduler)

The Syncfusion WPF Scheduler control provides all the common scheduling functionalities that creates and manages day to day business and personal events. When we develop WPF application, some of the most prevalent requirements are the ability to reduce loading time and system resource conservation.  

In this blog we are going to discuss about loading appointments on demand using web services in the WPF Scheduler control, which improves application performance and includes the ability to access data from web services. If you are new to the SfScheduler control, please read the Getting Started article in the scheduler documentation before proceeding.

## Creating a model class ##

Create a model class SchedulerAppointment that contains the similar data structure in Web API service containing the appointment subject, time and other related information.

    /// <summary>   
    /// Represents custom appointment properties.   
    /// </summary> 
    public class SchedulerAppointment
    {
        /// <summary>
        /// Gets or sets the subject of the appointment. 
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Gets or sets the id of the appointment. 
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the start time of the appointment. 
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// Gets or sets the end time of the appointment. 
        /// </summary>
        public DateTime EndTime { get; set; }

        /// <summary>
        /// Gets or sets teh value that indicates whether the appointment is all day. 
        /// </summary>
        public bool AllDay { get; set; }

        /// <summary>
        /// Gets or sets the recurrence rule of the appointment. 
        /// </summary>
        public string RecurrenceRule { get; set; }

        /// <summary>
        /// Gets or sets the background color of the appointment. 
        /// </summary>
        public Brush Color { get; set; }
    }

## Creating a web API service ## 

Web services are the server-side applications that are meant to serve data or logic to various client applications. REST and SOAP are the widely used industry standard web service architecture. 

Use the following reference to create an ASP.NET Core web API service and host it for public access. For demo purposes, we are going to use the following hosted service.

**Fetching data from Web API service**

•	Create an asynchronous method GetAppointmentsAsync and consume the API service URI.

•	Use the GetAsync method of HttpClient to retrieve the web appointment data as JSON.

•	Modify the received JSON data into a list of appointments.

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

## Configuring Scheduler ViewModel ##

The Scheduler control has LoadOnDemandCommand that can be invoked from ViewModel so that it ensures MVVM friendly applications, in addition while loading appointments from web services we can show the busy indicator on scheduler component.

The Scheduler supports to bind any collection to populate appointments, you can map properties of business object to ScheduleAppointment by configuring the AppointmentMapping property. Create SchedulerViewModel class with properties for load on-demand, showing busy indicator, binding scheduler ItemsSource and methods for fetching and binding web appointments on demand basis.

    public class SchedulerViewModel : NotificationObject
    {
        private WebAPIService webAPIService;
        private ObservableCollection<SchedulerAppointment> appointments;
        private List<Brush> colorCollection;
        private bool showBusyIndicator;

        /// <summary>
        /// Gets or sets load on demand command.
        /// </summary>
        public ICommand LoadOnDemand { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to show the busy indicator.
        /// </summary>
        public bool ShowBusyIndicator
        {
            get { return this.showBusyIndicator; }
            set
            {
                this.showBusyIndicator = value;
                this.RaisePropertyChanged("ShowBusyIndicator");
            }
        }

        /// <summary>
        /// Gets or sets the appointments. 
        /// </summary>
        public ObservableCollection<SchedulerAppointment> Appointments
        {
            get { return this.appointments; }
            set
            {
                this.appointments = value;
                this.RaisePropertyChanged("Appointments");
            }
        }

        public SchedulerViewModel()
        {
            this.webAPIService = new WebAPIService();
            this.Appointments = new ObservableCollection<SchedulerAppointment>();
            this.LoadOnDemand = new DelegateCommand(ExecuteOnDemandLoading, CanExecuteOnDemandLoading);            
        }
    }


## Fetching appointments for visible date range ##

In scheduler, LoadOnDemand can be achieved in two ways.
1.	QueryAppointments event
2.	LoadOnDemandCommand 

Both event and command will be invoked on initial loading, each swipe to next or previous views for all SchedulerViewTypes, SchedulerViewType change, ResourceCollection change and ResourceType change. In this sample we are using LoadOnDemandCommand approach to load appointments.

Here you can get the current visible date range from command argument and fetch appointments withing the current visible date range and update Appointments property that is bound with Scheduler ItemsSource.

    public class SchedulerViewModel : INotifyPropertyChanged
    {

         …

        /// <summary>
        /// Method to excute load on demand command and set scheduler appointments.
        /// </summary>
        /// <param name="queryAppointments">QueryAppointmentsEventArgs object.</param>
        public async void ExecuteOnDemandLoading(object queryAppointments)
        {
            this.ShowBusyIndicator = true;
            await this.GetDataFromWebAPI((queryAppointments as QueryAppointmentsEventArgs).VisibleDateRange);
            this.ShowBusyIndicator = false;
        }

        /// <summary>
        /// Method to check whether the load on demand command can be invoked or not.
        /// </summary>
        /// <param name="queryAppointments">QueryAppointmentsEventArgs object.</param>
        private bool CanExecuteOnDemandLoading(object queryAppointments)
        {
            return queryAppointments != null;
        }

         /// <summary>
        /// Method to get web appointments and update it to scheduler ItemsSource.
        /// </summary>
        /// <param name="visibleDateRange">Current visible date range.</param>
        private async Task GetDataFromWebAPI(DateRange visibleDateRange)
        {
            var appointmentWebData = await webAPIService.GetAppointmentsAsync();
            var random = new Random();
            foreach (var scheduleEvent in appointmentWebData)
            {
                //// Random color added for web appointments
                scheduleEvent.Color = this.colorCollection[random.Next(9)];
            }

            this.GetVisibleRangeAppointments(visibleDateRange, appointmentWebData);
        }

        /// <summary>
        /// Updates the appointment collection property to load appointments on demand.
        /// </summary>
        public void GetVisibleRangeAppointments(DateRange visibleDateRange, ObservableCol-lection<SchedulerAppointment> appointmentWebData)
        {
            if (appointmentWebData == null || appointmentWebData.Count == 0)
                return;

            var appointments = new ObservableCollection<SchedulerAppointment>();
            foreach (SchedulerAppointment appointment in appointmentWebData)
            {
                if ((visibleDateRange.StartDate <= appointment.StartTime.Date && visible-DateRange.EndDate >= appointment.StartTime.Date) ||
                    (visibleDateRange.StartDate <= appointment.EndTime.Date && visibleDat-eRange.EndDate >= appointment.EndTime.Date))
                {
                    appointments.Add(appointment);
                }
            }

            this.Appointments = appointments;
        }    
  }

You can bind the custom data with the scheduler component using mapping technique. Map the properties of the custom appointment with the equivalent properties of AppointmentMapping class. Now, set the SchedulerViewModel to the DataContext of scheduler to bind SchedulerViewModel properties to scheduler.

<Grid>
        <Grid.DataContext>
            <local:SchedulerViewModel/>
        </Grid.DataContext>

        <syncfusion:SfScheduler x:Name="scheduler"
                                ViewType="Month"
                                ItemsSource="{Binding Appointments}"
                                ShowBusyIndicator="{Binding ShowBusyIndicator}"
                                LoadOnDemandCommand="{Binding LoadOnDemand}">
            
            <syncfusion:SfScheduler.AppointmentMapping>
                <syncfusion:AppointmentMapping
                         Subject="Subject"
                         StartTime="StartTime"
                         EndTime="EndTime"
                         IsAllDay="AllDay"
                         AppointmentBackground="Color"
                         RecurrenceRule="RecurrenceRule"/>
            </syncfusion:SfScheduler.AppointmentMapping>
        </syncfusion:SfScheduler>
    </Grid>

Now, scheduler control is configured to load appointments on-demand using web API service. Just running the sample with the previous steps will render a scheduler with appointments.

## Conclusion ##

In this blog post, we’ve discussed about loading appointments on-demand using web services in WPF scheduler. You can also check out our project samples in this GitHub repository. Feel free to try out this sample and share your feedback or questions in the comments section. 

You can also contact us through our support forum, Direct-Trac, or feedback portal. We are always happy to assist you.

