import { SelectionModel } from '@angular/cdk/collections';
import {AfterViewInit, Component, Inject, ViewChild} from '@angular/core';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import {MatPaginator} from '@angular/material/paginator';
import {MatSort} from '@angular/material/sort';
import {MatTableDataSource} from '@angular/material/table';
import { ToastrService } from 'ngx-toastr';
import { CommonDialogComponent } from '../common-dialog/common-dialog.component';
import { DeviceDialogComponent } from './device-dialog.component';

export interface UserData {
  id: string;
  name: string;
  status: string;
  existingFilters: string[];
}


const NAMES: string[] = [
  'Map-100-KA', 'MAP-200-MH', 'MAP-200-TN', 'MAP-200-MP', 'MAP-200-UP', 'MAP-200-BH'
];

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
export class DevicesComponent implements AfterViewInit {
  displayedColumns: string[] = ['select', 'id', 'name', 'status' , 'filters', 'delete'];
  dataSource: MatTableDataSource<UserData>;
  fileToUpload: File = null;
  showDialog: boolean = false;
  animal: string;
  message: string = "Please press OK to continue.";
  initialSelection = [];
  allowMultiSelect = true;
  selection = new SelectionModel<UserData>(this.allowMultiSelect, this.initialSelection);

  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;

  constructor(public dialog: MatDialog, private toastr: ToastrService
    ) {
    // Create 100 users
    const users = Array.from({length: 100}, (_, k) => createNewUser(k + 1));

    // Assign the data to the data source for the table to render
    this.dataSource = new MatTableDataSource(users);
  }

  ngAfterViewInit() {
    this.getDevices();
    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;
  }
  getDevices() {

  }

  applyFilter(event: Event) {
    const filterValue = (event.target as HTMLInputElement).value;
    this.dataSource.filter = filterValue.trim().toLowerCase();

    if (this.dataSource.paginator) {
      this.dataSource.paginator.firstPage();
    }
  }


  openDialog(): void {
    const dialogRef = this.dialog.open(DeviceDialogComponent, {
      width: '500px',
      data: {heading : "Add a Device"},
      
    });

    // dialogRef.componentInstance.onNotificationBroadcast.subscribe(data => {
    //   this.toastr.success("Notification sent successfully!");
    //   this.getAllNotifications(); 
    //   dialogRef.close();
    // })

    dialogRef.afterClosed().subscribe(result => {
      console.log('The dialog was closed');
    });
  }



  openDeleteConfirmModal(): void {
    const dialogRef = this.dialog.open(CommonDialogComponent, {
      disableClose: true,
      data: {message: "Please confirm to continue with your selection", heading:'Confirm',
        buttons: this.deleteDeviceButtons()
      },
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result === 'proceed') {
        console.log('proceed');
        this.toastr.success("Successfully deleted device");
        this.getDevices();
      }
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

openFiltersConfirmModal(): void {
  const dialogRef = this.dialog.open(DeviceConfirmDialog, {
    data: {message: this.message},
    width: '40%'
  });

  dialogRef.afterClosed().subscribe(result => {
    console.log('The dialog was closed');
  });
}

}

@Component({
  selector: 'devices-confirm-dialog',
  templateUrl: 'devices-confirm-dialog.html',
})
export class DeviceConfirmDialog {

  constructor(
    public dialogRef: MatDialogRef<DeviceConfirmDialog>,
    @Inject(MAT_DIALOG_DATA) public data: DialogData) {}

  onCancelUpload(): void {
    this.dialogRef.close();
  }

  onConfirmUpload(): void {
    this.dialogRef.close();
  }

}


/** Builds and returns a new User. */
function createNewUser(id: number): UserData {
  const name = NAMES[Math.round(Math.random() * (NAMES.length - 1))] ;
  const status = id % 2 === 0 ? 'Active': 'Inactive';

  return {
    id: id.toString(),
    name: name,
    status: status,
    existingFilters:['filter2', 'filter5','filter1', 'filter4', 'filter0', 'filter10',
    'filter2', 'filter5','filter1', 'filter4', 'filter0', 'filter10']
  };
}



