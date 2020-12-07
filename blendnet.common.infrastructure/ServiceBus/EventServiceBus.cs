using blendnet.common.dto.Integration;
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

namespace blendnet.common.infrastructure.ServiceBus
{
    /// <summary>
    /// Register this class as singleton
    /// </summary>
    public class EventServiceBus : IEventBus
    {
        private ITopicClient _topicClient;

        private ISubscriptionClient _subscriptionClient;

        private EventBusConnectionData _eventBusConnectionData;

        private Dictionary<string, SubscriberData> _subscribers;

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
            _subscribers = new Dictionary<string, SubscriberData>();

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
            PerformSubscribe<T, TH>();
        }

        public void Subscribe<T, TH>(CustomPropertyCorrelationRule correlationRule)
           where T : IntegrationEvent
           where TH : IIntegrationEventHandler<T>
        {
            PerformSubscribe<T,TH>(correlationRule);
        }

        private void PerformSubscribe<T, TH>(CustomPropertyCorrelationRule correlationRule=null)
           where T : IntegrationEvent
           where TH : IIntegrationEventHandler<T>
        {
            string eventName;

            RuleDescription ruleDescription;

            if (correlationRule == null)
            {
                eventName = typeof(T).Name;

                ruleDescription = new RuleDescription
                {
                    Filter = new CorrelationFilter { Label = eventName },
                    Name = eventName
                };
            }
            else
            {
                eventName = typeof(T).Name;

                CorrelationFilter customerPropertyFilter = new CorrelationFilter();
                
                customerPropertyFilter.Properties.Add(correlationRule.PropertyName, correlationRule.PropertValue);

                ruleDescription = new RuleDescription
                {
                    Filter = customerPropertyFilter,
                    Name = eventName
                };
            }
            
            if (_subscribers.ContainsKey(eventName))
            {
                throw new InvalidOperationException($"Event {eventName} is already registered with another handler.");
            };

            //add the rule to the collection
            SubscriberData subscriberData = new SubscriberData()
            {
                EventType = typeof(T),
                EventHandlerType = typeof(TH),
                CorrelationRule = correlationRule
            };

            _subscribers.Add(eventName, subscriberData);

            var rules = _subscriptionClient.GetRulesAsync().GetAwaiter().GetResult();

            var existingRule = rules.Where(r => r.Name.Equals(eventName)).FirstOrDefault();

            if (existingRule == null)
            {
                //Add the rule to service bus subscription
                _subscriptionClient.AddRuleAsync(ruleDescription).GetAwaiter().GetResult();
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
            //for application generated events this values will always be present
            var eventName = $"{message.Label}";

            //in case event name is empty in lable..check if any custom property is configure.
            //Added to handle the events which are added by Event Grid on Blob.
            //In case of Application events the below will never be executed
            if (string.IsNullOrEmpty(eventName))
            {
                _logger.LogInformation($"No value found on Label property of message. Hence event name is empty. Looking for custom message properties");

                var customSubscribers = _subscribers.Where(sb => sb.Value.CustomCorrelationFiler).ToList();

                if (customSubscribers != null && customSubscribers.Count > 0)
                {
                    foreach(KeyValuePair<string,SubscriberData> customSubscriber in customSubscribers)
                    {
                        if (message.UserProperties.ContainsKey(customSubscriber.Value.CorrelationRule.PropertyName))
                        {
                            eventName = customSubscriber.Key;
                            break;
                        }
                    }
                }
            }

            var messageData = Encoding.UTF8.GetString(message.Body);

            if (_subscribers.ContainsKey(eventName))
            {
                var eventTypeData = _subscribers[eventName];

                Type eventType = eventTypeData.EventType;

                Type subscriber =  eventTypeData.EventHandlerType;

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

    internal class SubscriberData
    {
        public Type EventType { get; set; }

        public Type EventHandlerType { get; set; }

        public bool CustomCorrelationFiler 
        { 
            get
            {
                return !(CorrelationRule == null);
            } 
        }

        public CustomPropertyCorrelationRule CorrelationRule { get; set; }

    }
}
