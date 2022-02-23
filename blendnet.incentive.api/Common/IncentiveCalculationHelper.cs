using blendnet.incentive.repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using blendnet.common.dto.Incentive;
using blendnet.common.dto.Common;
using blendnet.incentive.api.Model;

namespace blendnet.incentive.api.Common
{
    /// <summary>
    /// Helper to calculate incentives
    /// </summary>
    public class IncentiveCalculationHelper
    {
        IEventRepository _eventRepository;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="incentiveRepository"></param>
        public IncentiveCalculationHelper(IEventRepository eventRepository)
        {
            _eventRepository = eventRepository;
        }


        /// <summary>
        /// Calculates the Regular Incentive Plan for Retailer
        /// </summary>
        /// <param name="incentivePlan"></param>
        /// <param name="retailerPartnerId"></param>
        /// <returns></returns>
        public async Task<IncentivePlan> CalculateIncentivePlanForRetailer(IncentivePlan incentivePlan, string retailerPartnerId)
        {
            await CalculateIncentivePlan(incentivePlan, retailerPartnerId);
            
            return incentivePlan;

        }

        /// <summary>
        /// Calculates the Milestone for Retailer
        /// </summary>
        /// <param name="incentivePlan"></param>
        /// <param name="retailerPartnerId"></param>
        /// <returns></returns>
        public async Task<IncentivePlan> CalculateMiletoneForRetailer(IncentivePlan incentivePlan, string retailerPartnerId)
        {
            await CalculateIncentivePlan(incentivePlan, retailerPartnerId);

            return incentivePlan;
        }

        /// <summary>
        /// Calculates Incentive Plan for Consumer
        /// </summary>
        /// <param name="incentivePlan"></param>
        /// <param name="phoneNumber"></param>
        /// <returns></returns>
        public async Task<IncentivePlan> CalculateIncentivePlanForConsumer(IncentivePlan incentivePlan, string phoneNumber)
        {
            await CalculateIncentivePlan(incentivePlan, phoneNumber);

            return incentivePlan;
        }

