import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { FormArray, FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { EventType, RetailerPartner, RuleType } from '../models/incentive.model';
import { ContentProviderService } from '../services/content-provider.service';
import { IncentiveService } from '../services/incentive.service';
import { Output, EventEmitter } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { MatStepper } from '@angular/material/stepper';

@Component({
  selector: 'app-add-incentive',
  templateUrl: './add-incentive.component.html',
  styleUrls: ['./add-incentive.component.css']
})
export class AddIncentiveComponent implements OnInit {

  @Input() audience: string;
  @Input() plan: any;
  @Output() newIncentiveEvent = new EventEmitter<any>();
  @ViewChild('stepper') stepper: MatStepper;


  incentiveFormGroup: FormGroup;
  eventsFormGroup: FormGroup;
  datesFormGroup: FormGroup;
  events: FormArray;
  ranges: FormArray;

  minStart: Date;
  minEnd: Date;

  eventTypes:any;
  selectedEventType: any;
  ruleTypes = [];
  selectedRuleType;
  step = 0;
  partners = [];
  contentProviders = [];
  // isCPDisabled= [];

  regularFormulas= [];
  milestoneFormulas =[];

  isLinear = false;

  panelOpenState= [];
  partnerDisabled = false;
  isPublished = false;
  isDraft = false;



  constructor(
    private formBuilder: FormBuilder,
    private incentiveService: IncentiveService,
    private contentProviderService: ContentProviderService,
    private toastr: ToastrService
  ) { 
    const currentYear = new Date().getFullYear();
    const currentMonth = new Date().getMonth();
    const currentDay = new Date().getDate();
    this.minStart = new Date(currentYear, currentMonth, currentDay);
    this.minEnd = new Date(currentYear, currentMonth, currentDay+1);
    
  }

  ngOnInit(): void {
    this.createEmptyFormDraft();
    this.getContentProviders();
    if(this.plan) {
      // call get plan details
      if(this.audience === "RETAILER") {
        this.incentiveService.getRetailerIncentivePlanByIdAndPartner(this.plan.id, this.plan.partner).subscribe(
          res => {
            console.log(res);
            this.createFilledForm(res.body);
          }
            ,
          err => console.log(err)
        )
      } else {
        this.incentiveService.getConsumerIncentivePlanById(this.plan.id).subscribe(
          res => {
            console.log(res);
            this.createFilledForm(res.body);
          },
          err => console.log(err)
        )
      }
  
    } 
    // else {
    //   this.createEmptyFormDraft();
    // }

    this.setFormulaConfig();
    if(this.audience === "RETAILER") {
      this.setConfigForRetailer();
    } else {
      this.setConfigForConsumer();
    }
  }

  setFormulaConfig(){
    this.regularFormulas = [
      "PLUS", 
      "MINUS",
      "MULTIPLY",
      "PERCENTAGE"
    ];
    this.milestoneFormulas = [
      "DIVIDE_AND_MULTIPLY", 
      "RANGE_AND_MULTIPLY"
    ];
    
  }

  setConfigForRetailer() {
    this.ruleTypes = [RuleType.SUM,RuleType.COUNT];
      this.partners = [RetailerPartner.NOVO, RetailerPartner.TSTP];
      this.eventTypes = [
        {
          name: 'Referral',
          value: EventType['Retailer Referral']
        },
        {
          name: 'Order Complete',
          value: EventType['Retailer Order Complete']
        },
      ]
  }

  setConfigForConsumer() {
    this.ruleTypes = [RuleType.COUNT, RuleType.SUM];
      this.eventTypes = [
        {
          name: 'App Open Once a Day',
          value: EventType['Consumer App Open Once a Day']
        },
        {
          name: 'First Sign-In',
          value: EventType['Consumer First Sign-In']
        },
        {
          name: 'Order Complete',
          value: EventType['Consumer Order Complete']
        },
        {
          name: 'Redeem Subscription',
          value: EventType['Consumer Redeem Subscription']
        },
      ]
  }

  createFilledForm(plan: any) {
    // this.incentiveFormGroup = this.formBuilder.group({
    //   name :  new FormControl(plan.planName),
    //   type : new FormControl(plan.planType),
    //   partner: new FormControl({value: plan.audience.subTypeName, disabled: true}),
    // });
    
    this.incentiveFormGroup.get('name').setValue(plan.planName);
    this.incentiveFormGroup.get('type').setValue(plan.planType);
    this.incentiveFormGroup.get('partner').setValue(plan.audience.subTypeName);
    this.partnerDisabled = true;

    this.eventsFormGroup = this.formBuilder.group({
      events: this.formBuilder.array([])
    });
    var eventArray = this.eventsFormGroup.get("events") as FormArray;
    this.createEventbyPlanDetails(eventArray, plan.planDetails);

    this.datesFormGroup = this.formBuilder.group({
      startDate: new FormControl(new Date(plan.startDate),[Validators.required]),
      endDate: new FormControl(new Date(plan.endDate),[Validators.required]),

    })

    if(plan.publishMode === 'DRAFT') {
      // this.createEmptyFormDraft();
      this.isDraft = true;
    } else {
      setTimeout(() => {           
        this.stepper.next();
       }, 1);
      this.isPublished = true;
       setTimeout(() => {           
        this.stepper.next();
       }, 1);

    }
  }

  createEventbyPlanDetails(eventArray, events) {

    events.forEach(event => {
      var eventForm = this.createEvent();
      if(event.formula.rangeOperand && event.formula.rangeOperand.length > 0) {
        var rangeArray = eventForm.get('ranges') as FormArray;
        eventForm.get('ranges').setValue(this.createRangeByDetails(rangeArray, event.formula.rangeOperand));
      }
      eventForm.get('eventType').setValue(event.eventType);
      eventForm.get('eventTitle').setValue(event.eventTitle);
      eventForm.get('contentProvider').setValue(event.eventSubType);
      eventForm.get('ruleType').setValue(event.ruleType);
      eventForm.get('formula').setValue(event.formula.formulaType);
      eventForm.get('target').setValue(event.formula.secondOperand);
      eventForm.get('incentive').setValue(event.formula.firstOperand);
      eventArray.push(eventForm);
    });

  }

  createRangeByDetails(rangeArray, ranges) {
    ranges.forEach( range => {
      // var rangeForm = this.formBuilder.group({
      //   start: new FormControl(range.start, [Validators.pattern(/^-?(0|[1-9]\d*)?$/)]),
      //   end: new FormControl(range.end, [Validators.pattern(/^-?(0|[1-9]\d*)?$/)]),
      //   incentive: new FormControl(range.output, [Validators.pattern(/^-?(0|[1-9]\d*)?$/)])
      // });
      var rangeForm = this.createRange();
      rangeForm.get('start').setValue(range.startRange);
      rangeForm.get('end').setValue(range.endRange);
      rangeForm.get('output').setValue(range.output);
      rangeArray.push(rangeForm);
    });
    rangeArray.remove(0);
    return rangeArray;
  }

  createConsumerFilledForm(plan) {

  }
  createEmptyFormDraft() {
    this.incentiveFormGroup = this.formBuilder.group({
      name :  new FormControl('', [Validators.required]),
      type : new FormControl('',[Validators.required]),
      partner: new FormControl(''),
    });
    if(this.audience === "RETAILER") {
      this.addPartnerValidator();
    }
    this.eventsFormGroup = this.formBuilder.group({
      events: this.formBuilder.array([ this.createEvent() ])
    });
      
    this.datesFormGroup = this.formBuilder.group({
      startDate: new FormControl('',[Validators.required]),
      endDate: new FormControl('',[Validators.required]),

    });
    
  }

  // createEmptyFormPublished() {
  //   this.incentiveFormGroup = this.formBuilder.group({
  //     name :  new FormControl({value:'', disabled: true}),
  //     type : new FormControl({value:'', disabled: true}),
  //     partner: new FormControl({value:'', disabled: true}),
  //   });
  //   this.eventsFormGroup = this.formBuilder.group({
  //     events: this.formBuilder.array([ this.createEventDisabled() ])
  //   });
      
  //   this.datesFormGroup = this.formBuilder.group({
  //     startDate: new FormControl({value:'', disabled: true},[Validators.required]),
  //     endDate: new FormControl('',[Validators.required]),

  //   })
  // }

  isMileStonePlanType() {
    return this.incentiveFormGroup.get('type').value === 'MILESTONE';
  }

  isRangeFormulaType(eventNumber) {
    var events = this.eventsFormGroup.get('events') as FormArray
    return events.controls[eventNumber].get('formula').value.includes("RANGE");
  }

  isDivideFormulaType(eventNumber) {
    var events = this.eventsFormGroup.get('events') as FormArray
    return events.controls[eventNumber].get('formula').value.includes("DIVIDE");
  }

  getTotalRanges(eventNumber) {
    var events = this.eventsFormGroup.get('events') as FormArray
    var ranges = events.controls[eventNumber].get('ranges')  as FormArray;
    return ranges.length;
  }
  getTotalEvents() {
    var events = this.eventsFormGroup.get('events') as FormArray;
    return events.length;
  }

  getEvents() {
    var event = this.eventsFormGroup.get('events') as FormArray;
    return event.controls;
  }

  getRanges(eventIndex) {
    var events = this.eventsFormGroup.get('events') as FormArray
    var ranges =  events.controls[eventIndex].get('ranges')  as FormArray;
    return ranges.controls;
  }
  
  removeRange(j, i){
    var events = this.eventsFormGroup.get('events') as FormArray
    this.ranges = events.controls[i].get('ranges') as FormArray;
    this.ranges.removeAt(j);
 }

 removeEvent(i){
  var events = this.eventsFormGroup.get('events') as FormArray;
  events.removeAt(i);
  this.panelOpenState = this.panelOpenState.splice(i,1);
  // this.isCPDisabled = this.isCPDisabled.splice(i, 1);

 }

 setPanelOpenState(i, status) {
  this.panelOpenState[i] = status;
 }

 getPanelOpenState(i) {
  return this.panelOpenState[i];
 }

  createEvent(): FormGroup {
      return this.formBuilder.group({
        eventType: new FormControl('',[Validators.required]),
        eventTitle: new FormControl('',[Validators.required]),
        contentProvider: new FormControl({value:'', disabled:true}),
        ruleType: new FormControl(''),
        formula: new FormControl('',[Validators.required]),
        target: new FormControl(''),
        incentive: new FormControl(''),
        ranges: this.formBuilder.array([this.createRange()])
    })
  }

//   createEventDisabled(): FormGroup {
//     return this.formBuilder.group({
//       eventType: new FormControl({value:'', disabled: true},[Validators.required]),
//       eventTitle: new FormControl({value:'', disabled: true},[Validators.required]),
//       contentProvider: new FormControl({value:'', disabled:true}),
//       ruleType: new FormControl({value:'', disabled: true}),
//       formula: new FormControl({value:'', disabled: true},[Validators.required]),
//       target: new FormControl({value:'', disabled: true}, [ Validators.pattern(/^-?(0|[1-9]\d*)?$/)]),
//       incentive: new FormControl({value:'', disabled: true}, [Validators.pattern(/^-?(0|[1-9]\d*)?$/)]),
//       ranges: this.formBuilder.array([this.createRangeDisabled()])
//   })
// }

// createRangeDisabled() : FormGroup {
//   return this.formBuilder.group({
//     start: new FormControl({value:'', disabled: true}, [Validators.pattern(/^-?(0|[1-9]\d*)?$/)]),
//     end: new FormControl({value:'', disabled: true}, [Validators.pattern(/^-?(0|[1-9]\d*)?$/)]),
//     incentive: new FormControl({value:'', disabled: true}, [Validators.pattern(/^-?(0|[1-9]\d*)?$/)])
//   })
// }


  addPartnerValidator() {
    this.incentiveFormGroup.get('partner').setValidators([Validators.required]);
  }


  onPlanTypeChange() {
    var events = this.eventsFormGroup.get('events') as FormArray;
    events.controls.forEach((e, i) => {
      events.controls[i].get('formula').setValue('');
      events.controls[i].get('formula').setValidators([Validators.required]);
      if(this.incentiveFormGroup.get('type').value === "MILESTONE") {
        events.controls[i].get('ruleType').setValidators([Validators.required]);
      } else {
        events.controls[i].get('ruleType').clearValidators();
      }
    })
    
  }
  onFomulaChange(eventNumber) {
    var events = this.eventsFormGroup.get('events') as FormArray;
    // var eventType = this.events.controls[eventNumber].get('eventType');
    var formula = events.controls[eventNumber].get('formula');
    // if(eventType.value.includes('Order')) {
      
    // } else {
    //   this.events.controls[eventNumber].get('contentProvider').clearValidators();
    // }
    // if(this.incentiveFormGroup.get('type').value === "MILESTONE") {
    //   events.controls[eventNumber].get('ruleType').setValidators([Validators.required]);
    // } else {
    //   events.controls[eventNumber].get('ruleType').clearValidators();
    // }

    if(formula.value === "DIVIDE_AND_MULTIPLY") {
      events.controls[eventNumber].get('target').setValidators([Validators.required,Validators.pattern(/^-?(0|[1-9]\d*)?$/)]);
      events.controls[eventNumber].get('incentive').setValidators([Validators.required,Validators.pattern(/^-?(0|[1-9]\d*)?$/)]);
    } else if(formula.value === "RANGE_AND_MULTIPLY") {
      //
      events.controls[eventNumber].get('target').clearValidators();
      events.controls[eventNumber].get('incentive').clearValidators();
    } else {
      events.controls[eventNumber].get('incentive').setValidators([Validators.required,Validators.pattern(/^-?(0|[1-9]\d*)?$/)]);
    }


  }

  createRange() : FormGroup {
    return this.formBuilder.group({
      start: new FormControl('', [Validators.pattern(/^-?(0|[1-9]\d*)?$/)]),
      end: new FormControl('', [Validators.pattern(/^-?(0|[1-9]\d*)?$/)]),
      incentive: new FormControl('', [Validators.pattern(/^-?(0|[1-9]\d*)?$/)])
    })
  }

  addEvent(): void {
    var events = this.eventsFormGroup.get('events') as FormArray;
    var event = this.createEvent();
    if(this.incentiveFormGroup.get('type').value === "MILESTONE") {
      event.get('ruleType').setValidators([Validators.required]);
    } else {
      event.get('ruleType').clearValidators();
    }
    events.push(event);
    this.eventsFormGroup.markAsUntouched();
    this.panelOpenState.fill(false, 0, this.getTotalEvents()-1);
    this.panelOpenState.push(true);
    this.nextStep();
    // this.isCPDisabled.push(false);
  }


  addRange(eventNumber): void {
    this.events = this.eventsFormGroup.get('events') as FormArray
    this.ranges = this.events.controls[eventNumber].get('ranges') as FormArray;
    this.ranges.push(this.createRange());
  }

  onEventTypeChange(eventNumber) {
    var events = this.eventsFormGroup.get('events') as FormArray;
    // if(events.controls[eventNumber].get('eventType').value.includes("ORDER")){
    //   this.getContentProviders(eventNumber);
    // } else {
    //   this.isCPDisabled[eventNumber] = true;
    //   var events = this.eventsFormGroup.get('events') as FormArray;
    //   events.controls[eventNumber].get('contentProvider').setValue(null);
      
    // }
    var events = this.eventsFormGroup.get('events') as FormArray;
    if(!events.controls[eventNumber].get('eventType').value.includes("ORDER")){
      // this.isCPDisabled[eventNumber] = true;
      events.controls[eventNumber].get('contentProvider').setValue(null);
      events.controls[eventNumber].get('contentProvider').clearValidators();
    } else {
      events.controls[eventNumber].get('contentProvider').setValidators([Validators.required]);
    }
  }

  isCPDisabledForEvent(eventNumber) {
    // return this.isCPDisabled[eventNumber];
    var events = this.eventsFormGroup.get('events') as FormArray;
    return !events.controls[eventNumber].get('eventType').value.includes('ORDER');
  }

  getFormDetails() {
    var selectedEndDate = this.datesFormGroup.get('endDate').value;
    selectedEndDate.setHours(selectedEndDate.getHours() + 23);
    selectedEndDate.setMinutes(selectedEndDate.getMinutes() + 59);
    selectedEndDate.setSeconds(selectedEndDate.getSeconds() + 59);
    var planDetails = this.createPlanDetails();
    var incentivePlan = {
      "planName": this.incentiveFormGroup.get('name').value,
      "planType": this.incentiveFormGroup.get('type').value,
      "startDate": this.datesFormGroup.get('startDate').value,
      "endDate": selectedEndDate,
      "audience": {
        "audienceType": this.audience,
        "subTypeName": this.incentiveFormGroup.get('partner').value,
      },
      "planDetails": planDetails
    }
  
  return incentivePlan;
  }
  
  createIncentive() {
    var incentivePlan = this.getFormDetails();
    if(this.audience === "RETAILER") {
      this.createRetailerIncentive(incentivePlan);
    } else {
      this.createConsumerIncentive(incentivePlan)
    }
    
  }

  createRetailerIncentive(incentivePlan) {
    this.incentiveService.createIncentivePlanRetailer(incentivePlan).subscribe(
      res => {
        this.toastr.success("Retailer Incentive plan drafted successfully");
        console.log(res),
        this.newIncentiveEvent.emit();
      },
      err =>  {
        console.log(err);
        this.toastr.error(err);
      }
    );
  }


  createConsumerIncentive(incentivePlan) {
    this.incentiveService.createIncentivePlanConsumer(incentivePlan).subscribe(
      res => {
        this.toastr.success("Consumer Incentive plan drafted successfully");
        console.log(res),
        this.newIncentiveEvent.emit();
      },
      err =>  {
        console.log(err);
        this.toastr.error(err);
      }
    );
  }

  displayIncentivePlan() {
    this.plan = null;
    this.newIncentiveEvent.emit();
  }

  createPlanDetails() {
    var planDetails: any[] = [];
    var events = this.eventsFormGroup.get('events') as FormArray;
    events.controls.forEach( event => {
      var eventDetail: any = {};
      eventDetail.eventType = event.get('eventType').value;
      eventDetail.eventTitle = event.get('eventTitle').value;
      eventDetail.eventSubType = event.get('contentProvider').value;
      eventDetail.ruleType = event.get('ruleType').value ? event.get('ruleType').value : 'SUM';
      // Formula details
      var formula: any = {};
      formula.formulaType = event.get('formula').value;
      formula.secondOperand = event.get('target').value ? event.get('target').value : null;
      formula.firstOperand = event.get('incentive').value;
      // rangeOperand Details
      var rangeOperand: any[] = [];
      if(event.get('formula').value.includes('RANGE')) {
        formula.firstOperand = null;
        formula.secondOperand = null;
        rangeOperand = this.createRangeDetails(event);
      }
      formula.rangeOperand = rangeOperand;
      // event-formula details
      eventDetail.formula = formula;
      planDetails.push(eventDetail);
    });
    return planDetails;
  }

  createRangeDetails(event) {
    var ranges = event.get('ranges')  as FormArray;
    var rangeDetails: any[] = [];
    ranges.controls.forEach(range => {
      var rangeDetail: any = {};
      rangeDetail.startRange = range.get('start').value;
      rangeDetail.endRange = range.get('end').value;
      rangeDetail.output = range.get('incentive').value;
      rangeDetails.push(rangeDetail);
    })
    return rangeDetails;
  }

  setStep(index: number) {
    this.step = index;
  }

  nextStep() {
    this.panelOpenState[this.step] = false;
    this.step++;
    this.panelOpenState[this.step] = true;
  }

  prevStep() {
    this.panelOpenState[this.step] = false;
    this.step--;
    this.panelOpenState[this.step] = true;
  }


  getSelectedEventType(i){
    var events = this.eventsFormGroup.get('events') as FormArray;
    if(events.controls[i].get('eventType').value) {
      return this.eventTypes.find(event => (event.value === events.controls[i].get('eventType').value)).name;
    }
    
  }
  getSelectedCP(i){
    var events = this.eventsFormGroup.get('events') as FormArray;
    return this.contentProviders.find(cp => cp.id === events.controls[i].get('contentProvider').value).name;
  
  }
  getSelectedRuleType(i){
    var events = this.eventsFormGroup.get('events') as FormArray;
    return events.controls[i].get('ruleType').value;
  
  }
  getSelectedFormulaType(i){
    var events = this.eventsFormGroup.get('events') as FormArray;
    return events.controls[i].get('formula').value;
  }

  isFormulaTypeRange(i) {
    return this.getSelectedFormulaType(i) === this.milestoneFormulas[1];
  }

  isFormulaTypeDivide(i) {
    return this.getSelectedFormulaType(i) === this.milestoneFormulas[0];
  }

  getSelectedTarget(i){
    var events = this.eventsFormGroup.get('events') as FormArray;
    return events.controls[i].get('target').value;
  }


  getSelectedIncentive(i){
    var events = this.eventsFormGroup.get('events') as FormArray;
    return events.controls[i].get('incentive').value;
  
  }

  getContentProviders() {
    if(this.contentProviders.length === 0) {
      this.contentProviderService.getContentProviders().subscribe(
        res => {
          this.contentProviders = res;
          // this.isCPDisabled[eventNumber] = false;
        },
        err => console.log(err)
      )
    }
    // this.isCPDisabled[eventNumber] = false;
    }


    changeDate() {
      var selectedEndDate = this.datesFormGroup.get('endDate').value;
      // selectedEndDate.setHours(selectedEndDate.getHours() + 23);
      // selectedEndDate.setMinutes(selectedEndDate.getMinutes() + 59);
      // selectedEndDate.setSeconds(selectedEndDate.getSeconds() + 59);
      var month = (selectedEndDate.getMonth()+'').length == 1 ? 
                  '0'+ selectedEndDate.getMonth() : selectedEndDate.getMonth();

      var newEndDateUTCString = selectedEndDate.getFullYear() + '-' +
                                month + '-' +
                                selectedEndDate.getDate() + 'T23:59:59Z';
      if(this.audience === "RETAILER") {
        var partner = this.incentiveFormGroup.get('partner').value;
        this.changeRetailerPlanEndDate(partner, newEndDateUTCString);
      } else {
        this.changeConsumerPlanEndDate(newEndDateUTCString)
      }
    }

    
    changeRetailerPlanEndDate(partner, endDate) {
      this.incentiveService.changeDateRetailerIncentivePlan(this.plan.id, partner, endDate).subscribe(
        res => {
          this.toastr.success("Retailer Incentive plan end date updated successfully");
          console.log(res),
          this.newIncentiveEvent.emit();
        },
        err =>  {
          console.log(err);
          this.toastr.error(err);
        }
      );
    }

    changeConsumerPlanEndDate(endDate) {
      this.incentiveService.changeDateConsumerIncentivePlan(this.plan.id, endDate).subscribe(
        res => {
          this.toastr.success("Consumer Incentive plan end date updated successfully");
          console.log(res),
          this.newIncentiveEvent.emit();
        },
        err =>  {
          console.log(err);
          this.toastr.error(err);
        }
      );
    }


    publishPlan() {
      if(this.audience === "RETAILER") {
        var partner = this.incentiveFormGroup.get('partner').value;
        this.publishRetailerIncentive(partner);
      } else {
        this.publishConsumerIncentive();
      }
    }

    publishRetailerIncentive(partner) {
      this.incentiveService.publishRetailerIncentivePlan(this.plan.id, partner).subscribe(
        res => {
          this.toastr.success("Retailer Incentive plan published successfully");
          console.log(res),
          this.newIncentiveEvent.emit();
        },
        err =>  {
          console.log(err);
          this.toastr.error(err);
        }
      );
    }

    publishConsumerIncentive(){
      this.incentiveService.publishConsumerIncentivePlan(this.plan.id).subscribe(
        res => {
          this.toastr.success("Consumer Incentive plan published successfully");
          console.log(res),
          this.newIncentiveEvent.emit();
        },
        err =>  {
          console.log(err);
          this.toastr.error(err);
        }
      );
    }


    updateRetailerIncentive(incentivePlan) {
      this.incentiveService.updateRetailerDraftPlan(this.plan.id,incentivePlan).subscribe(
        res => {
          this.toastr.success("Retailer Incentive draft plan updated successfully");
          console.log(res),
          this.newIncentiveEvent.emit();
        },
        err =>  {
          console.log(err);
          this.toastr.error(err);
        }
      );
    }

    updateConsumerIncentive(incentivePlan){
      this.incentiveService.updateConsumerDraftPlan(this.plan.id, incentivePlan).subscribe(
        res => {
          this.toastr.success("Consumer Incentive draft plan updated successfully");
          console.log(res),
          this.newIncentiveEvent.emit();
        },
        err =>  {
          console.log(err);
          this.toastr.error(err);
        }
      );
    }

    isFormUpdated() {
      return this.incentiveFormGroup.dirty || this.eventsFormGroup.dirty || this.datesFormGroup.dirty;
    }

    updateDraftPlan() {
      var incentivePlan = this.getFormDetails();
      if(this.audience === "RETAILER") {
        this.updateRetailerIncentive(incentivePlan);
      } else {
        this.updateConsumerIncentive(incentivePlan)
      }
    }

}
