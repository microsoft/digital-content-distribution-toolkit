import { Component, Input, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { EventType, FormulaType, RetailerPartner, RuleType } from '../models/incentive.model';
import { ContentProviderService } from '../services/content-provider.service';
import { IncentiveService } from '../services/incentive.service';

@Component({
  selector: 'app-add-incentive',
  templateUrl: './add-incentive.component.html',
  styleUrls: ['./incentive-management.component.css']
})
export class AddIncentiveComponent implements OnInit {

  @Input() audience: string;

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
  selectedRetailerPartner;
  contentProviders = [];
  isCPDisabled= true;

  regularFormulas= [];
  milestoneFormulas =[];

  isLinear = false;
  panelOpenState = false;

  constructor(
    private formBuilder: FormBuilder,
    private incentiveService: IncentiveService,
    private contentProviderService: ContentProviderService
  ) { 
    const currentYear = new Date().getFullYear();
    const currentMonth = new Date().getMonth();
    const currentDay = new Date().getDate();
    this.minStart = new Date(currentYear, currentMonth, currentDay);
    this.minEnd = new Date(currentYear, currentMonth, currentDay+1);
    
  }

  ngOnInit(): void {
    this.regularFormulas = ["PLUS", "MINUS", "MULTIPLY", "PERCENTAGE"];
    this.milestoneFormulas = ["DIVIDE", "RANGE"];
    if(this.audience === "Retailer") {
      this.ruleTypes = [RuleType.SUM];
      this.partners = [RetailerPartner.NOVO, RetailerPartner.TSTP];
      this.eventTypes = [
        {
          title: 'Referral',
          value: EventType['Retailer Referral']
        },
        {
          title: 'Order Complete',
          value: EventType['Retailer Order Complete']
        },
      ]


    } else {
      this.ruleTypes = [RuleType.COUNT, RuleType.SUM];
      this.eventTypes = [
        {
          title: 'App Open Once a Day',
          value: EventType['Consumer App Open Once a Day']
        },
        {
          title: 'First Sign-In',
          value: EventType['Consumer First Sign-In']
        },
        {
          title: 'Order Complete',
          value: EventType['Consumer Order Complete']
        },
        {
          title: 'Redeem Subscription',
          value: EventType['Consumer Redeem Subscription']
        },
      ]
    }

    
    this.incentiveFormGroup = this.formBuilder.group({
      name :  new FormControl('', [Validators.required, Validators.maxLength(10)]),
      type : new FormControl('',[Validators.required]),
      partner: new FormControl('', [Validators.required, Validators.maxLength(10)]),
    });
    this.eventsFormGroup = this.formBuilder.group({
      events: this.formBuilder.array([ this.createEvent() ])
    });
    this.datesFormGroup = this.formBuilder.group({
      startDate: new FormControl('',[Validators.required]),
      endDate: new FormControl('',[Validators.required]),

    })

  }

  isMileStonePlanType() {
    return this.incentiveFormGroup.get('type').value === 'MILESTONE';
  }

  isRangeFormulaType(eventNumber) {
    var events = this.eventsFormGroup.get('events') as FormArray
    return events.controls[eventNumber].get('formula').value === "RANGE";
  }

  isLastRange(eventNumber, currentRangeIndex) {
    var events = this.eventsFormGroup.get('events') as FormArray
    var ranges = events.controls[eventNumber].get('ranges')  as FormArray;
    return currentRangeIndex == ranges.length-1;
  }

  getEvents() {
    var event = this.eventsFormGroup.get('events') as FormArray;
    return event.controls;
  }

  getRanges(eventIndex) {
    var events = this.eventsFormGroup.get('events') as FormArray
    var ranges =  events.controls[eventIndex].get('ranges')  as FormArray;
    return ranges.controls
  }
  
  removeRange(j){
    this.events = this.eventsFormGroup.get('events') as FormArray
    this.ranges = this.events.controls[j].get('ranges') as FormArray;
    this.ranges.removeAt(j);
 }

 removeEvent(i){
  this.events = this.eventsFormGroup.get('events') as FormArray;
  this.events.removeAt(i);

 }

  createEvent(): FormGroup {
      return this.formBuilder.group({
        eventType: new FormControl('',[Validators.required]),
        contentProviderId: new FormControl(''),
        ruleType: new FormControl('',[Validators.required]),
        formula: new FormControl('',[Validators.required]),
        target: new FormControl(''),
        incentive: new FormControl('', [Validators.required]),
        ranges: this.formBuilder.array([this.createRange()])
    })
  }

  createRange() : FormGroup {
    return this.formBuilder.group({
      start: new FormControl('', [Validators.required]),
      end: new FormControl('', [Validators.required]),
      incentive: new FormControl('', [Validators.required])
    })
  }

  addEvent(): void {
    this.events = this.eventsFormGroup.get('events') as FormArray;
    this.events.push(this.createEvent());
  }

  addRange(eventNumber): void {
    this.events = this.eventsFormGroup.get('events') as FormArray
    this.ranges = this.events.controls[eventNumber].get('ranges') as FormArray;
    this.ranges.push(this.createRange());
  }

  onOrderCompleteEvent(eventNumber) {
    if(this.selectedEventType?.title.includes("Order Complete")){
      this.getContentProviders();
    } else {
      this.isCPDisabled = true;
      var events = this.eventsFormGroup.get('events') as FormArray
      events.controls[eventNumber].get('contentProviderId').setValue(null);
      
    }
  }

  
  createIncentive() {
    var incentivePlan = {
      "planName": "UIPlan1",
      "planType": "REGULAR",
      "startDate": "2021-11-17T16:34:09.672Z",
      "endDate": "2021-11-17T16:34:09.672Z",
      "audience": {
        "audienceType": "RETAILER",
        "subTypeName": "NOVO"
      },
      "planDetails": [
        {
          "eventType": "RETAILER_INCOME_ORDER_COMPLETED",
          "eventSubType": localStorage.getItem("contentProviderId"),
          "eventTitle": "Order Complete Incentive",
          "ruleType": "SUM",
          "formula": {
            "formulaType": "PLUS",
            "leftOperand": 10,
            "rightOperand": 0,
            "rangeOperand": [
              {
                "startRange": 0,
                "endRange": 0,
                "output": 0
              }
            ]
          }
        }
      ]
    }
    this.incentiveService.createIncentivePlanRetailer(incentivePlan).subscribe(
      res =>
      console.log(res),
      err => console.log(err)
    );

  }

  setStep(index: number) {
    this.step = index;
  }

  nextStep() {
    this.step++;
  }

  prevStep() {
    this.step--;
  }


  getContentProviders() {
    if(this.contentProviders.length === 0) {
      this.contentProviderService.getContentProviders().subscribe(
        res => {
          this.contentProviders = res;
          this.isCPDisabled = false;
        },
        err => console.log(err)
      )
    }
    this.isCPDisabled = false;
    }
}
