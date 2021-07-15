import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { FormArray, FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { EventType} from '../models/incentive.model';
import { ContentProviderService } from '../services/content-provider.service';
import { IncentiveService } from '../services/incentive.service';
import { Output, EventEmitter } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { MatStepper } from '@angular/material/stepper';
import { ConfigService } from '../services/config.service';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatDialog } from '@angular/material/dialog';
import { AddEventDialog } from './add-event-dialog';
import { CommonDialogComponent } from '../common-dialog/common-dialog.component';

@Component({
  selector: 'app-add-incentive',
  templateUrl: './add-incentive.component.html',
  styleUrls: ['./add-incentive.component.css']
})
export class AddIncentiveComponent implements OnInit {

  @Input() audience: string;
  @Input() plan: any;
  @Output() newIncentiveEvent = new EventEmitter<any>();

  displayedColumns: string[] = ['eventType','eventTitle','contentProvider', 'ruleType', 'formula',  'incentive', 'milestoneTarget', 'edit', 'delete'];
  dataSource: MatTableDataSource<any>;
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  eventList = [];


  incentiveFormGroup: FormGroup;
  events: FormArray;
  ranges: FormArray;

  minStart: Date;
  minEnd: Date;

  eventTypes:any;
  selectedEventType: any;
  selectedRuleType;
  partners = [];
  contentProviders = [];


  partnerDisabled = false;
  isPublished = false;
  isDraft = false;



  constructor(
    private formBuilder: FormBuilder,
    private incentiveService: IncentiveService,
    private contentProviderService: ContentProviderService,
    private toastr: ToastrService,
    private configService: ConfigService,
    public dialog: MatDialog
  ) { 
    const currentYear = new Date().getFullYear();
    const currentMonth = new Date().getMonth();
    const currentDay = new Date().getDate();
    this.minStart = new Date(currentYear, currentMonth, currentDay);
    this.minEnd = new Date(currentYear, currentMonth, currentDay+1);
    
  }

  ngOnInit() {
    this.createEmptyFormDraft();
    var cpList = JSON.parse(localStorage.getItem('cpList'));
    if(!cpList || cpList.length < 1) {
      this.getContentProviders();
    }
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

    if(this.audience === "RETAILER") {
      this.setConfigForRetailer();
    } else {
      this.setConfigForConsumer();
    }
  }

  applyFilter(event: Event) {
    const filterValue = (event.target as HTMLInputElement).value;
    this.dataSource.filter = filterValue.trim().toLowerCase();

    if (this.dataSource.paginator) {
      this.dataSource.paginator.firstPage();
    }
  }

  getPartnerCodes(partners) {
    var partnerCodes = [];
    if(partners && Array.isArray(partners)){
      partners.forEach(partner => {
        partnerCodes.push(partner.partnerCode);
      })
    }
    return partnerCodes;
  }
  
  setConfigForRetailer() {
    this.configService.getRetailerPartners().subscribe(
      res => this.partners = this.getPartnerCodes(res),
      err => console.log(err)
    );
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
    this.incentiveFormGroup.get('name').setValue(plan.planName);
    this.incentiveFormGroup.get('type').setValue(plan.planType);
    if(this.audience === "RETAILER"){
      this.incentiveFormGroup.get('partner').setValue(plan.audience.subTypeName);
      this.partnerDisabled = true;
    } else {
      this.incentiveFormGroup.removeControl('partner');
    }
    this.incentiveFormGroup.get('startDate').setValue(plan.startDate);
    this.incentiveFormGroup.get('endDate').setValue(plan.endDate);

    if(plan.planDetails.length > 0) {
      this.eventList = plan.planDetails;
      this.dataSource = this.createDataSource(this.eventList);
      this.dataSource.paginator = this.paginator;
      this.dataSource.sort = this.sort;
    } else {
      this.dataSource = this.createDataSource([]);
    }
    
    if(plan.publishMode === 'DRAFT') {
      this.isDraft = true;
    } else {
      this.isDraft = false;
      this.isPublished = true;
    }
      
  }

  createDataSource(rawDataList) {
    var dataSource: any[] =[];
    if(rawDataList) {
      var index = 0;
      rawDataList.forEach( rawData => {
        var eventName = this.eventTypes.find(e => e.value === rawData.eventType)
        rawData.eventTypeName = eventName ? eventName.name : "NA";
        if(rawData.eventSubType) {
          var cpList = JSON.parse(localStorage.getItem('cpList'));
          if(cpList && cpList.length > 0) {
            var cp = cpList.find(cp => cp.id === rawData.eventSubType);
            rawData.eventSubTypeName = cp ? cp.name : "NA";
          } else {
            rawData.eventSubTypeName = "Loading...";
          }
        } else {
          rawData.eventSubTypeName = "NA";
        }
        rawData.index = index++;
        dataSource.push(rawData);
      });
    }
    return new MatTableDataSource(dataSource);
  }

