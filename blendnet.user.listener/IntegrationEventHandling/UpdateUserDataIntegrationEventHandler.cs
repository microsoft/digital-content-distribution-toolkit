using blendnet.common.dto;
using blendnet.common.dto.Common;
using blendnet.common.dto.Events;
using blendnet.common.dto.User;
using blendnet.common.infrastructure;
using blendnet.user.repository.Interfaces;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.Extensions.Options;

namespace blendnet.user.listener.IntegrationEventHandling
{
    /// <summary>
    /// Responsible of inserting and deleting the user commands for the given user phone number.
    /// Id excludes the current command id from delete and insert
    /// </summary>
    public class UpdateUserDataIntegrationEventHandler : IIntegrationEventHandler<UpdateUserDataIntegrationEvent>
    {
        private readonly ILogger _logger;

        private TelemetryClient _telemetryClient;

        private readonly UserAppSettings _appSettings;

        IUserRepository _userRepository;

        private IEventBus _eventBus;


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="tc"></param>
        /// <param name="optionsMonitor"></param>
        /// <param name="userRepository"></param>
        /// <param name="eventBus"></param>
        public UpdateUserDataIntegrationEventHandler(ILogger<UpdateUserDataIntegrationEventHandler> logger,
                                                     TelemetryClient tc,
                                                     IOptionsMonitor<UserAppSettings> optionsMonitor,
                                                     IUserRepository userRepository,
                                                     IEventBus eventBus)
        {
            _logger = logger;

            _telemetryClient = tc;

            _appSettings = optionsMonitor.CurrentValue;

            _userRepository = userRepository;

            _eventBus = eventBus;

        }

        /// <summary>
        /// Handler to update the commands of user. 
        /// Basically replace the user phone number with user id
        /// </summary>
        /// <param name="integrationEvent"></param>
        /// <returns></returns>
        public async Task Handle(UpdateUserDataIntegrationEvent integrationEvent)
        {
            try
            {
                using (_telemetryClient.StartOperation<RequestTelemetry>("UpdateUserDataIntegrationEventHandler.User.Handle"))
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
                        //Update user commands which are owned by the user
                        bool updatePerformed = await UpdateUserCommands(integrationEvent);

                        //in  case no data found, raise the completion event notifying no data found to update
                        if (!updatePerformed)
                        {
                            _logger.LogInformation($"No user command details found in database for user id: {integrationEvent.UserId}. command id: {integrationEvent.CommandId}");

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

                    _logger.LogInformation($"User command update is complete for user id: {integrationEvent.UserId} command id : {integrationEvent.CommandId}.");

                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }


        /// <summary>
        /// Update User Commands except the current command which is in progress
        /// Performs the delete and insert in the batch of 100 as max 100 are allowed in Azure Cosmos for transaction size
        /// </summary>
        /// <param name="userPhoneNumber"></param>
        /// <param name="userId"></param>
        /// <param name="continuationToken"></param>
        /// <returns></returns>
        private async Task<bool> UpdateUserCommands(UpdateUserDataIntegrationEvent integrationEvent)
        {
            bool loadData = true;

            string? continuationToken = null;

            bool updatePerformed = false;

            List<Guid> exludeCommandIds = new List<Guid> { integrationEvent.CommandId };

            while (loadData)
            {
                //Since the transction allow to update 100 records in one transaction.
                ResultData<UserCommand> userCommands = await _userRepository.GetCommands(integrationEvent.UserPhoneNumber,
                                                                                       exludeCommandIds,
                                                                                       continuationToken,
                                                                                       ApplicationConstants.Common.ALLOWED_TRANSACTION_BATCH_SIZE);

                if (userCommands != null &&
                    userCommands.Data != null &&
                    userCommands.Data.Count > 0)
                {
                    _logger.LogInformation($"Updating user commands: {integrationEvent.UserId} record count: {userCommands.Data.Count} token : {userCommands.ContinuationToken}");

                    //get the commands to be deleted
                    List<Guid> commandToDelete = userCommands.Data.Select(uc => uc.Id).ToList();

                    foreach (UserCommand userCommand in userCommands.Data)
                    {
                        //replace the user phone number with user id
                        userCommand.PhoneNumber = integrationEvent.UserId.ToString();
                        //mark the record as deleted
                        userCommand.IsUserDeleted = true;
                    }

                    //insert the commands with new user id in a transaction
                    await _userRepository.InsertCommands(integrationEvent.UserId.ToString(), userCommands.Data);

                    //delete the commands for the given number.
                    //Ideal would have been Insert and Delete in one transaction.
                    //But Cosmos does not allow transaction across partion key values.
                    await _userRepository.DeleteBatch(integrationEvent.UserPhoneNumber, commandToDelete);

                    continuationToken = userCommands.ContinuationToken;

                    //check if there are any more records
                    if (string.IsNullOrEmpty(userCommands.ContinuationToken))
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
            UserDataUpdateCompletedIntegrationEvent userDataUpdateCompletedIntegrationEvent =
                                    new UserDataUpdateCompletedIntegrationEvent()
                                    {
                                        UserId = integrationEvent.UserId,
                                        UserPhoneNumber = integrationEvent.UserPhoneNumber,
                                        CommandId = integrationEvent.CommandId,
                                        CompletionDateTime = DateTime.UtcNow,
                                        NoDataToOperate = noDataToOperate,
                                        IsFailed = isFailed,
                                        FailureReason = failureReason
                                    };

            await _eventBus.Publish(userDataUpdateCompletedIntegrationEvent);

        }

    }
}
