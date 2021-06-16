import {Component, ViewChild} from '@angular/core';
import {MatAccordion} from '@angular/material/expansion';
import { SubscriptionService } from '../services/subscription.service';
import { ToastrService } from 'ngx-toastr';
import { MatDialog} from '@angular/material/dialog';
import { AddSubscriptionDialog } from './add-subscription-dialog';

@Component({
  selector: 'app-subscription',
  styleUrls: ['subscription.component.css'],
  templateUrl: 'subscription.component.html',
})
export class SubscriptionComponent {
  
  @ViewChild(MatAccordion) accordion: MatAccordion;
  cpSubscriptions;
  today;
  minEnd: Date;



  constructor(
    private subscriptionService: SubscriptionService,
    private toastr: ToastrService,
    public dialog: MatDialog
    
  ) {
    // this.cpSubscriptions = subscriptions;
    var date = new Date();
    this.today = date.toISOString();
    const currentYear = new Date().getFullYear();
    const currentMonth = new Date().getMonth();
    const currentDay = new Date().getDate();
    this.minEnd = new Date(currentYear, currentMonth, currentDay);
  
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


  disableSaveBtn(sub) {
    return (!sub.price || sub.price < 0 || 
      !sub.durationDays || sub.durationDays < 1 || sub.durationDays > 365);

  }
 
  openDialog(): void {
    const dialogRef = this.dialog.open(AddSubscriptionDialog, {
      width: '300px',
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
