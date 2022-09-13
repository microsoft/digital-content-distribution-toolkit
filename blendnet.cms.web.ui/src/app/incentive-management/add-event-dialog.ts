import { Component, EventEmitter, Inject, Output } from "@angular/core";
import { FormArray, FormBuilder, FormControl, FormGroup, Validators } from "@angular/forms";
import { MatDialogRef, MAT_DIALOG_DATA } from "@angular/material/dialog";
import { lengthConstants } from "../constants/length-constants";
import { CustomValidator } from "../custom-validator/custom-validator";
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
          eventSubType :  new FormControl({value:this.data.event.eventSubType==undefined? 'all' : this.data.event.eventSubType, disabled: true}),
          ruleType : new FormControl({value: this.data.event.ruleType, disabled: true}),
          formula : new FormControl({value: this.data.event.formula.formulaType, disabled: true}),
          firstOperand : new FormControl({value: this.data.event.formula.firstOperand, disabled: true}),
          secondOperand : new FormControl({value: this.data.event.formula.secondOperand, disabled: true}),
          entity1Operand : new FormControl({value: this.data.event.formula.entity1Operand, disabled: true}),
          entity2Operand : new FormControl({value: this.data.event.formula.entity2Operand, disabled: true}),
          entity3Operand : new FormControl({value: this.data.event.formula.entity3Operand, disabled: true}),
          entity4Operand : new FormControl({value: this.data.event.formula.entity4Operand, disabled: true}),
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
         this.contentProviders.push({
          contentProviderId: "all",
          name: "All"
         });
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
          if(!this.data.event.eventSubType && this.data.event.eventType.includes("ORDER")){
            this.eventForm.get('eventSubType').setValue("all");
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
          if(this.data.event.formula.entity1Operand) {
            this.eventForm.get('entity1Operand').setValue(this.data.event.formula.entity1Operand);
          } else {
            this.eventForm.get('entity1Operand').clearValidators();
            this.eventForm.get('entity1Operand').updateValueAndValidity();
          }
          if(this.data.event.formula.entity2Operand) {
            this.eventForm.get('entity2Operand').setValue(this.data.event.formula.entity2Operand);
          } else {
            this.eventForm.get('entity2Operand').clearValidators();
            this.eventForm.get('entity2Operand').updateValueAndValidity();
          }
          if(this.data.event.formula.entity3Operand) {
            this.eventForm.get('entity3Operand').setValue(this.data.event.formula.entity3Operand);
          } else {
            this.eventForm.get('entity3Operand').clearValidators();
            this.eventForm.get('entity3Operand').updateValueAndValidity();
          }
          if(this.data.event.formula.entity4Operand) {
            this.eventForm.get('entity4Operand').setValue(this.data.event.formula.entity4Operand);
          } else {
            this.eventForm.get('entity4Operand').clearValidators();
            this.eventForm.get('entity4Operand').updateValueAndValidity();
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
          output: range.output,
          entity1Output: range.entity1Output,
          entity2Output: range.entity2Output,
          entity3Output: range.entity3Output,
          entity4Output: range.entity4Output

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
        FormulaType.RANGE
      ];
      
    }
    
    createEmptyForm() {
      var ruleTypeValue = this.data.planType === "REGULAR" ? "SUM" : "COUNT";
      this.eventForm = new FormGroup({
        eventType :  new FormControl('',  [Validators.required, Validators.maxLength(lengthConstants.titleMaxLength), 
          Validators.minLength(lengthConstants.titleMinLength),
          CustomValidator.alphaNumericSplChar]),
        eventTitle :  new FormControl('',  [Validators.required, Validators.maxLength(lengthConstants.titleMaxLength), 
          Validators.minLength(lengthConstants.titleMinLength),
          CustomValidator.alphaNumericSplChar]),
        eventSubType :  new FormControl('',  [ Validators.maxLength(lengthConstants.titleMaxLength), 
          Validators.minLength(lengthConstants.titleMinLength),
          CustomValidator.alphaNumericSplChar]),
        ruleType : new FormControl(ruleTypeValue, [Validators.required]),
        formula : new FormControl('', [Validators.required]),
        firstOperand : new FormControl('',[Validators.minLength(lengthConstants.titleMinLength),
          CustomValidator.float, Validators.required]),
        secondOperand : new FormControl('', [Validators.required, Validators.minLength(lengthConstants.titleMinLength),
          CustomValidator.float]),
        entity1Operand : new FormControl('', [CustomValidator.float]),
        entity2Operand : new FormControl('', [CustomValidator.float]),
        entity3Operand : new FormControl('', [CustomValidator.float]),
        entity4Operand : new FormControl('', [CustomValidator.float]),
        ranges: this.formBuilder.array([this.createRange()])
        });
    }
  
    createRange() : FormGroup {
      return this.formBuilder.group({
        start: new FormControl('',[Validators.minLength(lengthConstants.titleMinLength),
          CustomValidator.numeric]),
        end: new FormControl('', [Validators.minLength(lengthConstants.titleMinLength),
          CustomValidator.numeric]),
        output: new FormControl('', [Validators.minLength(lengthConstants.titleMinLength),
          CustomValidator.float]),
        entity1Output : new FormControl('', [CustomValidator.float]),
        entity2Output : new FormControl('', [CustomValidator.float]),
        entity3Output : new FormControl('', [CustomValidator.float]),
        entity4Output : new FormControl('', [CustomValidator.float]),
      })
    }


    onNoClick(): void {
      this.dialogRef.close();
    }
      
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
      return (this.eventForm.get('formula').value === FormulaType.RANGE);
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
        this.eventForm.get('eventSubType').setValue("all");
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

    disableFormula(event) {
      if(event.includes("MINUS")) {
        return true;
      } else if(this.data.audience === "CONSUMER") {
        return event.includes("MULTIPLY") || event.includes("PERCENTAGE");
      }
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
        this.eventForm.get('firstOperand').setValidators([Validators.required, Validators.minLength(lengthConstants.titleMinLength),
          CustomValidator.float]);
        this.eventForm.get('firstOperand').updateValueAndValidity();
        this.eventForm.get('secondOperand').setValidators([Validators.required, Validators.minLength(lengthConstants.titleMinLength),
          CustomValidator.float]);
        this.eventForm.get('secondOperand').updateValueAndValidity();
        this.eventForm.updateValueAndValidity();
      } else if(formula.value === FormulaType.RANGE) {
        this.eventForm.get('firstOperand').clearValidators();
        this.eventForm.get('firstOperand').updateValueAndValidity();
        this.eventForm.get('secondOperand').clearValidators();
        this.eventForm.get('secondOperand').updateValueAndValidity();
      } else if(!this.isMileStonePlanType()){
        this.eventForm.get('firstOperand').setValidators([Validators.required, Validators.minLength(lengthConstants.titleMinLength),
          CustomValidator.float]);
        this.eventForm.get('secondOperand').clearValidators();
        this.eventForm.get('secondOperand').updateValueAndValidity();
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
        eventSubType : (this.eventForm.get('eventSubType').value && this.eventForm.get('eventSubType').value.includes("all"))? null : this.eventForm.get('eventSubType').value,
        ruleType: this.eventForm.get('ruleType').value,
        formula :  {
          formulaType : this.eventForm.get('formula').value,
          rangeOperand : []
        }
      }
      if(event.formula.formulaType === FormulaType.RANGE) {
        event.formula.rangeOperand = this.getRangeDetails();
      }else if(event.formula.formulaType === FormulaType.DIVIDE_AND_MULTIPLY){
        event.formula.firstOperand = this.eventForm.get('firstOperand').value;
        event.formula.secondOperand = this.eventForm.get('secondOperand').value;
        event.formula.entity1Operand = this.eventForm.get('entity1Operand').value ? this.eventForm.get('entity1Operand').value : null;
        event.formula.entity2Operand = this.eventForm.get('entity2Operand').value ? this.eventForm.get('entity2Operand').value : null;
        event.formula.entity3Operand = this.eventForm.get('entity3Operand').value ? this.eventForm.get('entity3Operand').value : null;
        event.formula.entity4Operand = this.eventForm.get('entity4Operand').value ? this.eventForm.get('entity4Operand').value : null;
      } else {
        event.formula.firstOperand = this.eventForm.get('firstOperand').value;
        event.formula.entity1Operand = this.eventForm.get('entity1Operand').value ? this.eventForm.get('entity1Operand').value : null;
        event.formula.entity2Operand = this.eventForm.get('entity2Operand').value ? this.eventForm.get('entity2Operand').value : null;
        event.formula.entity3Operand = this.eventForm.get('entity3Operand').value ? this.eventForm.get('entity3Operand').value : null;
        event.formula.entity4Operand = this.eventForm.get('entity4Operand').value ? this.eventForm.get('entity4Operand').value : null;
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
          output: r.get('output').value,
          entity1Output: r.get('entity1Output').value ? r.get('entity1Output').value : null,
          entity2Output: r.get('entity2Output').value ? r.get('entity2Output').value : null,
          entity3Output: r.get('entity3Output').value ? r.get('entity3Output').value : null,
          entity4Output: r.get('entity4Output').value ? r.get('entity4Output').value : null
        }
        rangeOperand.push(range);
      });

      return rangeOperand;
    }


    getErrorMessage() {

    }

  }
  
  