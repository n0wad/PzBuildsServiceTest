using System.Diagnostics;
using System.Net;
using System.Text.Json;
using RestSharp;

namespace PzBuilds
{
    [TestClass]
    public class BuildAndMajorUpdates
    {
        //[TestMethod]
        //[Description("Добавление несуществующего билда в базу")]
        //public void InsertIfNotExist()
        //{
        //    var client = new RestClient(PzBuildsUrl.TestEnvUrl);
        //    var request = new RestRequest(Endpoint.InsertIfNotExist, Method.Get);
        //    request.AddQueryParameter("pathToIdent", Parameters.PathToIdent);
        //    request.AddQueryParameter("pathToApi", Parameters.PathToApi);
        //    request.AddHeader("Authorization", $"Bearer {TokenData.CrmToken}");
        //    RestResponse response = client.Execute(request);
        //    if (response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.Conflict)
        //    {
        //        if (response.StatusCode == HttpStatusCode.OK)
        //        {
        //            Console.WriteLine($"Билд добавлен в базу - {response.StatusCode}");
        //        }
        //        if (response.StatusCode == HttpStatusCode.Conflict)
        //        {
        //            Console.WriteLine($"Билд уже существует в базе - {response.StatusCode}");
        //        }
        //    }
        //    else
        //    {
        //        Assert.Fail($"Ошибка. Ответ {response.StatusCode} отличен от {HttpStatusCode.OK} или {HttpStatusCode.Conflict}");
        //    }
        //}

        [TestMethod]
        [Description("Получение списка билдов в базе")]
        public void GetAllBuilds()
        {
            var client = new RestClient(PzBuildsUrl.TestEnvUrl);
            var request = new RestRequest(Endpoint.Builds, Method.Get);
            request.AddHeader("Authorization", $"Bearer {TokenData.CrmToken}");
            RestResponse response = client.Execute(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                Console.WriteLine(response.StatusCode);

                List<Builds> buildResponse = JsonSerializer.Deserialize<List<Builds>>
                    (response.Content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                Console.WriteLine($"Всего билдов: {buildResponse.Count}");
                Console.WriteLine("Список билдов в базе:");

                Parameters.BuildVersion = new List<string>();
                Parameters.BuildId = new List<int>();
                foreach (Builds build in buildResponse)
                {
                    Parameters.BuildVersion.Add(build.AppVersion.VersionString);
                    Parameters.BuildId.Add(build.Id);
                }
                foreach (var version in Parameters.BuildVersion)
                {
                    Console.WriteLine(version);
                }
            }
            else
            {
                Assert.Fail($"Ошибка - {response.StatusCode}");
            }
        }

        [TestMethod]
        [Description("Существует ли билд по версии")]
        public void CheckBuildExist()
        {
            foreach (var version in Parameters.BuildVersion)
            {
                var client = new RestClient(PzBuildsUrl.TestEnvUrl);
                var request = new RestRequest(Endpoint.Exist, Method.Get);
                request.AddQueryParameter("appVersion", version);
                request.AddHeader("Authorization", $"Bearer {TokenData.CrmToken}");
                RestResponse response = client.Execute(request);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    Console.WriteLine($"Билд {version} существует - {response.StatusCode}");
                }
                else
                {
                    Assert.Fail($"Билд не существует - {response.StatusCode}");
                }
            }
        }

        [TestMethod]
        [Description("Существует ли билд по версии (несуществующий билд)")]
        public void CheckBuildNotExist()
        {
            var client = new RestClient(PzBuildsUrl.TestEnvUrl);
            var request = new RestRequest(Endpoint.Exist, Method.Get);
            request.AddQueryParameter("appVersion", Parameters.NotExistingVers);
            request.AddHeader("Authorization", $"Bearer {TokenData.CrmToken}");
            RestResponse response = client.Execute(request);
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                Console.WriteLine($"Билда {Parameters.NotExistingVers} в базе НЕТ - {response.StatusCode}");
            }
            else
            {
                Assert.Fail($"Ошибка. Ответ {response.StatusCode} отличен от {HttpStatusCode.NotFound}");
            }
        }

