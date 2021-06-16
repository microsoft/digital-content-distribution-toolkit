using blendnet.incentive.repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using blendnet.common.dto.Incentive;

namespace blendnet.incentive.api.Common
{
    /// <summary>
    /// Helper to calculate incentives
    /// </summary>
    public class IncentiveCalculationHelper
    {
        IIncentiveRepository _incentiveRepository;

        IEventRepository _eventRepository;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="incentiveRepository"></param>
        public IncentiveCalculationHelper(IIncentiveRepository incentiveRepository,
                                          IEventRepository eventRepository)
        {
            _incentiveRepository = incentiveRepository;

            _eventRepository = eventRepository;
        }


        /// <summary>
        /// Calculates the Regular Incentive Plan for Retailer
        /// </summary>
        /// <param name="retailerPartnerId"></param>
        /// <param name="retailerProviderCode"></param>
        /// <returns></returns>
        public async Task<IncentivePlan> CalculateActiveIncentivePlanForRetailer(string retailerProviderCode,string retailerPartnerId)
        {
            //Get the current active plan for retailer
            IncentivePlan incentivePlan = await _incentiveRepository.GetCurrentRetailerPublishedPlan(PlanType.REGULAR, retailerProviderCode, null);

            await CalculateIncentivePlan(incentivePlan, retailerPartnerId);
            
            return incentivePlan;

        }

        /// <summary>
        /// Calculates the Milestone for Retailer
        /// </summary>
        /// <param name="retailerPartnerId"></param>
        /// <param name="retailerProviderCode"></param>
        /// <returns></returns>
        public async Task<IncentivePlan> CalculateActiveMiletoneForRetailer(string retailerProviderCode, string retailerPartnerId)
        {
            IncentivePlan incentivePlan = await _incentiveRepository.GetCurrentRetailerPublishedPlan(PlanType.MILESTONE, retailerProviderCode, null);

            await CalculateIncentivePlan(incentivePlan, retailerPartnerId);

            return incentivePlan;
        }

        /// <summary>
        /// Calculates Incentive Plan for Consumer
        /// Question : Calcule over all the past plans?
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <returns></returns>
        public async Task<IncentivePlan> CalculateActiveIncentivePlanForConsumer(string phoneNumber)
        {
            IncentivePlan incentivePlan = await _incentiveRepository.GetCurrentConsumerPublishedPlan(PlanType.REGULAR, null);

            await CalculateIncentivePlan(incentivePlan, phoneNumber);

            return incentivePlan;
        }

        /// <summary>
        /// Calculates the Milestone for Consumer
        /// Milestone is always calculated on active plan.
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <returns></returns>
        public async Task<IncentivePlan> CalculateActiveMilestoneForConsumer(string phoneNumber)
        {
            IncentivePlan incentivePlan = await _incentiveRepository.GetCurrentConsumerPublishedPlan(PlanType.MILESTONE, null);

            await CalculateIncentivePlan(incentivePlan, phoneNumber);

            return incentivePlan;
        }

        /// <summary>
        /// Calculates Retailer incentives for the given date range
        /// </summary>
        /// <param name="retailerPartnerId"></param>
        /// <param name="startDate"></param>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public async Task<List<EventAggregrateResponse>> CalculateRandomIncentiveForRetailer(string retailerPartnerId,
                                                                DateTime startDate,
                                                                DateTime endDate)
        {
            EventAggregrateRequest eventAggregrateRequest = new EventAggregrateRequest()
            {
                AggregrateType = RuleType.SUM,
                AudienceType = AudienceType.RETAILER,
                EventCreatedFor = retailerPartnerId,
                StartDate = startDate,
                EndDate = endDate
            };

            List<EventAggregrateResponse> response = await _eventRepository.GetEventAggregrates(eventAggregrateRequest);

            return response;
        }


