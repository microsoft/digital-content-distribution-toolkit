import { Component, OnInit, ViewChild } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { ContentService } from '../services/content.service';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import {MatAccordion} from '@angular/material/expansion';
import { MatPaginator } from '@angular/material/paginator';
import { MatDialog} from '@angular/material/dialog';
import { NotificationsDialog } from './notifications-dialog';
import { environment } from '../../environments/environment';
import {MatTableDataSource} from '@angular/material/table';
import { Content } from '../models/content.model';
import { ContentStatus } from '../models/content-status.enum';
import { SubscriptionService } from '../services/subscription.service';

export interface PeriodicElement {
  Title: string;
  Preview: string;
  Status: number;
  CreatedOn: string;
}

const ELEMENT_DATA: PeriodicElement[] = [
  {Preview: "https://th.bing.com/th/id/OIP.GnqZiwU7k5f_kRYkw8FNNwHaF3?pid=ImgDet&rs=1", Title: 'New Incentives', Status: 1, CreatedOn: '2021-06-14'},
  {Preview: "https://th.bing.com/th/id/OIP.GnqZiwU7k5f_kRYkw8FNNwHaF3?pid=ImgDet&rs=1", Title: 'Policy Update', Status: 4, CreatedOn: '2021-06-13'},
  {Preview: "https://th.bing.com/th/id/OIP.GnqZiwU7k5f_kRYkw8FNNwHaF3?pid=ImgDet&rs=1", Title: 'New Content Uploaded', Status: 6, CreatedOn: '2021-06-12'},
  {Preview: "https://th.bing.com/th/id/OIP.GnqZiwU7k5f_kRYkw8FNNwHaF3?pid=ImgDet&rs=1", Title: 'Content Uploaded', Status: 9, CreatedOn: '2021-06-10'},
  {Preview: "https://th.bing.com/th/id/OIP.GnqZiwU7k5f_kRYkw8FNNwHaF3?pid=ImgDet&rs=1", Title: 'New Feature', Status: 10, CreatedOn: '2021-06-09'},
  {Preview: "https://th.bing.com/th/id/OIP.GnqZiwU7k5f_kRYkw8FNNwHaF3?pid=ImgDet&rs=1", Title: 'App Update', Status: 12, CreatedOn: '2021-06-07'},
  {Preview: "https://th.bing.com/th/id/OIP.GnqZiwU7k5f_kRYkw8FNNwHaF3?pid=ImgDet&rs=1", Title: 'Test Notification', Status: 14, CreatedOn: '2021-06-06'},
];

@Component({
  selector: 'app-notifications',
  templateUrl: './notifications.component.html',
  styleUrls: ['./notifications.component.css']
})
export class NotificationsComponent implements OnInit {
  expiresIn:string= "";
  selectedNotifications: number=0;
  allowedMaxSelection: number = environment.allowedMaxSelection;
  deleteConfirmMessage: string = "Content once deleted can not be restored. Please press Continue to begin the deletion.";
  displayedColumns: string[] = ['Preview', 'Title', 'Status', 'CreatedOn'];
  dataSource = ELEMENT_DATA;

  @ViewChild(MatAccordion) accordion: MatAccordion;
  cpSubscriptions;
  today;
  sendDate: Date;
  
  //  dataSource = ELEMENT_DATA;

  constructor( 
    private subscriptionService: SubscriptionService,
    private toastr: ToastrService,
    public dialog: MatDialog
    ) {
      var date = new Date();
      this.today = date.toISOString();
      const currentYear = new Date().getFullYear();
      const currentMonth = new Date().getMonth();
      const currentDay = new Date().getDate();
     }

     ngOnInit(): void { }
  
  //Can be uncommented once the API is integrated
  applyFilter(event: Event) {
    const filterValue = (event.target as HTMLInputElement).value;
    // this.dataSource.filter = filterValue.trim().toLowerCase();

    // if (this.dataSource.paginator) {
    //   this.dataSource.paginator.firstPage();
    // }
  }

  allowSelection(row) {
    return (!row.isSelected && this.selectedNotifications >= this.allowedMaxSelection);
  }
  openDialog(): void {
    const dialogRef = this.dialog.open(NotificationsDialog, {
      width: '500px',
      height: '650px',
      data: {}
    });

    dialogRef.componentInstance.onNotifCreate.subscribe(data => {
      this.toastr.success("Notification sent successfully!");
      //this.getSubscriptions(); 
      dialogRef.close();
    })

    dialogRef.afterClosed().subscribe(result => {
      console.log('The dialog was closed');
    });
  }

  createDataSource(rawData) {
    var dataSource: Content[] =[];
    if(rawData) {
      rawData.forEach( data => {
        data.status = data.contentTransformStatus !== ContentStatus.TRANSFORM_NOT_INITIALIZED ? 
        data.contentTransformStatus : data.contentUploadStatus;
        data.isSelected = false;
        dataSource.push(data);
      });
    }
    return new MatTableDataSource(dataSource);
  }
  
}
