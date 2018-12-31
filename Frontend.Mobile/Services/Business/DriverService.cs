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
    public class DriverService : Service<Driver>
    {
        public void UpdateCoordinates()
        {
            Coordinates payload = new Coordinates();
            payload.DeviceId = DeviceId;

            Geolocator geolocator = new Geolocator();
            var geopositionTask = geolocator.GetGeopositionAsync(
                        maximumAge: TimeSpan.FromMinutes(5),
                        timeout: TimeSpan.FromSeconds(10)
                        );

            var client = new HttpClient();
            var requestUrl = _svcUrl + "UpdateCoordinates";
            var request = new HttpRequestMessage(HttpMethod.Put, requestUrl);

            geopositionTask.Completed = (task, status) =>
            {
                var position = task.GetResults();

               // Random r = new Random();    //Temporary solution to see the difference!
               // int n = r.Next() % 100;

                payload.Latitude = position.Coordinate.Latitude;
                payload.Longitude = position.Coordinate.Longitude;

                request.Content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
                client.SendAsync(request);
            };
        }

        public Order TakeOrders()
        {
            var client = new HttpClient();
            var requestUrl = _svcUrl + "TakeOrders?DeviceId=" + DeviceId;
            var request = new HttpRequestMessage(HttpMethod.Post, requestUrl);
            var svcCallTask = client.SendAsync(request);
            var response = svcCallTask.Result;
            var toStringTask = response.Content.ReadAsStringAsync();
            toStringTask.Wait();
            var result = JsonConvert.DeserializeObject<Order>(toStringTask.Result);
            return result;
        }

        public void RejectOrder(int orderId)
        {
            var client = new HttpClient();
            var requestUrl = _svcUrl + "RejectOrder?deviceId=" + DeviceId + "&orderId=" + orderId;
            var request = new HttpRequestMessage(HttpMethod.Post, requestUrl);
            client.SendAsync(request);
        }

        public void AcceptOrder(int orderId)
        {
            var client = new HttpClient();
            var requestUrl = _svcUrl + "AcceptOrder?orderId=" + orderId;
            var request = new HttpRequestMessage(HttpMethod.Post, requestUrl);
            client.SendAsync(request);
        }

        public void MarkAsDone(int orderId)
        {
            var client = new HttpClient();
            var requestUrl = _svcUrl + "MarkAsDone?orderId=" + orderId;
            var request = new HttpRequestMessage(HttpMethod.Post, requestUrl);
            client.SendAsync(request);
        }

        public void ArrivedToBasement()
        {
            var client = new HttpClient();
            var requestUrl = _svcUrl + "ArrivedToBasement?deviceId=" + DeviceId;
            var request = new HttpRequestMessage(HttpMethod.Post, requestUrl);
            client.SendAsync(request);
        }

        public Driver GetCurrent()
        {
            var client = new HttpClient();
            var requestUrl = _svcUrl + "GetCurrent?deviceId=" + DeviceId;
            var request = new HttpRequestMessage(HttpMethod.Post, requestUrl);
            var svcCallTask = client.SendAsync(request);
            var response = svcCallTask.Result;
            var toStringTask = response.Content.ReadAsStringAsync();
            toStringTask.Wait();
            var result = JsonConvert.DeserializeObject<Driver>(toStringTask.Result);
            return result;
        }

        public Driver SetState(bool state)
        {
            var client = new HttpClient();
            var requestUrl = _svcUrl + "SetState?deviceId=" + DeviceId + "&state=" + state;
            var request = new HttpRequestMessage(HttpMethod.Post, requestUrl);
            var svcCallTask = client.SendAsync(request);
            var response = svcCallTask.Result;
            var toStringTask = response.Content.ReadAsStringAsync();
            toStringTask.Wait();
            var result = JsonConvert.DeserializeObject<Driver>(toStringTask.Result);
            return result;
        }
    }
}