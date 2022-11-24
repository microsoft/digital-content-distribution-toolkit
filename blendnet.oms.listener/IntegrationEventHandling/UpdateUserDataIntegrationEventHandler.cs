// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using blendnet.common.dto;
using blendnet.common.dto.Common;
using blendnet.common.dto.Events;
using blendnet.common.dto.Oms;
using blendnet.common.infrastructure;
using blendnet.oms.repository.Interfaces;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blendnet.oms.listener.IntegrationEventHandling
{
    /// <summary>
    /// Event handler to Update User Order Data
    /// </summary>
    public class UpdateUserDataIntegrationEventHandler : IIntegrationEventHandler<UpdateUserDataIntegrationEvent>
    {
        private readonly ILogger _logger;

        private TelemetryClient _telemetryClient;

        private readonly OmsAppSettings _appSettings;

        IOMSRepository _omsRepository;

        private IEventBus _eventBus;


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="tc"></param>
        /// <param name="optionsMonitor"></param>
        /// <param name="eventsRepository"></param>
        /// <param name="eventBus"></param>
        public UpdateUserDataIntegrationEventHandler(ILogger<UpdateUserDataIntegrationEventHandler> logger,
                                                     TelemetryClient tc,
                                                     IOptionsMonitor<OmsAppSettings> optionsMonitor,
                                                     IOMSRepository eventsRepository,
                                                     IEventBus eventBus)
        {
            _logger = logger;

            _telemetryClient = tc;

            _appSettings = optionsMonitor.CurrentValue;

            _omsRepository = eventsRepository;

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
                using (_telemetryClient.StartOperation<RequestTelemetry>("UpdateUserDataIntegrationEventHandler.OMS.Handle"))
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
                        //Update orders which are owned by the user
                        bool updatePerformed = await UpdateUserOrders(integrationEvent);

                        //in  case no data found, raise the completion event notifying no data found to update
                        if (!updatePerformed)
                        {
                            _logger.LogInformation($"No order details found in database for user id: {integrationEvent.UserId}. command id: {integrationEvent.CommandId}");

                            await RaiseCompletionEvent(integrationEvent, true);

                            return;
                        }

                        //raise service bus completion event so that request status could be updated by calling listener
                        await RaiseCompletionEvent(integrationEvent, false);
                    }
                    catch (Exception ex)
                    {
                        await RaiseCompletionEvent(integrationEvent, false, true, ex.Message);

                        throw;
                    }

                    _logger.LogInformation($"Order update is complete for user id: {integrationEvent.UserId} command id : {integrationEvent.CommandId}.");

                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }

       
        /// <summary>
        /// Update User Orders
        /// Performs the delete and insert in the batch of 100 as max 100 are allowed in Azure Cosmos for transaction size
        /// </summary>
        /// <param name="userPhoneNumber"></param>
        /// <param name="userId"></param>
        /// <param name="continuationToken"></param>
        /// <returns></returns>
        private async Task<bool> UpdateUserOrders(UpdateUserDataIntegrationEvent integrationEvent)
        {
            bool loadData = true;

            string continuationToken = null;

            bool updatePerformed = false;

            while (loadData)
            {
                //Since the transction allow to update 100 records in one transaction.
                ResultData<Order> userOrders = await _omsRepository.GetOrdersByPhoneNumber(integrationEvent.UserPhoneNumber,
                                                                                                             continuationToken,
                                                                                                             ApplicationConstants.Common.ALLOWED_TRANSACTION_BATCH_SIZE);

                if (userOrders != null &&
                    userOrders.Data != null &&
                    userOrders.Data.Count > 0)
                {
                    _logger.LogInformation($"Updating user data: {integrationEvent.UserId} record count: {userOrders.Data.Count} token : {userOrders.ContinuationToken}");

                    //get the orders to be deleted
                    List<Guid> orderIdsToDelete = userOrders.Data.Select(uo => uo.Id.Value).ToList();

                    foreach (Order userOrder in userOrders.Data)
                    {
                        //replace the user phone number with user id
                        userOrder.PhoneNumber = integrationEvent.UserId.ToString();
                        //mark the record as deleted
                        userOrder.IsUserDeleted = true;
                    }

                    //insert the users with new user id in a transaction
                    await _omsRepository.InsertOrders(integrationEvent.UserId.ToString(), userOrders.Data);

                    //delete the events for the given number.
                    //Ideal would have been Insert and Delete in one transaction. But Cosmos does not allow transaction across partion key values.
                    await _omsRepository.DeleteOrders(integrationEvent.UserPhoneNumber, orderIdsToDelete);

                    continuationToken = userOrders.ContinuationToken;

                    //check if there are any more records
                    if (string.IsNullOrEmpty(userOrders.ContinuationToken))
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
            OrderDataUpdateCompletedIntegrationEvent orderDataUpdateCompletedIntegrationEvent =
                                    new OrderDataUpdateCompletedIntegrationEvent()
                                    {
                                        UserId = integrationEvent.UserId,
                                        UserPhoneNumber = integrationEvent.UserPhoneNumber,
                                        CommandId = integrationEvent.CommandId,
                                        CompletionDateTime = DateTime.UtcNow,
                                        NoDataToOperate = noDataToOperate,
                                        IsFailed = isFailed,
                                        FailureReason = failureReason
                                    };

            await _eventBus.Publish(orderDataUpdateCompletedIntegrationEvent);

        }

    }

}
