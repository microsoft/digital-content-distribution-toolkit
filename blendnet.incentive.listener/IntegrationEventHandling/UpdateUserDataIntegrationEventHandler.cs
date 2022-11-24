// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using blendnet.common.dto;
using blendnet.common.dto.Common;
using blendnet.common.dto.Events;
using blendnet.common.dto.Incentive;
using blendnet.common.infrastructure;
using blendnet.incentive.repository.Interfaces;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blendnet.incentive.listener.IntegrationEventHandling
{
    /// <summary>
    /// Event handler to Update User Incentive Data
    /// </summary>
    public class UpdateUserDataIntegrationEventHandler : IIntegrationEventHandler<UpdateUserDataIntegrationEvent>
    {
        private readonly ILogger _logger;

        private TelemetryClient _telemetryClient;

        private readonly IncentiveAppSettings _appSettings;

        IEventRepository _eventsRepository;

        private IEventBus _eventBus;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="tc"></param>
        /// <param name="blobClientFactory"></param>
        /// <param name="optionsMonitor"></param>
        /// <param name="eventsRepository"></param>
        /// <param name="eventBus"></param>
        public UpdateUserDataIntegrationEventHandler(ILogger<UpdateUserDataIntegrationEventHandler> logger,
                                                     TelemetryClient tc,
                                                     IOptionsMonitor<IncentiveAppSettings> optionsMonitor,
                                                     IEventRepository eventsRepository,
                                                     IEventBus eventBus)
        {
            _logger = logger;

            _telemetryClient = tc;

            _appSettings = optionsMonitor.CurrentValue;

            _eventsRepository = eventsRepository;

            _eventBus = eventBus;

        }

        /// <summary>
        /// Handler to update the data of user. 
        /// Basically replace the user phone number with user id
        /// </summary>
        /// <param name="integrationEvent"></param>
        /// <returns></returns>
        public async Task Handle(UpdateUserDataIntegrationEvent integrationEvent)
        {
            try
            {
                using (_telemetryClient.StartOperation<RequestTelemetry>("UpdateUserDataIntegrationEventHandler.Incentive.Handle"))
                {
                    if (integrationEvent.CommandId == Guid.Empty ||
                        integrationEvent.UserId == Guid.Empty ||
                        string.IsNullOrEmpty(integrationEvent.UserPhoneNumber))
                    {
                        _logger.LogWarning($"No user details found in integration event. Pass correct data to integation event");

                        return;
                    }

                    _logger.LogInformation($"Message Recieved for user id: {integrationEvent.UserId} command id: {integrationEvent.CommandId}");

                    try
                    {
                        //Update events which are owned by the user
                        bool  updatePerformed = await UpdateUserData(integrationEvent);

                        //in  case no data found, raise the completion event notifying no data found to update
                        if (!updatePerformed)
                        {
                            _logger.LogInformation($"No incentive event details found in database for user id: {integrationEvent.UserId}. command id: {integrationEvent.CommandId}");

                            await RaiseCompletionEvent(integrationEvent, true);

                            return;
                        }

                        //raise service bus completion event so that request status could be updated by calling listener
                        await RaiseCompletionEvent(integrationEvent, false);
                    }
                    catch (Exception ex)
                    {
                        await RaiseCompletionEvent(integrationEvent,false,true,ex.Message);

                        throw;
                    }
                    
                    _logger.LogInformation($"Incentive event update is complete for user id: {integrationEvent.UserId} command id : {integrationEvent.CommandId}.");

                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }

        /// <summary>
        /// Update User And Retailer Events
        /// </summary>
        /// <param name="userPhoneNumber"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        private async Task<bool> UpdateUserData(UpdateUserDataIntegrationEvent integrationEvent)
        {
            bool userEventsUpdated = await UpdateUserEvents(integrationEvent.UserPhoneNumber, integrationEvent.UserId);

            if (!userEventsUpdated)
            {
                _logger.LogInformation($"No User event details found in database for user id: {integrationEvent.UserId}. command id: {integrationEvent.CommandId}");
            }

            bool retailerEventsUpdated = await UpdateRetailerEvents(integrationEvent.UserPhoneNumber, integrationEvent.UserId);

            if (!retailerEventsUpdated)
            {
                _logger.LogInformation($"No Retailer event details found in database for user id: {integrationEvent.UserId}. command id: {integrationEvent.CommandId}");
            }

            if (!userEventsUpdated && !retailerEventsUpdated)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Update User Events
        /// Performs the delete and insert in the batch of 100 as max 100 are allowed in Azure Cosmos for transaction size
        /// </summary>
        /// <param name="userPhoneNumber"></param>
        /// <param name="userId"></param>
        /// <param name="continuationToken"></param>
        /// <returns></returns>
        private async Task<bool> UpdateUserEvents(string userPhoneNumber, Guid userId)
        {
            bool loadData = true;

            string continuationToken = null;

            bool updatePerformed = false;

            while (loadData)
            {
                //Since the transction allow to update 100 records in one transaction.
                ResultData<IncentiveEvent> userIncentiveEvents = await _eventsRepository.GetUserEvents( userPhoneNumber, 
                                                                                                        continuationToken, 
                                                                                                        ApplicationConstants.Common.ALLOWED_TRANSACTION_BATCH_SIZE);

                if (userIncentiveEvents != null &&
                    userIncentiveEvents.Data != null &&
                    userIncentiveEvents.Data.Count > 0)
                {
                    _logger.LogInformation($"Updating incentive data: {userId} record count: {userIncentiveEvents.Data.Count} token : {userIncentiveEvents.ContinuationToken}");

                    //get the events to be deleted
                    List<Guid> eventsIdsToDelete = userIncentiveEvents.Data.Select(ue => ue.EventId.Value).ToList();

                    foreach (IncentiveEvent incentiveEvent in userIncentiveEvents.Data)
                    {
                        //replace the user phone number with user id
                        incentiveEvent.EventCreatedFor = userId.ToString();
                        //mark the record as deleted
                        incentiveEvent.IsUserDeleted = true;
                    }

                    //insert the users with new user id in a transaction
                    await _eventsRepository.InsertIncentiveEvents(userId.ToString(), userIncentiveEvents.Data);

                    //delete the events for the given number.
                    //Ideal would have been Insert and Delete in one transaction. But Cosmos does not allow transaction across partion key values.
                    await _eventsRepository.DeleteIncentiveEvents(userPhoneNumber, eventsIdsToDelete);

                    continuationToken = userIncentiveEvents.ContinuationToken;

                    //check if there are any more records
                    if (string.IsNullOrEmpty(userIncentiveEvents.ContinuationToken))
                    {
                        loadData = false;
                    }

                    updatePerformed = true;
                }
                else
                {
                    loadData = false;
                }
            }

            return updatePerformed;
        }


        /// <summary>
        /// Update Retailer Events in the batch of 100
        /// </summary>
        /// <param name="userPhoneNumber"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        private async Task<bool> UpdateRetailerEvents(string userPhoneNumber, Guid userId)
        {
            bool loadData = true;

            string continuationToken = null;

            bool updatePerformed = false;

            while (loadData)
            {
                //Since the transction allow to update 100 records in one transaction.
                ResultData<IncentiveEvent> retailerEvents = await _eventsRepository.GetRetailerEventsForUser(userPhoneNumber,
                                                                                                      continuationToken,
                                                                                                      ApplicationConstants.Common.ALLOWED_TRANSACTION_BATCH_SIZE);

                if (retailerEvents != null && 
                    retailerEvents.Data != null && 
                    retailerEvents.Data.Count > 0)
                {
                    _logger.LogInformation($"Updating retailer incentive data for : {userId} record count: {retailerEvents.Data.Count} token : {retailerEvents.ContinuationToken}");

                    //Replace the phone number with user id
                    foreach (IncentiveEvent incentiveEvent in retailerEvents.Data)
                    {
                        Property foundProperty = incentiveEvent.Properties.Where(p => (p.Name == ApplicationConstants.IncentiveEventAdditionalPropertyKeys.UserPhone &&
                                                                                 p.Value == userPhoneNumber)).First();

                        foundProperty.Value = userId.ToString();

                        //mark the record as deleted
                        incentiveEvent.IsUserDeleted = true;
                    }

                    //Update the records for each unique event created for / retailer
                    List<string> uniqueEventCreatedFor = retailerEvents.Data.Select(ie => ie.EventCreatedFor).Distinct().ToList();

                    _logger.LogInformation($"Updating retailer incentive data for : {userId} unique eventCreatedFor count : {uniqueEventCreatedFor.Count} ");

                    //update the Azure cosmos for each event created for / retailer
                    foreach (string eventCreatedFor in uniqueEventCreatedFor)
                    {
                        List<IncentiveEvent> recordsToUpdate = retailerEvents.Data.Where(re => re.EventCreatedFor.Equals(eventCreatedFor)).ToList();

                        //insert the users with new user id in a transaction
                        await _eventsRepository.UpdateIncentiveEvents(eventCreatedFor, recordsToUpdate);

                        _logger.LogInformation($"Updated retailer incentive data for : {userId} eventCreatedFor : {eventCreatedFor} ");
                    }

                    continuationToken = retailerEvents.ContinuationToken;

                    if (string.IsNullOrEmpty(retailerEvents.ContinuationToken))
                    {
                        loadData = false;
                    }

                    updatePerformed = true;
                }
                else
                {
                    loadData = false;
                }

            }

            return updatePerformed;
        }


        /// <summary>
        /// Raise Completion Event
        /// </summary>
        /// <param name="integrationEvent"></param>
        /// <param name="noDataToOperate"></param>
        /// <returns></returns>
        private async Task RaiseCompletionEvent(UpdateUserDataIntegrationEvent integrationEvent,
                                                bool noDataToOperate,
                                                bool isFailed = false,
                                                string failureReason = "")
        {
            ////publish the complted event
            IncentiveDataUpdateCompletedIntegrationEvent incentiveDataUpdateCompletedIntegrationEvent =
                                    new IncentiveDataUpdateCompletedIntegrationEvent()
                                    {
                                        UserId = integrationEvent.UserId,
                                        UserPhoneNumber = integrationEvent.UserPhoneNumber,
                                        CommandId = integrationEvent.CommandId,
                                        CompletionDateTime = DateTime.UtcNow,
                                        NoDataToOperate = noDataToOperate,
                                        IsFailed = isFailed,
                                        FailureReason = failureReason
                                    };

            await _eventBus.Publish(incentiveDataUpdateCompletedIntegrationEvent);

        }

    }
}
