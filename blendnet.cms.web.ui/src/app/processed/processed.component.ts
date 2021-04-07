import { SelectionModel } from '@angular/cdk/collections';
import { AfterViewInit, Component, Inject, ViewChild} from '@angular/core';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { environment } from '../../environments/environment';
import { Content } from '../models/content.model';
import { ContentService } from '../services/content.service';
import {interval, of, Subscription} from 'rxjs';
import {startWith, switchMap} from 'rxjs/operators';
import { ToastrService } from 'ngx-toastr';
import { ContentStatus } from '../models/content-status.enum';

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
  displayedColumns: string[] = ['select', 'title', 'status', 'url', 'isBroadcastable', 'isDeletable'];
  dataSource: MatTableDataSource<Content>;
  showDialog: boolean = false;
  deleteConfirmMessage: string = "Content once archived can not be restored. Please press Continue to begin the archival.";
  processConfirmMessage: string = "Please press Continue to begin the transformation.";
  initialSelection = [];
  allowMultiSelect = true;
  selection = new SelectionModel<Content>(this.allowMultiSelect, this.initialSelection);
  selectedContents: number =0;
  allowedMaxSelection: number = environment.allowedMaxSelection;
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  //polling: Subscription;

  constructor(public dialog: MatDialog,
    public contentService: ContentService,
    private toastr: ToastrService
    ) {
  }

  

  ngOnInit(): void {
    this.getProcessedContent();
    // this.polling = interval(50000)
    // .pipe(
    //   startWith(0),
    //   switchMap(() => 
    //   this.contentService.getContentByCpIdAndFilters(unprocessedContentFilters)
    //   )
    // ).subscribe(
    //   res => {
    //     this.dataSource = this.createDataSource(res.body);
    //     this.selectedContents=0;
    //   },
    // err => console.log('HTTP Error', err));
    
  };
  getProcessedContent() {
    var processedContentFilters = {
      "contentUploadStatuses": [
        ContentStatus.UPLOAD_COMPLETE
      ],
      "contentTransformStatuses": [
        ContentStatus.TRANSFORM_COMPLETE
      ],
      "contentBroadcastStatuses": [
        ContentStatus.BROADCAST_NOT_INITIALIZED, 
        ContentStatus.BROADCAST_INPROGRESS,
        ContentStatus.BROADCAST_FAILED
      ]
    }
    this.contentService.getContentByCpIdAndFilters(processedContentFilters).subscribe(
    res => {
      this.dataSource = this.createDataSource(res.body);
      this.selectedContents=0;
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
  // ngOnDestroy() {
  //   if(this.polling){    
  //   this.polling.unsubscribe();
  //   }
  // }

  createDataSource(rawData) {
    var dataSource: Content[] =[];
    rawData.forEach( data => {
      data.isSelected = false;
      dataSource.push(data);
    });
    return new MatTableDataSource(dataSource);
  }

  isContentNotBroadcastable(row) {
    return row.contentTransformStatus !== ContentStatus.TRANSFORM_COMPLETE 
    || row.contentBroadcastStatus !== ContentStatus.BROADCAST_NOT_INITIALIZED;
  }

  isContentNotDeletable(row) {
    return row.contentTransformStatus !== ContentStatus.TRANSFORM_COMPLETE 
    || row.contentBroadcastStatus !== ContentStatus.BROADCAST_NOT_INITIALIZED;
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


openBroadcastConfirmModal(): void {
  const dialogRef = this.dialog.open(ProcessConfirmDialog, {
    data: {
      message: this.processConfirmMessage,
      action: "BROADCAST"
    },
    width: '60%'
  });

  dialogRef.afterClosed().subscribe(result => {
    console.log('The dialog was closed');
  });
}

openDeleteConfirmModal(): void {
  const dialogRef = this.dialog.open(ProcessConfirmDialog, {
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

viewURL(selectedContent) : void {
  const dialogRef = this.dialog.open(ContentTokenDialog, {
    data: {content: selectedContent},
    width: '60%',
    height: '50%'
  });

  dialogRef.afterClosed().subscribe(result => {
    console.log('The dialog was closed');
  });
}
}

@Component({
  selector: 'content-token-dialog',
  templateUrl: 'content-token-dialog.html',
  styleUrls: ['processed.component.css']
})
export class ContentTokenDialog {

  constructor(
    public dialogRef: MatDialogRef<ContentTokenDialog>,
    @Inject(MAT_DIALOG_DATA) public data: any,
    public contentService: ContentService,
    private toastr: ToastrService) {}
    contentToken;
    dashUrl: string;

  ngOnInit(): void {
    this.contentService.getContentToken(this.data.content.id).subscribe(
      res => {
        this.contentToken = res;
        this.dashUrl = "https://ampdemo.azureedge.net/?url=" +
        this.data.content.dashUrl +
        "&widevine=true&token=Bearer%3D" +
        this.contentToken;
        
      },
      err => this.toastr.error(err));
    //this.data.content;
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
  templateUrl: 'process-action-confirm-dialog.html',
  styleUrls: ['processed.component.css']
})
export class ProcessConfirmDialog {

  constructor(
    public dialogRef: MatDialogRef<ProcessConfirmDialog>,
    @Inject(MAT_DIALOG_DATA) public data: any,
    public contentService: ContentService,
    private toastr: ToastrService) {}

  onCancel (action) {
    if(action === "DELETE") {
      this.onCancelDelete();
    } else {
      this.onCancelProcess();
    }
  }

  onConfirm(action) {
    if(action === "DELETE") {
      this.onConfirmDelete();
    } else {
      this.onConfirmProcess();
    }
  }

  onCancelProcess(): void {
    this.dialogRef.close();
  }

  onConfirmProcess(): void {
    this.contentService.processContent(null).subscribe(
      res => this.toastr.success("Content/s submitted for transformation sucessfully!!"),
      err => this.toastr.error(err));
    this.dialogRef.close();
  }

  
  onCancelDelete(): void {
    this.dialogRef.close();
  }

  onConfirmDelete(): void {
    this.contentService.processContent(null).subscribe(
      res => this.toastr.success("Content/s submitted for transformation sucessfully!!"),
      err => this.toastr.error(err));
    this.dialogRef.close();
  }

}




