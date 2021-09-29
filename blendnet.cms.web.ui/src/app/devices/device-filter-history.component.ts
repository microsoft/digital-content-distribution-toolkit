import { Component, OnInit, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { ActivatedRoute, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { CommonDialogComponent } from '../common-dialog/common-dialog.component';
import { DeviceService } from '../services/device.service';
import { AdditionalHistoryDialog } from './device-additional-history.component';

@Component({
  selector: 'app-device-filter-history',
  templateUrl: './device-filter-history.component.html',
  styleUrls: ['./devices.component.css']
})
export class DeviceFilterHistoryComponent implements OnInit {
  deviceid;
  displayedColumns: string[] = ['id', 'deviceCommandStatus', 'filters' , 'createdDate',  'details', 'cancel_command'];
  dataSource: MatTableDataSource<any>;
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  errMessage;
  error= false;
  cancelConfirmMessage: string = "WARNING!!!!! Please use this action cautiously. Cancelling a command takes few minutes. Press Continue to CANCEL the command.";
  constructor(
    private deviceService: DeviceService,
    public dialog: MatDialog,
    private toastr: ToastrService,
    private route: ActivatedRoute,
    private router: Router

  ) { 
    this.route.params.subscribe(device => this.deviceid = device.id);
  }

  ngOnInit(): void {
    this.deviceService.getDeviceHistory(this.deviceid).subscribe(res => {
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

  disabledCancelFilters(row){
    return row.deviceCommandStatus !== 'DeviceCommandPushToDevice';
  }


  openConfirmCancelCommand(row) {

    const dialogRef = this.dialog.open(CommonDialogComponent, {
      data: {
        heading: 'Confirm',
        message: this.cancelConfirmMessage,
        contents: row,
        action: "PROCESS",
        buttons: this.openSelectCPModalButtons()
      },
      maxHeight: '400px'
    });
  
  
    dialogRef.afterClosed().subscribe(result => {
      if (result === 'proceed') {
        this.cancelCommand(row);
      }
    });
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

  cancelCommand(row) {
    this.deviceService.cancelCommand(this.deviceid , row.id).subscribe(res => {
      this.toastr.success("Cancel command initiated. This may take a while for completion");
    },
    err => {
      this.toastr.error(err);
    })
  }

  createDataSource(rawDataList) {
    var dataSource: any[] =[];
    if(rawDataList) {
      rawDataList.forEach( rawData => {
        dataSource.push(rawData);
      });
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

  openAdditionalDetailsHistory(row) {
    const dialogRef = this.dialog.open(AdditionalHistoryDialog, {
      data: {
        history : row
      },
      width: '60%'
    });
  
    dialogRef.afterClosed().subscribe(result => {
      console.log('The dialog was closed');
    });
  }
  showDevicesPage() {
    this.router.navigateByUrl('/admin/devices');
  }


}
