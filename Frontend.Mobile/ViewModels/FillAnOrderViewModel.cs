using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Frontend.Mobile.Models;
using System.Net.Http;
using System.Runtime.InteropServices.ComTypes;
using System.Threading;
using System.Windows.Input;
using Windows.UI.Xaml;
using Frontend.Mobile.Services.Business;
using Frontend.Mobile.Views;
using Xamarin.Forms;
using Frame = Windows.UI.Xaml.Controls.Frame;

namespace Frontend.Mobile.ViewModels
{
    class FillInAnOrderViewModel : ViewModelBase
    {
        private readonly DriverService _svc;
        private DispatcherTimer _dispatcherTimer;


        public Order Order { get; set; }
        public Driver Driver { get; set; }
        public string ElapsedTimeSeconds { get; set; }
        public string ElapsedTimeMinutes { get; set; }

        private string _status;

        public string Status
        {
            get { return _status;; }
            set
            {
                _status = value;
                RaisePropertyChanged("Status");
            }
        }

        private ICommand _markAsDone;

        public ICommand MarkAsDone
        {
            get
            {
                return _markAsDone;;
            }
        }

        private int _elapsedTime;

        private void MarkAsDoneAction()
        {
            _svc.MarkAsDone(Order.Id);
            ((Frame)Window.Current.Content).Navigate(typeof(BackToBasement), Order);
        }


        public FillInAnOrderViewModel(Order order)
        {
            _svc = new DriverService();
            Order = order;

            Status = "Realizuje zlecenie";

            _elapsedTime = 0;

            _markAsDone = new Command(MarkAsDoneAction);

            _dispatcherTimer = new DispatcherTimer();
            _dispatcherTimer.Tick += UpdateElapsedTime;
            _dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            _dispatcherTimer.Start();
        }

        private void UpdateElapsedTime(object sender, object e)
        {
            _elapsedTime++;
            var elapsedTimeMinutes = _elapsedTime/60;
            var elapsedTimeSeconds = _elapsedTime%60;

            ElapsedTimeMinutes = string.Format("{0:00}", elapsedTimeMinutes);
            ElapsedTimeSeconds = string.Format("{0:00}", elapsedTimeSeconds);

            RaisePropertyChanged("ElapsedTimeSeconds");
            RaisePropertyChanged("ElapsedTimeMinutes");
        }
    }
}