        /// <summary>
        /// Calculates the Milestone for Consumer
        /// Milestone is always calculated on active plan.
        /// </summary>
        /// <param name="incentivePlan"></param>
        /// <param name="phoneNumber"></param>
        /// <returns></returns>
        public async Task<IncentivePlan> CalculateMilestoneForConsumer(IncentivePlan incentivePlan,string phoneNumber)
        {
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
        public async Task<EventAggregateData> CalculateRandomIncentiveForRetailer(string retailerPartnerId,
                                                                DateTime startDate,
                                                                DateTime endDate)
        {
            EventAggregrateRequest eventAggregrateRequest = new EventAggregrateRequest()
            {
                AudienceType = AudienceType.RETAILER,
                EventCreatedFor = new string[] { retailerPartnerId },
                StartDate = startDate,
                EndDate = endDate
            };

            List<EventAggregrateResponse> response = await _eventRepository.GetEventAggregrates(eventAggregrateRequest);

            EventAggregateData eventAggregateData = new EventAggregateData()
            {
                EventAggregateResponses = response,
                TotalValue = response.Select(x => x.AggregratedCalculatedValue).Sum()
            };

            return eventAggregateData;
        }


        /// <summary>
        /// Calculates Consumer incentives for the given date range 
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <param name="startDate"></param>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public async Task<EventAggregateData> CalculateRandomIncentiveForConsumer(string phoneNumber,
                                                              DateTime startDate, 
                                                              DateTime endDate)
        {
            EventAggregrateRequest eventAggregrateRequest = new EventAggregrateRequest()
            {
                AudienceType = AudienceType.CONSUMER,
                EventCreatedFor = new string[] { phoneNumber },
                StartDate = startDate,
                EndDate = endDate
            };

            List<EventAggregrateResponse> response = await _eventRepository.GetEventAggregrates(eventAggregrateRequest);

            EventAggregateData eventAggregateData = new EventAggregateData()
            {
                EventAggregateResponses = response,
                TotalValue = response.Select(x => x.AggregratedCalculatedValue).Sum()
            };

            return eventAggregateData;
        }

        /// <summary>
        /// Sets the start date time to datetime to
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <returns></returns>
        public async Task<EventAggregateData> CalculateRandomIncentiveForConsumer(string phoneNumber)
        {
            EventAggregrateRequest eventAggregrateRequest = new EventAggregrateRequest()
            {
                AudienceType = AudienceType.CONSUMER,
                EventCreatedFor = new string[] { phoneNumber }
            };

            List<EventAggregrateResponse> response = await _eventRepository.GetEventAggregrates(eventAggregrateRequest);

            EventAggregateData eventAggregateData = new EventAggregateData()
            {
                EventAggregateResponses = response,
                TotalValue = response.Select(x => x.AggregratedCalculatedValue).Sum()
            };

            return eventAggregateData;
        }

        /// <summary>
        /// Calculates Incentive Plan for unique retailer or consumers
        /// </summary>
        /// <param name="incentivePlan"></param>
        /// <param name="continuationToken"></param>
        /// <param name="batchSize"></param>
        /// <returns></returns>

        public async Task<CalculatedIncentivePlan> CalculateIncentivePlan(IncentivePlan incentivePlan,
                                                                          DateTime reportingDateTime,
                                                                          string[] eventCreatedFor)
        {
            CalculatedIncentivePlan calculatedIncentivePlan = new CalculatedIncentivePlan();

            calculatedIncentivePlan.IncentivePlan = incentivePlan;

            //Get the unique Rules with assosiated events
            List<EventType> uniqueEvents = incentivePlan.PlanDetails.Select(pd => pd.EventType).Distinct().ToList();

            EventAggregrateRequest eventAggregrateRequest = new EventAggregrateRequest()
            {
                EventTypes = uniqueEvents,
                AudienceType = incentivePlan.Audience.AudienceType,
                SubTypeName = incentivePlan.Audience.SubTypeName,
                StartDate = incentivePlan.StartDate,
                EndDate = reportingDateTime,
                EventCreatedFor = eventCreatedFor
            };

            //get the data for the given list of retailers.
            List<EventAggregrateResponse> eventAggregrates = await _eventRepository.GetEventAggregrates(eventAggregrateRequest);

            //ideally this should never happen and count should be greater than 0
            if (eventAggregrates != null && eventAggregrates.Count > 0)
            {
                calculatedIncentivePlan.CalculatedPlanDetails = new List<CalculatedPlanDetails>();

                CalculatedPlanDetails calculatedPlanDetails;

                foreach (string uniqueAudience in eventCreatedFor)
                {
                    //create the clone for plan detail collection
                    List<PlanDetail> planDetails = incentivePlan.PlanDetails.Select(item => (PlanDetail)item.Clone()).ToList();

                    //this method set the result for each event on the plan detail
                    CalculatePlanDetails(incentivePlan.PlanType, planDetails, eventAggregrates, uniqueAudience);

                    //remove all the plan details where no result was calculated
                    planDetails.RemoveAll(pd => pd.Result == null);

                    //in case none of the events have result calculated, dont return back.
                    if (planDetails.Count > 0)
                    {
                        calculatedPlanDetails = new CalculatedPlanDetails()
                        {
                            CalculatedFor = uniqueAudience,
                            PlanDetails = planDetails
                        };

                        calculatedIncentivePlan.CalculatedPlanDetails.Add(calculatedPlanDetails);
                    }
                }
            }

            return calculatedIncentivePlan;
        }


        /// <summary>
        /// Returns incentive events for consumer
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <param name="eventType"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public async Task<List<IncentiveEvent>> GetConsumerIncentiveEvents(string phoneNumber, EventType eventType, DateTime startDate, DateTime endDate)
        {
            List<IncentiveEvent> incentiveEvents = await GetIncentiveEvents(phoneNumber, AudienceType.CONSUMER, eventType, startDate, endDate);

            return incentiveEvents;
        }

        

        /// <summary>
        /// Returns incentive events for retailer
        /// </summary>
        /// <param name="retailerPartnerProvidedId"></param>
        /// <param name="eventType"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public async Task<List<IncentiveEvent>> GetRetailerIncentiveEvents(string retailerPartnerProvidedId, EventType eventType, DateTime startDate, DateTime endDate)
        {
            List<IncentiveEvent> incentiveEvents = await GetIncentiveEvents(retailerPartnerProvidedId, AudienceType.RETAILER, eventType, startDate, endDate);

            return incentiveEvents;
        }

        /// <summary>
        /// Returns list of incentive events for given parameters
        /// </summary>
        /// <param name="eventCreatedFor"></param>
        /// <param name="audienceType"></param>
        /// <param name="eventType"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        private async Task<List<IncentiveEvent>> GetIncentiveEvents(string eventCreatedFor, AudienceType audienceType, EventType eventType, DateTime startDate, DateTime endDate)
        {
            List<EventType> eventTypes = new List<EventType>();

            eventTypes.Add(eventType);
            
            EventCriteriaRequest eventCriteriaRequest = new EventCriteriaRequest()
            {
                EventCreatedFor = eventCreatedFor,
                AudienceType = audienceType,
                EventTypes = eventTypes,
                StartDate = startDate,
                EndDate = endDate
            };

            List<IncentiveEvent> incentiveEvents = await _eventRepository.GetEvents(eventCriteriaRequest);
            
            return incentiveEvents;
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
                List<EventType> uniqueEvents = incentivePlan.PlanDetails.Select(pd => pd.EventType).Distinct().ToList();

                //create event aggregrate request
                EventAggregrateRequest eventAggregrateRequest = new EventAggregrateRequest()
                {
                    EventTypes = uniqueEvents,
                    AudienceType = incentivePlan.Audience.AudienceType,
                    EventCreatedFor = new string[] { eventCreatedFor },
                    StartDate = incentivePlan.StartDate,
                    EndDate = incentivePlan.EndDate
                };

                List<EventAggregrateResponse> eventAggregrates = await _eventRepository.GetEventAggregrates(eventAggregrateRequest);

                CalculatePlanDetails( incentivePlan.PlanType,
                                      incentivePlan.PlanDetails,
                                      eventAggregrates,
                                      eventCreatedFor);
            }
        }

