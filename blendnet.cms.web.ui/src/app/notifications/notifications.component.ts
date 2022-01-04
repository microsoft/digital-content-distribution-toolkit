import { Component, Inject, LOCALE_ID, OnInit, ViewChild } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import {MatAccordion} from '@angular/material/expansion';
import { MatPaginator } from '@angular/material/paginator';
import { MatDialog} from '@angular/material/dialog';
import { NotificationsDialog } from './notifications-dialog';
import {MatTableDataSource} from '@angular/material/table';
import { Content } from '../models/content.model';
import { NotificationService } from '../services/notification.service';
import { MatSort } from '@angular/material/sort';
import { DatePipe } from '@angular/common';




@Component({
  selector: 'app-notifications',
  templateUrl: './notifications.component.html',
  styleUrls: ['./notifications.component.css']
})
export class NotificationsComponent implements OnInit {
  expiresIn:string= "";
  selectedNotifications: number=0;
  displayedColumns: string[] = [ 'title', 'body', 'type', 'attachmentUrl', 'tags','createdDate'];

  @ViewChild(MatAccordion) accordion: MatAccordion;
  cpSubscriptions;
  today;
  sendDate: Date;
  errMessage;
  error= false;
  pipe;
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  
  dataSource: MatTableDataSource<any>;

  constructor( 
    private notificationService: NotificationService,
    private toastr: ToastrService,
    public dialog: MatDialog,
    @Inject(LOCALE_ID) locale: string
    ) {
      var date = new Date();
      this.today = date.toISOString();
      this.pipe = new DatePipe(locale);
     }

     ngOnInit(): void {
       this.getAllNotifications();
      }

      getAllNotifications() {

        this.notificationService.getAllNotifications().subscribe(
          res => {
            this.dataSource = this.createDataSource(res.data);
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


  openDialog(): void {
    const dialogRef = this.dialog.open(NotificationsDialog, {
      width: '500px',
      data: {}
    });

    dialogRef.componentInstance.onNotificationBroadcast.subscribe(data => {
      this.toastr.success("Notification sent successfully!");
      this.getAllNotifications(); 
      dialogRef.close();
    })

    dialogRef.afterClosed().subscribe(result => {
      console.log('The dialog was closed');
    });
  }

  createDataSource(rawData) {
    var dataSource: Content[] =[];
    if(rawData && rawData.length > 0) {
      this.errMessage = "";
      this.error = false;
      rawData.forEach( data => {
        data.createdDateString =  this.pipe.transform(data.createdDate, 'short');
        dataSource.push(data);
      });
    } else {
      this.errMessage = "No data found";
      this.error = true;
    }
    return new MatTableDataSource(dataSource);
  }

  
}
