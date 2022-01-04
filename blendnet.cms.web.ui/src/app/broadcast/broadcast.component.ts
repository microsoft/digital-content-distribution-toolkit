import { SelectionModel } from '@angular/cdk/collections';
import { Component, Inject, LOCALE_ID, ViewChild} from '@angular/core';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { environment } from '../../environments/environment';
import { Content } from '../models/content.model';
import { ContentService } from '../services/content.service';
import { ToastrService } from 'ngx-toastr';
import { ContentStatus } from '../models/content-status.enum';
import { ContentDetailsDialog } from '../unprocessed/unprocessed.component';
import { CommonDialogComponent } from '../common-dialog/common-dialog.component';
import { ContentTokenDialog } from '../processed/processed.component';
import { DatePipe } from '@angular/common';

export interface DialogData {
  message: string;
}
/**
 * @title Data table with sorting, pagination, and filtering.
 */
@Component({
  selector: 'app-broadcast',
  styleUrls: ['broadcast.component.css'],
  templateUrl: 'broadcast.component.html',
})
export class BroadcastComponent {
  displayedColumns: string[] = ['select', 'title', 'status', 'createdDate', 'modifiedDate',  'isBroadcastCancellable', 'view', 'url', 'broadcastDetails'];
  dataSource: MatTableDataSource<Content>;
  showDialog: boolean = false;
  deleteConfirmMessage: string = "Content once archived can not be restored. Please press Continue to begin the archival.";
  cancelConfirmMessage: string = "Please press Continue to CANCEL the broadcast.";
  initialSelection = [];
  allowMultiSelect = true;
  selection = new SelectionModel<Content>(this.allowMultiSelect, this.initialSelection);
  selectedContents: number =0;
  allowedMaxSelection: number = environment.allowedMaxSelection;
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  filterValue = "";
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
    this.getBroadcastContent();
  };

  refreshPage() {
    this.filterValue ="";
    this.getBroadcastContent();
  }

  getBroadcastContent() {
    var broadcastContentFilters = {
      "contentUploadStatuses": [
        ContentStatus.UPLOAD_COMPLETE
      ],
      "contentTransformStatuses": [
        ContentStatus.TRANSFORM_COMPLETE
      ],
      "contentBroadcastStatuses": [
        ContentStatus.BROADCAST_COMPLETE,
        ContentStatus.BROADCAST_TAR_PUSHED,
        ContentStatus.BROADCAST_CANCEL_INPROGRESS,
        ContentStatus.BROADCAST_ORDER_ACTIVE,
        ContentStatus.BROADCAST_ORDER_CANCELLED,
        ContentStatus.BROADCAST_ORDER_CREATED,
        ContentStatus.BROADCAST_ORDER_COMPLETE,
        ContentStatus.BROADCAST_ORDER_REJECTED,
        ContentStatus.BROADCAST_ORDER_FAILED,
        ContentStatus.BROADCAST_COMPLETE,
        ContentStatus.BROADCAST_CANCEL_SUBMITTED,
        ContentStatus.BROADCAST_CANCEL_INPROGRESS,
        ContentStatus.BROADCAST_CANCEL_FAILED,
        ContentStatus.BROADCAST_CANCEL_COMPLETE
      ]
    }
    this.contentService.getContentByCpIdAndFilters(broadcastContentFilters).subscribe(
    res => {
      this.dataSource = this.createDataSource(res.body);
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

  viewContent(selectedContent) : void {
    const dialogRef = this.dialog.open(ContentDetailsDialog, {
      data: {content: selectedContent}
    });
  
    dialogRef.afterClosed().subscribe(result => {
      console.log('The dialog was closed');
    });
  
  }
  isBroadcastNotActive(selectedContent) {
    var today = new Date();
    return selectedContent.contentBroadcastedBy?.broadcastRequest.endDate < today.toISOString();
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

getBroadcastDetails(selectedContent) {
  const dialogRef = this.dialog.open(BroadcastDetailsDialog, {
    data: {content: selectedContent},
    width: '60%'
  });

  dialogRef.afterClosed().subscribe(result => {
    console.log('The dialog was closed');
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

  createDataSource(rawData) {
    var dataSource: Content[] =[];
    if(rawData && rawData.length > 0) {
      this.errMessage = "";
      this.error = false;
      rawData.forEach( data => {
        data.status = data.contentBroadcastStatus;
        data.isSelected = false;
        data.createdDateString =  this.pipe.transform(data.createdDate, 'short');
        data.modifiedDateString =  this.pipe.transform(data.modifiedDate, 'short');
        dataSource.push(data);
      });
    } else {
      this.errMessage = "No data found";
      this.error = true;
    }
    return new MatTableDataSource(dataSource);
  }

  
  isBroadcastCancellable(row) {
    return row.contentTransformStatus !== ContentStatus.TRANSFORM_COMPLETE 
    || (row.contentBroadcastStatus !== ContentStatus.BROADCAST_ORDER_COMPLETE && row.contentBroadcastStatus !== ContentStatus.BROADCAST_CANCEL_FAILED);
  }

  showBroadcastDetails(row) {
    return row.contentBroadcastStatus !== ContentStatus.BROADCAST_ORDER_COMPLETE && row.contentBroadcastStatus !== ContentStatus.BROADCAST_CANCEL_COMPLETE 
    && row.contentBroadcastStatus !== ContentStatus.BROADCAST_CANCEL_FAILED && row.contentBroadcastStatus !== ContentStatus.BROADCAST_CANCEL_INPROGRESS
    && row.contentBroadcastStatus !== ContentStatus.BROADCAST_CANCEL_SUBMITTED;
  }

  applyFilter() {
    this.dataSource.filter = this.filterValue.trim().toLowerCase();
    if (this.dataSource.paginator) {
      this.dataSource.paginator.firstPage();
    }
  }

  openBroadcastCancelModal(row) {
    const dialogRef = this.dialog.open(CommonDialogComponent, {
      data: {
        heading: 'Confirm',
        message: this.cancelConfirmMessage,
        contents: row,
        action: "PROCESS",
        buttons: this.openSelectCPModalButtons()
      },
      maxHeight: '400px'
    });
  
  
    dialogRef.afterClosed().subscribe(result => {
      if (result === 'proceed') {
        this.onConfirmCancelBroadcast(row);
      }
    });
  }

  
  onConfirmCancelBroadcast(content): void {
  this.contentService.cancelBroadcast(content.id).subscribe(
    res => {
      this.toastr.success("Cancellation request for the broadcast submitted successfully");
      this.getBroadcastContent();
    },
    err => this.toastr.error(err));
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

}

@Component({
  selector: 'broadcast-details-dialog',
  templateUrl: 'broadcast-details-dialog.html',
  styleUrls: ['broadcast.component.css']
})

export class BroadcastDetailsDialog {

  constructor(
    public dialogRef: MatDialogRef<ContentTokenDialog>,
    @Inject(MAT_DIALOG_DATA) public data: any,
    public contentService: ContentService,
    private toastr: ToastrService) {}
    startDate;
    endDate;
    filters;
    additionalData;
    additionalDataJson;
  ngOnInit(): void {
    this.contentService.getCommandDetails(this.data.content.id, 
      this.data.content.contentBroadcastedBy.commandId).subscribe(
      res => {
        this.additionalDataJson = JSON.stringify(res);
      },
      err => {
        this.additionalDataJson = err;
        }
      );
      this.startDate = this.data.content.contentBroadcastedBy.broadcastRequest.startDate;
      this.endDate = this.data.content.contentBroadcastedBy.broadcastRequest.endDate;
      this.filters = this.data.content.contentBroadcastedBy.broadcastRequest.filters.join();
    }

  onClose(): void {
    this.dialogRef.close();
    }

}