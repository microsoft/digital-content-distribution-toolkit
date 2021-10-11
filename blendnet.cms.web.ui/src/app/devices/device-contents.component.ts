import { Component, OnInit, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { ActivatedRoute, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { ContentProviderService } from '../services/content-provider.service';
import { DeviceService } from '../services/device.service';
import { AdditionalHistoryDialog } from './device-additional-history.component';

@Component({
  selector: 'app-device-contents',
  templateUrl: './device-contents.component.html',
  styleUrls: ['./devices.component.css']
})
export class DeviceContentsComponent implements OnInit {
  deviceid;
  displayedColumns: string[] = ['content_name', 'content_id', 'availability', 'operationTimeStamp',  'details'];
  dataSource: MatTableDataSource<any>;
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  cpList;
  selectedCP;
  errMessage;
  error= false;
  totalActiveBroacastedContent = 'NA';
  totalValidActiveAvailableContent= 0;
  totalValidActiveBroacastedContent= 'NA';
  constructor(
    private deviceService: DeviceService,
    public dialog: MatDialog,
    private toastr: ToastrService,
    private route: ActivatedRoute,
    private router: Router,
    private contentProviderService: ContentProviderService

  ) { 
    this.route.params.subscribe(device => this.deviceid = device.id);
  }

  ngOnInit(): void {
    this.getContentProviders();
  }

  getContentProviders() {
    this.cpList = JSON.parse(sessionStorage.getItem("CONTENT_PROVIDERS"));
    if(! this.cpList ||  this.cpList.length < 1) {
      this.contentProviderService.browseContentProviders().subscribe(
        res => {
          sessionStorage.setItem("CONTENT_PROVIDERS", JSON.stringify(res));
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
    if(rawDataList) {
      rawDataList.forEach( rawData => {
        dataSource.push(rawData);
      });
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
    const dialogRef = this.dialog.open(AdditionalHistoryDialog, {
      data: {
        history : row
      },
      width: '60%'
    });
  
    dialogRef.afterClosed().subscribe(result => {
      console.log('The dialog was closed');
    });
  }
  showDevicesPage() {
    this.router.navigateByUrl('/admin/devices');
  }


}
