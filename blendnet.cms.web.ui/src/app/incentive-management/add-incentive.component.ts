import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { FormArray, FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { FormulaType, PublishMode, RuleType} from '../models/incentive.model';
import { ContentProviderService } from '../services/content-provider.service';
import { IncentiveService } from '../services/incentive.service';
import { Output, EventEmitter } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatDialog } from '@angular/material/dialog';
import { AddEventDialog } from './add-event-dialog';
import { CommonDialogComponent } from '../common-dialog/common-dialog.component';
import { RetailerService } from '../services/retailer.service';

@Component({
  selector: 'app-add-incentive',
  templateUrl: './add-incentive.component.html',
  styleUrls: ['./add-incentive.component.css']
})
export class AddIncentiveComponent implements OnInit {

  @Input() audience: string;
  @Input() plan: any;
  @Output() newIncentiveEventR = new EventEmitter<any>();
  @Output() newIncentiveEventC = new EventEmitter<any>();


  displayedColumns: string[] = ['eventType','eventTitle','contentProvider', 'ruleType', 'formula',  'firstOperand', 'secondOperand', 'edit', 'delete'];
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


  disablePlanTypePartner = false;
  isPublished = false;
  isDraft = false;



  constructor(
    private formBuilder: FormBuilder,
    private incentiveService: IncentiveService,
    private contentProviderService: ContentProviderService,
    private toastr: ToastrService,
    private retailerService: RetailerService,
    public dialog: MatDialog
  ) { 
    this.minStart = new Date();
    this.minEnd = new Date();
    this.minEnd.setDate(this.minEnd.getDate() +1);
    
  }

  ngOnInit() {
    this.createEmptyFormDraft();
    var cpList = JSON.parse(sessionStorage.getItem("CONTENT_PROVIDERS"));
    if(!cpList || cpList.length < 1) {
      this.contentProviderService.browseContentProviders().subscribe(
        res => {
          sessionStorage.setItem("CONTENT_PROVIDERS", JSON.stringify(res));
          this.initializeValues();
        },
        err => this.toastr.error("Unable to fetch content providers. Please contact admin")
      )
    } else {
      this.initializeValues();
    }
  }