        [TestMethod]
        [Description("Получение всех билдов по версии")]
        public void GetAllMajorBuildsByVersion()
        {
            foreach (var version in Parameters.BuildVersion)
            {
                var client = new RestClient(PzBuildsUrl.TestEnvUrl);
                var request = new RestRequest(Endpoint.ByVersion, Method.Get);
                request.AddQueryParameter("appVersion", version);
                request.AddHeader("Authorization", $"Bearer {TokenData.ClientToken}");
                RestResponse response = client.Execute(request);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    Console.WriteLine($"Билд {version} - {response.StatusCode}");
                }
                else
                {
                    Assert.Fail($"Ошибка - {response.StatusCode}");
                }
            }
        }

        [TestMethod]
        [Description("Получение несуществующего билда")]
        public void GetMajorBuildNotExist()
        {
            var client = new RestClient(PzBuildsUrl.TestEnvUrl);
            var request = new RestRequest(Endpoint.ByVersion, Method.Get);
            request.AddQueryParameter("appVersion", Parameters.NotExistingVers);
            request.AddHeader("Authorization", $"Bearer {TokenData.ClientToken}");
            RestResponse response = client.Execute(request);
            if (response.StatusCode == HttpStatusCode.NoContent)
            {
                Console.WriteLine($"Билда {Parameters.NotExistingVers} в базе НЕТ - {response.StatusCode}");
            }
            else
            {
                Assert.Fail($"Ошибка. Ответ {response.StatusCode} отличен от {HttpStatusCode.NoContent}");
            }
        }

        [TestMethod]
        [Description("Получение метаданных файлов")]
        public void GetMajorBuildFilesMetaInfo()
        {
            var client = new RestClient(PzBuildsUrl.TestEnvUrl);

            foreach (int id in Parameters.BuildId)
            {
                var request = new RestRequest($"api/IDENT/v1/MajorUpdates/build/{id}/filesMetaInfo", Method.Get);
                request.AddHeader("Authorization", $"Bearer {TokenData.ClientToken}");
                RestResponse response = client.Execute(request);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    List<MetaInfo> filesInfoResponse = JsonSerializer.Deserialize<List<MetaInfo>>
                        (response.Content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    Console.WriteLine($"Всего файлов в {Parameters.BuildVersion[Parameters.Index]} билде: {filesInfoResponse.Count} - {response.StatusCode}");
                    Parameters.Index++;

                    if (Parameters.Index == Parameters.BuildId.Count)
                    {
                        Parameters.BuildFileId = new List<int>();
                        Parameters.BuildFileMd5 = new List<string>();
                        foreach (MetaInfo file in filesInfoResponse)
                        {
                            Parameters.BuildFileId.Add(file.FileId);
                            Parameters.BuildFileMd5.Add(file.Md5);
                        }
                    }
                }
                else
                {
                    Assert.Fail($"Ошибка - {response.StatusCode}");
                }
            }
        }

        [TestMethod]
        [Description("Получение файлов билда")]
        public void GetMajortIdentPackage()
        {
            var client = new RestClient(PzBuildsUrl.TestEnvUrl);
            var request = new RestRequest(Endpoint.GetIdentPackage, Method.Get);
            request.AddHeader("Authorization", $"Bearer {TokenData.ClientToken}");
            RestResponse response = client.Execute(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                Console.WriteLine($"Файлы получены - {response.StatusCode}");
            }
            else
            {
                Assert.Fail($"Ошибка - {response.StatusCode}");
            }
        }

        [TestMethod]
        [Description("Получение данных по id файла")]
        public void GetMajorFileInfoById()
        {
            foreach (var id in Parameters.BuildFileId)
            {
                var client = new RestClient(PzBuildsUrl.TestEnvUrl);
                var request = new RestRequest($"{Endpoint.GetById}{id}", Method.Get);
                request.AddHeader("Authorization", $"Bearer {TokenData.ClientToken}");
                RestResponse response = client.Execute(request);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    Console.WriteLine($"Данные по файлу {id} из билда " +
                        $"{Parameters.BuildVersion[Parameters.Index - 1]} получены - {response.StatusCode}");
                }
                else
                {
                    Assert.Fail($"Ошибка - {response.StatusCode}");
                }
            }
        }

        [TestMethod]
        [Description("Получение данных по MD5")]
        public void GetMajorFileInfoByMD5()
        {
            foreach (var hash in Parameters.BuildFileMd5)
            {
                var client = new RestClient(PzBuildsUrl.TestEnvUrl);
                var request = new RestRequest($"{Endpoint.GetByMD5}{hash}", Method.Get);
                request.AddHeader("Authorization", $"Bearer {TokenData.ClientToken}");
                RestResponse response = client.Execute(request);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    Console.WriteLine($"Данные по файлу {hash} из билда " +
                        $"{Parameters.BuildVersion[Parameters.Index - 1]} получены - {response.StatusCode}");
                }
                else
                {
                    Assert.Fail($"Ошибка - {response.StatusCode}");
                }
            }
        }
    }
}
