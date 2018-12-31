using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Frontend.Mobile.Models;
using Frontend.Mobile.ViewModels;

namespace Frontend.Mobile.Views
{
    public sealed partial class FillInAnOrder : Page
    {
        public FillInAnOrder()
		{
			InitializeComponent();
		}

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var order = e.Parameter as Order;
            this.DataContext = new FillInAnOrderViewModel(order);
        }
    }
}
