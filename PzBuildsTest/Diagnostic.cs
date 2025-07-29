using System.Net;
using System.Text.Json;
using RestSharp;

namespace PzBuilds
{
    [TestClass]
    public class Diagnostic
    {

        [TestMethod]
        [Description("Пингуем сервис")]
        public void Ping()
        {
            var client = new RestClient(PzBuildsUrl.TestEnvUrl);
            var request = new RestRequest(Endpoint.Ping, Method.Get);
            RestResponse response = client.Execute(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                Console.WriteLine(response.StatusCode);
            }
            else
            {
                Assert.Fail($"Не пингуется - {response.StatusCode}");
            }
        }

        [TestMethod]
        [Description("Health Check")]
        public void Health()
        {
            var client = new RestClient(PzBuildsUrl.TestEnvUrl);
            var request = new RestRequest(Endpoint.Health, Method.Get);
            request.AddHeader("Authorization", $"Bearer {TokenData.CrmToken}");
            RestResponse response = client.Execute(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                Console.WriteLine(response.StatusCode);
                Console.WriteLine(response.Content);
            }
            else
            {
                Assert.Fail($"Ошибка - {response.StatusCode}");
            }
        }

        [TestMethod]
        public void EndpointStats()
        {
            var client = new RestClient(PzBuildsUrl.TestEnvUrl);
            var request = new RestRequest(Endpoint.EnpointStats, Method.Get);
            request.AddHeader("Authorization", $"Bearer {TokenData.CrmToken}");
            RestResponse response = client.Execute(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                Console.WriteLine(response.StatusCode);
                Console.WriteLine(response.Content);
            }
            else
            {
                Assert.Fail($"Ошибка - {response.StatusCode}");
            }
        }

        [TestMethod]
        public void Stats()
        {
            var client = new RestClient(PzBuildsUrl.TestEnvUrl);
            var request = new RestRequest(Endpoint.Stats, Method.Get);
            request.AddHeader("Authorization", $"Bearer {TokenData.CrmToken}");
            RestResponse response = client.Execute(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                Console.WriteLine(response.StatusCode);
                Console.WriteLine(response.Content);
            }
            else
            {
                Assert.Fail($"Ошибка - {response.StatusCode}");
            }
        }
    }
}
