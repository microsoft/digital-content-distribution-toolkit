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
import { RetailerService } from '../services/retailer.service';

@Component({
  selector: 'app-device-retailer-history',
  templateUrl: './device-retailer-history.component.html',
  styleUrls: ['./devices.component.css']
})
export class DeviceRetailerHistoryComponent implements OnInit {
  deviceid;
  displayedColumns: string[] = ['retailerId', 'name', 'phoneNumber','partnerCode' , 'assignmentStartDate',  'assignmentEndDate'];
  dataSource: MatTableDataSource<any>;
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  errMessage;
  error= false;
  filterValue;
  pipe;

  constructor(
    private retailerService: RetailerService,
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
    this.retailerService.getRetailerByDeviceId(this.deviceid ).subscribe(res => {
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
    if(rawDataList && rawDataList.length > 0) {
      this.errMessage = "";
      this.error = false;
      rawDataList.forEach( rawData => {
        var currentDeviceList = rawData.deviceAssignments.filter(d =>  d.deviceId === this.deviceid);
        currentDeviceList.forEach(currentDevice => {
          var data = {
            retailerId: rawData.partnerProvidedId,
            name: rawData.name,
            phoneNumber: rawData.phoneNumber,
            partnerCode: rawData.partnerCode,
            assignmentStartDateString: this.pipe.transform(currentDevice.assignmentStartDate, 'short'),
            assignmentEndDateString: this.pipe.transform(currentDevice.assignmentEndDate, 'short')
          }
          dataSource.push(data);
        });
      });
      dataSource.sort((a, b) => (a.assignmentStartDate < b.assignmentStartDate ? 1: -1));
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

  showDevicesPage() {
    this.router.navigate(['/admin/devices', {filterValue: this.filterValue}]);
  }


}