        /// <summary>
        /// Calculates Consumer incentives for the given date range 
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <param name="startDate"></param>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public async Task<List<EventAggregrateResponse>> CalculateRandomIncentiveForConsumer(string phoneNumber,
                                                              DateTime startDate, 
                                                              DateTime endDate)
        {
            EventAggregrateRequest eventAggregrateRequest = new EventAggregrateRequest()
            {
                AggregrateType = RuleType.SUM,
                AudienceType = AudienceType.CONSUMER,
                EventCreatedFor = phoneNumber,
                StartDate = startDate,
                EndDate = endDate
            };

            List<EventAggregrateResponse> response = await _eventRepository.GetEventAggregrates(eventAggregrateRequest);

            return response;
        }

        /// <summary>
        /// Sets the start date time to datetime to
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <returns></returns>
        public async Task<List<EventAggregrateResponse>> CalculateRandomIncentiveForConsumer(string phoneNumber)
        {
            EventAggregrateRequest eventAggregrateRequest = new EventAggregrateRequest()
            {
                AggregrateType = RuleType.SUM,
                AudienceType = AudienceType.CONSUMER,
                EventCreatedFor = phoneNumber
            };

            List<EventAggregrateResponse> response = await _eventRepository.GetEventAggregrates(eventAggregrateRequest);

            return response;
        }

        #region Private Methods
        /// <summary>
        /// Calculates Incentive Plan
        /// </summary>
        /// <param name="incentivePlan"></param>
        /// <param name="eventCreatedFor"></param>
        /// <returns></returns>
        private async Task CalculateIncentivePlan(IncentivePlan incentivePlan, string eventCreatedFor)
        {
            //one active incentive plan has to exists and it should have plan details
            if (incentivePlan != null &&
                incentivePlan.PlanDetails != null &&
                incentivePlan.PlanDetails.Count > 0)
            {
                await CalculatePlanDetails(incentivePlan.PlanType,
                                            incentivePlan.Audience.AudienceType,
                                            incentivePlan.PlanDetails,
                                            incentivePlan.StartDate,
                                            incentivePlan.EndDate,
                                            eventCreatedFor);
            }
        }


