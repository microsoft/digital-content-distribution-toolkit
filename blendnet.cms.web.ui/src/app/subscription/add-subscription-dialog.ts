import { Component, EventEmitter, Inject, Output } from "@angular/core";
import { FormControl, FormGroup, Validators } from "@angular/forms";
import { MatDialogRef, MAT_DIALOG_DATA } from "@angular/material/dialog";
import { ToastrService } from "ngx-toastr";
import { SubscriptionService } from "../services/subscription.service";

@Component({
    selector: 'app-add-subscription',
    templateUrl: 'add-subscription-dialog.html',
    styleUrls: ['subscription.component.css'],
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
      @Inject(MAT_DIALOG_DATA) public data: any,
      private subscriptionService: SubscriptionService,
      private toastr: ToastrService
      ) {
 
        this.minStart = new Date();
        this.minEnd  = new Date();
        this.minEnd.setDate(this.minEnd.getDate() +1);
  
      }
  
    ngOnInit() {
     this.createEmptyForm();
     if(this.data) {
      this.subForm.get('name').setValue(this.data.title);
      this.subForm.get('price').setValue(this.data.price);
      this.subForm.get('durationDays').setValue(this.data.durationDays);
      this.subForm.get('startDate').setValue(this.data.startDate);
      this.subForm.get('endDate').setValue(this.data.endDate);
      if(this.data.isRedeemable) {
        this.subForm.get('isRedeemable').setValue("Yes");
        this.subForm.get('redemptionValue').setValue(this.data.redemptionValue);
      } else {
        this.subForm.get('isRedeemable').setValue("No");
      }
      
     } 
    }
    
    createEmptyForm() {
      this.subForm = new FormGroup({
        name :  new FormControl('', [Validators.required]),
        price : new FormControl('', [Validators.required, Validators.pattern(/^-?(0|[1-9]\d*)?$/), Validators.min(1)]),
        durationDays : new FormControl('', [Validators.required, Validators.max(365), Validators.min(1)]),
        startDate : new FormControl(null, [Validators.required]),
        endDate : new FormControl(null, [Validators.required]),
        isRedeemable : new FormControl('', [Validators.required]),
        redemptionValue: new FormControl('')
        });
    }
  
    onNoClick(): void {
      this.dialogRef.close();
    }
  
    isRedeemable() {
      return this.subForm.get('isRedeemable').value === 'Yes';
    }

    editSubscription() {
      var sub = this.getSubDetails();
      sub.id = this.data.id;
      this.subscriptionService.editSubscription(sub).subscribe(
        res => this.onSubCreate.emit("Subscription updated successfully!"),
        err => this.toastr.error(err)
      );
    }

    getSubDetails() {
      var selectedEndDate = this.subForm.get('endDate').value;
      if(Object.prototype.toString.call(selectedEndDate) !== "[object Date]") {
        selectedEndDate = new Date(selectedEndDate);
      } else {
        selectedEndDate.setHours(selectedEndDate.getHours() + 23);
        selectedEndDate.setMinutes(selectedEndDate.getMinutes() + 59);
        selectedEndDate.setSeconds(selectedEndDate.getSeconds() + 59);
      }

      var sub: any = {
        contentProviderId : sessionStorage.getItem("contentProviderId"),
        title: this.subForm.get('name').value,
        durationDays: this.subForm.get('durationDays').value,
        price: this.subForm.get('price').value,
        startDate: this.subForm.get('startDate').value,
        endDate: selectedEndDate,
        isRedeemable:  this.subForm.get('isRedeemable').value === "Yes",
        redemptionValue :  this.subForm.get('isRedeemable').value === "Yes" ? 
            this.subForm.get('redemptionValue').value : 0
      }
      return sub;
    }

    createSubscription() {
      var sub = this.getSubDetails();
      this.subscriptionService.createSubscription(sub).subscribe(
        res => this.onSubCreate.emit("Subscription created successfully!"),
        err => this.toastr.error(err)
      );
  
    }

    submitSubscription() {
      if(this.data) {
        this.editSubscription();
      } else {
        this.createSubscription();
      }
    }
    
    public errorHandling = (control: string, error: string) => {
        return this.subForm.controls[control].hasError(error);
      }
   
  }
  
  