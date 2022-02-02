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
  selector: 'app-export-user-data',
  templateUrl: './export-user-data.component.html',
  styleUrls: ['./export-user-data.component.css']
})
export class ExportUserDataComponent implements OnInit {
  today;
  displayedColumns: string[] = [ 'username', 'phonenumber', 'status',  'modifiedDate','metadata', 'action'];
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
    this.getAllExportUserDataRequests();
  }

  refreshPage() {
    this.getAllExportUserDataRequests();
  }

  getAllExportUserDataRequests() {
    this.userService.getExportUserDataRequests().subscribe(res =>
      {
        this.dataSource = this.createDataSource(res);
        this.dataSource.paginator = this.paginator;
        this.dataSource.sort = this.sort;
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
    var request = {
      "phoneNumber": e.phoneNumber,
      "commandId": e.dataExportStatusUpdatedBy
    }
    
    this.userService.getExportRequestDetails(request).subscribe(
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
        message: "Do you want to complete the data export request by user " + e.phonenumber,
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
    this.userService.completeDataExportRequest(user).subscribe(
      res => 
      {
        this.toastr.success("Export data request for user " + e.phoneNumber + "send for completion");
        this.getAllExportUserDataRequests();
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
      label: 'Continue',
      type: 'primary',
      value: 'submit',
      class: 'update-btn'
    }
    ]
  }


  isRequestSubmitted(e) {
    return e.dataExportRequestStatus === "Submitted";
  }
}
