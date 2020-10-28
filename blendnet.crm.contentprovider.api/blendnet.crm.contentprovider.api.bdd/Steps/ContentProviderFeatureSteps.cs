using blendnet.crm.common.dto;
using blendnet.crm.contentprovider.api.bdd.Drivers;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using TechTalk.SpecFlow;
using Xunit;

namespace blendnet.crm.contentprovider.api.bdd.Steps
{
    [Binding]
    public class ContentProviderFeatureSteps
    {
        private HttpClientDriver _httpClientDriver;

        private string _apiBaseUrl = "https://localhost:44335/api/v1/";

        ScenarioContext _scenarioContext;

        public ContentProviderFeatureSteps(ScenarioContext scenarioContext, HttpClientDriver httpClientDriver)
        {
            _httpClientDriver = httpClientDriver;

            _scenarioContext = scenarioContext;
        }

        [Given(@"the created content provider")]
        public void GivenTheCreatedContentProvider()
        {

        }
        
        [When(@"I submit the request to read")]
        public async Task WhenISubmitTheRequestToRead()
        {
            string url = $"{_apiBaseUrl}ContentProviders";

            HttpClientResponse<List<ContentProviderDto>> response = await _httpClientDriver.Get<List<ContentProviderDto>>(url);
            
            _scenarioContext.Set<HttpClientResponse<List<ContentProviderDto>>>(response, _scenarioContext.ScenarioInfo.Title);
        }
        
        [Then(@"response should recieve success")]
        public void ThenResponseShouldRecieveSuccess()
        {
            HttpClientResponse<List<ContentProviderDto>> response = _scenarioContext.Get<HttpClientResponse<List<ContentProviderDto>>>(_scenarioContext.ScenarioInfo.Title);

            Assert.Equal(response.RawMessage.StatusCode.ToString(), HttpStatusCode.OK.ToString());

            Assert.NotNull(response.Data);

        }


        [Given(@"the data to create content provider")]
        public void GivenTheDataToCreateContentProvider()
        {
            ContentProviderDto contentProvider = new ContentProviderDto();

            contentProvider.Name = "Eros International";
            contentProvider.Address = new AddressDto() { 
                City = "Heaven", 
                Pin = "110089",
                State = "Parlok",
                StreetName = "Seeri",
                Town = "Trishankhu",
                MapLocation = new MapLocationDto() { Latitude = 19 , Longitude = 20 } };

            contentProvider.ContentAdministrators = new List<ContentAdministratorDto>();

            ContentAdministratorDto contentAdministrator = new ContentAdministratorDto()
            {
                Email = "eros@hotmail.com",
                FirstName = "Eros",
                LastName = "Admin",
                DateOfBirth = DateTime.Now,
                Mobile = 9999999999,
                Address = new AddressDto
                {
                    City = "Bengaleru",
                    Pin = "7878777",
                    State = "KA",
                    StreetName = "ST",
                    MapLocation = new MapLocationDto { Latitude = 89, Longitude = 90 }
                }
            };

            contentProvider.ContentAdministrators.Add(contentAdministrator);

            _scenarioContext.Set<ContentProviderDto>(contentProvider, $"{_scenarioContext.ScenarioInfo.Title}_request");
        }

        [When(@"I submit the request to create")]
        public async Task WhenISubmitTheRequestToCreate()
        {
            string url = $"{_apiBaseUrl}ContentProviders";

            ContentProviderDto contentProviderRequest = _scenarioContext.Get<ContentProviderDto>($"{_scenarioContext.ScenarioInfo.Title}_request");

            HttpClientResponse<string> response = await _httpClientDriver.Post<ContentProviderDto, string>(url, contentProviderRequest);

            _scenarioContext.Set<HttpClientResponse<string>>(response, $"{_scenarioContext.ScenarioInfo.Title}_response");
        }

        [Then(@"response should recieve created")]
        public void ThenResponseShouldRecieveCreated()
        {
            HttpClientResponse<string> response = _scenarioContext.Get<HttpClientResponse<string>>($"{_scenarioContext.ScenarioInfo.Title}_response");

            Assert.Equal(response.RawMessage.StatusCode.ToString(), HttpStatusCode.Created.ToString());

            Assert.NotNull(response.Data);
        }
    }
}
