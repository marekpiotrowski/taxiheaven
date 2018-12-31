using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Frontend.Mobile.Models;
using System.Net.Http;
using Windows.Devices.Geolocation;
using CrossPlatformLibrary.Geolocation;
using Microsoft.Practices.ServiceLocation;
using Newtonsoft.Json;

namespace Frontend.Mobile.Services.Business
{
    public class OrderService : Service<Order>
    {
        public IList<AbbreviatedOrder> GetRecent()
        {
            var client = new HttpClient();
            var requestUrl = _svcUrl + "GetRecentOrders?driverDeviceId=" + DeviceId;
            var request = new HttpRequestMessage(HttpMethod.Post, requestUrl);
            var svcCallTask = client.SendAsync(request);
            var response = svcCallTask.Result;
            var toStringTask = response.Content.ReadAsStringAsync();
            toStringTask.Wait();
            var result = JsonConvert.DeserializeObject<List<AbbreviatedOrder>>(toStringTask.Result);
            return result;
        }

    }
}