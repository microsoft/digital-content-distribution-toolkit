import { Component, OnInit, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { ActivatedRoute, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { RetailerService } from '../services/retailer.service';
import { AdditionalHistoryDialog } from './device-additional-history.component';

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

  constructor(
    private retailerService: RetailerService,
    public dialog: MatDialog,
    private toastr: ToastrService,
    private route: ActivatedRoute,
    private router: Router

  ) { 
    this.route.params.subscribe(device => this.deviceid = device.id);
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
    if(rawDataList) {
      rawDataList.forEach( rawData => {
        var currentDevice = rawData.deviceAssignments.find(d =>  d.deviceId === this.deviceid   );
        var data = {
          retailerId: rawData.partnerProvidedId,
          name: rawData.name,
          phoneNumber: rawData.phoneNumber,
          partnerCode: rawData.partnerCode,
          assignmentStartDate: currentDevice.assignmentStartDate,
          assignmentEndDate: currentDevice.assignmentEndDate
        }
        dataSource.push(data);
      });
    }
    dataSource.sort((a, b) => (a.assignmentStartDate < b.assignmentStartDate ? 1: -1));
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
