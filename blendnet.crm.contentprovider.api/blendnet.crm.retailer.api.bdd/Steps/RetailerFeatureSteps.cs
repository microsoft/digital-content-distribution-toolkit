using blendnet.crm.common.dto;
using blendnet.crm.retailer.api.bdd.Drivers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using TechTalk.SpecFlow;
using Xunit;

namespace blendnet.crm.retailer.api.bdd.Steps
{
    [Binding]
    public class RetailerFeatureSteps
    {
        private HttpClientDriver _httpClientDriver;

        private string _apiBaseUrl = "https://localhost:44335/api/v1/";

        ScenarioContext _scenarioContext;

        struct ScenarioContenxtKeys
        {
            public const string CREATE_REQUEST_DATA = "CREATE_REQUEST_DATA";
            public const string CREATE_RESPONSE_DATA = "CREATE_RESPONSE_DATA";
            public const string READ_RESPONSE_DATA = "READ_RESPONSE_DATA";
            public const string UPDATE_REQUEST_DATA = "UPDATE_REQUEST_DATA";
            public const string UPDATE_RESPONSE_DATA = "UPDATE_RESPONSE_DATA";
            public const string READ_UPDATE_RESPONSE_DATA = "READ_UPDATE_RESPONSE_DATA";
            public const string CREATE_HUB_REQUEST_DATA = "CREATE_HUB_REQUEST_DATA";
            public const string CREATE_HUB_RESPONSE_DATA = "CREATE_HUB_RESPONSE_DATA";
            public const string UPDATE_HUB_REQUEST_DATA = "UPDATE_HUB_REQUEST_DATA";
            public const string UPDATE_HUB_RESPONSE_DATA = "UPDATE_HUB_RESPONSE_DATA";
        } 

        public RetailerFeatureSteps(ScenarioContext scenarioContext, HttpClientDriver httpClientDriver)
        {
            _httpClientDriver = httpClientDriver;

            _scenarioContext = scenarioContext;
        }
    
        [When(@"I submit the request to read content")]
        public async Task WhenISubmitTheRequestToReadContent()
        {
            HttpClientResponse<string> response = _scenarioContext.Get<HttpClientResponse<string>>(ScenarioContenxtKeys.CREATE_RESPONSE_DATA);

            string url = $"{_apiBaseUrl}Retailer/{response.Data}";

            HttpClientResponse<RetailerDto> readResponse = await _httpClientDriver.Get<RetailerDto>(url);

            _scenarioContext.Set<HttpClientResponse<RetailerDto>>(readResponse, ScenarioContenxtKeys.READ_RESPONSE_DATA);
        }

        [Then(@"read content response should recieve success with created id")]
        public void ThenReadContentResponseShouldRecieveSuccess()
        {
            HttpClientResponse<RetailerDto> readResponse = _scenarioContext.Get<HttpClientResponse<RetailerDto>>(ScenarioContenxtKeys.READ_RESPONSE_DATA);

            HttpClientResponse<string> createResponse = _scenarioContext.Get<HttpClientResponse<string>>(ScenarioContenxtKeys.CREATE_RESPONSE_DATA);

            Assert.Equal(readResponse.RawMessage.StatusCode.ToString(), HttpStatusCode.OK.ToString());

            Assert.NotNull(readResponse.Data);

            Assert.Equal(createResponse.Data, readResponse.Data.Id.ToString());
        }

        [Given(@"hub is ""(.*)"" in the given data to create")]
        public void GivenHubIsInTheGivenDataToCreate(bool p0)
        {
            string retailerName = Guid.NewGuid().ToString();

            RetailerDto createRequest = GetRetailerDto(retailerName, p0);

            _scenarioContext.Set<RetailerDto>(createRequest, ScenarioContenxtKeys.CREATE_REQUEST_DATA);
        }

        [When(@"I submit the request to create")]
        public async Task WhenISubmitTheRequestToCreate()
        {
            string url = $"{_apiBaseUrl}Retailer";

            RetailerDto retailerRequest = _scenarioContext.Get<RetailerDto>(ScenarioContenxtKeys.CREATE_REQUEST_DATA);

            HttpClientResponse<string> response = await _httpClientDriver.Post<RetailerDto, string>(url, retailerRequest);

            _scenarioContext.Set<HttpClientResponse<string>>(response, ScenarioContenxtKeys.CREATE_RESPONSE_DATA);
        }

        [Then(@"create response should recieve created")]
        public void ThenCreateResponseShouldRecieveCreated()
        {
            HttpClientResponse<string> response = _scenarioContext.Get<HttpClientResponse<string>>(ScenarioContenxtKeys.CREATE_RESPONSE_DATA);

            Assert.Equal(response.RawMessage.StatusCode.ToString(), HttpStatusCode.Created.ToString());

            Assert.NotNull(response.Data);
        }

        [When(@"I submit the request to read updated content for (.*)")]
        public async Task WhenISubmitTheRequestToReadUpdatedContent(string action)
        {
            RetailerDto updateRequest = _scenarioContext.Get<RetailerDto>(ScenarioContenxtKeys.UPDATE_REQUEST_DATA);

            string url = $"{_apiBaseUrl}Retailer/{updateRequest.Id.ToString()}";

            HttpClientResponse<RetailerDto> readResponse = await _httpClientDriver.Get<RetailerDto>(url);

            _scenarioContext.Set<HttpClientResponse<RetailerDto>>(readResponse, ScenarioContenxtKeys.READ_UPDATE_RESPONSE_DATA);
        }