  initializeValues() {
    if(this.plan) {
      // call get plan details
      if(this.audience === "RETAILER") {
        this.incentiveService.getRetailerIncentivePlanByIdAndPartner(this.plan.id, this.plan.partner).subscribe(
          res => {
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
    if(sessionStorage.getItem("RETAILER_PARTNERS")) {
      this.partners = JSON.parse(sessionStorage.getItem('RETAILER_PARTNERS'));
    } else {
      this.retailerService.getRetailerPartners().subscribe(
        res => {
          this.partners = this.getPartnerCodes(res);
          sessionStorage.setItem("RETAILER_PARTNERS", JSON.stringify(this.partners));
        },
        err => console.log(err)
      );
    }
   
  }

  setConfigForConsumer() {
    // No config for customer plans
  }

  createFilledForm(plan: any) {    
    this.incentiveFormGroup.get('name').setValue(plan.planName);
    this.incentiveFormGroup.get('type').setValue(plan.planType);
    if(this.audience === "RETAILER"){
      this.incentiveFormGroup.get('partner').setValue(plan.audience.subTypeName);
      //this.partnerDisabled = true;
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
      this.disablePlanTypePartner = true;
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
        // var eventName = this.eventTypes.find(e => e.value === rawData.eventType)
        // rawData.eventTypeName = eventName ? eventName.name : "NA";
        if(rawData.eventSubType) {
          var cpList = JSON.parse(sessionStorage.getItem("CONTENT_PROVIDERS"));
          if(cpList && cpList.length > 0) {
            var cp = cpList.find(cp => cp.contentProviderId === rawData.eventSubType);
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


  createEmptyFormDraft() {
    this.incentiveFormGroup = this.formBuilder.group({
      name :  new FormControl('', [Validators.required, Validators.pattern(/^[^\s].*[^\s]$/)]),
      type : new FormControl('REGULAR',[Validators.required]),
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

  addPartnerValidator() {
    this.incentiveFormGroup.get('partner').setValidators([Validators.required]);
  }

  changePlanType() {
      this.dataSource.data = [];
      this.dataSource._updateChangeSubscription();
      this.dataSource.paginator = this.paginator;
  }
  
  disablePlanChange() {
    return this.isPublished || this.disablePlanTypePartner;
  }
  
  onPlanTypeChange($event) {
  if(this.isPublished || this.disablePlanTypePartner) {
    $event.stopPropagation();
  } else if(this.dataSource.data.length > 0) {
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
        "subTypeName": this.incentiveFormGroup.get('partner') ? this.incentiveFormGroup.get('partner').value : "CONSUMER",
      },
      "planDetails": this.getEventsFromDS()
    }
  
  return incentivePlan;
  }

  getEventsFromDS() {
    var planDetails: any[] = [];
    this.dataSource.data.forEach( e => {
      var event: any= {
        eventType : e.eventType,
        eventTitle : e.eventTitle,
        ruleType: e.ruleType,
        formula: {
          formulaType: e.formula.formulaType,
          firstOperand: e.formula.firstOperand,
          secondOperand: e.formula.secondOperand,
          rangeOperand: e.formula.rangeOperand
        }
      }
      if(e.eventSubType && e.eventSubType !== '') {
        event.eventSubType = e.eventSubType
      }
      planDetails.push(event);
    })
    if(this.audience === 'CONSUMER' && this.plan?.status !== PublishMode.DRAFT) {
      if(sessionStorage.getItem('CONSUMER_EVENTS')) {
        this.eventTypes = JSON.parse(sessionStorage.getItem('CONSUMER_EVENTS'))
      } else {
        this.incentiveService.getEventList("CONSUMER").subscribe(
          res => {
            this.eventTypes = res.body;
            sessionStorage.setItem("CONSUMER_EVENTS",  JSON.stringify(res.body));
          },
          err => console.log(err)
        );
      }

      this.eventTypes.forEach(type => {
          if(type.includes("EXPENSE")) {
            var event: any = {
              eventType: type,
              eventTitle: "Expense",
              ruleType: RuleType.SUM,
              formula : {
                formulaType: FormulaType.MINUS
              }
            }
            planDetails.push(event)
          }
        });
      }
    
    return planDetails;
  }

  isExpensePlan(plan) {
    return plan.eventType.includes("EXPENSE");
  }

  createDefaultExpenseEvents() {
    var events: any[] = [];
    if(sessionStorage.getItem('CONSUMER_EVENTS') && JSON.parse(sessionStorage.getItem('CONSUMER_EVENTS')).length > 0) {
      JSON.parse(sessionStorage.getItem('CONSUMER_EVENTS')).forEach(type => {
        if(type.includes("EXPENSE")) {
          // ['eventType','eventTitle','contentProvider', 'ruleType', 'formula',  'firstOperand', 'secondOperand', 'edit', 'delete'];
          var event: any = {
            eventType: type,
            eventTitle: "Expense",
            ruleType: RuleType.SUM,
            eventSubTypeName: "NA",
            formula : null

          }
          events.push(event)
        }
      });
      return events;
    }
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
        console.log(res);
        var incentiveDetails = {
          planType :incentivePlan.planType,
          partner: incentivePlan.audience.subTypeName
        }
        this.newIncentiveEventR.emit(incentiveDetails);
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
        console.log(res);
        this.newIncentiveEventC.emit(incentivePlan.planType);
      },
      err =>  {
        console.log(err);
        this.toastr.error(err);
      }
    );
  }

  displayIncentivePlan() {
    this.plan = null;
    if(this.audience === 'RETAILER') {
      this.newIncentiveEventR.emit(null);
    } else {
      this.newIncentiveEventC.emit(null);
    }
  }




    changeDate() {
      var newEndDateUTCString;
      if(typeof this.incentiveFormGroup.get('endDate').value === "string") {
        newEndDateUTCString = this.incentiveFormGroup.get('endDate').value;
      } else {
        var selectedEndDate = this.incentiveFormGroup.get('endDate').value;
        selectedEndDate.setHours(selectedEndDate.getHours() + 23);
        selectedEndDate.setMinutes(selectedEndDate.getMinutes() + 59);
        selectedEndDate.setSeconds(selectedEndDate.getSeconds() + 59);
        newEndDateUTCString  = selectedEndDate.toISOString()
      }
      
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
      // return !this.incentiveFormGroup.get('type').valid || !this.incentiveFormGroup.get('name').valid ||
      // !this.incentiveFormGroup.get('startDate').valid || !this.incentiveFormGroup.get('endDate').valid  ||
      // (this.audience === "RETAILER" ? !this.incentiveFormGroup.get('startDate').valid: true) ;
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

    openDialog(event, rowIndex): void {
      const dialogRef = this.dialog.open(AddEventDialog, {
        width: '60%',
        data: {
          event: event,
          rowIndex: rowIndex,
          audience : this.audience,
          planType: this.incentiveFormGroup.get('type').value
        }
      });
  
      dialogRef.componentInstance.onEventCreate.subscribe(event => {
        var successMsg = "";
        // var eventName = this.eventTypes.find(e => e.value === event.eventType);
        // event.eventTypeName = eventName ? eventName.name : "NA";
        if(event.eventSubType) {
          var cpList = JSON.parse(sessionStorage.getItem("CONTENT_PROVIDERS"));
          if(cpList && cpList.length > 0) {
            var cp = cpList.find(cp => cp.contentProviderId === event.eventSubType);
            event.eventSubTypeName = cp ? cp.name : "NA";
          } else {
            event.eventSubTypeName = "Loading...";
          }
        } else {
          event.eventSubTypeName = "NA";
        }
        if(event.index !== undefined && event.index !== null) {
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