  // createEventbyPlanDetails(eventArray, events) {
  //   events.forEach(event => {
  //     var eventForm = this.createEvent();
  //     if(event.formula.rangeOperand && event.formula.rangeOperand.length > 0) {
  //       var rangeArray = eventForm.get('ranges') as FormArray;
  //       eventForm.get('ranges').setValue(this.createRangeByDetails(rangeArray, event.formula.rangeOperand));
  //     }
  //     eventForm.get('eventType').setValue(event.eventType);
  //     eventForm.get('eventTitle').setValue(event.eventTitle);
  //     eventForm.get('contentProvider').setValue(event.eventSubType);
  //     eventForm.get('ruleType').setValue(event.ruleType);
  //     eventForm.get('formula').setValue(event.formula.formulaType);
  //     eventForm.get('target').setValue(event.formula.secondOperand);
  //     eventForm.get('incentive').setValue(event.formula.firstOperand);
  //     eventArray.push(eventForm);
  //   });

  // }

  // createRangeByDetails(rangeArray, ranges) {
  //   ranges.forEach( range => {
  //     var rangeForm = this.createRange();
  //     rangeForm.get('start').setValue(range.startRange);
  //     rangeForm.get('end').setValue(range.endRange);
  //     rangeForm.get('output').setValue(range.output);
  //     rangeArray.push(rangeForm);
  //   });
  //   rangeArray.remove(0);
  //   return rangeArray;
  // }

  // createConsumerFilledForm(plan) {

  // }
  createEmptyFormDraft() {
    this.incentiveFormGroup = this.formBuilder.group({
      name :  new FormControl('', [Validators.required]),
      type : new FormControl('',[Validators.required]),
      partner: new FormControl(''),
      startDate: new FormControl('',[Validators.required]),
      endDate: new FormControl('',[Validators.required]),
    });
    if(this.audience === "RETAILER") {
      this.addPartnerValidator();
    }
    this.dataSource = this.createDataSource([]);

  }

    removeEvent(index){
      this.dataSource.data.splice(index, 1);
      this.dataSource._updateChangeSubscription();
      this.dataSource.paginator = this.paginator;
    }



  // createEvent(): FormGroup {
  //     return this.formBuilder.group({
  //       eventType: new FormControl('',[Validators.required]),
  //       eventTitle: new FormControl('',[Validators.required]),
  //       contentProvider: new FormControl({value:'', disabled:true}),
  //       ruleType: new FormControl(''),
  //       formula: new FormControl('',[Validators.required]),
  //       target: new FormControl(''),
  //       incentive: new FormControl(''),
  //       ranges: this.formBuilder.array([this.createRange()])
  //   })
  // }

  addPartnerValidator() {
    this.incentiveFormGroup.get('partner').setValidators([Validators.required]);
  }

  changePlanType() {
      // if(this.incentiveFormGroup.get('type').value === "REGULAR") {
      //   this.dataSource.data.forEach( d => {
      //     d.ruleType = RuleType.SUM;
      //     d.formula.formulaType= '';
      //   });
      // } else {
      //   this.dataSource.data.forEach( d => {
      //     d.formula.formulaType= '';
      //     d.ruleType = '';
      //   });
      // }
      // this.dataSource._updateChangeSubscription();
      this.dataSource.data = [];
      this.dataSource._updateChangeSubscription();
      this.dataSource.paginator = this.paginator;
  }
  
    
  onPlanTypeChange() {
  if(this.dataSource.data.length > 0) {
    const dialogRef = this.dialog.open(CommonDialogComponent, {
      data: {
        heading: 'Confirm',
        message: "WARNING!!!! All events added to the existing plan type will be removed. Please click confirm to change the incentive plan Type.",
        action: "PROCESS",
        buttons: this.openSelectCPModalButtons()
      },
      maxHeight: '400px'
    });
  
  
    dialogRef.afterClosed().subscribe(result => {
      if (result === 'proceed') {
        this.changePlanType();
      }
      if (result === 'cancel') {
        this.incentiveFormGroup.get('type').setValue(this.incentiveFormGroup.get('type').value === "MILESTONE" ? "REGULAR" : "MILESTONE");
      }
    });
  }
 
}

openSelectCPModalButtons(): Array<any> {
  return [{
    label: 'Cancel',
    type: 'basic',
    value: 'cancel',
    class: 'discard-btn'
  },
  {
    label: 'Confirm',
    type: 'primary',
    value: 'submit',
    class: 'update-btn'
  }
  ]
}


