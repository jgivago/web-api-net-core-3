using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;
using Xunit.Abstractions;
using WebApi.Entities;
using WebApi.Models;

namespace WebApi.Tests
{
    public class FeatureTests: IDisposable
    {
        private readonly ITestOutputHelper _output;
        private readonly HttpClient _apiClient;        

        public FeatureTests(ITestOutputHelper output)
        {
            _output = output;
            HttpClientHandler clientHandler = new HttpClientHandler();
            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
            _apiClient = new HttpClient(clientHandler);
        }

        public void Dispose()
        {
            _output.WriteLine($"Disposing api client");
            _apiClient.Dispose();
        }

        [Fact]
        public async Task Authenticate_With_Wrong_User_Data()
        {
            var data = new AuthenticateRequest() { Username = "wrongusername", Password = "654321" };
            var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
            var apiResponse = await _apiClient.PostAsync("http://localhost:4000/users/authenticate", content);

            _output.WriteLine($"Authenticate_With_Wrong_User_Data - Status code: {apiResponse.StatusCode}");

            Assert.False(apiResponse.StatusCode == HttpStatusCode.OK);
            Assert.True(apiResponse.StatusCode == HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Authenticate_With_Correct_User_Data()
        {
            var data = new AuthenticateRequest() { Username = "lionel", Password = "123456" };
            var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
            var apiResponse = await _apiClient.PostAsync("http://localhost:4000/users/authenticate", content);

            _output.WriteLine($"Authenticate_With_Correct_User_Data - Status code: {apiResponse.StatusCode}");

            Assert.True(apiResponse.StatusCode == HttpStatusCode.OK);
        }

        [Theory]
        [InlineData("wrongusername", "654321")]
        public async Task Authenticate_With_Wrong_Inline_User_Data(string username, string password)
        {
            var data = new AuthenticateRequest() { Username = username, Password = password };
            var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
            var apiResponse = await _apiClient.PostAsync("http://localhost:4000/users/authenticate", content);

            _output.WriteLine($"Authenticate_With_Wrong_Inline_User_Data - Status code: {apiResponse.StatusCode}");

            Assert.True(apiResponse.StatusCode == HttpStatusCode.NotFound);
        }

        [Theory]
        [InlineData("lionel", "123456")]
        public async Task Authenticate_With_Correct_Inline_User_Data(string username, string password)
        {
            var data = new AuthenticateRequest() { Username = username, Password = password };
            var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
            var apiResponse = await _apiClient.PostAsync("http://localhost:4000/users/authenticate", content);

            _output.WriteLine($"Authenticate_With_Correct_Inline_User_Data - Status code: {apiResponse.StatusCode}");

            Assert.True(apiResponse.IsSuccessStatusCode);
        }

        [Fact]
        public async Task Try_To_Get_All_Users_Without_Authentication()
        {
            var apiResponse = await _apiClient.GetAsync("http://localhost:4000/users");

            _output.WriteLine($"Try_To_Get_All_Users_Without_Authentication - Status code: {apiResponse.StatusCode}");

            Assert.True(apiResponse.StatusCode == HttpStatusCode.Unauthorized);
        }

        [Theory]
        [InlineData("lionel", "123456", "http://localhost:4000/users")]
        public async Task Try_To_Get_All_Users_Authenticated(string username, string password, string url)
        {
            var data = new AuthenticateRequest() { Username = username, Password = password };
            var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
            var apiResponse = await _apiClient.PostAsync("http://localhost:4000/users/authenticate", content);

            _output.WriteLine($"Try_To_Get_All_Users_Authenticated - Status code: {apiResponse.StatusCode}");

            Assert.True(apiResponse.IsSuccessStatusCode);

            var response = await apiResponse.Content.ReadAsStringAsync();

            dynamic authData = JObject.Parse(response);
            string token =  (string)authData["token"];

            _output.WriteLine($"Try_To_Get_All_Users_Authenticated - Token: {token}");

            _apiClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            apiResponse = await _apiClient.GetAsync(url);

            Assert.True(apiResponse.IsSuccessStatusCode);
        }

        [Fact]
        public async Task Try_To_Create_A_New_User_With_All_Fields()
        {
            var data = new User() { FirstName = "Andrea", LastName = "Pirlo", Username = "andrea.pirlo", Password = "mypass" };
            var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
            var apiResponse = await _apiClient.PostAsync("http://localhost:4000/users/new", content);

            _output.WriteLine($"Try_To_Create_A_New_User_With_All_Fields - Status code: {apiResponse.StatusCode}");

            Assert.True(apiResponse.StatusCode == HttpStatusCode.OK);

            var response = await apiResponse.Content.ReadAsStringAsync();
            User user = JsonConvert.DeserializeObject<User>(response);

            _output.WriteLine($"Try_To_Create_A_New_User_With_All_Fields - User: {user}");
    
            Assert.IsType<User>(user);
        }

        [Fact]
        public async Task Try_To_Create_A_New_User_Without_All_Fields()
        {
            var data = new User() { Username = "andrea.pirlo", Password = "mypass" };
            var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
            var apiResponse = await _apiClient.PostAsync("http://localhost:4000/users/new", content);

            _output.WriteLine($"Try_To_Create_A_New_User_Without_All_Fields - Status code: {apiResponse.StatusCode}");

            Assert.True(apiResponse.StatusCode == HttpStatusCode.BadRequest);
        }
    }
}
