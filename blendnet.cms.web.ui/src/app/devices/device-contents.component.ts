import { DatePipe } from '@angular/common';
import { Component, Inject, LOCALE_ID, OnInit, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { ActivatedRoute, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { environment } from 'src/environments/environment';
import { BroadcastDetailsDialog } from '../broadcast/broadcast.component';
import { ContentProviderLtdInfo } from '../models/contentprovider.model';
import { ContentProviderService } from '../services/content-provider.service';
import { ContentService } from '../services/content.service';
import { DeviceService } from '../services/device.service';
import { ContentDetailsDialog } from '../unprocessed/unprocessed.component';

@Component({
  selector: 'app-device-contents',
  templateUrl: './device-contents.component.html',
  styleUrls: ['./devices.component.css']
})
export class DeviceContentsComponent implements OnInit {
  deviceid;
  displayedColumns: string[] = ['title', 'id', 'availability', 'operationTimeStamp', 'metadata'];
  dataSource: MatTableDataSource<any>;
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  cpInfoList: ContentProviderLtdInfo[];
  cpList;
  selectedCP;
  errMessage;
  error= false;
  totalActiveBroacastedContent = 'NA';
  totalValidActiveAvailableContent= 0;
  totalValidActiveBroacastedContent= 'NA';
  filterValue;
  pipe;
  constructor(
    private deviceService: DeviceService,
    public dialog: MatDialog,
    private toastr: ToastrService,
    private route: ActivatedRoute,
    private router: Router,
    private contentProviderService: ContentProviderService,
    private contentService: ContentService,
    @Inject(LOCALE_ID) locale: string

  ) { 
    this.route.params.subscribe(device => this.deviceid = device.id);
    this.route.paramMap.subscribe(params => {
      this.filterValue = params.get("filterValue")
     });
    this.pipe = new DatePipe(locale);
  }

  ngOnInit(): void {
    if(sessionStorage.getItem('roles').includes(environment.roles.SuperAdmin)) {
      this.displayedColumns.push('details')
    }
    this.getContentProviders();
  }

  getContentProviders() {
    this.cpList = JSON.parse(sessionStorage.getItem("CONTENT_PROVIDERS"));
    if(! this.cpList ||  this.cpList.length < 1) {
      this.contentProviderService.browseContentProviders().subscribe(
        res => {
          this.cpInfoList = res;
          sessionStorage.setItem("CONTENT_PROVIDERS", JSON.stringify(this.cpInfoList));
          this.cpList =  JSON.parse(sessionStorage.getItem("CONTENT_PROVIDERS"));
          this.selectedCP = this.cpList[0].contentProviderId;
          this.getContentForDeviceByCP(this.selectedCP);
        },
        err => this.toastr.warning("Unable to fetch content providers. Please contact admin")
      )
    } else {
      this.selectedCP = this.cpList[0].contentProviderId;
      this.getContentForDeviceByCP(this.selectedCP);
    }
  }

  getContentForDeviceByCP(selectedCP) {
    this.totalActiveBroacastedContent = 'NA';
    this.totalValidActiveAvailableContent= 0;
    this.totalValidActiveBroacastedContent= 'NA';
    this.deviceService.getContentsOnDeviceByCP(this.deviceid, selectedCP).subscribe(res => {
      var response: any = res;
      this.totalActiveBroacastedContent = response.totalActiveBroacastedContent;
      this.totalValidActiveAvailableContent = response.totalValidActiveAvailableContent;
      this.totalValidActiveBroacastedContent = response.totalValidActiveBroacastedContent;
      this.dataSource = this.createDataSource(response.validActiveBroadcastedContentList);
      this.dataSource.paginator = this.paginator;
      this.dataSource.sort = this.sort;
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

  createDataSource(rawDataList) {
    var dataSource: any[] =[];
    if(rawDataList && rawDataList.length > 0) {
      this.errMessage = "";
      this.error = false;
      rawDataList.forEach( rawData => {
        var content : any = {};
        content.title = rawData.validActiveBroadcastedContent.title;
        content.id = rawData.validActiveBroadcastedContent.id;
        content.availability = rawData.isAvailableOnDevice;
        content.operationTimeStampString = this.pipe.transform(rawData.deviceContent?.operationTimeStamp, 'short');
        content.details = rawData;
        dataSource.push(content);
      });
    } else {
      this.errMessage = "No data found";
      this.error = true;
    }
    return new MatTableDataSource(dataSource);
  }

  applyFilter(event: Event) {
    const filterValue = (event.target as HTMLInputElement).value;
    this.dataSource.filter = filterValue.trim().toLowerCase();

    if (this.dataSource.paginator) {
      this.dataSource.paginator.firstPage();
    }
  }

  openAdditionalDetailsHistory(row) {
    this.contentService.getContentById(row.id).subscribe(
      res => {
        const dialogRef = this.dialog.open(BroadcastDetailsDialog, {
          data: {content: res},
          width: '60%'
        });
      
        dialogRef.afterClosed().subscribe(result => {
          console.log('The dialog was closed');
        });
      }
    )

  }
  showDevicesPage() {
    this.router.navigate(['/admin/devices', {filterValue: this.filterValue}]);
  }

  viewContent(selectedContent) : void {
    const dialogRef = this.dialog.open(ContentDetailsDialog, {
      data: {content: selectedContent.details.validActiveBroadcastedContent}
    });
  
    dialogRef.afterClosed().subscribe(result => {
      console.log('The dialog was closed');
    });
  
  }
}
