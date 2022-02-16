import { Component, EventEmitter, Inject, OnInit, Output } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { ToastrService } from 'ngx-toastr';
import { IncentiveService } from '../services/incentive.service';

@Component({
  selector: 'app-edit-incentive-enddate',
  templateUrl: './edit-incentive-enddate.component.html',
  styleUrls: ['./incentive-management.component.css']
})
export class EditIncentiveEndDateComponent implements OnInit {

  @Output() onDateChange = new EventEmitter<any>();
  incentiveForm: FormGroup;
  minStart: Date;
  minEnd: Date;
  incentive: any;


  constructor(  public dialogRef: MatDialogRef<EditIncentiveEndDateComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any,
    private incentiveService: IncentiveService,
    private toastr: ToastrService) { 
    this.minEnd  = new Date();

  }

  ngOnInit(): void {
    if(this.data.incentive) {
        this.incentiveForm = new FormGroup({
            oldEndDate : new FormControl(''),
            newEndDate: new FormControl('',  [Validators.required])
          });
        if(this.data.audience === "RETAILER") {
            this.incentiveService.getRetailerIncentivePlanByIdAndPartner(this.data.incentive.id, this.data.incentive.partner).subscribe(
              res => {
                this.incentive= res.body;
                this.incentiveForm.get('oldEndDate').setValue(this.incentive.endDate);
              },
              err => console.log(err)
            )
          } else {
            this.incentiveService.getConsumerIncentivePlanById(this.data.incentive.id).subscribe(
              res => {
                    this.incentive= res.body;
                    this.incentiveForm.get('oldEndDate').setValue(this.incentive.endDate);
              },
              err => console.log(err)
            )
          }
     
    }

  }

  changeEndDate() {
    var newEndDateUTCString;
    if(typeof this.incentiveForm.get('newEndDate').value === "string") {
      newEndDateUTCString = this.incentiveForm.get('newEndDate').value;
    } else {
      var selectedEndDate = this.incentiveForm.get('newEndDate').value;
      selectedEndDate.setHours(selectedEndDate.getHours() + 23);
      selectedEndDate.setMinutes(selectedEndDate.getMinutes() + 59);
      selectedEndDate.setSeconds(selectedEndDate.getSeconds() + 59);
      newEndDateUTCString  = selectedEndDate.toISOString()
    }
    
    if(this.incentive.audience.audienceType === "RETAILER") {
      var partner = this.incentive.audience.subTypeName;
      this.changeRetailerPlanEndDate(partner, newEndDateUTCString);
    } else {
      this.changeConsumerPlanEndDate(newEndDateUTCString)
    }
  }


  changeRetailerPlanEndDate(partner, endDate) {
    this.incentiveService.changeDateRetailerIncentivePlan(this.data.incentive.id, partner, endDate).subscribe(
      res => {
        this.onDateChange.emit("Retailer Incentive end date changed successfully")
      },
      err =>  {
        console.log(err);
        this.toastr.error(err);
      }
    );
  }

  changeConsumerPlanEndDate(endDate) {
    this.incentiveService.changeDateConsumerIncentivePlan(this.data.incentive.id, endDate).subscribe(
      res => {
        this.onDateChange.emit("Consumer Incentive end date changed successfully")
      },
      err =>  {
        console.log(err);
        this.toastr.error(err);
      }
    );
  }
  
  closeDialog(): void {
    this.dialogRef.close();
  }

}
