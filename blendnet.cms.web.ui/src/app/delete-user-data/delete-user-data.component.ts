// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

import { DatePipe } from '@angular/common';
import { Component, Inject, LOCALE_ID, OnInit, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { ToastrService } from 'ngx-toastr';
import { CommonDialogComponent } from '../common-dialog/common-dialog.component';
import { AdditionalHistoryDialog } from '../devices/device-additional-history.component';
import { UserService } from '../services/user.service';

@Component({
  selector: 'app-delete-user-data',
  templateUrl: './delete-user-data.component.html',
  styleUrls: ['./delete-user-data.component.css']
})
export class DeleteUserDataComponent implements OnInit {
  today;
  displayedColumns: string[] = [ 'name', 'phoneNumber', 'status',  'displayModifiedDate','metadata', 'action'];
  pipe;
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  dataSource: MatTableDataSource<any>;
  errMessage;
  error= false;

  constructor(
    private userService: UserService,
    private toastr: ToastrService,
    public dialog: MatDialog,
    @Inject(LOCALE_ID) locale: string) {
      var date = new Date();
      this.today = date.toISOString();
      this.pipe = new DatePipe(locale);
    }

  ngOnInit(): void {
    this.getAllDeleteUserDataRequest();
  }

  refreshPage() {
    this.getAllDeleteUserDataRequest();
  }

  getAllDeleteUserDataRequest() {
    this.userService.getDeleteUserDataRequests().subscribe(res =>
      {
        this.dataSource = this.createDataSource(res);
        this.dataSource.paginator = this.paginator;
        this.dataSource.sort = this.sort;
        this.dataSource.sortingDataAccessor = (
          data: any,
          sortHeaderId: string
          ) => {
            if(sortHeaderId === "displayModifiedDate") {
              return new Date(data[sortHeaderId]);
            }
          if (typeof data[sortHeaderId] === 'string') {
            return data[sortHeaderId].toLocaleLowerCase();
          }
    
          return data[sortHeaderId];
        };
      },
      err =>
      console.log(err)
      );
  }



  createDataSource(rawData: any[]) {
    var dataSource: any[] =[];
    if(rawData && rawData.length > 0) {
      this.errMessage = "";
      this.error = false;
      rawData.forEach( data => {
        data.displayCreatedDate =  this.pipe.transform(data.createdDate, 'short');
        data.displayModifiedDate =  this.pipe.transform(data.modifiedDate, 'short');
        dataSource.push(data);
      });
    } else {
      this.errMessage = "No data found";
      this.error = true;
    }
    return new MatTableDataSource(dataSource);
  }

    
  applyFilter(event: Event) {
    const filterValue = (event.target as HTMLInputElement).value;
    this.dataSource.filter = filterValue.trim().toLowerCase();

    if (this.dataSource.paginator) {
      this.dataSource.paginator.firstPage();
    }
  }


  getRequestDetails(e) {
    if( e.phoneNumber.length === 10) {
      var request: any = {
        "phoneNumber": e.phoneNumber,
        "commandId": e.dataUpdateStatusUpdatedBy
      }
    } else {
      var request: any = {
        "userId": e.userId,
        "commandId": e.dataUpdateStatusUpdatedBy
      }
    }
   
    
    this.userService.getRequestDetails(request).subscribe(
      res => {
        const dialogRef = this.dialog.open(AdditionalHistoryDialog, {
          data: {content: res},
          width: '60%'
        });
      
        dialogRef.afterClosed().subscribe(result => {
          console.log('The dialog was closed');
        });
      },
      err => this.toastr.error(err)
    );
  

  }


  completeRequest(e) {
    const dialogRef = this.dialog.open(CommonDialogComponent, {
      data: {
        heading: 'Confirm',
        message: "Do you want to complete the data delete request by user " + e.phoneNumber,
        contents: e,
        action: "PROCESS",
        buttons: this.openSelectCPModalButtons()
      },
      maxHeight: '400px'
    });


    dialogRef.afterClosed().subscribe(result => {
      if (result === 'proceed') {
        this.onConfirmComplete(e);
      }
    });
  }

  onConfirmComplete(e): void {
  var user = {
    "userPhoneNumber": e.phoneNumber
  }
    this.userService.completeDataDeleteRequest(user).subscribe(
      res => 
      {
        this.toastr.success("Delete data request for user " + e.phoneNumber + " send for completion");
        this.getAllDeleteUserDataRequest();
      },
      err => this.toastr.error(err));
  }

  openSelectCPModalButtons(): Array<any> {
    return [{
      label: 'Cancel',
      type: 'basic',
      value: 'cancel',
      class: 'discard-btn'
    },
    {
      label: 'Confirm',
      type: 'primary',
      value: 'submit',
      class: 'update-btn'
    }
    ]
  }


  isRequestSubmitted(e) {
    return e.dataUpdateRequestStatus === "Submitted";
  }
}
