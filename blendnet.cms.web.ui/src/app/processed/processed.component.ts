import { SelectionModel } from '@angular/cdk/collections';
import { Component, EventEmitter, Inject, LOCALE_ID, Output, ViewChild} from '@angular/core';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { environment } from '../../environments/environment';
import { Content } from '../models/content.model';
import { ContentService } from '../services/content.service';
import { ToastrService } from 'ngx-toastr';
import { ContentStatus } from '../models/content-status.enum';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { CommonDialogComponent } from '../common-dialog/common-dialog.component';
import { ContentDetailsDialog } from '../unprocessed/unprocessed.component';
import { ThemePalette } from '@angular/material/core';
import { DatePipe } from '@angular/common';
import { processedContentFilters } from '../constants/content-status-filters';
import { ContentView } from '../models/content-view.model';


export interface DialogData {
  message: string;
}

@Component({
  selector: 'app-processed',
  styleUrls: ['processed.component.css'],
  templateUrl: 'processed.component.html',
})
export class ProcessedComponent {
  displayedColumns: string[] = ['select', 'title', 'status', 'createdDate', 'modifiedDate', 'url', 'view', 'isBroadcastable', 'activeStatus'];
  dataSource: MatTableDataSource<ContentView>;
  showDialog: boolean = false;
  deleteConfirmMessage: string = "Content once archived can not be restored. Please press Continue to begin the archival.";
  processConfirmMessage: string = "Please press Continue to begin the transformation.";
  initialSelection = [];
  allowMultiSelect = true;
  selection = new SelectionModel<Content>(this.allowMultiSelect, this.initialSelection);
  selectedContents: number =0;
  allowedMaxSelection: number = environment.allowedMaxSelection;
  @ViewChild(MatPaginator) paginator: MatPaginator;
  filterValue = "";
  @ViewChild(MatSort) sort: MatSort;
  color: ThemePalette = 'primary';
  errMessage;
  error= false;
  pipe;

  constructor(public dialog: MatDialog,
    public contentService: ContentService,
    private toastr: ToastrService,
    @Inject(LOCALE_ID) locale: string
    ) {
      this.pipe = new DatePipe(locale);
  }

  ngOnInit(): void {
    this.getProcessedContent();
  };

  refreshPage() {
    this.filterValue = "";
    this.getProcessedContent();
  }

