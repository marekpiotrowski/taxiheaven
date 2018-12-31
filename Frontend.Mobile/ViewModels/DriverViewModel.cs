using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Frontend.Mobile.Models;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices.ComTypes;
using Windows.Networking.Connectivity;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Frontend.Mobile.Models.Enum;
using Frontend.Mobile.Services.Business;
using Frontend.Mobile.Views;

namespace Frontend.Mobile.ViewModels
{
    class DriverViewModel : ViewModelBase
    {
        private readonly DriverService _svc;
        private readonly OrderService _orderSvc;
        private DispatcherTimer _dispatcherTimer;

        private string _status;
        public string Status
        {
            get { return _status; }
            set
            {
                this._status = value;
                this.RaisePropertyChanged("Status");
            }
        }

        private bool _active;
        public bool Active
        {
            get { return _active; }
            set
            {
                this._active = value;
                var driver = _svc.SetState(value);
                Status = driver.Status;
                this.RaisePropertyChanged("Active");
            }
        }
        public List<AbbreviatedOrder> RecentOrders { get; set; }

        public DriverViewModel()
        {
            _svc = new DriverService();
            _orderSvc = new OrderService();

            var orders = _orderSvc.GetRecent();
            RecentOrders = orders != null ? orders.ToList().Take(2).ToList() : new List<AbbreviatedOrder>();

            var driver = _svc.GetCurrent();

            _active = driver.StatusId != (int) DriverStatusEnum.Inactive &&
                      driver.StatusId != (int) DriverStatusEnum.NotApplicable;

            Status = driver.Status;

            _dispatcherTimer = new DispatcherTimer();
            _dispatcherTimer.Tick += TakeOrders;
            _dispatcherTimer.Interval = new TimeSpan(0, 0, 5);
            _dispatcherTimer.Start();
        }

        private void TakeOrders(object sender, object e)
        {
            var order =  _svc.TakeOrders();
            if (order == null)
                return;
            _dispatcherTimer.Stop();
            ((Frame)Window.Current.Content).Navigate(typeof(ApproveOrder), order);
        }
    }
}
