// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

import { DatePipe } from '@angular/common';
import { Component, Inject, LOCALE_ID, OnInit, ViewChild } from '@angular/core';
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
  filterValue;
  pipe;
  constructor(
    private deviceService: DeviceService,
    public dialog: MatDialog,
    private toastr: ToastrService,
    private route: ActivatedRoute,
    private router: Router,
    @Inject(LOCALE_ID) locale: string

  ) { 
    this.route.params.subscribe(device => this.deviceid = device.id);
    this.route.paramMap.subscribe(params => {
      this.filterValue = params.get("filterValue")
     });
     this.pipe = new DatePipe(locale);
  }

  ngOnInit(): void {
    this.deviceService.getDeviceHistory(this.deviceid).subscribe(res => {
      this.dataSource = this.createDataSource(res);
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
        rawData.createdDateString =  this.pipe.transform(rawData.createdDate, 'short');
        dataSource.push(rawData);
      });
      dataSource.sort((a, b) => (a.createdDate < b.createdDate ? 1: -1));
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
    const dialogRef = this.dialog.open(AdditionalHistoryDialog, {
      data: {
        content : row
      },
      width: '60%'
    });
  
    dialogRef.afterClosed().subscribe(result => {
      console.log('The dialog was closed');
    });
  }
  showDevicesPage() {
    this.router.navigate(['/admin/devices', {filterValue: this.filterValue}]);
  }


}
