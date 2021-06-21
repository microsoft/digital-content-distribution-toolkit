import { Component, Inject, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';

@Component({
  selector: 'app-add-incentive',
  templateUrl: './add-incentive.component.html',
  styleUrls: ['./incentive-management.component.css']
})
export class AddIncentiveComponent implements OnInit {

  audience: string = "";
  incentiveForm: FormGroup;
  events: FormArray;
  ranges: FormArray;


  constructor(
    @Inject(MAT_DIALOG_DATA) public data: any,
    private formBuilder: FormBuilder
  ) { 
    
  }

  ngOnInit(): void {
    this.audience = this.data.audience;
    if(this.audience === "Retailer") {
      //createRetailerIncentiveForm()
    } else {
      //createConsumerIncentiveForm()
    }
    this.incentiveForm = new FormGroup({
      name :  new FormControl(' ', [Validators.required, Validators.maxLength(10)]),
      type : new FormControl(' ',[Validators.required]),
      startDate: new FormControl('',[Validators.required]),
      endDate: new FormControl('',[Validators.required]),
      partner: new FormControl(''),
      events: this.formBuilder.array([ this.createEvent() ])
    });

  }

  getEvents(form) {
    //console.log(form.get('events').controls);
    return form.controls.events.controls;
  }

  getRanges(form) {
   //console.log(form.controls.ranges.controls);
    return form.controls.ranges.controls;
  }
  
  removeRange(j){
    // const control = <FormArray>this.incentiveForm.get('events')
    // .controls[j].get('questions');
    this.events = this.incentiveForm.get('events') as FormArray
    this.ranges = this.events.controls[j].get('ranges') as FormArray;
    this.ranges.removeAt(j);
 }

 removeEvent(i){
  // const control = <FormArray>this.survey.get('sections');
  this.events = this.incentiveForm.get('events') as FormArray;
  this.events.removeAt(i);

 }

  createEvent(): FormGroup {
      return this.formBuilder.group({
        eventType: new FormControl('',[Validators.required]),
        contentProvider: new FormControl(''),
        ruleType: new FormControl('',[Validators.required]),
        formula: new FormControl('',[Validators.required]),
        target: new FormControl(''),
        amount: new FormControl('', [Validators.required]),
        ranges: this.formBuilder.array([this.createRange()])
    })
  }

  createRange() : FormGroup {
    return this.formBuilder.group({
      start: new FormControl('', [Validators.required]),
      end: new FormControl('', [Validators.required]),
      amount: new FormControl('', [Validators.required])
    })
  }

  addEvent(): void {
    this.events = this.incentiveForm.get('events') as FormArray;
    this.events.push(this.createEvent());
  }

  addRange(eventNumber): void {
    this.events = this.incentiveForm.get('events') as FormArray
    this.ranges = this.events.controls[eventNumber].get('ranges') as FormArray;
    this.ranges.push(this.createRange());
  }

}
