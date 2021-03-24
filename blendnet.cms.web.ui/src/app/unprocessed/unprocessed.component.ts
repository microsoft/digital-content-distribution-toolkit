import { SelectionModel } from '@angular/cdk/collections';
import {AfterViewInit, Component, Inject, ViewChild} from '@angular/core';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import {MatPaginator} from '@angular/material/paginator';
import {MatSort} from '@angular/material/sort';
import {MatTableDataSource} from '@angular/material/table';
import { environment } from '../../environments/environment';
import { Content } from '../models/content.model';
import { ContentService } from '../services/content.service';


export interface DialogData {
  message: string;
}
/**
 * @title Data table with sorting, pagination, and filtering.
 */
@Component({
  selector: 'app-unprocessed',
  styleUrls: ['unprocessed.component.css'],
  templateUrl: 'unprocessed.component.html',
})
export class UnprocessedComponent implements AfterViewInit {
  displayedColumns: string[] = ['select', 'title', 'status', 'view', 'isProcessable', 'isDeletable'];
  dataSource: MatTableDataSource<Content>;
  // fileToUpload: File = null;
  fileUploadError: string ="";
  showDialog: boolean = false;
  animal: string;
  message: string = "Please press OK to continue.";
  initialSelection = [];
  allowMultiSelect = true;
  selection = new SelectionModel<Content>(this.allowMultiSelect, this.initialSelection);

  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;

  constructor(public dialog: MatDialog,
    public contentService: ContentService
    ) {
  }

  

  ngOnInit(): void {
    this.contentService.getContentByCpIdAndFilters().subscribe(res => {
      if(res.status === 200) {
        this.dataSource = res.body;
      }
    })
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

  handleFileInput(files: any) {
    this.fileUploadError = null;
    if (files.target.files && files.target.files[0]) {
      if(files.target.files[0].size > environment.maxFileUploadSize) {
        this.fileUploadError="Max file size allowed is : " +  environment.maxFileUploadSize;
        return false;
      }
      if(files.target.files[0].type !== environment.fileAllowedType) {
        this.fileUploadError="Only " + environment.fileAllowedType + " files are allowed";
        return false;
      }
      this.uploadFile(files.target.files[0]);
    }
  }

  uploadFile(file: File) {
    window.alert("Call upload file service for : " + file.name);
    // this.fileUploadService.postFile(this.fileToUpload).subscribe(data => {
    //   // do something, if upload success
    //   }, error => {
    //     console.log(error);
    //   });
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



 openUploadConfirmModal(): void {
  const dialogRef = this.dialog.open(UnprocessConfirmDialog, {
    data: {message: this.message},
    width: '40%'
  });

  dialogRef.afterClosed().subscribe(result => {
    console.log('The dialog was closed');
  });
}
openProcessConfirmModal(): void {
  const dialogRef = this.dialog.open(UnprocessConfirmDialog, {
    data: {message: this.message}
  });

  dialogRef.afterClosed().subscribe(result => {
    console.log('The dialog was closed');
  });
}
openDeleteConfirmModal(): void {
  const dialogRef = this.dialog.open(UnprocessConfirmDialog, {
    data: {message: this.message},
    width: '40%'
  });

  dialogRef.afterClosed().subscribe(result => {
    console.log('The dialog was closed');
  });
}

viewContent() : void {
  const dialogRef = this.dialog.open(ContentDetailsDialog, {
    data: {content: null},
    // width: '40%'
  });

  dialogRef.afterClosed().subscribe(result => {
    console.log('The dialog was closed');
  });

}

}

@Component({
  selector: 'content-detail-dialog',
  templateUrl: 'content-detail-dialog.html',
  styleUrls: ['unprocessed.component.css']
})
export class ContentDetailsDialog {

  constructor(
    public dialogRef: MatDialogRef<ContentDetailsDialog>,
    @Inject(MAT_DIALOG_DATA) public data: any) {}
    content: Content

  ngOnInit(): void {
    this.content = this.data.content;
  }
  onCancelUpload(): void {
    this.dialogRef.close();
  }

  onConfirmUpload(): void {
    this.dialogRef.close();
  }

}


@Component({
  selector: 'upload-confirm-dialog',
  templateUrl: 'upload-confirm-dialog.html',
  styleUrls: ['unprocessed.component.css']
})
export class UnprocessConfirmDialog {

  constructor(
    public dialogRef: MatDialogRef<UnprocessConfirmDialog>,
    @Inject(MAT_DIALOG_DATA) public data: DialogData) {}

  onCancelUpload(): void {
    this.dialogRef.close();
  }

  onConfirmUpload(): void {
    this.dialogRef.close();
  }

}




