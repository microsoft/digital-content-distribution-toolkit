using blendnet.crm.common.dto.Integration;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Management;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace blendnet.crm.common.api.ServiceBus
{
    /// <summary>
    /// Register this class as singleton
    /// </summary>
    public class EventServiceBus : IEventBus
    {
        private ITopicClient _topicClient;

        private ISubscriptionClient _subscriptionClient;

        private EventBusConnectionData _eventBusConnectionData;

        private Dictionary<string,Tuple<Type, Type>> _subscribers;

        private readonly ILogger _logger;

        private IServiceProvider _serviceProvider;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventBusConnectionData"></param>
        public EventServiceBus( EventBusConnectionData eventBusConnectionData, 
                                ILogger<EventServiceBus> logger, 
                                IServiceProvider serviceProvider)
        {
            _subscribers = new Dictionary<string, Tuple<Type,Type>>();

            _logger = logger;

            _serviceProvider = serviceProvider;

            _eventBusConnectionData = eventBusConnectionData;
                        
            _topicClient = new TopicClient(_eventBusConnectionData.ServiceBusConnectionString, _eventBusConnectionData.TopicName);
            
            if (!string.IsNullOrEmpty(_eventBusConnectionData.SubscriptionName))
            {
                _subscriptionClient = new  SubscriptionClient(_eventBusConnectionData.ServiceBusConnectionString, _eventBusConnectionData.TopicName, _eventBusConnectionData.SubscriptionName);

                RemoveRule(RuleDescription.DefaultRuleName);

                RegisterOnMessageHandlerAndReceiveMessages();
            }
        }

        /// <summary>
        /// Publish the event to Azure Service Bus.
        /// </summary>
        /// <param name="integrationEvent"></param>
        /// <returns></returns>
        public async Task Publish(IntegrationEvent integrationEvent)
        {
            var eventName = integrationEvent.EventName;

            var jsonMessage = System.Text.Json.JsonSerializer.Serialize(integrationEvent, integrationEvent.GetType());

            var body = Encoding.UTF8.GetBytes(jsonMessage);
           
            var message = new Message
            {
                MessageId = integrationEvent.Id.ToString(),
                Body = body,
                Label = eventName
            };

            await _topicClient.SendAsync(message);
        }

        /// <summary>
        /// Registers the event handler for the given event.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TH"></typeparam>
        public void Subscribe<T, TH>()
           where T : IntegrationEvent
           where TH : IIntegrationEventHandler<T>
        {
            string eventName = typeof(T).Name;

            if (_subscribers.ContainsKey(eventName))
            {
                throw new InvalidOperationException($"Event {eventName} is already registered with another handler.");
            };

            //add the rule to the collection
            Tuple<Type, Type> eventTypes = new Tuple<Type, Type>(typeof(T), typeof(TH));

            _subscribers.Add(eventName, eventTypes);

            var rules = _subscriptionClient.GetRulesAsync().GetAwaiter().GetResult();

            var existingRule = rules.Where(r => r.Name.Equals(eventName)).FirstOrDefault();

            if(existingRule == null)
            {
                //Add the rule to service bus subscription
                _subscriptionClient.AddRuleAsync(new RuleDescription
                {
                    Filter = new CorrelationFilter { Label = eventName },
                    Name = eventName
                }).GetAwaiter().GetResult();
            }
        }

        /// <summary>
        /// Unsubscribe the subscribers
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TH"></typeparam>
        public void Unsubscribe<T, TH>()
        where T : IntegrationEvent
        where TH : IIntegrationEventHandler<T>
        {
            string eventName = typeof(T).Name;

            _subscribers.Remove(eventName);

            RemoveRule(eventName);

        }

        /// <summary>
        /// Register Subscriber
        /// </summary>
        private void RegisterOnMessageHandlerAndReceiveMessages()
        {
            // Configure the message handler options in terms of exception handling, number of concurrent messages to deliver, etc.
            var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
            {
                // Maximum number of concurrent calls to the callback ProcessMessagesAsync(), set to 1 for simplicity.
                // Set it according to how many messages the application wants to process in parallel.
                MaxConcurrentCalls = _eventBusConnectionData.MaxConcurrentCalls,

                // Indicates whether the message pump should automatically complete the messages after returning from user callback.
                // False below indicates the complete operation is handled by the user callback as in ProcessMessagesAsync().
                AutoComplete = false
            };

            // Register the function that processes messages.
            _subscriptionClient.RegisterMessageHandler(ProcessMessagesAsync, messageHandlerOptions);
        }

        /// <summary>
        /// Gets invoked on the subscription
        /// </summary>
        /// <param name="message"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        private async Task ProcessMessagesAsync(Message message, CancellationToken token)
        {
            var eventName = $"{message.Label}";

            var messageData = Encoding.UTF8.GetString(message.Body);

            if (_subscribers.ContainsKey(eventName))
            {
                var eventTypeData = _subscribers[eventName];

                Type eventType = eventTypeData.Item1;

                Type subscriber =  eventTypeData.Item2;

                var integrationEvent = System.Text.Json.JsonSerializer.Deserialize(messageData, eventType);
                
                var registeredHandler = _serviceProvider.GetService(subscriber);

                if (registeredHandler != null)
                {
                    //Get the actual event handler
                    var concreteType = typeof(IIntegrationEventHandler<>).MakeGenericType(eventType);

                    //call the handle method
                    await (Task)concreteType.GetMethod("Handle").Invoke(registeredHandler, new object[] { integrationEvent });

                    // Complete the message so that it is not received again.
                    // This can be done only if the subscriptionClient is created in ReceiveMode.PeekLock mode (which is the default).
                    await _subscriptionClient.CompleteAsync(message.SystemProperties.LockToken);
                }
                else
                {
                    _logger.LogInformation($"No integration event handler found for event {eventName} and eventhandler {subscriber.Name}.");
                }
            }
            else
            {
                _logger.LogInformation($"No subscriber found for {eventName}.");
            }
            
            // Note: Use the cancellationToken passed as necessary to determine if the subscriptionClient has already been closed.
            // If subscriptionClient has already been closed, you can choose to not call CompleteAsync() or AbandonAsync() etc.
            // to avoid unnecessary exceptions.
        }

        /// <summary>
        /// Use this handler to examine the exceptions received
        /// </summary>
        /// <param name="exceptionReceivedEventArgs"></param>
        /// <returns></returns>
        private Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            _logger.LogError(exceptionReceivedEventArgs.Exception, exceptionReceivedEventArgs.Exception.Message);

            var context = exceptionReceivedEventArgs.ExceptionReceivedContext;

            string exceptionContext = $"Action : {context.Action} |  Endpoint : {context.Endpoint} | Endpoint : {context.EntityPath} | ClientId : {context.ClientId}";

            _logger.LogError(exceptionContext);

            return Task.CompletedTask;
        }

        /// <summary>
        /// Remove the default rule
        /// </summary>
        private void RemoveRule(string ruleName)
        {
            try
            {
                _subscriptionClient
                 .RemoveRuleAsync(ruleName)
                 .GetAwaiter()
                 .GetResult();
            }
            catch (MessagingEntityNotFoundException)
            {
                _logger.LogInformation($"The messaging entity {ruleName} Could not be found.");
            }
        }


    }
}
