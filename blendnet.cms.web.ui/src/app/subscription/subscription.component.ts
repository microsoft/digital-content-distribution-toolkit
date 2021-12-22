import {Component, ViewChild} from '@angular/core';
import {MatAccordion} from '@angular/material/expansion';
import { SubscriptionService } from '../services/subscription.service';
import { ToastrService } from 'ngx-toastr';
import { MatDialog} from '@angular/material/dialog';
import { AddSubscriptionDialog } from './add-subscription-dialog';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { EditSubscriptionComponent } from './edit-subscription.component';
import { CommonDialogComponent } from '../common-dialog/common-dialog.component';

@Component({
  selector: 'app-subscription',
  styleUrls: ['subscription.component.css'],
  templateUrl: 'subscription.component.html',
})
export class SubscriptionComponent {
  
  cpSubscriptions;
  today;
  minEnd: Date;
  deleteConfirmMessage: string = "Subscription against which an order is placed will not be deleted.  Please press Continue to delete the selected subscription.";
  displayedColumns: string[] = ['status','title', 'price', 'durationDays', 'startDate', 'endDate', 'isRedeemable', 'redemptionValue', 'extend', 'edit', 'delete'];
  dataSource: MatTableDataSource<any>;
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  filterValue;
  errMessage;
  error= false;

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

  refreshPage() {
    this.filterValue ="";
    this.getSubscriptions();
  }
  getSubscriptions() {
    this.subscriptionService.getSubscriptionsForCP().subscribe(
      res => {
        this.dataSource = this.createDataSource(res.body);
        this.dataSource.paginator = this.paginator;
        this.dataSource.sort = this.sort;

      },
      err => {
        this.error = true;
        this.dataSource = this.createDataSource([]);
        if(err === 'Not Found') {
          this.errMessage = "No data found";
        } else {
          this.toastr.error(err);
          this.errMessage = err;
        }
      });
  }

  applyFilter() {
    this.dataSource.filter = this.filterValue.trim().toLowerCase();

    if (this.dataSource.paginator) {
      this.dataSource.paginator.firstPage();
    }
  }

  createDataSource(rawDataList) {
    var dataSource: any[] =[];
    if(rawDataList && rawDataList.length > 0) {
      this.errMessage = "";
      this.error = false;
      rawDataList.forEach( rawData => {
        rawData.status = (rawData.endDate >= this.today || rawData.startDate >= this.today)
                       ? true : false;
        dataSource.push(rawData);
      });
    } else {
      this.errMessage = "No data found";
      this.error = true;
    }
    return new MatTableDataSource(dataSource);
  }


   
  changeEndDate(sub): void {
    const dialogRef = this.dialog.open(EditSubscriptionComponent, {
      width: '300px',
      data: {
        sub: sub
      }
    });

    dialogRef.componentInstance.onDateChange.subscribe(data => {
      this.toastr.success("Subscription end date changed successfully!");
      this.getSubscriptions();
      dialogRef.close();
    })

    dialogRef.afterClosed().subscribe(result => {
      console.log('The dialog was closed');
    });
  }

 
  openDialog(sub): void {
    const dialogRef = this.dialog.open(AddSubscriptionDialog, {
      width: '50%',
      data: {
        sub: sub
      }
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


  openDeleteConfirmModal(row): void {
    const dialogRef = this.dialog.open(CommonDialogComponent, {
      data: {
        heading: 'Confirm',
        message: this.deleteConfirmMessage,
        action: "DELETE",
        sub: row,
        buttons: this.openSelectCPModalButtons()
      },
      maxWidth: '400px'
    });
  
    dialogRef.afterClosed().subscribe(result => {
      if (result === 'proceed') {
        this.onConfirmDelete(row.id);
      }
    });
  
  }
  
  onConfirmDelete(id): void {
    this.subscriptionService.deleteSubscription(id).subscribe(
      res => this.onDeleteSuccess(),
      err => this.toastr.error(err));
  }

  onDeleteSuccess() {
    this.toastr.success("Subscription deleted successfully");
    this.getSubscriptions();
  }
    
  openSelectCPModalButtons(): Array<any> {
    return [{
      label: 'Cancel',
      type: 'basic',
      value: 'cancel',
      class: 'discard-btn'
    },
    {
      label: 'Continue',
      type: 'primary',
      value: 'submit',
      class: 'update-btn'
    }
    ]
  }

}
