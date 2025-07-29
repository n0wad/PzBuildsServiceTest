using System.Net;
using System.Text.Json;
using RestSharp;

namespace PzBuilds
{
    [TestClass]
    public class Admin
    {

        [TestMethod]
        [Description("Удаление билда из базы по его версии")]
        public void DeleteByVersion()
        {
            var client = new RestClient(PzBuildsUrl.TestEnvUrl);
            var request = new RestRequest(Endpoint.Delete, Method.Delete);
            request.AddQueryParameter("version", Parameters.VersForDelete);
            request.AddHeader("Authorization", $"Bearer {TokenData.AdminToken}");
            RestResponse response = client.Execute(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                Console.WriteLine($"Билд {Parameters.NotExistingVers} удален - {response.StatusCode}");
            }
            else
            {
                Assert.Fail($"Ошибка - {response.StatusCode}");
            }
        }

        [TestMethod]
        [Description("Отчистка кэша")]
        public void CacheReset()
        {
            var client = new RestClient(PzBuildsUrl.TestEnvUrl);
            var request = new RestRequest(Endpoint.CacheReset, Method.Delete);
            request.AddHeader("Authorization", $"Bearer {TokenData.AdminToken}");
            RestResponse response = client.Execute(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                Console.WriteLine($"Кэш отчищен - {response.StatusCode}");
            }
            else
            {
                Assert.Fail($"Ошибка - {response.StatusCode}");
            }
        }
    }
}