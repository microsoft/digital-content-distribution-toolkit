import { SelectionModel } from '@angular/cdk/collections';
import { Component,ViewChild} from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import {MatPaginator} from '@angular/material/paginator';
import {MatSort} from '@angular/material/sort';
import {MatTableDataSource} from '@angular/material/table';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
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
  displayedColumns: string[] = ['id', 'status', 'filterUpdateStatus' , 'filters', 'filters_history', 'assignment', 'assign_history'];
  dataSource: MatTableDataSource<any>;
  showDialog: boolean = false;
  initialSelection = [];
  allowMultiSelect = true;
  selection = new SelectionModel<any>(this.allowMultiSelect, this.initialSelection);

  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;

  errMessage;
  error= false;

  constructor(
    public dialog: MatDialog, 
    private toastr: ToastrService,
    private deviceService: DeviceService,
    private router: Router
    ) {

  }

  ngOnInit() {
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

  disableRetailerAssignment(row) {
    return (row.deviceStatus === 'Registered');
  }
  disabledCancelFilters(row) {

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

  

  showFilterHistory(deviceId): void {
    this.router.navigateByUrl('/devices/filters-history/'+ deviceId);
  }

  showAssignHistory(deviceId): void {
    this.router.navigateByUrl('/devices/assignment-history/'+ deviceId);
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
    width: '500px',
    data: {device : device},
    
  });

  dialogRef.componentInstance.onDeviceFilterUpdate.subscribe(data => {
    this.toastr.success("Device filters updated successfully!");
    // this.getDevices(); 
    dialogRef.close();
  })

  dialogRef.afterClosed().subscribe(result => {
    console.log('The dialog was closed');
  });

}

}

