import { formatDate } from '@angular/common';
import { Component, OnInit, QueryList, ViewChildren } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { Incentive, PlanType, RetailerPartner } from '../models/incentive.model';
import { IncentiveService } from '../services/incentive.service';
import { AddIncentiveComponent } from './add-incentive.component';
import { LOCALE_ID, Inject } from '@angular/core';

@Component({
  selector: 'app-incentive-management',
  templateUrl: './incentive-management.component.html',
  styleUrls: ['./incentive-management.component.css']
})
export class IncentiveManagementComponent implements OnInit {
  displayedColumnsRetailers: string[] = ['name', 'type', 'partner', 'dateRange', 'status' , 'actions'];
  dataSourceRetailers: MatTableDataSource<Incentive>;

  dataSourceConsumers: MatTableDataSource<Incentive>;
  displayedColumnsConsumers: string[] = ['name', 'type', 'dateRange' , 'status' , 'actions'];


  @ViewChildren(MatPaginator) paginator = new QueryList<MatPaginator>();
  @ViewChildren(MatSort) sort = new QueryList<MatSort>();


  constructor( private incentiveService: IncentiveService,
    public dialog: MatDialog,
    @Inject(LOCALE_ID) public locale: string) { }

  ngOnInit(): void {
    this.getRetailerIncentivePlans();

  }
  tabClick(event) {
    if(event.tab.textLabel === "Retailer") {
      this.getRetailerIncentivePlans();
    } else if(event.tab.textLabel === "Consumer"){
      this.getConsumerIncentivePlans();
    }
  }
  getRetailerIncentivePlans() {
    this.incentiveService.getRetailerIncentives().subscribe(
      res => {
          const validResult = res.filter((result) => Array.isArray(result));
          var mergedResult = [].concat.apply([], validResult);
          this.dataSourceRetailers = this.createDataSource(mergedResult);
          // this.dataSourceRetailers.paginator = this.paginator;
          // this.dataSourceRetailers.sort = this.sort;          
        },
      err => {
        this.dataSourceRetailers = this.createDataSource([]);
        // this.toastr.error(err);
        console.log('HTTP Error', err);
      }
    );
  }

  createDataSource(response) {
    var dataSource: any[] =[];
    if(response) {
      response.forEach( data => {
        var row: any = {};
        row.name = data.planName;
        row.status = data.publishMode;
        row.type = data.planType;
        row.partner = data.audience.subTypeName;
        row.dateRange = formatDate(data.startDate, "dd-mm-yyyy", this.locale) + " " + formatDate(data.endDate, "dd-mm-yyyy", this.locale);
        console.log(row.dateRange);
        dataSource.push(row);
      });
    }
    return new MatTableDataSource(dataSource);
  }


  getConsumerIncentivePlans() {
    this.incentiveService.getConsumerIncentives().subscribe(
      res => { 
          const validResult = res.filter((result) => Array.isArray(result));
          var mergedResult = [].concat.apply([], validResult);
          this.dataSourceConsumers = this.createDataSource(mergedResult);
          // this.dataSourceConsumers.paginator = this.paginator;
          // this.dataSourceConsumers.sort = this.sort;          
        },
      err => {
        this.dataSourceConsumers = this.createDataSource([]);
        // this.toastr.error(err);
        console.log('HTTP Error', err);
      }
    );
  }


  createConsumerIncentive() {

  }
  s
  createRetailerIncentive() {
    var incentivePlan = {
      "planName": "UIPlan1",
      "planType": "REGULAR",
      "startDate": "2021-11-17T16:34:09.672Z",
      "endDate": "2021-11-17T16:34:09.672Z",
      "audience": {
        "audienceType": "RETAILER",
        "subTypeName": "NOVO"
      },
      "planDetails": [
        {
          "eventType": "RETAILER_INCOME_ORDER_COMPLETED",
          "eventTitle": "Order Complete Incentive",
          "ruleType": "SUM",
          "formula": {
            "formulaType": "PLUS",
            "leftOperand": 10,
            "rightOperand": 0,
            "rangeOperand": [
              {
                "startRange": 0,
                "endRange": 0,
                "output": 0
              }
            ]
          }
        }
      ]
    }
    this.incentiveService.createIncentivePlan(incentivePlan).subscribe(
      res =>
      console.log(res),
      err => console.log(err)
    );

  }


  openAddIncentiveDialog(audience): void {
    const dialogRef = this.dialog.open(AddIncentiveComponent, {
      data: {
        audience: audience,
      },
      width: '60%'
    });
  
    dialogRef.afterClosed().subscribe(result => {
      console.log('The dialog was closed');
    });
  }
  
}
