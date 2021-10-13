import { SelectionModel } from '@angular/cdk/collections';
import { Component,ViewChild} from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import {MatPaginator} from '@angular/material/paginator';
import {MatSort} from '@angular/material/sort';
import {MatTableDataSource} from '@angular/material/table';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { environment } from 'src/environments/environment';
import { CommonDialogComponent } from '../common-dialog/common-dialog.component';
import { DeviceService } from '../services/device.service';
import { DeviceAssignComponent } from './device-assign.component';
import { DeviceDialogComponent } from './device-dialog.component';
import { DeviceFiltersComponent } from './device-filters.component';

export interface UserData {
  id: string;
  name: string;
  status: string;
  existingFilters: string[];
}


export interface DialogData {
  message: string;
}
/**
 * @title Data table with sorting, pagination, and filtering.
 */
@Component({
  selector: 'app-devices',
  styleUrls: ['devices.component.css'],
  templateUrl: 'devices.component.html',
})
export class DevicesComponent {
  displayedColumns: string[] = ['id', 'status', 'status_update_date', 'filterUpdateStatus' , 'filters', 'cancel_command', 'assignment', 'content', 'history'];
  dataSource: MatTableDataSource<any>;
  showDialog: boolean = false;
  initialSelection = [];
  allowMultiSelect = true;
  selection = new SelectionModel<any>(this.allowMultiSelect, this.initialSelection);
  cancelConfirmMessage: string = "WARNING!!!!! Please use this action cautiously. Cancelling a command takes few minutes. Press Continue to CANCEL the command.";

  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;

  errMessage;
  error= false;
  isDeviceMgmtRole = true;
  constructor(
    public dialog: MatDialog, 
    private toastr: ToastrService,
    private deviceService: DeviceService,
    private router: Router
    ) {

  }

  ngOnInit() {
    const userRoles = sessionStorage.getItem("roles")?.split(",");
    this.isDeviceMgmtRole =  userRoles.includes(environment.roles.HubDeviceManagement);
    this.getDevices();
   
  }

  getDevices() {
    this.deviceService.getDevices().subscribe(
      res => {
        var data:any = res.body;
        if(data.length>0) {
          this.dataSource = this.createDataSource(data);
          this.dataSource.paginator = this.paginator;
          this.dataSource.sort = this.sort;
        } else {
          this.error = true;
          this.dataSource = this.createDataSource([]);
          this.errMessage = "No data found";
        }
        
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
  disabledApplyFilters(row){
    return   (row.filterUpdateStatus === 'DeviceCommandSubmitted ' || row.filterUpdateStatus === 'DeviceCommandInProcess ' || row.filterUpdateStatus === 'DeviceCommandPushedToDevice') 
    || row.deviceStatus === 'Registered';
  }

  isDeviceNotProvisioned(row) {
    return (row.deviceStatus === 'Registered');
  }

  createDataSource(rawDataList) {
    var dataSource: any[] =[];
    if(rawDataList && rawDataList.length > 0) {
      this.errMessage = "";
      this.error = false;
      rawDataList.forEach( rawData => {
        dataSource.push(rawData);
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

  
  disabledCancelFilters(row){
    return row.filterUpdateStatus !== 'DeviceCommandPushedToDevice';
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
    this.deviceService.cancelCommand(row.id, row.filterUpdateStatusUpdatedBy ).subscribe(res => {
      this.toastr.success("Device command cancelled successfully.");
      this.getDevices(); 
    },
    err => {
      this.toastr.error(err);
    })
  }

  showFilterHistory(deviceId): void {
    this.router.navigateByUrl('/admin/devices/filters-history/'+ deviceId);
  }

  showAssignHistory(deviceId): void {
    this.router.navigateByUrl('/admin/devices/assignment-history/'+ deviceId);
  }

  showDeviceContents(deviceId): void {
    this.router.navigateByUrl('/admin/devices/contents/'+ deviceId);
  }
  assignment(deviceId) {
    const dialogRef = this.dialog.open(DeviceAssignComponent, {
      width: '500px',
      data: {
        heading : "Device Assignment/Unassignment",
        deviceId: deviceId
    },
      
    });

    dialogRef.afterClosed().subscribe(result => {
      console.log('The dialog was closed');
    });
  }

  openCreateDeviceDialog(): void {
    const dialogRef = this.dialog.open(DeviceDialogComponent, {
      width: '500px',
      data: {heading : "Add a Device"},
      
    });

    dialogRef.componentInstance.onDeviceCreate.subscribe(data => {
      this.toastr.success("New device added successfully!");
      this.getDevices(); 
      dialogRef.close();
    })

    dialogRef.afterClosed().subscribe(result => {
      console.log('The dialog was closed');
    });
  }



  deleteDeviceButtons(): Array<any> {
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

openFiltersConfirmModal(device) {
  const dialogRef = this.dialog.open(DeviceFiltersComponent, {
    width: '60%',
    data: {device : device},
    
  });

  dialogRef.componentInstance.onDeviceFilterUpdate.subscribe(data => {
    this.toastr.success("Apply filters command submitted. This may take a while for completion");
    dialogRef.close();
    this.getDevices(); 
  })

  dialogRef.afterClosed().subscribe(result => {
    console.log('The dialog was closed');
  });

}

}

