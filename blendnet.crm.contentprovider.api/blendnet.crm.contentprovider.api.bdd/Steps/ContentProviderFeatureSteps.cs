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

        struct ScenarioContenxtKeys
        {
            public const string CREATE_REQUEST_DATA = "CREATE_REQUEST_DATA";
            public const string CREATE_RESPONSE_DATA = "CREATE_RESPONSE_DATA";
            public const string READ_RESPONSE_DATA = "READ_RESPONSE_DATA";
            public const string UPDATE_REQUEST_DATA = "UPDATE_REQUEST_DATA";
            public const string UPDATE_RESPONSE_DATA = "UPDATE_RESPONSE_DATA";
            public const string READ_UPDATE_RESPONSE_DATA = "READ_UPDATE_RESPONSE_DATA";
        }


        public ContentProviderFeatureSteps(ScenarioContext scenarioContext, HttpClientDriver httpClientDriver)
        {
            _httpClientDriver = httpClientDriver;

            _scenarioContext = scenarioContext;
        }

        [When(@"I submit the request to read content")]
        public async Task WhenISubmitTheRequestToReadContent()
        {
            HttpClientResponse<string> response = _scenarioContext.Get<HttpClientResponse<string>>(ScenarioContenxtKeys.CREATE_RESPONSE_DATA);

            string url = $"{_apiBaseUrl}ContentProviders/{response.Data}";

            HttpClientResponse<ContentProviderDto> readResponse = await _httpClientDriver.Get<ContentProviderDto>(url);

            _scenarioContext.Set<HttpClientResponse<ContentProviderDto>>(readResponse, ScenarioContenxtKeys.READ_RESPONSE_DATA);
        }

        [Then(@"read content response should recieve success with created id")]
        public void ThenReadContentResponseShouldRecieveSuccess()
        {
            HttpClientResponse<ContentProviderDto> readResponse = _scenarioContext.Get<HttpClientResponse<ContentProviderDto>>(ScenarioContenxtKeys.READ_RESPONSE_DATA);
            
            HttpClientResponse<string> createResponse = _scenarioContext.Get<HttpClientResponse<string>>(ScenarioContenxtKeys.CREATE_RESPONSE_DATA);

            Assert.Equal(readResponse.RawMessage.StatusCode.ToString(), HttpStatusCode.OK.ToString());

            Assert.NotNull(readResponse.Data);

            Assert.Equal(createResponse.Data, readResponse.Data.Id.ToString());
        }

        [Given(@"admin is ""(.*)"" in the given data to create")]
        public void GivenAdminIsInTheGivenDataToCreate(bool p0)
        {
            string contentProviderName = Guid.NewGuid().ToString();

            ContentProviderDto createRequest = GetContentProviderDto(contentProviderName, p0);

            _scenarioContext.Set<ContentProviderDto>(createRequest, ScenarioContenxtKeys.CREATE_REQUEST_DATA);
        }

        [When(@"I submit the request to create")]
        public async Task WhenISubmitTheRequestToCreate()
        {
            string url = $"{_apiBaseUrl}ContentProviders";

            ContentProviderDto contentProviderRequest = _scenarioContext.Get<ContentProviderDto>(ScenarioContenxtKeys.CREATE_REQUEST_DATA);

            HttpClientResponse<string> response = await _httpClientDriver.Post<ContentProviderDto, string>(url, contentProviderRequest);

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
            ContentProviderDto updateRequest = _scenarioContext.Get<ContentProviderDto>(ScenarioContenxtKeys.UPDATE_REQUEST_DATA);

            string url = $"{_apiBaseUrl}ContentProviders/{updateRequest.Id.ToString()}";

            HttpClientResponse<ContentProviderDto> readResponse = await _httpClientDriver.Get<ContentProviderDto>(url);

            _scenarioContext.Set<HttpClientResponse<ContentProviderDto>>(readResponse, ScenarioContenxtKeys.READ_UPDATE_RESPONSE_DATA);
        }

        [When(@"I submit the request to update content (.*)")]
        public async Task WhenISubmitTheRequestToUpdateContent(string actionToPerform)
        {
            HttpClientResponse<string> response = _scenarioContext.Get<HttpClientResponse<string>>(ScenarioContenxtKeys.CREATE_RESPONSE_DATA);

            string url = string.Empty;

            if (actionToPerform.Equals("name"))
            {
                url = $"{_apiBaseUrl}ContentProviders/{response.Data}";
            }
            else if (actionToPerform.Equals("activation"))
            {
                url = $"{_apiBaseUrl}ContentProviders/{response.Data}/activate";
            }
            else if (actionToPerform.Equals("deactivation"))
            {
                url = $"{_apiBaseUrl}ContentProviders/{response.Data}/deactivate";
            }
           
            ContentProviderDto contentProviderRequest = _scenarioContext.Get<ContentProviderDto>(ScenarioContenxtKeys.CREATE_REQUEST_DATA);

            contentProviderRequest.Name = Guid.NewGuid().ToString();

            contentProviderRequest.Id = Guid.Parse(response.Data);

            _scenarioContext.Set<ContentProviderDto>(contentProviderRequest, ScenarioContenxtKeys.UPDATE_REQUEST_DATA);

            HttpClientResponse<string> updatedResponse = await _httpClientDriver.Post<ContentProviderDto, string>(url, contentProviderRequest);

            _scenarioContext.Set<HttpClientResponse<string>>(updatedResponse, ScenarioContenxtKeys.UPDATE_RESPONSE_DATA);

        }

        [Then(@"update content response should receive nocontent and updated (.*) value")]
        public void ThenUpdateContentResponseShouldRecieveNocontentAndUpdateValue(string actionValue)
        {
            ContentProviderDto updateRequest = _scenarioContext.Get<ContentProviderDto>(ScenarioContenxtKeys.UPDATE_REQUEST_DATA);

            HttpClientResponse<string> updateResponse = _scenarioContext.Get<HttpClientResponse<string>>(ScenarioContenxtKeys.UPDATE_RESPONSE_DATA);

            HttpClientResponse<ContentProviderDto> updateReadResponse = _scenarioContext.Get<HttpClientResponse<ContentProviderDto>>(ScenarioContenxtKeys.READ_UPDATE_RESPONSE_DATA);

            Assert.Equal(updateResponse.RawMessage.StatusCode.ToString(), HttpStatusCode.NoContent.ToString());

            Assert.NotNull(updateReadResponse.Data);

            if (actionValue.Equals("name"))
            {
                Assert.Equal(updateReadResponse.Data.Name.ToString(), updateRequest.Name);
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

            string contentProviderId = response.Data;

            string url = $"{_apiBaseUrl}ContentProviders/{contentProviderId}";

            HttpClientResponse<string> deleteResponse = await _httpClientDriver.Delete<string>(url);
        }

        [When(@"I submit the request to delete")]
        public async Task WhenISubmitTheRequestToDelete()
        {
            HttpClientResponse<string> response = _scenarioContext.Get<HttpClientResponse<string>>(ScenarioContenxtKeys.CREATE_RESPONSE_DATA);

            string url = $"{_apiBaseUrl}ContentProviders/{response.Data}";

            var deleteResponse = await _httpClientDriver.Delete<string>(url);
        }

        [Then(@"read content response should recieve notfound")]
        public void ThenReadContentResponseShouldRecieveNotfound()
        {
            HttpClientResponse<ContentProviderDto> readResponse = _scenarioContext.Get<HttpClientResponse<ContentProviderDto>>(ScenarioContenxtKeys.READ_RESPONSE_DATA);

            Assert.Equal(readResponse.RawMessage.StatusCode.ToString(), HttpStatusCode.NotFound.ToString());

            Assert.Null(readResponse.Data);
        }


        #region Data Generation Methods
        private ContentProviderDto GetContentProviderDto(string contentProviderName, bool addAdmistrator)
        {
            ContentProviderDto contentProvider = new ContentProviderDto();

            contentProvider.Name = contentProviderName;

            contentProvider.Address = new AddressDto()
            {
                City = "Delhi",
                Pin = "110089",
                State = "Delhi",
                StreetName = "Street 1",
                Town = "Delhi Town",
                MapLocation = new MapLocationDto() { Latitude = 19, Longitude = 20 }
            };

            if (addAdmistrator)
            {
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
            }

            return contentProvider;
        }

        #endregion

    }
}
