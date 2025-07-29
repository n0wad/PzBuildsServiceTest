using System.Net;
using System.Text.Json;
using RestSharp;

namespace PzBuilds
{
    [TestClass]
    public class Authorize
    {
        [AssemblyInitialize]
        [Description("Получение всех токенов один раз для дальнейших тестов")]
        public static async Task GetAllTokens(TestContext context)
        {
            await GetCrmToken(context);
            await GetClientToken(context);
            await GetAdminToken(context);

            context.WriteLine("Получение токенов завершено");

            if (string.IsNullOrEmpty(TokenData.CrmToken) || 
                string.IsNullOrEmpty(TokenData.ClientToken) || 
                string.IsNullOrEmpty(TokenData.AdminToken))
            {
                Assert.Fail("Ошибка, токены не получены");
            }
        }

        [Description("Получение crm токена")]
        private static async Task GetCrmToken(TestContext context)
        {
            var Credentials = new
            {
                userName = UserCredentials.CrmName,
                password = UserCredentials.CrmPassword
            };
            string jsonBody = JsonSerializer.Serialize(Credentials);

            var client = new RestClient(PzBuildsUrl.AuthUrl);
            var request = new RestRequest(Endpoint.GetToken, Method.Post);
            request.AddQueryParameter("requiredRole", Roles.Crm);
            request.AddJsonBody(jsonBody);
            RestResponse response = await client.ExecuteAsync(request);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                TokenResponse responseBody = JsonSerializer.Deserialize<TokenResponse>(response.Content,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                TokenData.CrmToken = responseBody.AccessToken;
                context.WriteLine($"Токен для пользователя {Credentials.userName} получен");
            }
            else context.WriteLine("Ошибка. Токен не получен");
        }

        [Description("Получение сlient токена")]
        private static async Task GetClientToken(TestContext context)
        {
            var Credentials = new
            {
                userName = UserCredentials.Testbgsvc,
                password = UserCredentials.TestbgsvcPassword
            };
            string jsonBody = JsonSerializer.Serialize(Credentials);

            var client = new RestClient(PzBuildsUrl.AuthUrl);
            var request = new RestRequest(Endpoint.GetToken, Method.Post);
            request.AddQueryParameter("requiredRole", Roles.Client);
            request.AddJsonBody(jsonBody);
            RestResponse response = await client.ExecuteAsync(request);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                TokenResponse responseBody = JsonSerializer.Deserialize<TokenResponse>(response.Content,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                TokenData.ClientToken = responseBody.AccessToken;
                context.WriteLine($"Токен для пользователя {Credentials.userName} получен");
            }
            else context.WriteLine("Ошибка. Токен не получен");
        }

        [Description("Получение admin токена")]
        private static async Task GetAdminToken(TestContext context)
        {
            var Credentials = new
            {
                userName = UserCredentials.AdminName,
                password = UserCredentials.AdminPassword
            };
            string jsonBody = JsonSerializer.Serialize(Credentials);

            var client = new RestClient(PzBuildsUrl.AuthUrl);
            var request = new RestRequest(Endpoint.GetToken, Method.Post);
            request.AddQueryParameter("requiredRole", Roles.Admin);
            request.AddJsonBody(jsonBody);
            RestResponse response = await client.ExecuteAsync(request);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                TokenResponse responseBody = JsonSerializer.Deserialize<TokenResponse>(response.Content,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                TokenData.AdminToken = responseBody.AccessToken;
                context.WriteLine($"Токен для пользователя {Credentials.userName} получен");
            }
            else context.WriteLine("Ошибка. Токен не получен");
        }
    }
}