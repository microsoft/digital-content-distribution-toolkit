import { Component, EventEmitter, Inject, Output } from "@angular/core";
import { FormControl, FormGroup, Validators } from "@angular/forms";
import { MatDialogRef, MAT_DIALOG_DATA } from "@angular/material/dialog";
import { ToastrService } from "ngx-toastr";
import { DialogData } from "../broadcast/broadcast.component";
import { SubscriptionService } from "../services/subscription.service";

@Component({
    selector: 'app-add-subscription',
    templateUrl: 'add-subscription-dialog.html',
  })
  export class AddSubscriptionDialog {
     
    name;
    price;
    durationDays;
    startDate;
    endDate; 
    minStart: Date;
    minEnd: Date;
    @Output() onSubCreate = new EventEmitter<any>();
    subForm: FormGroup;
  
    constructor(
      public dialogRef: MatDialogRef<AddSubscriptionDialog>,
      @Inject(MAT_DIALOG_DATA) public data: DialogData,
      private subscriptionService: SubscriptionService,
      private toastr: ToastrService
      ) {
        const currentYear = new Date().getFullYear();
        const currentMonth = new Date().getMonth();
        const currentDay = new Date().getDate();
        this.minStart = new Date(currentYear, currentMonth, currentDay);
        this.minEnd = new Date(currentYear, currentMonth, currentDay+1);
  
      }
  
    ngOnInit() {
      this.subForm = new FormGroup({
      name :  new FormControl('', [Validators.required]),
      price : new FormControl('', [Validators.required, Validators.pattern(/^-?(0|[1-9]\d*)?$/), Validators.min(1)]),
      durationDays : new FormControl('', [Validators.required, Validators.max(365), Validators.min(1)]),
      startDate : new FormControl(null, [Validators.required]),
      endDate : new FormControl(null, [Validators.required])
      });
    }
  
    onNoClick(): void {
      this.dialogRef.close();
    }
  
    createSubscription() {
      var sub = {
        contentProviderId : localStorage.getItem("contentProviderId"),
        title: this.subForm.get('name').value,
        durationDays: this.subForm.get('durationDays').value,
        price: this.subForm.get('price').value,
        startDate: this.subForm.get('startDate').value,
        endDate: this.subForm.get('endDate').value
      }
      this.subscriptionService.createSubscription(sub).subscribe(
        res => this.onSubCreate.emit("Subscription created successfully!"),
        err => this.toastr.error(err)
      );
  
    }
    
    public errorHandling = (control: string, error: string) => {
        return this.subForm.controls[control].hasError(error);
      }
   
  }
  
  