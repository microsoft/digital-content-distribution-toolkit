// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;
using blendnet.common.dto.Events;
using blendnet.common.dto.Integration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Runtime.CompilerServices;
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
        private ServiceBusSender _topicSender;

        private ServiceBusProcessor _serviceBusProcessor;

        private EventBusConnectionData _eventBusConnectionData;

        private Dictionary<string, SubscriberData> _subscribers;

        private ServiceBusAdministrationClient _serviceBusAdministrationClient;

        private readonly ILogger _logger;

        private IServiceProvider _serviceProvider;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventBusConnectionData"></param>
        public EventServiceBus( EventBusConnectionData eventBusConnectionData, 
                                ILogger<EventServiceBus> logger, 
                                IServiceProvider serviceProvider,
                                ServiceBusClient serviceBusClient)
        {
            _subscribers = new Dictionary<string, SubscriberData>();

            _logger = logger;

            _serviceProvider = serviceProvider;

            _eventBusConnectionData = eventBusConnectionData;

            _topicSender = serviceBusClient.CreateSender(eventBusConnectionData.TopicName);

            _serviceBusAdministrationClient = new ServiceBusAdministrationClient(eventBusConnectionData.ServiceBusConnectionString);

            if (!string.IsNullOrEmpty(_eventBusConnectionData.SubscriptionName))
            {
                RemoveRule(CreateRuleOptions.DefaultRuleName);

                var options = new ServiceBusProcessorOptions
                {
                    // Indicates whether the message pump should automatically complete the messages after returning from user callback.
                    // False below indicates the complete operation is handled by the user callback as in ProcessMessagesAsync().
                    // AutoCompleteMessages = false,

                    // Maximum number of concurrent calls to the callback ProcessMessagesAsync(), set to 1 for simplicity.
                    // Set it according to how many messages the application wants to process in parallel.
                    MaxConcurrentCalls = _eventBusConnectionData.MaxConcurrentCalls,

                    ReceiveMode = ServiceBusReceiveMode.ReceiveAndDelete
                };

                _serviceBusProcessor = serviceBusClient.CreateProcessor(_eventBusConnectionData.TopicName, _eventBusConnectionData.SubscriptionName, options);

                _serviceBusProcessor.ProcessMessageAsync += MessageHandler;

                _serviceBusProcessor.ProcessErrorAsync += ErrorHandler;
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

            ServiceBusMessage message = new ServiceBusMessage(body);

            message.MessageId = integrationEvent.Id.ToString();

            message.Subject = eventName;
            
            await _topicSender.SendMessageAsync(message);
        }
        

        /// <summary>
        /// Start listening
        /// </summary>
        /// <returns></returns>
        public async Task StartProcessing()
        {
            await _serviceBusProcessor.StartProcessingAsync();
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

            CreateRuleOptions ruleOptions;

            if (correlationRule == null)
            {
                eventName = GetEventName(typeof(T));

                ruleOptions = new CreateRuleOptions
                {
                    Filter = new CorrelationRuleFilter { Subject = eventName },
                    Name = eventName
                };
            }
            else
            {
                eventName = GetEventName(typeof(T));

                CorrelationRuleFilter customerPropertyFilter = new CorrelationRuleFilter();
                
                customerPropertyFilter.ApplicationProperties.Add(correlationRule.PropertyName, correlationRule.PropertValue);

                ruleOptions = new CreateRuleOptions
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

            bool ruleExists = _serviceBusAdministrationClient.RuleExistsAsync(_eventBusConnectionData.TopicName, _eventBusConnectionData.SubscriptionName, eventName).Result;
            
            if (!ruleExists)
            {
                //Add the rule to service bus subscription
                _serviceBusAdministrationClient.CreateRuleAsync(_eventBusConnectionData.TopicName, _eventBusConnectionData.SubscriptionName, ruleOptions);
            }
        }

        /// <summary>
        /// Returns the event Name
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private string GetEventName(Type type)
        {
            object[] eventNameAttribute = type.GetCustomAttributes(typeof(EventDetailsAttribute), true);

            if (eventNameAttribute != null && eventNameAttribute.Length > 0)
            {
                return ((EventDetailsAttribute)eventNameAttribute[0]).EventName;
            }
            else
            {
                return type.Name;
            }
        }

        /// <summary>
        /// Check If Serialization Needed
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private bool CheckIfSerializationNeeded(Type type)
        {
            object[] eventNameAttribute = type.GetCustomAttributes(typeof(EventDetailsAttribute), true);

            if (eventNameAttribute != null && eventNameAttribute.Length > 0)
            {
                return ((EventDetailsAttribute)eventNameAttribute[0]).PerformJsonSerialization;
            }
            else
            {
                return true;
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
            string eventName = GetEventName(typeof(T));

            _subscribers.Remove(eventName);

            RemoveRule(eventName);

        }

       
        /// <summary>
        /// On Message Recieved
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private async Task MessageHandler(ProcessMessageEventArgs args)
        {
            //for application generated events this values will always be present
            var eventName = $"{args.Message.Subject}";

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
                        if (args.Message.ApplicationProperties.ContainsKey(customSubscriber.Value.CorrelationRule.PropertyName))
                        {
                            eventName = customSubscriber.Key;
                            break;
                        }
                    }
                }
            }

            var messageData = Encoding.UTF8.GetString(args.Message.Body);

            if (_subscribers.ContainsKey(eventName))
            {
                var eventTypeData = _subscribers[eventName];

                Type eventType = eventTypeData.EventType;

                Type subscriber =  eventTypeData.EventHandlerType;

                object integrationEvent;

                if (CheckIfSerializationNeeded(eventType))
                {
                    integrationEvent = System.Text.Json.JsonSerializer.Deserialize(messageData, eventType);
                }
                else
                {
                    integrationEvent = Activator.CreateInstance(eventType);
                }
                                
                var registeredHandler = _serviceProvider.GetService(subscriber);

                if (registeredHandler != null)
                {
                    //Get the actual event handler
                    var concreteType = typeof(IIntegrationEventHandler<>).MakeGenericType(eventType);

                    //Set the Correlation id in case any handler wants to use.
                    ((IntegrationEvent)integrationEvent).CorrelationId = args.Message.CorrelationId;

                    ((IntegrationEvent)integrationEvent).Body = messageData;

                    //call the handle method
                    await (Task)concreteType.GetMethod("Handle").Invoke(registeredHandler, new object[] { integrationEvent });

                    // Complete the message so that it is not received again.
                    // This can be done only if the subscriptionClient is created in ReceiveMode.PeekLock mode (which is the default).
                    //await args.CompleteMessageAsync(args.Message);
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
        /// 
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private Task ErrorHandler(ProcessErrorEventArgs args)
        {
            _logger.LogError(args.Exception, args.Exception.Message);

            string exceptionContext = $"Error Soruce : {args.ErrorSource} |  Namespace : {args.FullyQualifiedNamespace} | Endpoint : {args.EntityPath}";

            _logger.LogError(exceptionContext);

            return Task.CompletedTask;
        }

        /// <summary>
        /// Remove the default rule
        /// </summary>
        private void RemoveRule(string ruleName)
        {
            bool ruleExists = _serviceBusAdministrationClient.RuleExistsAsync(_eventBusConnectionData.TopicName,
                                                                _eventBusConnectionData.SubscriptionName,
                                                                ruleName).Result;
            if (ruleExists)
            {
                var response = _serviceBusAdministrationClient.DeleteRuleAsync(_eventBusConnectionData.TopicName,
                                                                _eventBusConnectionData.SubscriptionName,
                                                                ruleName).Result;
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