  // createRange() : FormGroup {
  //   return this.formBuilder.group({
  //     start: new FormControl('', [Validators.pattern(/^-?(0|[1-9]\d*)?$/), Validators.min(1)]),
  //     end: new FormControl('', [Validators.pattern(/^-?(0|[1-9]\d*)?$/), Validators.min(1)]),
  //     incentive: new FormControl('', [Validators.pattern(/^-?(0|[1-9]\d*)?$/), Validators.min(1)])
  //   })
  // }



  getFormDetails() {
    var selectedEndDate;
    if(typeof this.incentiveFormGroup.get('endDate').value === 'string') {
      selectedEndDate = this.incentiveFormGroup.get('endDate').value;
    } else {
      selectedEndDate = this.incentiveFormGroup.get('endDate').value;
      selectedEndDate.setHours(selectedEndDate.getHours() + 23);
      selectedEndDate.setMinutes(selectedEndDate.getMinutes() + 59);
      selectedEndDate.setSeconds(selectedEndDate.getSeconds() + 59);
    }

    
    var incentivePlan = {
      "planName": this.incentiveFormGroup.get('name').value,
      "planType": this.incentiveFormGroup.get('type').value,
      "startDate": this.incentiveFormGroup.get('startDate').value,
      "endDate": selectedEndDate,
      "audience": {
        "audienceType": this.audience,
        "subTypeName": this.incentiveFormGroup.get('partner').value,
      },
      "planDetails": this.getEventsFromDS()
    }
  
  return incentivePlan;
  }

  getEventsFromDS() {
    var planDetails: any[] = [];
    this.dataSource.data.forEach( e => {
      var event = {
        eventType : e.eventType,
        eventTitle : e.eventTitle,
        eventSubType: e.eventSubType,
        formula: {
          formulaType: e.formula.formulaType,
          firstOperand: e.formula.firstOperand,
          secondOperand: e.formula.seondOperand,
          rangeOperand: e.formula.rangeOperand
        }
      }
      planDetails.push(event);
    })
    return planDetails;
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


  getContentProviders() {
    if(this.contentProviders.length === 0) {
      this.contentProviderService.getContentProviders().subscribe(
        res => {
          this.contentProviders = res;
          localStorage.setItem("cpList", JSON.stringify(res));
        },
        err => console.log(err)
      )
    }
    }


    changeDate() {
      var selectedEndDate = this.incentiveFormGroup.get('endDate').value;
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
          console.log(res);
          // this.newIncentiveEvent.emit();
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
          console.log(res);
          // this.newIncentiveEvent.emit();
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
          console.log(res);
          // this.newIncentiveEvent.emit();
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
          console.log(res);
          // this.newIncentiveEvent.emit();
        },
        err =>  {
          console.log(err);
          this.toastr.error(err);
        }
      );
    }

    isIncentiveFormNotValid() {
      return !this.incentiveFormGroup.valid;
    }

    isFormUpdated(){
      return this.incentiveFormGroup.dirty;
    }

    isEventFormNotValid(){
      return this.dataSource.data.length < 1;
    }

    updateDraftPlan() {
      var incentivePlan = this.getFormDetails();
      if(this.audience === "RETAILER") {
        this.updateRetailerIncentive(incentivePlan);
      } else {
        this.updateConsumerIncentive(incentivePlan)
      }
    }

    openDialog(event): void {
      const dialogRef = this.dialog.open(AddEventDialog, {
        width: '50%',
        data: {
          event: event,
          audience : this.audience,
          planType: this.incentiveFormGroup.get('type').value
        }
      });
  
      dialogRef.componentInstance.onEventCreate.subscribe(event => {
        var successMsg = "";
        var eventName = this.eventTypes.find(e => e.value === event.eventType);
        event.eventTypeName = eventName ? eventName.name : "NA";
        if(event.eventSubType) {
          var cpList = JSON.parse(localStorage.getItem('cpList'));
          if(cpList && cpList.length > 0) {
            var cp = cpList.find(cp => cp.id === event.eventSubType);
            event.eventSubTypeName = cp ? cp.name : "NA";
          } else {
            event.eventSubTypeName = "Loading...";
          }
        } else {
          event.eventSubTypeName = "NA";
        }
        if(event.index !== undefined) {
          this.dataSource.data.splice(event.index, 1, event);
          successMsg = "Event updated sucessfully!";
        } else {
          event.index = this.dataSource.data.length;
          this.dataSource.data.push(event);
          successMsg = "Event added successfully!";
        }
        this.dataSource._updateChangeSubscription();
        this.dataSource.paginator = this.paginator;
        this.toastr.success(successMsg);
        dialogRef.close();
      });
  
      dialogRef.afterClosed().subscribe(result => {
        console.log('The dialog was closed');
      });
    }
  

}
