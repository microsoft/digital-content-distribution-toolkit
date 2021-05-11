import {Component, EventEmitter, Inject, Output, ViewChild} from '@angular/core';
import {MatAccordion} from '@angular/material/expansion';
import { SubscriptionService } from '../services/subscription.service';
import { ToastrService } from 'ngx-toastr';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { DialogData } from '../content-provider/content-provider.component';
import { FormControl, Validators } from '@angular/forms';

@Component({
  selector: 'app-subscription',
  styleUrls: ['subscription.component.css'],
  templateUrl: 'subscription.component.html',
})
export class SubscriptionComponent {
  
  @ViewChild(MatAccordion) accordion: MatAccordion;
  cpSubscriptions;
  today;

  constructor(
    private subscriptionService: SubscriptionService,
    private toastr: ToastrService,
    public dialog: MatDialog
    
  ) {
    // this.cpSubscriptions = subscriptions;
    var date = new Date();
    this.today = date.toISOString();
  }

  ngOnInit(): void {
    this.getSubscriptions();
  }

  getSubscriptions() {
    this.subscriptionService.getSubscriptionsForCP().subscribe(
      res => this.cpSubscriptions = res,
      err => {
        this.toastr.error(err);
    });
  }
  save(sub) {
    this.subscriptionService.editSubscription(sub).subscribe(
      res => {
        this.toastr.success("Subscription updated successfully!");
        this.getSubscriptions();
      },
      err => this.toastr.error(err)
    );

  }

  reset(sub) {
    
  }
  disableSaveBtn(sub) {
    if(!sub.price || sub.price.value < 0 || !sub.durationDays || sub.durationDays.value < 1 || sub.durationDays.value > 365 ||
      !sub.startDate || !sub.endDate ||
      sub.endDate <= sub.startDate) {
        return true;
      } else {
        return false;
      }

  }
 
  openDialog(): void {
    const dialogRef = this.dialog.open(AddSubscriptionDialog, {
      width: '30%',
      data: {}
    });

    dialogRef.componentInstance.onSubCreate.subscribe(data => {
      this.toastr.success("Subscription created successfully!");
      this.getSubscriptions();
      dialogRef.close();
    })

    dialogRef.afterClosed().subscribe(result => {
      console.log('The dialog was closed');
    });
  }

}

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
  @Output() onSubCreate = new EventEmitter<any>();

  constructor(
    public dialogRef: MatDialogRef<AddSubscriptionDialog>,
    @Inject(MAT_DIALOG_DATA) public data: DialogData,
    private subscriptionService: SubscriptionService,
    private toastr: ToastrService
    ) {}

  ngOnInit() {
    this.name =  new FormControl('', [Validators.required]);
    this.price = new FormControl('', [Validators.required, Validators.pattern(/^-?(0|[1-9]\d*)?$/)]);
    this.durationDays = new FormControl('', [Validators.required, Validators.max(365), Validators.min(1)]);
    this.startDate = new FormControl(null, [Validators.required]);
    this.endDate = new FormControl(null, [Validators.required]);
  }
  onNoClick(): void {
    this.dialogRef.close();
  }

  createSubscription() {
    var sub = {
      contentProviderId : localStorage.getItem("contentProviderId"),
      title: this.name.value,
      durationDays: this.durationDays.value,
      price: this.price.value,
      startDate: this.startDate.value,
      endDate: this.endDate.value
    }
    this.subscriptionService.createSubscription(sub).subscribe(
      res => this.onSubCreate.emit("Subscription created successfully!"),
      err => this.toastr.error(err)
    );

  }
  
  nameError() {
    if (this.name.hasError('required')) {
      return 'You must enter a value';
    }
  }

  priceError() {
    if (this.price.hasError('required')) {
      return 'You must enter a value';
    }
    return this.price.invalid ? 'Not a valid Price' : '';
  }

  durationDaysError() {
    if (this.durationDays.hasError('required')) {
      return 'You must enter a value';
    }
    return this.durationDays.invalid ? 'Not a valid duration' : '';

  }

  disableSaveBtn() {
    if(!this.price.value || !this.name.value || !this.durationDays.value ||
      !this.startDate.value || !this.endDate.value) {
        return true;
      }
      return false;
  }
}

