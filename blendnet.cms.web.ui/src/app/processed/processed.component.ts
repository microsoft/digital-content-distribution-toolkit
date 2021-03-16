import { SelectionModel } from '@angular/cdk/collections';
import {AfterViewInit, Component, Inject, ViewChild} from '@angular/core';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import {MatPaginator} from '@angular/material/paginator';
import {MatSort} from '@angular/material/sort';
import {MatTableDataSource} from '@angular/material/table';

export interface UserData {
  id: string;
  name: string;
  status: string;
  isBroadcastable: boolean;
  isDeletable: boolean
}


const NAMES: string[] = [
  'Dabangg', 'Bajrangi Bhaijaan', 'Don', 'RamLeela', 'Race 3', 'KingKong'
];

export interface DialogData {
  message: string;
}
/**
 * @title Data table with sorting, pagination, and filtering.
 */
@Component({
  selector: 'app-processed',
  styleUrls: ['processed.component.css'],
  templateUrl: 'processed.component.html',
})
export class ProcessedComponent implements AfterViewInit {
  displayedColumns: string[] = ['select', 'id', 'name', 'status', 'isBroadcastable', 'isDeletable'];
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

  constructor(public dialog: MatDialog
    ) {
    // Create 100 users
    const users = Array.from({length: 100}, (_, k) => createNewUser(k + 1));

    // Assign the data to the data source for the table to render
    this.dataSource = new MatTableDataSource(users);
  }

  ngAfterViewInit() {
    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;
  }

  applyFilter(event: Event) {
    const filterValue = (event.target as HTMLInputElement).value;
    this.dataSource.filter = filterValue.trim().toLowerCase();

    if (this.dataSource.paginator) {
      this.dataSource.paginator.firstPage();
    }
  }

  isAllSelected() {
    const numSelected = this.selection.selected.length;
    const numRows = this.dataSource.data.length;
    return numSelected == numRows;
  }

  masterToggle() {
    this.isAllSelected() ?
        this.selection.clear() :
        this.dataSource.data.forEach(row => this.selection.select(row));
  }



 openDeleteConfirmModal(): void {
  const dialogRef = this.dialog.open(ProcessConfirmDialog, {
    data: {message: this.message},
    width: '40%'
  });

  dialogRef.afterClosed().subscribe(result => {
    console.log('The dialog was closed');
  });
}
openBroadcastConfirmModal(): void {
  const dialogRef = this.dialog.open(ProcessConfirmDialog, {
    data: {message: this.message},
    width: '40%'
  });

  dialogRef.afterClosed().subscribe(result => {
    console.log('The dialog was closed');
  });
}

}

@Component({
  selector: 'process-confirm-dialog',
  templateUrl: 'process-confirm-dialog.html',
})
export class ProcessConfirmDialog {

  constructor(
    public dialogRef: MatDialogRef<ProcessConfirmDialog>,
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
  const status = id % 2 === 0 ? 'Processed': 'Broadcasting';
  const isBroadcastableVal = status === 'Processed'? true: false;
  const isDeletableVal = status === 'Processed'? true: false;

  return {
    id: id.toString(),
    name: name,
    status: status,
    isBroadcastable: isBroadcastableVal,
    isDeletable: isDeletableVal
  };
}



