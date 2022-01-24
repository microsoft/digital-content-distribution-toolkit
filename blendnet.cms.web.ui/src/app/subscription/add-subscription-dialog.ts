import { Component, EventEmitter, Inject, Output } from "@angular/core";
import { FormControl, FormGroup, Validators } from "@angular/forms";
import { MatDialogRef, MAT_DIALOG_DATA } from "@angular/material/dialog";
import { ToastrService } from "ngx-toastr";
import { lengthConstants } from "../constants/length-constants";
import { CustomValidator } from "../custom-validator/custom-validator";
import { SubscriptionService } from "../services/subscription.service";

@Component({
    selector: 'app-add-subscription',
    templateUrl: 'add-subscription-dialog.html',
    styleUrls: ['subscription.component.css'],
  })
  export class AddSubscriptionDialog {
    
    isUpdate = false;
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
  
      }
  
    ngOnInit() {
     
     if(this.data.sub) {
      this.isUpdate = true;
      var isRedeemable = this.data.sub.isRedeemable ? "Yes" : "No";
      this.minStart = null;
      this.minEnd = null;
        this.subForm = new FormGroup({
              name :  new FormControl(this.data.sub.title,[Validators.maxLength(lengthConstants.titleMaxLength),
                Validators.minLength(lengthConstants.titleMinLength),
               CustomValidator.alphaNumericSplChar]),
              price : new FormControl(this.data.sub.price, [Validators.required, CustomValidator.numeric, Validators.min(1)]),
              durationDays : new FormControl(this.data.sub.durationDays, [Validators.required, Validators.max(365), Validators.min(1)]),
              startDate : new FormControl( this.data.sub.startDate, [Validators.required]),
              endDate : new FormControl(this.data.sub.endDate, [Validators.required]),
              isRedeemable : new FormControl(isRedeemable, [Validators.required]),
              redemptionValue: new FormControl(this.data.sub.redemptionValue)
              });
      } else {
        this.createEmptyForm();
     }
    }
    
    createEmptyForm() {
      this.subForm = new FormGroup({
        name :  new FormControl('', [Validators.required, 
          Validators.maxLength(lengthConstants.titleMaxLength),
          Validators.minLength(lengthConstants.titleMinLength),
          CustomValidator.alphaNumericSplChar]),
        price : new FormControl('', [Validators.required, CustomValidator.numeric, Validators.min(1)]),
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
  
    isNotRedeemable() {
      return this.subForm.get('isRedeemable').value !== 'Yes';
    }

    editSubscription() {
      var sub = this.getSubDetails();
      sub.id = this.data.sub.id;
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
      if(this.data.sub) {
        this.editSubscription();
      } else {
        this.createSubscription();
      }
    }

    get f() { 
      return this.subForm.controls; 
    }
    
    public errorHandling = (control: string, error: string) => {
        return this.subForm.controls[control].hasError(error);
      }

      closeDialog(): void {
        this.dialogRef.close();
      }
   
  }
  
  