        [When(@"I submit the request to update content (.*)")]
        public async Task WhenISubmitTheRequestToUpdateContent(string actionToPerform)
        {
            HttpClientResponse<string> response = _scenarioContext.Get<HttpClientResponse<string>>(ScenarioContenxtKeys.CREATE_RESPONSE_DATA);

            string url = string.Empty;

            if (actionToPerform.Equals("name"))
            {
                url = $"{_apiBaseUrl}Retailer/{response.Data}";
            }
            else if (actionToPerform.Equals("activation"))
            {
                url = $"{_apiBaseUrl}Retailer/{response.Data}/activate";
            }
            else if (actionToPerform.Equals("deactivation"))
            {
                url = $"{_apiBaseUrl}Retailer/{response.Data}/deactivate";
            }

            RetailerDto retailerRequest = _scenarioContext.Get<RetailerDto>(ScenarioContenxtKeys.CREATE_REQUEST_DATA);

            retailerRequest.FirstName = Guid.NewGuid().ToString();

            retailerRequest.Id = Guid.Parse(response.Data);

            _scenarioContext.Set<RetailerDto>(retailerRequest, ScenarioContenxtKeys.UPDATE_REQUEST_DATA);

            HttpClientResponse<string> updatedResponse = await _httpClientDriver.Post<RetailerDto, string>(url, retailerRequest);

            _scenarioContext.Set<HttpClientResponse<string>>(updatedResponse, ScenarioContenxtKeys.UPDATE_RESPONSE_DATA);

        }

        [Then(@"update content response should receive nocontent and updated (.*) value")]
        public void ThenUpdateContentResponseShouldRecieveNocontentAndUpdateValue(string actionValue)
        {
            RetailerDto updateRequest = _scenarioContext.Get<RetailerDto>(ScenarioContenxtKeys.UPDATE_REQUEST_DATA);

            HttpClientResponse<string> updateResponse = _scenarioContext.Get<HttpClientResponse<string>>(ScenarioContenxtKeys.UPDATE_RESPONSE_DATA);

            HttpClientResponse<RetailerDto> updateReadResponse = _scenarioContext.Get<HttpClientResponse<RetailerDto>>(ScenarioContenxtKeys.READ_UPDATE_RESPONSE_DATA);

            Assert.Equal(updateResponse.RawMessage.StatusCode.ToString(), HttpStatusCode.NoContent.ToString());

            Assert.NotNull(updateReadResponse.Data);

            if (actionValue.Equals("name"))
            {
                Assert.Equal(updateReadResponse.Data.FirstName.ToString(), updateRequest.FirstName);
            }
            else if (actionValue.Equals("activated"))
            {
                Assert.True(updateReadResponse.Data.IsActive);
                Assert.NotNull(updateReadResponse.Data.ActivationDate);
            }
            else if (actionValue.Equals("deactivated"))
            {
                Assert.False(updateReadResponse.Data.IsActive);
                Assert.NotNull(updateReadResponse.Data.DeactivationDate);
            }
        }

        [Then(@"I should delete the created record")]
        public async Task ThenIShouldDeleteTheCreatedRecord()
        {
            HttpClientResponse<string> response = _scenarioContext.Get<HttpClientResponse<string>>(ScenarioContenxtKeys.CREATE_RESPONSE_DATA);

            string retailerId = response.Data;

            string url = $"{_apiBaseUrl}Retailer/{retailerId}";

            HttpClientResponse<string> deleteResponse = await _httpClientDriver.Delete<string>(url);
        }

        [When(@"I submit the request to delete")]
        public async Task WhenISubmitTheRequestToDelete()
        {
            HttpClientResponse<string> response = _scenarioContext.Get<HttpClientResponse<string>>(ScenarioContenxtKeys.CREATE_RESPONSE_DATA);

            string url = $"{_apiBaseUrl}Retailer/{response.Data}";

            var deleteResponse = await _httpClientDriver.Delete<string>(url);
        }

        [Then(@"read content response should recieve notfound")]
        public void ThenReadContentResponseShouldRecieveNotfound()
        {
            HttpClientResponse<RetailerDto> readResponse = _scenarioContext.Get<HttpClientResponse<RetailerDto>>(ScenarioContenxtKeys.READ_RESPONSE_DATA);

            Assert.Equal(readResponse.RawMessage.StatusCode.ToString(), HttpStatusCode.NotFound.ToString());

            Assert.Null(readResponse.Data);
        }

        #region Data Generation Methods
        private RetailerDto GetRetailerDto(string retailerName, bool addHub)
        {
            RetailerDto retailer = new RetailerDto();

            retailer.FirstName = retailerName;
            retailer.LastName = retailerName;

            retailer.Address = new AddressDto()
            {
                City = "Delhi",
                Pin = "110089",
                State = "Delhi",
                StreetName = "Street 1",
                Town = "Delhi Town",
                MapLocation = new MapLocationDto() { Latitude = 19, Longitude = 20 }
            };

            if (addHub)
            {
                retailer.Hubs = new List<HubDto>();

                HubDto hub = GetHubDto();

                retailer.Hubs.Add(hub);
            }

            return retailer;
        }

        private HubDto GetHubDto()
        {
            HubDto hub = new HubDto()
            {
                Name = "SKML_Store-1",
                Address = new AddressDto
                {
                    City = "Bengaleru",
                    Pin = "7878777",
                    State = "KA",
                    StreetName = "ST",
                    MapLocation = new MapLocationDto { Latitude = 89, Longitude = 90 }
                }
            };

            return hub;
        }
        #endregion

    }
}