import {Component, ViewChild} from '@angular/core';
import {MatAccordion} from '@angular/material/expansion';
import { SubscriptionService } from '../services/subscription.service';
import { ToastrService } from 'ngx-toastr';
import { MatDialog} from '@angular/material/dialog';
import { AddSubscriptionDialog } from './add-subscription-dialog';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';

@Component({
  selector: 'app-subscription',
  styleUrls: ['subscription.component.css'],
  templateUrl: 'subscription.component.html',
})
export class SubscriptionComponent {
  
  cpSubscriptions;
  today;
  minEnd: Date;

  displayedColumns: string[] = ['status','title', 'price', 'durationDays', 'startDate', 'endDate', 'isRedeemable', 'redemptionValue', 'edit'];
  dataSource: MatTableDataSource<any>;
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
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

  applyFilter(event: Event) {
    const filterValue = (event.target as HTMLInputElement).value;
    this.dataSource.filter = filterValue.trim().toLowerCase();

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


 
  openDialog(sub): void {
    const dialogRef = this.dialog.open(AddSubscriptionDialog, {
      width: '50%',
      data: sub
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
