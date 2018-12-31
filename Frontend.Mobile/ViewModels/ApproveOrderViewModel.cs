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
using Frontend.Mobile.Models.Enum;
using Frontend.Mobile.Services.Business;
using Frontend.Mobile.Views;
using Xamarin.Forms;
using Frame = Windows.UI.Xaml.Controls.Frame;

namespace Frontend.Mobile.ViewModels
{
    class ApproveOrderViewModel : ViewModelBase
    {
        private readonly DriverService _svc;
        private DispatcherTimer _dispatcherTimer;
        private readonly int INITIAL_TIME = 20;

        private ICommand _rejectOrder;
        private ICommand _acceptOrder;

        public ICommand RejectOrder
        {
            get { return _rejectOrder; }
        }

        public ICommand AcceptOrder
        {
            get { return _acceptOrder; }
        }

        public Order Order { get; set; }
        public Driver Driver { get; set; }
        public int RemainingTime { get; set; }

        public ApproveOrderViewModel(Order order)
        {
            _svc = new DriverService();
            Order = order;
            Driver = _svc.GetCurrent();
            _rejectOrder = new Command(RejectOrderAction);
            _acceptOrder = new Command(AcceptOrderAction);

            _dispatcherTimer = new DispatcherTimer();
            _dispatcherTimer.Tick += UpdateRemainingTime;
            _dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            _dispatcherTimer.Start();

            RemainingTime = INITIAL_TIME;
        }

        private void RejectOrderAction()
        {
            _dispatcherTimer.Stop();
            _svc.RejectOrder(Order.Id);
            ((Frame) Window.Current.Content).Navigate(typeof(Default));
        }

        private void SendSMS(string phoneNumber)
        {
            Windows.ApplicationModel.Chat.ChatMessage msg = new Windows.ApplicationModel.Chat.ChatMessage();
            msg.Body = "Kierowca " + Driver.FirstName + " będzie u państwa (" + Order.Start + ") w ciągu 15 minut.";
            msg.Recipients.Add(phoneNumber);
            Windows.ApplicationModel.Chat.ChatMessageManager.ShowComposeSmsMessageAsync(msg);
        }

        private void AcceptOrderAction()
        {
            _dispatcherTimer.Stop();
            _svc.AcceptOrder(Order.Id);
            SendSMS(Order.PhoneNumber);
            ((Frame)Window.Current.Content).Navigate(typeof(FillInAnOrder), Order);
        }

        private void UpdateRemainingTime(object sender, object e)
        {
            RemainingTime--;
            RaisePropertyChanged("RemainingTime");
            if (RemainingTime == 0)
            {
                RejectOrderAction();
            }
        }
    }
}
