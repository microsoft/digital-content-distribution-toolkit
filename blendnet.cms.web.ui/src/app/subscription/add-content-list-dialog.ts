import { SelectionModel } from "@angular/cdk/collections";
import { DatePipe } from "@angular/common";
import { Component, EventEmitter, Inject, Output, OnInit, ViewChild, LOCALE_ID } from "@angular/core";
import { MAT_DIALOG_DATA } from "@angular/material/dialog";
import { MatPaginator } from "@angular/material/paginator";
import { MatTableDataSource } from "@angular/material/table";
import { ToastrService } from "ngx-toastr";
import { broadcastCompleteContentFilters } from "../constants/content-status-filters";
import { ContentView } from "../models/content-view.model";
import { Content } from "../models/content.model";
import { ContentService } from "../services/content.service";

@Component({
    selector: 'app-add-content-list-dialog',
    templateUrl: 'add-content-list-dialog.html',
    styleUrls: ['./add-subscription.component.css']
})

export class AddContentListDialog implements OnInit {

    @Output() onEventCreate = new EventEmitter<any>();
    dataSource: MatTableDataSource<ContentView>;
    @ViewChild(MatPaginator) paginator: MatPaginator;
    error = false;
    errMessage;
    allowMultiSelect = true;
    selectedContents: number = 0;
    selectedContentList = [];
    selection = new SelectionModel<Content>(this.allowMultiSelect, []);
    pipe;
    displayedColumns: string[] = ['select', 'title', 'status', 'broadcastEndDate'];

    constructor(
        public toastr: ToastrService,
        public contentService: ContentService,
        @Inject(LOCALE_ID) locale: string,
        @Inject(MAT_DIALOG_DATA) public data: any)
    {
        this.pipe = new DatePipe(locale);
    }

    ngOnInit(): void {
        this.selectedContentList = this.data.selectedItem;
        this.getContentList();
    }

    getContentList() {
        console.log("opening content list");
        this.contentService.getContentByCpIdAndFilters(broadcastCompleteContentFilters).subscribe(
            res => {
              let data: ContentView[] = res;
              this.dataSource = this.createDataSource(data);
              this.dataSource.paginator = this.paginator;
              this.selectedContents = 0;
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
            }
        );
    }

    createDataSource(rawData: ContentView[]) {
        let dataSource: ContentView[] =[];
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

    addContent() {
        this.onEventCreate.emit(this.selection.selected);
    }

    shouldDisable(row) {
        return (this.selectedContentList.some(c => c.contentId===row.contentId));
    }

}