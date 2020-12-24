﻿using Syncfusion.UI.Xaml.Scheduler;
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
        private ObservableCollection<Event> events;
        private List<Brush> colorCollection;
        private bool showBusyIndicator;

        /// <summary>
        /// Gets or sets load on demand command.
        /// </summary>
        public ICommand LoadOnDemandCommand { get; set; }

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
        public ObservableCollection<Event> Events
        {
            get { return this.events; }
            set
            {
                this.events = value;
                this.RaisePropertyChanged("Events");
            }
        }

        public SchedulerViewModel()
        {
            this.webAPIService = new WebAPIService();
            this.Events = new ObservableCollection<Event>();
            this.LoadOnDemandCommand = new DelegateCommand(ExecuteOnDemandLoading, CanExecuteOnDemandLoading);
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
            var events  = await webAPIService.GetAppointmentsAsync(visibleDateRange);
            var random = new Random();
            foreach (var scheduleEvent in events)
            {
                //// Random color added for web appointments
                scheduleEvent.Color = this.colorCollection[random.Next(9)];
            }

            this.Events = events;
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
