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
    class BackToBasementViewModel : ViewModelBase
    {
        private readonly DriverService _svc;
        private DispatcherTimer _dispatcherTimer;

        public Order Order { get; set; }
        public Driver Driver { get; set; }

        private string _status;

        public string Status
        {
            get { return _status; ; }
            set
            {
                _status = value;
                RaisePropertyChanged("Status");
            }
        }
        private ICommand _arrived;
        public ICommand Arrived
        {
            get { return _arrived; }
        }

        public void ArrivedAction()
        {
            _svc.ArrivedToBasement();
            ((Frame)Window.Current.Content).Navigate(typeof(Default));
        }

        public BackToBasementViewModel(Order order)
        {
            _svc = new DriverService();
            Order = order;

            Driver = _svc.GetCurrent();

            Status = "Wraca do bazy";

            _arrived = new Command(ArrivedAction);

            _dispatcherTimer = new DispatcherTimer();
            _dispatcherTimer.Tick += TakeOrders;
            _dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            _dispatcherTimer.Start();
        }

        private void TakeOrders(object sender, object e)
        {
            var order = _svc.TakeOrders();
            if (order == null)
                return;
            _dispatcherTimer.Stop();
            ((Frame)Window.Current.Content).Navigate(typeof(ApproveOrder), order);
        }
    }
}
