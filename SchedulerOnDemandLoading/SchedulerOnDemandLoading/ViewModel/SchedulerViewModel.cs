using Syncfusion.UI.Xaml.Scheduler;
using Syncfusion.Windows.Shared;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

namespace SchedulerOnDemandLoading
{
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

        public DateTime DisplayDate { get; set; }

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
            this.DisplayDate = new DateTime(2017, 6, 1, 9, 0, 0);
            this.InitializeEventColor();
        }

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
        public void GetVisibleRangeAppointments(DateRange visibleDateRange, ObservableCollection<SchedulerAppointment> appointmentWebData)
        {
            if (appointmentWebData == null || appointmentWebData.Count == 0)
                return;

            var appointments = new ObservableCollection<SchedulerAppointment>();
            foreach (SchedulerAppointment appointment in appointmentWebData)
            {
                if ((visibleDateRange.StartDate <= appointment.StartTime.Date && visibleDateRange.EndDate >= appointment.StartTime.Date) ||
                    (visibleDateRange.StartDate <= appointment.EndTime.Date && visibleDateRange.EndDate >= appointment.EndTime.Date))
                {
                    appointments.Add(appointment);
                }
            }

            this.Appointments = appointments;
        }

        private void InitializeEventColor()
        {
            this.colorCollection = new List<Brush>();
            this.colorCollection.Add(new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF339933")));
            this.colorCollection.Add(new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF00ABA9")));
            this.colorCollection.Add(new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFE671B8")));
            this.colorCollection.Add(new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF1BA1E2")));
            this.colorCollection.Add(new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFD80073")));
            this.colorCollection.Add(new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFA2C139")));
            this.colorCollection.Add(new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFA2C139")));
            this.colorCollection.Add(new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFD80073")));
            this.colorCollection.Add(new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF339933")));
            this.colorCollection.Add(new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFE671B8")));
            this.colorCollection.Add(new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF00ABA9")));
        }
    }
}
