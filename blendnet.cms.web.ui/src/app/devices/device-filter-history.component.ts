import { Component, OnInit, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { ActivatedRoute, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { DeviceService } from '../services/device.service';
import { AdditionalHistoryDialog } from './device-additional-history.component';

@Component({
  selector: 'app-device-filter-history',
  templateUrl: './device-filter-history.component.html',
  styleUrls: ['./devices.component.css']
})
export class DeviceFilterHistoryComponent implements OnInit {
  deviceid;
  displayedColumns: string[] = ['id', 'deviceCommandStatus', 'filters' , 'createdDate',  'details'];
  dataSource: MatTableDataSource<any>;
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  errMessage;
  error= false;
  constructor(
    private deviceService: DeviceService,
    public dialog: MatDialog,
    private toastr: ToastrService,
    private route: ActivatedRoute,
    private router: Router

  ) { 
    this.route.params.subscribe(device => this.deviceid = device.id);
  }

  ngOnInit(): void {
    this.deviceService.getDeviceHistory(this.deviceid).subscribe(res => {
      this.dataSource = this.createDataSource(res.body);
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
    dataSource.sort((a, b) => (a.createdDate < b.createdDate ? 1: -1));
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