        /// <summary>
        /// Calculates Incentive Plan
        /// </summary>
        /// <param name="planDetails"></param>
        /// <param name="eventCreatedFor"></param>
        /// <returns></returns>
        private void CalculatePlanDetails(  PlanType planType,
                                            List<PlanDetail> planDetails,
                                            List<EventAggregrateResponse> eventAggregrates,
                                            string eventCreatedFor)
        {
            
            //check if there is sum data / events recorded in database for any of the events mentioned in incentive plan
            if (eventAggregrates != null && eventAggregrates.Count > 0)
            {
                foreach (PlanDetail planDetail in planDetails)
                {
                    EventAggregrateResponse eventAggregrate = eventAggregrates.Where(ea => (ea.EventType == planDetail.EventType
                                                                                            && ea.EventSubType == planDetail.EventSubType
                                                                                            && ea.EventCreatedFor.Equals(eventCreatedFor))).FirstOrDefault();
                    if (eventAggregrate != default(EventAggregrateResponse))
                    {
                        //Formula is applied at the time of insertion hence formula is not required to be applied here.
                        if (planType == PlanType.REGULAR)
                        {
                            planDetail.Result = new Result() { 
                                Value = eventAggregrate.AggregratedCalculatedValue,
                                Entity1Value = eventAggregrate.AggregratedE1CalculatedValue,
                                Entity2Value = eventAggregrate.AggregratedE2CalculatedValue,
                                Entity3Value = eventAggregrate.AggregratedE3CalculatedValue,
                                Entity4Value = eventAggregrate.AggregratedE4CalculatedValue,
                            };

                            planDetail.Result.RawData = GetRawData(eventAggregrate);

                        }//Formula needes to be applied for Milestone types of plan
                        else if (planType == PlanType.MILESTONE)
                        {
                            //get the calculated value
                            double valueToOperate = GetValueToOperate(planDetail.RuleType, eventAggregrate);

                            planDetail.Result = ApplyFormula(planDetail.Formula, valueToOperate);

                            planDetail.Result.RawData = GetRawData(eventAggregrate);
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Get Raw Data
        /// </summary>
        /// <param name="eventAggregrate"></param>
        /// <returns></returns>
        private RawData GetRawData(EventAggregrateResponse eventAggregrate )
        {
            RawData rawData = new RawData()
            {
                AggregratedCalculatedValue = eventAggregrate.AggregratedCalculatedValue,
                AggregratedE1CalculatedValue = eventAggregrate.AggregratedE1CalculatedValue,
                AggregratedE2CalculatedValue = eventAggregrate.AggregratedE2CalculatedValue,
                AggregratedE3CalculatedValue = eventAggregrate.AggregratedE3CalculatedValue,
                AggregratedE4CalculatedValue = eventAggregrate.AggregratedE4CalculatedValue,
                AggregratedOriginalValue = eventAggregrate.AggregratedOriginalValue,
                AggregratedCount = eventAggregrate.AggregratedCount
            };

            return rawData;
        }

        /// <summary>
        /// GetValueToOperate
        /// </summary>
        /// <param name="ruleType"></param>
        /// <param name="eventAggregrate"></param>
        /// <returns></returns>
        private double GetValueToOperate(  RuleType ruleType, 
                                            EventAggregrateResponse eventAggregrate)
        {
            double calculatedValue = double.MinValue;
            
            if (ruleType == RuleType.COUNT)
            {
                calculatedValue = eventAggregrate.AggregratedCount;
            }
            else
            {
                calculatedValue = eventAggregrate.AggregratedOriginalValue;
            }
            
            return calculatedValue;
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
                        result.Value = value + formula.FirstOperand.Value;

                        result.Entity1Value = formula.Entity1Operand.HasValue ? value + formula.Entity1Operand.Value : null;
                        
                        result.Entity2Value = formula.Entity2Operand.HasValue ? value + formula.Entity2Operand.Value : null;

                        result.Entity3Value = formula.Entity3Operand.HasValue ? value + formula.Entity3Operand.Value : null;
                        
                        result.Entity4Value = formula.Entity4Operand.HasValue ? value + formula.Entity4Operand.Value : null;

                        break;
                    }
                case FormulaType.MINUS:
                    {
                        result.Value = value - formula.FirstOperand.Value;

                        result.Entity1Value = formula.Entity1Operand.HasValue ? value - formula.Entity1Operand.Value : null;

                        result.Entity2Value = formula.Entity2Operand.HasValue ? value - formula.Entity2Operand.Value : null;

                        result.Entity3Value = formula.Entity3Operand.HasValue ? value - formula.Entity3Operand.Value : null;

                        result.Entity4Value = formula.Entity4Operand.HasValue ? value - formula.Entity4Operand.Value : null;

                        break;
                    }
                case FormulaType.MULTIPLY:
                    {
                        result.Value = value * formula.FirstOperand.Value;

                        result.Entity1Value = formula.Entity1Operand.HasValue ? value * formula.Entity1Operand.Value : null;

                        result.Entity2Value = formula.Entity2Operand.HasValue ? value * formula.Entity2Operand.Value : null;

                        result.Entity3Value = formula.Entity3Operand.HasValue ? value * formula.Entity3Operand.Value : null;

                        result.Entity4Value = formula.Entity4Operand.HasValue ? value * formula.Entity4Operand.Value : null;

                        break;
                    }
                case FormulaType.PERCENTAGE:
                    {
                        result.Value = (value * formula.FirstOperand.Value) / 100;

                        result.Entity1Value = formula.Entity1Operand.HasValue ? (value * formula.Entity1Operand.Value) / 100 : null;

                        result.Entity2Value = formula.Entity2Operand.HasValue ? (value * formula.Entity2Operand.Value) / 100 : null;

                        result.Entity3Value = formula.Entity3Operand.HasValue ? (value * formula.Entity3Operand.Value) / 100 : null;

                        result.Entity4Value = formula.Entity4Operand.HasValue ? (value * formula.Entity4Operand.Value) / 100 : null;

                        break;
                    }
                case FormulaType.DIVIDE_AND_MULTIPLY:
                    {
                        result.ResidualValue = value % formula.FirstOperand.Value;

                        result.Value = Math.Floor(value / formula.FirstOperand.Value) * formula.SecondOperand.Value;

                        result.Entity1Value = formula.Entity1Operand.HasValue ? Math.Floor(value / formula.FirstOperand.Value) * formula.Entity1Operand.Value : null;

                        result.Entity2Value = formula.Entity2Operand.HasValue ? Math.Floor(value / formula.FirstOperand.Value) * formula.Entity2Operand.Value : null;
                        
                        result.Entity3Value = formula.Entity3Operand.HasValue ? Math.Floor(value / formula.FirstOperand.Value) * formula.Entity3Operand.Value : null ;
                        
                        result.Entity4Value = formula.Entity4Operand.HasValue ? Math.Floor(value / formula.FirstOperand.Value) * formula.Entity4Operand.Value : null;

                        break;
                    }
                case FormulaType.RANGE:
                    {
                        RangeValue rangeValue = formula.RangeOperand.Where(rv => (value >= rv.StartRange && value <= rv.EndRange)).FirstOrDefault();

                        if (rangeValue != default(RangeValue))
                        {
                            result.Value = rangeValue.Output;

                            result.Entity1Value = rangeValue.Entity1Output;

                            result.Entity2Value = rangeValue.Entity2Output;

                            result.Entity3Value = rangeValue.Entity3Output;

                            result.Entity4Value = rangeValue.Entity4Output;
                        }

                        break;
                    }
            }

            return result;
        }

        #endregion

    }
}