        /// <summary>
        /// Calculates Incentive Plan
        /// </summary>
        /// <param name="planDetails"></param>
        /// <param name="eventCreatedFor"></param>
        /// <returns></returns>
        private async Task CalculatePlanDetails(PlanType planType,
                                                AudienceType audienceType,
                                                List<PlanDetail> planDetails,
                                                DateTime startDate,
                                                DateTime endDate,
                                                string eventCreatedFor)
        {
            //Get the unique Rules with assosiated events
            Dictionary<RuleType, List<EventType>> uniqueRuleWithEvents = GetUniqueRuleWithEvents(planDetails);

            //Get the COUNT / SUM from actual event data EACH UNIQUE EVENT TYPE and EVENT SUBTYPE
            List<EventAggregrateResponse> eventAggregrates = await GetEventAggregrates(audienceType,
                                                                                        startDate,
                                                                                        endDate,
                                                                                        eventCreatedFor,
                                                                                        uniqueRuleWithEvents);

            //check if there is sum data / events recorded in database for any of the events mentioned in incentive plan
            if (eventAggregrates != null && eventAggregrates.Count > 0)
            {
                foreach (PlanDetail planDetail in planDetails)
                {
                    EventAggregrateResponse eventAggregrate = eventAggregrates.Where(ea => (ea.EventType == planDetail.EventType
                                                                                            && ea.EventSubType == planDetail.EventSubType
                                                                                            && ea.RuleType == planDetail.RuleType)).FirstOrDefault();
                    if (eventAggregrate != default(EventAggregrateResponse))
                    {
                        //Formula is applied at the time of insertion hence formula is not required to be applied here.
                        if (planType == PlanType.REGULAR)
                        {
                            planDetail.Result = new Result() { Value = eventAggregrate.AggregratedValue };

                            //Formula needes to be applied for Milestone types of plan
                        }
                        else if (planType == PlanType.MILESTONE)
                        {
                            planDetail.Result = ApplyFormula(planDetail.Formula, eventAggregrate.AggregratedValue);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Returns the unique combination of Rule Type with the associated events
        /// </summary>
        /// <param name="planDetails"></param>
        /// <returns></returns>
        private Dictionary<RuleType, List<EventType>> GetUniqueRuleWithEvents(List<PlanDetail> planDetails)
        {
            Dictionary<RuleType, List<EventType>> ruleEventCombinations = new Dictionary<RuleType, List<EventType>>();

            foreach (PlanDetail planDetail in planDetails)
            {
                if (ruleEventCombinations.ContainsKey(planDetail.RuleType))
                {
                    //if the same event type has been added with different subtype, add only once to the value collection
                    if (!ruleEventCombinations[planDetail.RuleType].Exists(etype => (etype == planDetail.EventType)))
                    {
                        ruleEventCombinations[planDetail.RuleType].Add(planDetail.EventType);
                    }
                }
                else
                {
                    List<EventType> eventTypes = new List<EventType>();

                    eventTypes.Add(planDetail.EventType);

                    ruleEventCombinations.Add(planDetail.RuleType, eventTypes);
                }
            }

            return ruleEventCombinations;
        }


        /// <summary>
        /// Get Event Aggregrates for the unique Rule and List<EventTypes> combinations
        /// This makes the call to  repository hence can be costly but the call is on partition key
        /// </summary>
        /// <param name="incentivePlan"></param>
        /// <param name="eventCreatedFor"></param>
        /// <param name="uniqueRuleWithEvents"></param>
        /// <returns></returns>
        private async Task<List<EventAggregrateResponse>> GetEventAggregrates(AudienceType audienceType,
                                                                                DateTime startDate,
                                                                                DateTime endDate,
                                                                                string eventCreatedFor,
                                                                                Dictionary<RuleType, List<EventType>> uniqueRuleWithEvents)
        {
            List<EventAggregrateResponse> eventAggregrates = new List<EventAggregrateResponse>();

            EventAggregrateRequest eventAggregrateRequest = null;

            foreach (KeyValuePair<RuleType, List<EventType>> rule in uniqueRuleWithEvents)
            {
                eventAggregrateRequest = new EventAggregrateRequest()
                {
                    AggregrateType = rule.Key,
                    EventTypes = rule.Value,
                    AudienceType = audienceType,
                    EventCreatedFor = eventCreatedFor,
                    StartDate = startDate,
                    EndDate = endDate
                };

                eventAggregrates.AddRange(await _eventRepository.GetEventAggregrates(eventAggregrateRequest));
            }

            return eventAggregrates;
        }

        /// <summary>
        /// Apply formula on the retrieved aggregrated value
        /// </summary>
        /// <param name="formula"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private Result ApplyFormula(Formula formula, double value)
        {
            Result result = new Result();

            switch (formula.FormulaType)
            {
                case FormulaType.PLUS:
                    {
                        result.Value = value + formula.FirstOperand;
                        break;
                    }
                case FormulaType.MINUS:
                    {
                        result.Value = value - formula.FirstOperand;
                        break;
                    }
                case FormulaType.MULTIPLY:
                    {
                        result.Value = value * formula.FirstOperand;
                        break;
                    }
                case FormulaType.PERCENTAGE:
                    {
                        result.Value = (value * formula.FirstOperand) / 100;
                        break;
                    }
                case FormulaType.DIVIDE_AND_MULTIPLY:
                    {
                        result.Value = Math.Floor(value / formula.FirstOperand) * formula.SecondOperand.Value;

                        result.Value1 = value % formula.FirstOperand;

                        break;
                    }
                case FormulaType.RANGE_AND_MULTIPLY:
                    {
                        RangeValue rangeValue = formula.RangeOperand.Where(rv => (value >= rv.StartRange && value <= rv.EndRange)).FirstOrDefault();

                        if (rangeValue != default(RangeValue))
                        {
                            result.Value = rangeValue.Output * formula.SecondOperand.Value;
                        }

                        break;
                    }
            }

            return result;
        }

        #endregion

    }
}
