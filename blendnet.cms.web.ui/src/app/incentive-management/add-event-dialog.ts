import { Component, EventEmitter, Inject, Output } from "@angular/core";
import { FormArray, FormBuilder, FormControl, FormGroup, Validators } from "@angular/forms";
import { MatDialogRef, MAT_DIALOG_DATA } from "@angular/material/dialog";
import { FormulaType, PlanType, RuleType } from "../models/incentive.model";
import { IncentiveService } from "../services/incentive.service";


@Component({
    selector: 'app-add-event',
    templateUrl: 'add-event-dialog.html',
    styleUrls: ['add-incentive.component.css'],
  })
  export class AddEventDialog {
     

    @Output() onEventCreate = new EventEmitter<any>();
    eventForm: FormGroup;
    showCP = false;
    ruleTypes = [];
    eventTypes:any;
    contentProviders=[];
    regularFormulas= [];
    milestoneFormulas =[];
    isViewOnly = false;
    

    constructor(
      public dialogRef: MatDialogRef<any>,
      @Inject(MAT_DIALOG_DATA) public data: any,
      private formBuilder: FormBuilder,
      private incentiveService: IncentiveService
      ) {
  
      }
  
    ngOnInit() {
      if(this.data.isViewOnly) {
        this.isViewOnly = true;
        this.eventForm = new FormGroup({
          eventType :  new FormControl({value:this.data.event.eventType, disabled: true}),
          eventTitle :  new FormControl({value:this.data.event.eventTitle, disabled: true}),
          eventSubType :  new FormControl({value:this.data.event.eventSubType, disabled: true}),
          ruleType : new FormControl({value: 'SUM', disabled: true}),
          formula : new FormControl({value: this.data.event.formula.formulaType, disabled: true}),
          firstOperand : new FormControl({value: this.data.event.formula.firstOperand, disabled: true}),
          secondOperand : new FormControl({value: this.data.event.formula.secondOperand, disabled: true}),
          ranges: this.formBuilder.array([this.createRange()])
          });
          //set range details config
          if(this.data.event.formula.rangeOperand && this.data.event.formula.rangeOperand.length > 0) {
            this.eventForm.get('ranges').patchValue([this.setRangeDetail(this.data.event.formula.rangeOperand)]);
            this.eventForm.get('ranges').disable();
            }
      } else {
        this.createEmptyForm();
        this.setEventsData();
      }

       //get list of Content providers
       var cpList = JSON.parse(sessionStorage.getItem("CONTENT_PROVIDERS"));
       if(cpList && cpList.length > 0) {
         this.contentProviders = cpList;
       }

       //set formula config
       this.setFormulaConfig();

       //setting configurations
       if(this.data.audience === "RETAILER") {
         this.setConfigForRetailer();
       } else {
         this.setConfigForConsumer();
       }

       //setting milestone configs
       if(this.isMileStonePlanType()) {
         this.ruleTypes = [RuleType.SUM,RuleType.COUNT];
       } else {
         this.ruleTypes = [RuleType.SUM];
       }
    }


    setEventsData() {
      if(this.data.event) {
          this.eventForm.get('eventType').setValue(this.data.event.eventType);
          this.eventForm.get('eventTitle').setValue(this.data.event.eventTitle);

          if(this.data.event.eventSubType) {
            this.eventForm.get('eventSubType').setValue(this.data.event.eventSubType);
            this.showCP = true;
          }

          this.eventForm.get('ruleType').setValue(this.data.event.ruleType);
          this.eventForm.get('formula').setValue(this.data.event.formula.formulaType);
  
          // setting the operands
          if(this.data.event.formula.firstOperand) {
            this.eventForm.get('firstOperand').setValue(this.data.event.formula.firstOperand);
          } else {
            this.eventForm.get('firstOperand').clearValidators();
            this.eventForm.get('firstOperand').updateValueAndValidity();
          }

          // setting the operands
          if(this.data.event.formula.secondOperand) {
            this.eventForm.get('secondOperand').setValue(this.data.event.formula.secondOperand);
          } else {
            this.eventForm.get('secondOperand').clearValidators();
            this.eventForm.get('secondOperand').updateValueAndValidity();
          }

          // set range details config
          if(this.data.event.formula.rangeOperand && this.data.event.formula.rangeOperand.length > 0) {
            this.eventForm.get('ranges').patchValue([this.setRangeDetail(this.data.event.formula.rangeOperand)]);
            }
        } 
    }

    setRangeDetail(rangeOperand) {
      var existingRanges = this.eventForm.get('ranges') as FormArray;
      
      rangeOperand.forEach(range => {
        var newRange = this.formBuilder.group({
          start: range.startRange,
          end: range.endRange,
          output: range.output
        })
        existingRanges.controls.push(newRange);
      })
      existingRanges.removeAt(0);
      return existingRanges;
    }

    setFormulaConfig(){
      this.regularFormulas = [
        FormulaType.PLUS, 
        FormulaType.MINUS,
        FormulaType.MULTIPLY,
        FormulaType.PERCENTAGE
      ];
      this.milestoneFormulas = [
        FormulaType.DIVIDE_AND_MULTIPLY, 
        FormulaType.RANGE_AND_MULTIPLY
      ];
      
    }
    
    createEmptyForm() {
      this.eventForm = new FormGroup({
        eventType :  new FormControl('',  [Validators.required, Validators.pattern(/^[^\s].*[^\s]$/)]),
        eventTitle :  new FormControl('',  [Validators.required, Validators.pattern(/^[^\s].*[^\s]$/)]),
        eventSubType :  new FormControl('',  [Validators.required, Validators.pattern(/^[^\s].*[^\s]$/)]),
        ruleType : new FormControl('SUM', [Validators.required]),
        formula : new FormControl('', [Validators.required]),
        firstOperand : new FormControl('',[Validators.required,Validators.pattern(/^-?(0|[1-9]\d*)?$/), Validators.min(1)]),
        secondOperand : new FormControl('', [Validators.required,Validators.pattern(/^-?(0|[1-9]\d*)?$/), Validators.min(1)]),
        ranges: this.formBuilder.array([this.createRange()])
        });
    }
  
    createRange() : FormGroup {
      return this.formBuilder.group({
        start: new FormControl('', [Validators.pattern(/^-?(0|[1-9]\d*)?$/), Validators.min(1)]),
        end: new FormControl('', [Validators.pattern(/^-?(0|[1-9]\d*)?$/),  Validators.min(1)]),
        output: new FormControl('', [Validators.pattern(/^-?(0|[1-9]\d*)?$/),  Validators.min(1)])
      })
    }


    onNoClick(): void {
      this.dialogRef.close();
    }
  
    
    // public errorHandling = (control: string, error: string) => {
    //     return this.eventForm.controls[control].hasError(error);
    // }

    
    
    addRange(): void {
      var range = this.createRange();
      var existingRanges = this.eventForm.get('ranges') as FormArray;
      existingRanges.push(range);
    }
    
        
    removeRange(j){
      var ranges = this.eventForm.get('ranges') as FormArray;
      ranges.removeAt(j);
    }
    
    getTotalRanges() {
      var ranges = this.eventForm.get('ranges') as FormArray;
      return ranges.length;
    }

    getRanges() {
      var ranges =  this.eventForm.get('ranges') as FormArray;
      return ranges.controls;
    }

    isMileStonePlanType() {
      return this.data.planType === PlanType.MILESTONE;
    }

    isRangeFormulaType() {
      return (this.eventForm.get('formula').value === FormulaType.RANGE_AND_MULTIPLY);
    }

    onEventTypeChange() {
      if(!this.eventForm.get('eventType').value.includes("ORDER")){
        this.eventForm.get('eventSubType').setValue(null);
        this.eventForm.get('eventSubType').clearValidators();
        this.showCP = false;
        this.eventForm.get('eventSubType').updateValueAndValidity();
      } else {
        this.eventForm.get('eventSubType').setValidators([Validators.required]);
        this.showCP = true;
        this.eventForm.get('eventSubType').updateValueAndValidity();
      }
    }
  

    setConfigForRetailer() {
        if(sessionStorage.getItem('RETAILERS_EVENTS')) {
          this.eventTypes = JSON.parse(sessionStorage.getItem('RETAILERS_EVENTS'))
        } else {
          this.incentiveService.getEventList("RETAILER").subscribe(
            res => {
              this.eventTypes = res.body;
              sessionStorage.setItem("RETAILERS_EVENTS",  JSON.stringify(res.body));
            },
            err => console.error(err)
          );
        }
        
    }

    isExpenseEvent(event) {
      return event.includes("EXPENSE");
    }

    setConfigForConsumer() {
      if(sessionStorage.getItem('CONSUMER_EVENTS')) {
        this.eventTypes = JSON.parse(sessionStorage.getItem('CONSUMER_EVENTS'))
      } else {
        this.incentiveService.getEventList("CONSUMER").subscribe(
          res => {
            this.eventTypes = res.body;
            sessionStorage.setItem("CONSUMER_EVENTS",  JSON.stringify(res.body));
          },
          err => console.error(err)
        );
      }
    }

    onFormulaChange() {
      var formula = this.eventForm.get('formula');
      if(formula.value === FormulaType.DIVIDE_AND_MULTIPLY) {
        this.eventForm.get('firstOperand').setValidators([Validators.required,Validators.pattern(/^-?(0|[1-9]\d*)?$/), Validators.min(1)]);
        this.eventForm.get('firstOperand').updateValueAndValidity();
        this.eventForm.get('secondOperand').setValidators([Validators.required,Validators.pattern(/^-?(0|[1-9]\d*)?$/), Validators.min(1)]);
        this.eventForm.get('secondOperand').updateValueAndValidity();
        this.eventForm.updateValueAndValidity();
      } else if(formula.value === FormulaType.RANGE_AND_MULTIPLY) {
        //this.eventForm.addControl('ranges', this.formBuilder.array([this.createRange()]));
        //this.eventForm.removeControl('milestoneTarget');
        //this.eventForm.removeControl('incentive');
        this.eventForm.get('firstOperand').clearValidators();
        this.eventForm.get('firstOperand').updateValueAndValidity();
        this.eventForm.get('secondOperand').clearValidators();
        this.eventForm.get('secondOperand').updateValueAndValidity();
        // this.eventForm.updateValueAndValidity();
      } else if(!this.isMileStonePlanType()){
        //this.eventForm.addControl('incentive', new FormControl('', [Validators.required,Validators.pattern(/^-?(0|[1-9]\d*)?$/),  Validators.min(1)]));
        //this.eventForm.removeControl('milestoneTarget');
        this.eventForm.get('firstOperand').setValidators([Validators.required,Validators.pattern(/^-?(0|[1-9]\d*)?$/), Validators.min(1)]);
        this.eventForm.get('secondOperand').clearValidators();
        this.eventForm.get('secondOperand').updateValueAndValidity();
        // this.eventForm.updateValueAndValidity();
      }  
    }

    isDivideFormulaType() {
      return this.eventForm.get('formula').value.includes("DIVIDE");
    }
   
    addEvent() {
      var event: any = {
        index : this.data.rowIndex,
        eventType : this.eventForm.get('eventType').value,
        eventTitle : this.eventForm.get('eventTitle').value,
        eventSubType : this.eventForm.get('eventSubType').value,
        ruleType: this.eventForm.get('ruleType').value,
        formula :  {
          formulaType : this.eventForm.get('formula').value,
          rangeOperand : []
        }
      }
      if(event.formula.formulaType === FormulaType.RANGE_AND_MULTIPLY) {
        event.formula.rangeOperand = this.getRangeDetails();
      }else if(event.formula.formulaType === FormulaType.DIVIDE_AND_MULTIPLY){
        event.formula.firstOperand = this.eventForm.get('firstOperand').value;
        event.formula.secondOperand = this.eventForm.get('secondOperand').value;
      } else {
        event.formula.firstOperand = this.eventForm.get('firstOperand').value;
      }
      
      this.onEventCreate.emit(event);
    }

    getRangeDetails() {
      var rangeOperand: any[] = []
      var ranges = this.eventForm.get('ranges') as FormArray;
      ranges.controls.forEach(r => {
        var range = {
          startRange: r.get('start').value,
          endRange: r.get('end').value,
          output: r.get('output').value
        }
        rangeOperand.push(range);
      });

      return rangeOperand;
    }


    getErrorMessage() {

    }

  }
  
  