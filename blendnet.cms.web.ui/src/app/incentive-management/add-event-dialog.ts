import { Component, EventEmitter, Inject, Output } from "@angular/core";
import { FormArray, FormBuilder, FormControl, FormGroup, Validators } from "@angular/forms";
import { MatDialogRef, MAT_DIALOG_DATA } from "@angular/material/dialog";
import { ToastrService } from "ngx-toastr";
import { EventType, RuleType } from "../models/incentive.model";
import { IncentiveService } from "../services/incentive.service";


@Component({
    selector: 'app-add-event',
    templateUrl: 'add-event-dialog.html',
    styleUrls: ['add-incentive.component.css'],
  })
  export class AddEventDialog {
     

    @Output() onEventCreate = new EventEmitter<any>();
    eventForm: FormGroup;
    isPublished = false;
    showCP = false;
    ruleTypes = [];
    eventTypes:any;
    contentProviders=[];
    regularFormulas= [];
    milestoneFormulas =[];
    

    constructor(
      public dialogRef: MatDialogRef<any>,
      @Inject(MAT_DIALOG_DATA) public data: any,
      private toastr: ToastrService,
      private formBuilder: FormBuilder,
      private incentiveService: IncentiveService
      ) {
  
      }
  
    ngOnInit() {
      
     this.createEmptyForm();
     if(this.data.event) {
       if(this.data.event.formula.secondOperand) {
        this.eventForm.addControl('milestoneTarget', new FormControl('', [Validators.required,Validators.pattern(/^-?(0|[1-9]\d*)?$/),  Validators.min(1)]));
        this.eventForm.get('milestoneTarget').setValue(this.data.event.formula.secondOperand);
       }
      this.eventForm.get('eventType').setValue(this.data.event.eventType);
      this.eventForm.get('eventTitle').setValue(this.data.event.eventTitle);
      if(this.data.event.eventSubType) {
        this.eventForm.get('eventSubType').setValue(this.data.event.eventSubType);
        this.showCP = true;
      }
      this.eventForm.get('ruleType').setValue(this.data.event.ruleType);
      this.eventForm.get('formula').setValue(this.data.event.formula.formulaType);
      this.eventForm.get('incentive').setValue(this.data.event.formula.firstOperand);
      
      //TODO range data
      if(this.data.event.formula.rangeOperand.length > 0) {
        this.eventForm.get('ranges').patchValue([this.setRangeDetail(this.data.event.formula.rangeOperand)]);
      }
     } 

      var cpList = JSON.parse(sessionStorage.getItem("CONTENT_PROVIDERS"));
      if(cpList && cpList.length > 0) {
        this.contentProviders = cpList;
      }
      this.setFormulaConfig();
      if(this.data.audience === "RETAILER") {
        this.setConfigForRetailer();
      } else {
        this.setConfigForConsumer();
      }
      if(this.isMileStonePlanType()) {
        this.ruleTypes = [RuleType.SUM,RuleType.COUNT];
      } else {
        this.ruleTypes = [RuleType.SUM];
      }
    }

    setRangeDetail(rangeOperand) {
      var existingRanges = this.eventForm.get('ranges') as FormArray;
      
      rangeOperand.forEach(range => {
        var newRange = this.formBuilder.group({
          start: range.startRange,
          end: range.endRange,
          incentive: range.output
        })
        existingRanges.controls.push(newRange);
      })
      existingRanges.removeAt(0);
      return existingRanges;
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
    
    createEmptyForm() {
      this.eventForm = new FormGroup({
        eventType :  new FormControl('', [Validators.required]),
        eventTitle :  new FormControl('', [Validators.required]),
        eventSubType :  new FormControl(''),
        ruleType : new FormControl('', [Validators.required]),
        formula : new FormControl('', [Validators.required]),
        incentive : new FormControl('',[Validators.required,Validators.pattern(/^-?(0|[1-9]\d*)?$/), Validators.min(1)]),
        // milestoneTarget : new FormControl(0, [Validators.required,Validators.pattern(/^-?(0|[1-9]\d*)?$/)]),
        ranges: this.formBuilder.array([this.createRange()])
        });
    }
  
    createRange() : FormGroup {
      return this.formBuilder.group({
        start: new FormControl('', [Validators.pattern(/^-?(0|[1-9]\d*)?$/), Validators.min(1)]),
        end: new FormControl('', [Validators.pattern(/^-?(0|[1-9]\d*)?$/),  Validators.min(1)]),
        incentive: new FormControl('', [Validators.pattern(/^-?(0|[1-9]\d*)?$/),  Validators.min(1)])
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
      return this.data.planType === "MILESTONE";
    }

    isRangeFormulaType() {
      return this.eventForm.get('formula').value.includes("RANGE");
    }

    onEventTypeChange() {
      if(!this.eventForm.get('eventType').value.includes("ORDER")){
        this.eventForm.get('eventSubType').setValue(null);
        this.eventForm.get('eventSubType').clearValidators();
        this.showCP = false;
      } else {
        this.eventForm.get('eventSubType').setValidators([Validators.required]);
        this.showCP = true;
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
            err => console.log(err)
          );
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
          err => console.log(err)
        );
      }
    }

    onFormulaChange() {
      var formula = this.eventForm.get('formula');
      if(formula.value === "DIVIDE_AND_MULTIPLY") {
        this.eventForm.removeControl('ranges');
        this.eventForm.addControl('milestoneTarget', new FormControl('', [Validators.required,Validators.pattern(/^-?(0|[1-9]\d*)?$/),  Validators.min(1)]));
      } else if(formula.value === "RANGE_AND_MULTIPLY") {
        this.eventForm.addControl('ranges', this.formBuilder.array([this.createRange()]));
        this.eventForm.removeControl('milestoneTarget');
        this.eventForm.removeControl('incentive');
      } else if(!this.isMileStonePlanType()){
        this.eventForm.addControl('incentive', new FormControl('', [Validators.required,Validators.pattern(/^-?(0|[1-9]\d*)?$/),  Validators.min(1)]));
      }  
    }

    isDivideFormulaType() {
      return this.eventForm.get('formula').value.includes("DIVIDE");
    }
   
    addEvent() {
      var event = {
        index : this.data.rowIndex,
        eventType : this.eventForm.get('eventType').value,
        eventTitle : this.eventForm.get('eventTitle').value,
        eventSubType : this.eventForm.get('eventSubType').value,
        ruleType: this.eventForm.get('ruleType').value,
        formula :  {
          formulaType : this.eventForm.get('formula').value,
          firstOperand: '',
          secondOperand: '',
          rangeOperand : []
        }
      }
      if(event.formula.formulaType.includes("RANGE")) {
        event.formula.rangeOperand = this.getRangeDetails();
      } else if(event.formula.formulaType.includes("DIVIDE")){
        event.formula.firstOperand = this.eventForm.get('incentive').value;
        event.formula.secondOperand = this.eventForm.get('milestoneTarget').value;
      } else {
        event.formula.firstOperand = this.eventForm.get('incentive').value;
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
          output: r.get('incentive').value
        }
        rangeOperand.push(range);
      });

      return rangeOperand;
    }


    getErrorMessage() {

    }
  }
  
  