  getProcessedContent() {
    this.contentService.getContentByCpIdAndFilters(processedContentFilters).subscribe(
    res => {
      this.dataSource = this.createDataSource(res);
      this.dataSource.paginator = this.paginator;
      this.dataSource.sort = this.sort;
      this.selectedContents=0;
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
        if(data.contentBroadcastStatus !== ContentStatus.BROADCAST_ORDER_COMPLETE) {
          data.displayStatus = (data.contentBroadcastStatus !== ContentStatus.BROADCAST_NOT_INITIALIZED 
            && data.contentBroadcastStatus !== ContentStatus.BROADCAST_CANCEL_COMPLETE) ? 
          data.contentBroadcastStatus : data.contentTransformStatus;
          data.isSelected = false;
          data.displayCreatedDate =  this.pipe.transform(data.createdDate, 'short');
          data.displayModifiedDate =  this.pipe.transform(data.modifiedDate, 'short');
          dataSource.push(data);
        } else {
          var today = new Date();
          if(data.contentBroadcastedBy.broadcastRequest.endDate < today) {
            data.displayStatus = data.contentTransformStatus;
            data.isSelected = false;
            data.displayCreatedDate =  this.pipe.transform(data.createdDate, 'short');
            data.displayModifiedDate =  this.pipe.transform(data.modifiedDate, 'short');
            dataSource.push(data);
          }
        }
      });
    } else {
      this.errMessage = "No data found";
      this.error = true;
    }
    return new MatTableDataSource(dataSource);
  } 

  
  isContentNotBroadcastable(row) {
    return row.contentTransformStatus !== ContentStatus.TRANSFORM_COMPLETE 
    || (row.contentBroadcastStatus !== ContentStatus.BROADCAST_NOT_INITIALIZED
    && row.contentBroadcastStatus !== ContentStatus.BROADCAST_FAILED 
    && row.contentBroadcastStatus !== ContentStatus.BROADCAST_CANCEL_COMPLETE
    && row.contentBroadcastStatus !== ContentStatus.BROADCAST_ORDER_COMPLETE
    && row.contentBroadcastStatus !== ContentStatus.BROADCAST_ORDER_FAILED
    && row.contentBroadcastStatus !== ContentStatus.BROADCAST_ORDER_REJECTED
    && row.contentBroadcastStatus !== ContentStatus.BROADCAST_ORDER_CANCELLED);
  }

  isContentNotDeletable(row) {
    return row.contentTransformStatus !== ContentStatus.TRANSFORM_COMPLETE 
    || (row.contentBroadcastStatus !== ContentStatus.BROADCAST_NOT_INITIALIZED 
    && row.contentBroadcastStatus !== ContentStatus.BROADCAST_FAILED);
  }


  applyFilter() {
    this.dataSource.filter = this.filterValue.trim().toLowerCase();

    if (this.dataSource.paginator) {
      this.dataSource.paginator.firstPage();
    }
  }

  openBroadcastConfirmModal(row): void {
    var rows = [];
    if(row) {
      rows.push(row);
      this.openBroadcastConfirmDialog(rows);
    } else {
      rows = this.dataSource ? this.dataSource.data.filter(d => d.isSelected) : [];
      if(rows.length <1) {
        this.toastr.warning("Please select one or more content/s for broadcasting");
      } else {
        var notEligibleForBroadcast= rows.filter(d => 
          (d.contentTransformStatus !== ContentStatus.TRANSFORM_COMPLETE 
            || d.contentUploadStatus !== ContentStatus.UPLOAD_COMPLETE
          || (d.contentBroadcastStatus !== ContentStatus.BROADCAST_NOT_INITIALIZED
          && d.contentBroadcastStatus!== ContentStatus.BROADCAST_FAILED 
          && (d.contentTransformStatus === ContentStatus.TRANSFORM_COMPLETE 
          && d.contentBroadcastStatus !== ContentStatus.BROADCAST_ORDER_COMPLETE 
          && d.contentBroadcastStatus !== ContentStatus.BROADCAST_CANCEL_COMPLETE))));
          if(notEligibleForBroadcast.length > 0) {
            this.toastr.warning("One or more selected content/s cannot be broadcast");
          } else {
              this.openBroadcastConfirmDialog(rows);          
          }
      }    
    }
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
  

  toggleButtons(): Array<any> {
    return [{
      label: 'Cancel',
      type: 'basic',
      value: 'cancel',
      class: 'discard-btn'
    },
    {
      label: 'Confirm',
      type: 'primary',
      value: 'submit',
      class: 'update-btn'
    }
    ]
  }
  onToggleChange($event, row) {
      $event.preventDefault();
      var newStatus = row.isActive ? "INACTIVATE" : "ACTIVATE";
      var newStatusValue = !row.isActive;
      const dialogRef = this.dialog.open(CommonDialogComponent, {
        data: {message: "Are you sure you want to " +newStatus+ " the content ?" , heading:'Warning',
          buttons: this.toggleButtons()
        },
      });
  
      dialogRef.afterClosed().subscribe(result => {
        if (result === 'proceed') {
          this.contentService.changeContentActiveStatus(row.id, newStatusValue).subscribe(res =>{
            this.getProcessedContent();
          },
          err => {
            this.toastr.error(err);
          });
        }
      });   
  }

openBroadcastConfirmDialog(content): void {
  const dialogRef = this.dialog.open(ProcessConfirmDialog, {
    data: {
      message: this.processConfirmMessage,
      contents : content
    },
    width: '60%'
  });
  dialogRef.componentInstance.onSuccessfulSubmission.subscribe(res => 
    this.successEmit(res),
    err => this.toastr.error(err));

  dialogRef.afterClosed().subscribe(result => {
    console.log('The dialog was closed');
  });
}

successEmit(res) {
  if(res.length == 0) {
    this.toastr.success("Content/s submitted successfully for broadcast");
  } else {
    this.toastr.warning(res[0]);
  }
  this.getProcessedContent();
}

openDeleteConfirmModal(row): void {
  const dialogRef = this.dialog.open(CommonDialogComponent, {
    data: {
      heading: 'Confirm',
      message: this.deleteConfirmMessage,
      contents: row,
      buttons: this.openSelectCPModalButtons()
    },
    width: '400px'
  });

  dialogRef.afterClosed().subscribe(result => {
    if (result === 'proceed') {
      this.contentService.deleteContent(row.id).subscribe(
        res => this.onDeleteSuccess(),
        err => this.toastr.error(err));
    }
  });

}

onDeleteSuccess() {
  this.toastr.success("Content submitted for deletion successfully");
  this.getProcessedContent();
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

viewURL(selectedContent) : void {
  const dialogRef = this.dialog.open(ContentTokenDialog, {
    data: {content: selectedContent},
    width: '60%'
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
        this.dashUrl =  environment.dashUrlPrefix.concat(this.data.content.dashUrl).concat(
          environment.widewineTokenPrefix).concat(this.contentToken);
      },
      err => this.toastr.error(err));
  }
  onClose(): void {
    this.dialogRef.close();
    }

}


@Component({
  selector: 'process-confirm-dialog',
  templateUrl: 'process-action-confirm-dialog.html',
  styleUrls: ['processed.component.css']
})
export class ProcessConfirmDialog {

  filters;
  initialSelection = [];
  allowMultiSelect = true;
  selection = new SelectionModel<Content>(this.allowMultiSelect, this.initialSelection);
  appliedFilters =[];
  minStart: Date;
  range = new FormGroup({
    start: new FormControl(null, [Validators.required]),
    end: new FormControl(null, [Validators.required])
  });

  @Output() onSuccessfulSubmission= new EventEmitter<any>();

  constructor(
    public dialogRef: MatDialogRef<ProcessConfirmDialog>,
    @Inject(MAT_DIALOG_DATA) public data: any,
    public contentService: ContentService,
    private toastr: ToastrService) {
      this.filters = environment.filters;
      this.minStart = new Date();
    }

  onCancel(): void {
    this.dialogRef.close();
  }

  isSaveDisabled() {
    return this.appliedFilters.length < 1 || !this.range.controls.start.value || !this.range.controls.end.value;
  }

  onConfirm(): void {
    if(this.appliedFilters.length < 1 && (!this.range.controls.start.value || !this.range.controls.end.value)) {
      this.toastr.warning("Please select broadcast date range and one or more filter/s");
    } else if(this.appliedFilters.length < 1) {
      this.toastr.warning("Please select one or more filter/s");
    } else if (!this.range.controls.start.value || !this.range.controls.end.value){
      this.toastr.warning("Please broadcast date range");
    } else {
      var selectedIds = this.data.contents.map(content => {
        return content.id
    });
      var selectedEndDate = this.range.controls.end.value;
      selectedEndDate.setHours(selectedEndDate.getHours() + 23);
      selectedEndDate.setMinutes(selectedEndDate.getMinutes() + 59);
      selectedEndDate.setSeconds(selectedEndDate.getSeconds() + 59);
      var broadcastRequest = {
        "contentIds": selectedIds,
        "filters": this.appliedFilters,
        "startDate": this.range.controls.start.value,
        "endDate": selectedEndDate
      }
      this.contentService.broadcastContent(broadcastRequest).subscribe(
        res => //this.toastr.success("Content/s submitted for broadcast sucessfully!!"),
        this.onSuccessfulSubmission.emit(res),
        err => this.toastr.error(err));
      this.dialogRef.close();
    }
    
  }

  toggleSelection(event, f) {
    if(event.checked){
      this.appliedFilters.push(f)
    }else{
      this.appliedFilters =  this.appliedFilters.filter(filter => {
        return filter !== f;
      });
    }
  }



}









