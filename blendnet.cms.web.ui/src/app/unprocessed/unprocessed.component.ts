import { SelectionModel } from '@angular/cdk/collections';
import { Component, ElementRef, Inject, LOCALE_ID, ViewChild} from '@angular/core';
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
import { CommonDialogComponent } from '../common-dialog/common-dialog.component';
import { DatePipe } from '@angular/common';
import { ContentView } from '../models/content-view.model';
import { unprocessedContentFilters } from '../constants/content-status-filters';


@Component({
  selector: 'app-unprocessed',
  styleUrls: ['unprocessed.component.css'],
  templateUrl: 'unprocessed.component.html',
})
export class UnprocessedComponent {
  displayedColumns: string[] = ['select', 'title', 'status', 'createdDate', 'modifiedDate', 'view', 'isDeletable', 'isProcessable'];
  dataSource: MatTableDataSource<ContentView>;
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
  filterValue;
  myfilename = 'Choose a meta-data file to upload content';
  errMessage;
  error= false;
  fileName = '';
  pipe;
  contentList: ContentView[] = [];

  constructor(public dialog: MatDialog,
    public contentService: ContentService,
    private toastr: ToastrService,
    @Inject(LOCALE_ID) locale: string
    ) {
      this.pipe = new DatePipe(locale);
  }

  

  ngOnInit(): void {
    this.getUnprocessedContent();
  }

  refreshPage() {
    this.filterValue = '';
    this.getUnprocessedContent();
  }

  getUnprocessedContent() { 
    this.contentService.getContentByCpIdAndFilters(unprocessedContentFilters).subscribe(
      res => {
        this.contentList = res;
        this.dataSource = this.createDataSource(res);
        this.dataSource.paginator = this.paginator;
        this.dataSource.sort = this.sort;
        this.selectedContents = 0;
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
    return (!row.isSelected && this.selectedContents >= this.allowedMaxSelection);
  }

  createDataSource(rawData: ContentView[]) {
    var dataSource: ContentView[] =[];
    if(rawData && rawData.length > 0) {
      this.errMessage = "";
      this.error = false;
      rawData.forEach( data => {
        data.displayStatus = data.contentTransformStatus !== ContentStatus.TRANSFORM_NOT_INITIALIZED ? 
        data.contentTransformStatus : data.contentUploadStatus;
        data.isSelected = false;
        data.displayCreatedDate =  this.pipe.transform(data.createdDate, 'short');
        data.displayModifiedDate=  this.pipe.transform(data.modifiedDate, 'short');
        dataSource.push(data);
      });
    } else {
      this.errMessage = "No data found";
      this.error = true;
    }
    return new MatTableDataSource(dataSource);
  }

  isContentNotProcessable(row) {
    return row.contentUploadStatus !== ContentStatus.UPLOAD_COMPLETE
    || (row.contentTransformStatus !== ContentStatus.TRANSFORM_NOT_INITIALIZED && row.contentTransformStatus !== ContentStatus.TRANSFORM_FAILED);
  }

  isContentNotDeletable(row) {
    return (row.contentUploadStatus !== ContentStatus.UPLOAD_COMPLETE
    && row.contentUploadStatus !== ContentStatus.UPLOAD_FAILED)
    || (row.contentTransformStatus !== ContentStatus.TRANSFORM_NOT_INITIALIZED);  
  }


  applyFilter() { 
    this.dataSource.filter = this.filterValue.trim().toLowerCase();

    if (this.dataSource.paginator) {
      this.dataSource.paginator.firstPage();
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
        // this.fileName ='';
        return of(`Upload failed: ${file.data.name}`);
      })).subscribe((event: any) => {
        if (typeof (event) === 'object') {
          console.log(event.body);
          this.toastr.success(file.data.name + " Content uploaded successfully.");
          this.getUnprocessedContent();
          this.fileName ='';
        }  
      });  
      this.jsonFileInput.nativeElement.value = '';
     
  }

  onFileSelected(event) {

    this.fileUploadError = null;
    if (event.target.files && event.target.files[0]) {
      if(event.target.files[0].size > environment.maxFileUploadSize) {
        this.fileUploadError="Max file size allowed is : " +  environment.maxFileUploadSize;
        return false;
      }
      if(event.target.files[0].type !== environment.fileAllowedType) {
        this.fileUploadError="Only " + environment.fileAllowedType + " files are allowed";
        return false;
      }
      this.fileName = event.target.files[0].name;
      this.file = {
        data: event.target.files[0], 
        inProgress: false, 
        progress: 0
      }
      this.uploadFile(this.file);
      }
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
  const dialogRef = this.dialog.open(CommonDialogComponent, {
    data: {
      heading: 'Confirm',
      message: this.processConfirmMessage,
      contents: rows,
      action: "PROCESS",
      buttons: this.openSelectCPModalButtons()
    },
    maxHeight: '400px'
  });


  dialogRef.afterClosed().subscribe(result => {
    if (result === 'proceed') {
      this.onConfirmProcess(rows);
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

onConfirmProcess(contents): void {
  var selectedIds = contents.map(content => 
    {return content.id});
  var contentIds ={
    "contentIds": selectedIds
  }
  this.contentService.processContent(contentIds).subscribe(
    res => //this.toastr.success("Content/s submitted for transformation sucessfully!!"),
    this.successEmit(res),
    err => this.toastr.error(err));
}

successEmit(res) {
  if(res.length == 0) {
    this.toastr.success("Content/s submitted successfully for transformation");
  } else {
    this.toastr.warning(res[0]);
  }
  this.getUnprocessedContent();
}

openDeleteConfirmModal(row): void {
  const dialogRef = this.dialog.open(CommonDialogComponent, {
    data: {
      heading: 'Confirm',
      message: this.deleteConfirmMessage,
      action: "DELETE",
      contents: row,
      buttons: this.openSelectCPModalButtons()
    },
    maxWidth: '400px'
  });

  dialogRef.afterClosed().subscribe(result => {
    if (result === 'proceed') {
      this.onConfirmDelete(row.id);
    }
  });

}

  onConfirmDelete(contentId): void {
    this.contentService.deleteContent(contentId).subscribe(
      res => this.onDeleteSuccess(),
      err => this.toastr.error(err));
  }

  onDeleteSuccess() {
    this.toastr.success("Content submitted for deletion successfully");
    this.getUnprocessedContent();
  }

viewContent(id) : void {
  this.contentService.getContentById(id).subscribe(
    res => {
      const dialogRef = this.dialog.open(ContentDetailsDialog, {
        data: {content: res}
      });
    
      dialogRef.afterClosed().subscribe(result => {
        console.log('The dialog was closed');
      });
    },
    err => {
      this.toastr.error(err);
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
    content: Content;
    cast: string = '';
    attachments: string = '';

  ngOnInit(): void {
    this.content = this.data.content;
    this.data.content.people.filter(each => 
      this.cast += (each.role === 'Actor') ?  each.name + ' ' : ''
    );
    this.content.attachments.filter( each => {
      this.attachments +=  each.name + ' ';
    })
  }


}



