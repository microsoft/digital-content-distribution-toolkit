import { SelectionModel } from '@angular/cdk/collections';
import {AfterViewInit, Component, Inject, ViewChild} from '@angular/core';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import {MatPaginator} from '@angular/material/paginator';
import {MatSort} from '@angular/material/sort';
import {MatTableDataSource} from '@angular/material/table';
import { environment } from '../../environments/environment';
import { Content } from '../models/content.model';
import { ContentService } from '../services/content.service';
import {interval, of, Subscription} from 'rxjs';
import {catchError, map, startWith, switchMap} from 'rxjs/operators';
import { HttpErrorResponse, HttpEventType } from '@angular/common/http';
import { ToastrService } from 'ngx-toastr';

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
  file;
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  @ViewChild('jsonFileInput') jsonFileInput;
  polling: Subscription;

  constructor(public dialog: MatDialog,
    public contentService: ContentService,
    private toastr: ToastrService
    ) {
  }

  

  ngOnInit(): void {
    this.polling = interval(50000)
    .pipe(
      startWith(0),
      switchMap(() => this.contentService.getContentByCpIdAndFilters())
    ).subscribe(res => this.dataSource = this.createDataSource(res.body),
    err => console.log('HTTP Error', err));
  }

  ngOnDestroy() {
    if(this.polling){    
    this.polling.unsubscribe();
    }
  }

  createDataSource(rawData) {
    var dataSource: Content[] =[];
    rawData.forEach( data => {
      dataSource.push(data);
    });
    return new MatTableDataSource(dataSource);
  }
  isContentProcessable(row) {
    return row.contentUploadStatus !== "UploadComplete";
  }

  isContentDeletable(row) {
    return (row.contentUploadStatus === "UploadInProgress" 
      || row.contentUploadStatus === "UploadSubmitted");

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
      this.file = {
        data: files.target.files[0], 
        inProgress: false, 
        progress: 0
      }
      //this.uploadFile(files.target.files[0]);
      this.uploadFile(this.file);
    }
  }

  uploadFile(file) {
    const formData = new FormData(); 
    formData.append('file', file.data);  
    file.inProgress = true;
    this.contentService.uploadContent(formData).pipe(
      map(event => {
        switch (event.type) {
          case HttpEventType.UploadProgress:
            file.progress = Math.round(event.loaded * 100 / event.total);
            break;
          case HttpEventType.Response:
            return event;
        }  
      }),  
      catchError((error) => {
        file.inProgress = false;
        this.toastr.error(error);
        return of(`Upload failed: ${file.data.name}`);
      })).subscribe((event: any) => {
        if (typeof (event) === 'object') {
          console.log(event.body);
        }  
      });  

      this.jsonFileInput.nativeElement.value = '';
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

viewContent(selectedContent) : void {
  const dialogRef = this.dialog.open(ContentDetailsDialog, {
    data: {content: selectedContent},
    width: '70%',
    height: '70%'
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




