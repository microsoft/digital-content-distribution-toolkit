import { Component, OnInit, ViewChild } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import {MatAccordion} from '@angular/material/expansion';
import { MatPaginator } from '@angular/material/paginator';
import { MatDialog} from '@angular/material/dialog';
import { NotificationsDialog } from './notifications-dialog';
import {MatTableDataSource} from '@angular/material/table';
import { Content } from '../models/content.model';
import { NotificationService } from '../services/notification.service';
import { MatSort } from '@angular/material/sort';




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

  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  
  dataSource: MatTableDataSource<any>;

  constructor( 
    private notificationService: NotificationService,
    private toastr: ToastrService,
    public dialog: MatDialog
    ) {
      var date = new Date();
      this.today = date.toISOString();
      const currentYear = new Date().getFullYear();
      const currentMonth = new Date().getMonth();
      const currentDay = new Date().getDate();
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
    if(rawData) {
      rawData.forEach( data => {
        dataSource.push(data);
      });
    }
    return new MatTableDataSource(dataSource);
  }

  
}
