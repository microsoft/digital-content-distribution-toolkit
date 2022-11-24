// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

import { SelectionModel } from '@angular/cdk/collections';
import {AfterViewInit, Component, Inject, QueryList, ViewChildren} from '@angular/core';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import {MatPaginator} from '@angular/material/paginator';
import {MatSort} from '@angular/material/sort';
import {MatTableDataSource} from '@angular/material/table';

export interface UserData {
  id: string;
  name: string;
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
  selector: 'app-manage-content',
  styleUrls: ['manage-content.component.css'],
  templateUrl: 'manage-content.component.html',
})
export class ManageContentComponent implements AfterViewInit {
  displayedColumns: string[] = ['select', 'id', 'name', 'action'];
  dataSourceExistingDevices: MatTableDataSource<UserData>;
  dataSourceNewDevices: MatTableDataSource<UserData>;
  fileToUpload: File = null;
  showDialog: boolean = false;
  animal: string;
  message: string = "Please press OK to continue.";
  initialSelection = [];
  allowMultiSelect = true;
  selection = new SelectionModel<UserData>(this.allowMultiSelect, this.initialSelection);

  @ViewChildren(MatPaginator) paginator = new QueryList<MatPaginator>();
  @ViewChildren(MatSort) sort = new QueryList<MatSort>();

  constructor(public dialog: MatDialog
    ) {
    // Create 100 users
    const users = Array.from({length: 100}, (_, k) => createNewUser(k + 1));

    // Assign the data to the data source for the table to render
    this.dataSourceExistingDevices = new MatTableDataSource(users);
    this.dataSourceNewDevices = new MatTableDataSource(users);
  }

  ngAfterViewInit() {
    this.dataSourceExistingDevices.paginator = this.paginator.toArray()[0];
    this.dataSourceExistingDevices.sort = this.sort.toArray()[0];
    this.dataSourceNewDevices.paginator = this.paginator.toArray()[1];
    this.dataSourceNewDevices.sort = this.sort.toArray()[1];
  }

  applyFilterExistingDevices(event: Event) {
    const filterValue = (event.target as HTMLInputElement).value;
    this.dataSourceExistingDevices.filter = filterValue.trim().toLowerCase();

    if (this.dataSourceExistingDevices.paginator) {
      this.dataSourceExistingDevices.paginator.firstPage();
    }
  }

  applyFilterNewDevices(event: Event) {
    const filterValue = (event.target as HTMLInputElement).value;
    this.dataSourceNewDevices.filter = filterValue.trim().toLowerCase();

    if (this.dataSourceNewDevices.paginator) {
      this.dataSourceNewDevices.paginator.firstPage();
    }
  }

  isAllSelectedExistingDevices() {
    const numSelected = this.selection.selected.length;
    const numRows = this.dataSourceExistingDevices.data.length;
    return numSelected == numRows;
  }

  isAllSelectedNewDevices() {
    const numSelected = this.selection.selected.length;
    const numRows = this.dataSourceNewDevices.data.length;
    return numSelected == numRows;
  }

  masterToggleExistingDevices() {
    this.isAllSelectedExistingDevices() ?
        this.selection.clear() :
        this.dataSourceExistingDevices.data.forEach(row => this.selection.select(row));
  }

  
  masterToggleNewDevices() {
    this.isAllSelectedNewDevices() ?
        this.selection.clear() :
        this.dataSourceNewDevices.data.forEach(row => this.selection.select(row));
  }



  openDeleteConfirmModal(): void {
  const dialogRef = this.dialog.open(DeviceConfirmDialog, {
    data: {message: this.message}
  });

  dialogRef.afterClosed().subscribe(result => {
    console.log('The dialog was closed');
  });
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
  selector: 'manage-content-confirm-dialog',
  templateUrl: 'manage-content-confirm-dialog.html',
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

  return {
    id: id.toString(),
    name: name,
  };
}



