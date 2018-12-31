using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.Connectivity;
using Windows.Storage;
using Frontend.Mobile.Services.Base;
using Newtonsoft.Json;

namespace Frontend.Mobile
{
    public abstract class Service<T> : IService<T> where T : class
    {
        protected readonly string _svcUrl;

        public Service()
        {
            var controller = this.GetType().Name.Replace("Service", "");
            var baseUrl = ApplicationData.Current.LocalSettings.Values["API_URL"].ToString();
            _svcUrl = baseUrl + controller + "/";
        }

        public T Get(int id)
        {
            var client = new HttpClient();

            var requestUrl = _svcUrl + id.ToString();

            var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);

            var svcCallTask = client.SendAsync(request);
            svcCallTask.Wait();

            var response = svcCallTask.Result;

            var toStringTask = response.Content.ReadAsStringAsync();
            toStringTask.Wait();

            var result = JsonConvert.DeserializeObject<T>(toStringTask.Result);

            return result;
        }

        public List<T> Get()
        {
            var client = new HttpClient();

            var request = new HttpRequestMessage(HttpMethod.Get, _svcUrl);

            var svcCallTask = client.SendAsync(request);
            svcCallTask.Wait();

            var response = svcCallTask.Result;

            var toStringTask = response.Content.ReadAsStringAsync();
            toStringTask.Wait();

            var result = JsonConvert.DeserializeObject<List<T>>(toStringTask.Result);

            return result;
        }

        public T Post(T model)
        {
            throw new NotImplementedException();
        }

        public T Put(T model)
        {
            throw new NotImplementedException();
        }

        public int Delete(int id)
        {
            throw new NotImplementedException();
        }

        public string DeviceId
        {
            get
            {
                var token = Windows.System.Profile.HardwareIdentification.GetPackageSpecificToken(null);
                var hardwareId = token.Id;
                var dataReader = Windows.Storage.Streams.DataReader.FromBuffer(hardwareId);

                byte[] bytes = new byte[hardwareId.Length];
                dataReader.ReadBytes(bytes);

                return BitConverter.ToString(bytes).Replace("-", "");
            }
        }
    }
}
