// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

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
import { CommandDetail } from '../models/command-detail.model';
import { ContentView } from '../models/content-view.model';
import { broadcastContentFilters } from '../constants/content-status-filters';

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
  dataSource: MatTableDataSource<ContentView>;
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
    this.contentService.getContentByCpIdAndFilters(broadcastContentFilters).subscribe(
    res => {
      var data: ContentView[] = res;
      this.dataSource = this.createDataSource(data);
      this.dataSource.paginator = this.paginator;
      this.dataSource.sort = this.sort;
      this.selectedContents=0;
      this.dataSource.sortingDataAccessor = (
        data: any,
        sortHeaderId: string
      ) => {
          if(sortHeaderId === "createdDate" || sortHeaderId === "modifiedDate") {
            return new Date(data[sortHeaderId]);
          }
        if (typeof data[sortHeaderId] === 'string') {
          return data[sortHeaderId].toLocaleLowerCase();
        }
  
        return data[sortHeaderId];
      };
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

  createDataSource(rawData: ContentView[]) {
    var dataSource: ContentView[] =[];
    if(rawData && rawData.length > 0) {
      this.errMessage = "";
      this.error = false;
      rawData.forEach( data => {
        data.displayStatus = data.contentBroadcastStatus;
        data.isSelected = false;
        data.displayCreatedDate =  this.pipe.transform(data.createdDate, 'short');
        data.displayModifiedDate =  this.pipe.transform(data.modifiedDate, 'short');
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
      this.toastr.success("Broadcast cancellation request for content" + content.id + "submitted successfully");
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
    public contentService: ContentService) {}
    startDate;
    endDate;
    filters;
    additionalData: CommandDetail;
    additionalDataJson;
  ngOnInit(): void {
    this.contentService.getCommandDetails(this.data.content.id, 
      this.data.content.contentBroadcastStatusUpdatedBy).subscribe(
      res => {
        this.additionalData = res;
        this.additionalDataJson = JSON.stringify(this.additionalData);
        this.startDate = this.additionalData.broadcastRequest.startDate;
        this.endDate = this.additionalData.broadcastRequest.endDate;
        this.filters = this.additionalData.broadcastRequest.filters.join();
      },
      err => {
        this.additionalDataJson = err;
        }
      );
    }

  onClose(): void {
    this.dialogRef.close();
    }

}