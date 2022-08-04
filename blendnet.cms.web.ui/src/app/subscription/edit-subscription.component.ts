import { Component, EventEmitter, Inject, OnInit, Output } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { ToastrService } from 'ngx-toastr';
import { SubscriptionService } from '../services/subscription.service';

@Component({
  selector: 'app-edit-subscription',
  templateUrl: './edit-subscription.component.html',
  styleUrls: ['./subscription.component.css']
})
export class EditSubscriptionComponent implements OnInit {

  @Output() onDateChange = new EventEmitter<any>();
  subForm: FormGroup;
  minStart: Date;
  minEnd: Date;


  constructor(  public dialogRef: MatDialogRef<EditSubscriptionComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any,
    private subscriptionService: SubscriptionService,
    private toastr: ToastrService) { 
    this.minEnd  = new Date();

  }

  ngOnInit(): void {
    if(this.data.sub) {
      this.subForm = new FormGroup({
        oldEndDate : new FormControl(this.data.sub.endDate),
        newEndDate: new FormControl('',  [Validators.required])
      });
    }

  }

  changeEndDate() {
    var selectedEndDate = this.subForm.get('newEndDate').value;
    selectedEndDate.setHours(selectedEndDate.getHours() + 23);
    selectedEndDate.setMinutes(selectedEndDate.getMinutes() + 59);
    selectedEndDate.setSeconds(selectedEndDate.getSeconds() + 59);
    var newEndDateUTCString  = selectedEndDate.toISOString()
    var endDate = {
      "endDate":  newEndDateUTCString
    }
    this.subscriptionService.updateEndDate(this.data.sub.id, endDate).subscribe(
      res => this.onDateChange.emit("Subscription end date changed successfully"),
      err => this.toastr.error(err)
    );
    
  }
  
  closeDialog(): void {
    this.dialogRef.close();
  }

  isEmptyEndDate() {
    return this.subForm.get('newEndDate').value==='';
  }

}
