import { SelectionModel } from '@angular/cdk/collections';
import {AfterViewInit, Component, Inject, Output, ViewChild, EventEmitter} from '@angular/core';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import {MatPaginator} from '@angular/material/paginator';
import {MatSort} from '@angular/material/sort';
import {MatTableDataSource} from '@angular/material/table';
import { environment } from '../../environments/environment';
import { Content } from '../models/content.model';
import { ContentService } from '../services/content.service';
import { of} from 'rxjs';
import {catchError, map, } from 'rxjs/operators';
import {  HttpEventType } from '@angular/common/http';
import { ToastrService } from 'ngx-toastr';
import { ContentStatus } from '../models/content-status.enum';

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
  fileUploadError: string ="";
  showDialog: boolean = false;
  deleteConfirmMessage: string = "Content once deleted can not be restored. Please press Continue to begin the deletion.";
  processConfirmMessage: string = "Please press Continue to begin the transformation.";
  initialSelection = [];
  allowMultiSelect = true;
  selection = new SelectionModel<Content>(this.allowMultiSelect, this.initialSelection);
  file;
  selectedContents: number =0;
  allowedMaxSelection: number = environment.allowedMaxSelection;
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  @ViewChild('jsonFileInput') jsonFileInput;
  // polling: Subscription;

  constructor(public dialog: MatDialog,
    public contentService: ContentService,
    private toastr: ToastrService
    ) {
  }

  

  ngOnInit(): void {
    this.getUnprocessedContent();
  }

  getUnprocessedContent() {
    var unprocessedContentFilters = {
      "contentUploadStatuses": [
         ContentStatus.UPLOAD_SUBMITTED, 
         ContentStatus.UPLOAD_INPROGRESS, 
         ContentStatus.UPLOAD_FAILED,
         ContentStatus.UPLOAD_COMPLETE
      ],
      "contentTransformStatuses": [
        ContentStatus.TRANSFORM_NOT_INITIALIZED,
        ContentStatus.TRANSFORM_SUBMITTED,
        ContentStatus.TRANSFORM_INPROGRESS,
        ContentStatus.TRANSFORM_AMS_JOB_INPROGRESS,
        ContentStatus.TRANSFORM_DOWNLOAD_INPROGRESS
        // ContentStatus.TRANSFORM_DOWNLOAD_COMPLETE
      ],
      "contentBroadcastStatuses": [
         ContentStatus.BROADCAST_NOT_INITIALIZED
      ]
    }
    this.contentService.getContentByCpIdAndFilters(unprocessedContentFilters).subscribe(
      res => {
        this.dataSource = this.createDataSource(res.body);
        this.selectedContents = 0;
      },
      err => {
        this.toastr.error(err);
        console.log('HTTP Error', err)
      }
    );
  }
  toggleSelection(event, row) {
    if(event.checked){
        this.selectedContents++;
        row.isSelected = true;
      }else{
        this.selectedContents--;
        row.isSelected = false;
      }
  }

  allowSelection(row) {
    if(!row.isSelected && this.selectedContents >= this.allowedMaxSelection)
      return true;
    return false;
  }

  createDataSource(rawData) {
    var dataSource: Content[] =[];
    rawData.forEach( data => {
      data.isSelected = false;
      dataSource.push(data);
    });
    return new MatTableDataSource(dataSource);
  }

  isContentNotProcessable(row) {
    return row.contentUploadStatus !== ContentStatus.UPLOAD_COMPLETE
    || (row.contentTransformStatus !== ContentStatus.TRANSFORM_NOT_INITIALIZED
    && row.contentTransformStatus !== ContentStatus.TRANSFORM_FAILED);
  }

  isContentNotDeletable(row) {
    return row.contentUploadStatus !== ContentStatus.UPLOAD_COMPLETE
    || (row.contentTransformStatus !== ContentStatus.TRANSFORM_NOT_INITIALIZED
    && row.contentTransformStatus !== ContentStatus.TRANSFORM_FAILED);  
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
          this.getUnprocessedContent();
        }  
      });  

      this.jsonFileInput.nativeElement.value = '';
  }




openProcessConfirmModal(row): void {
  var rows = [];
  if(row) {
    rows.push(row);
    this.openProcessDialog(rows);
  } else {
    rows = this.dataSource ? this.dataSource.data.filter(d => d.isSelected) : [];
    if(rows.length <1) {
      this.toastr.warning("Please select one or more content/s for processing");
    } else {
      var notEligibleForTransform = rows.filter(d => 
        (d.contentUploadStatus !== ContentStatus.UPLOAD_COMPLETE 
        || (d.contentTransformStatus !== ContentStatus.TRANSFORM_NOT_INITIALIZED
        && d.contentTransformStatus!== ContentStatus.TRANSFORM_FAILED)));
        if(notEligibleForTransform.length > 0) {
          this.toastr.warning("One or more selected content/s cannot be processed");
        } else {
            this.openProcessDialog(rows);          
        }
    }    
  }
}
openProcessDialog(rows) {
  const dialogRef = this.dialog.open(UnprocessConfirmDialog, {
    data: {
      message: this.processConfirmMessage,
      contents: rows,
      action: "PROCESS"
    },
    width: '60%'
  });
  dialogRef.componentInstance.onSuccessfulSubmission.subscribe(res => {
    if(res.body.length == 0) {
      this.toastr.success("Content/s submitted successfully for transformation");
    } else {
      this.toastr.warning(res.body[0]);
    }
    this.getUnprocessedContent();
    dialogRef.close();
  })

  dialogRef.afterClosed().subscribe(result => {
    console.log('The dialog was closed');
  });
}

openDeleteConfirmModal(): void {
  const dialogRef = this.dialog.open(UnprocessConfirmDialog, {
    data: {
      message: this.deleteConfirmMessage,
      action: "DELETE"
    },
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
  selector: 'process-confirm-dialog',
  templateUrl: 'unprocess-action-confirm-dialog.html',
  styleUrls: ['unprocessed.component.css']
})
export class UnprocessConfirmDialog {
  
  @Output() onSuccessfulSubmission= new EventEmitter<any>();

  constructor(
    public dialogRef: MatDialogRef<UnprocessConfirmDialog>,
    @Inject(MAT_DIALOG_DATA) public data: any,
    public contentService: ContentService,
    private toastr: ToastrService) {}


  onConfirm() {
    if(this.data.action === "DELETE") {
      this.onConfirmDelete(this.data.contents);
    } else {
      this.onConfirmProcess(this.data.contents);
    }
  }

  onCancel(): void {
    this.dialogRef.close();
  }

  onConfirmProcess(contents): void {
    var selectedIds = contents.map(content => 
      {return content.id});
    var contentIds ={
      "contentIds": selectedIds
    }
    this.contentService.processContent(contentIds).subscribe(
      res => //this.toastr.success("Content/s submitted for transformation sucessfully!!"),
      this.onSuccessfulSubmission.emit(res),
      err => this.toastr.error(err));
    this.dialogRef.close();
  }



  onConfirmDelete(contentId): void {
    this.contentService.processContent(null).subscribe(
      res => this.toastr.success("Content/s submitted for transformation sucessfully!!"),
      err => this.toastr.error(err));
    this.dialogRef.close();
  }

}




