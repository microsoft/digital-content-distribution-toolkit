import { SelectionModel } from '@angular/cdk/collections';
import { Component, ViewChild} from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { environment } from '../../environments/environment';
import { Content } from '../models/content.model';
import { ContentService } from '../services/content.service';
import { ToastrService } from 'ngx-toastr';
import { ContentStatus } from '../models/content-status.enum';
import { FormControl, FormGroup, Validators } from '@angular/forms';

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
  displayedColumns: string[] = ['select', 'title', 'status', 'manageDevices', 'isBroadcastCancellable'];
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
    this.getBroadcastContent();
  };

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
        ContentStatus.BROADCAST_CANCEL_INPROGRESS
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
      this.dataSource = this.createDataSource([]);
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
    return (!row.isSelected && this.selectedContents >= this.allowedMaxSelection);
  }

  createDataSource(rawData) {
    var dataSource: Content[] =[];
    if(rawData) {
      rawData.forEach( data => {
        data.status = data.contentBroadcastStatus;
        data.isSelected = false;
        dataSource.push(data);
      });
    }
    return new MatTableDataSource(dataSource);
  }

  
  isBroadcastCancellable(row) {
    return row.contentTransformStatus !== ContentStatus.TRANSFORM_COMPLETE 
    || (row.contentBroadcastStatus !== ContentStatus.BROADCAST_NOT_INITIALIZED
    && row.contentBroadcastStatus !== ContentStatus.BROADCAST_FAILED);
  }

  applyFilter(event: Event) {
    const filterValue = (event.target as HTMLInputElement).value;
    this.dataSource.filter = filterValue.trim().toLowerCase();

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
          && d.contentBroadcastStatus!== ContentStatus.BROADCAST_FAILED)));
          if(notEligibleForBroadcast.length > 0) {
            this.toastr.warning("One or more selected content/s cannot be broadcast");
          } else {
              this.openBroadcastConfirmDialog(rows);          
          }
      }    
    }
  }

openBroadcastConfirmDialog(content): void {
  // const dialogRef = this.dialog.open(BroadcastConfirmDialog, {
  //   data: {
  //     message: this.processConfirmMessage,
  //     contents : content
  //   },
  //   width: '60%'
  // });

  // dialogRef.afterClosed().subscribe(result => {
  //   console.log('The dialog was closed');
  // });
}

openDeleteConfirmModal(): void {
  // const dialogRef = this.dialog.open(BroadcastConfirmDialog, {
  //   data: {
  //     message: this.deleteConfirmMessage
  //   },
  //   width: '40%'
  // });

  // dialogRef.afterClosed().subscribe(result => {
  //   console.log('The dialog was closed');
  // });

}

viewURL(selectedContent) : void {
//   const dialogRef = this.dialog.open(TokenDialog, {
//     data: {content: selectedContent},
//     width: '60%'
//   });

//   dialogRef.afterClosed().subscribe(result => {
//     console.log('The dialog was closed');
//   });
// }
}

// @Component({
//   selector: 'broadcast-token-dialog',
//   templateUrl: 'broadcast.component.html',
//   styleUrls: ['broadcast.component.css']
// })
// export class TokenDialog {

//   constructor(
//     public dialogRef: MatDialogRef<TokenDialog>,
//     @Inject(MAT_DIALOG_DATA) public data: any,
//     public contentService: ContentService,
//     private toastr: ToastrService) {}
//     contentToken;
//     dashUrl: string;

//   ngOnInit(): void {
//     this.contentService.getContentToken(this.data.content.id).subscribe(
//       res => {
//         this.contentToken = res;
//         this.dashUrl =  environment.dashUrlPrefix + this.data.content.dashUrl +
//          + environment.widewineTokenPrefix + this.contentToken;
//       },
//       err => this.toastr.error(err));
//     //this.data.content;
//   }
//   onCancelUpload(): void {
//     this.dialogRef.close();
//   }

//   onConfirmUpload(): void {
//     this.dialogRef.close();
//   }

// }


// @Component({
//   selector: 'broadcast-confirm-dialog',
//   templateUrl: 'broadcast.component.html',
//   styleUrls: ['broadcast.component.css']
// })
// export class BroadcastConfirmDialog {

//   filters;
//   initialSelection = [];
//   allowMultiSelect = true;
//   selection = new SelectionModel<Content>(this.allowMultiSelect, this.initialSelection);
//   appliedFilters =[];
//   range = new FormGroup({
//     start: new FormControl(null, [Validators.required]),
//     end: new FormControl(null, [Validators.required])
//   });

//   constructor(
//     public dialogRef: MatDialogRef<BroadcastConfirmDialog>,
//     @Inject(MAT_DIALOG_DATA) public data: any,
//     public contentService: ContentService,
//     private toastr: ToastrService) {
//       this.filters = environment.filters;
//     }

//   onCancel(): void {
//     this.dialogRef.close();
//   }

//   onConfirm(): void {
//     if(this.appliedFilters.length < 1) {
//       this.toastr.warning("Please select one or more filter/s");
//     } else {
//       var selectedIds = this.data.contents.map(content => 
//         {return content.id});
//       var broadcastRequest = {
//         "contentIds": selectedIds,
//         "filters": this.appliedFilters,
//         "startDate": this.range.controls.start.value,
//         "endDate": this.range.controls.end.value
//       }
//       this.contentService.boradcastContent(broadcastRequest).subscribe(
//         res => this.toastr.success("Content/s submitted for broadcast sucessfully!!"),
//         err => this.toastr.error(err));
//       this.dialogRef.close();
//     }
    
//   }

//   toggleSelection(event, f) {
//     if(event.checked){
//       this.appliedFilters.push(f)
//     }else{
//       this.appliedFilters =  this.appliedFilters.filter(filter => {
//         return filter !== f;
//       });
//     }
//   }
// }

}