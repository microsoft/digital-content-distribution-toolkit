import { DatePipe } from '@angular/common';
import { Component, Inject, LOCALE_ID, OnInit, QueryList, ViewChildren } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { ToastrService } from 'ngx-toastr';
import { CommonDialogComponent } from '../common-dialog/common-dialog.component';
import { ContentProviderLtdInfo } from '../models/contentprovider.model';
import { Incentive, PlanType, PublishMode } from '../models/incentive.model';
import { ContentProviderService } from '../services/content-provider.service';
import { IncentiveService } from '../services/incentive.service';
import { RetailerService } from '../services/retailer.service';
import { EditIncentiveEndDateComponent } from './edit-incentive-enddate.component';

@Component({
  selector: 'app-incentive-management',
  templateUrl: './incentive-management.component.html',
  styleUrls: ['./incentive-management.component.css']
})
export class IncentiveManagementComponent implements OnInit {
  displayedColumnsRetailers: string[] = ['name', 'type', 'partner', 'startDate', 'endDate', 'status' , 'view', 'modifyEndDate', 'publish', 'delete'];
  dataSourceRetailers: MatTableDataSource<Incentive>;

  dataSourceConsumers: MatTableDataSource<Incentive>;
  displayedColumnsConsumers: string[] = ['name', 'type', 'startDate', 'endDate', 'status' , 'view', 'modifyEndDate', 'publish', 'delete'];


  @ViewChildren(MatPaginator) paginator = new QueryList<MatPaginator>();
  @ViewChildren(MatSort) sort = new QueryList<MatSort>();

  showRetailerIncentive = true;
  createRetailerIncentive = false;
  showConsumerIncentive = true;
  createConsumerIncentive = false;
  plans= [];
  selectedStatusConsumer;
  selectedPlan = null;
  missingListConsumer: any ={};
  missingListPartner: any = {};
  partners = [];
  selectedPartner;
  missingPlansforPartner;
  errMessage;
  errMessageCust;
  error= false;
  isErrorRetailer = false;
  isErrorConsumer=false;
  selectedRetailerPartner;
  selectedPlanTypeR;
  selectedPlanTypeC;
  pipe;
  filterValueC;
  filterValueR;
  cpInfoList: ContentProviderLtdInfo[];
  audience;

  constructor( private incentiveService: IncentiveService,
    public dialog: MatDialog,
    private toastr: ToastrService,
    private contentProviderService: ContentProviderService,
    private retailerService: RetailerService,
    @Inject(LOCALE_ID) locale: string
    ) { 
      this.pipe = new DatePipe(locale);
    }

  ngOnInit(): void {
    this.plans = [ 'REGULAR', 'MILESTONE'];
    this.selectedPlanTypeR = this.plans[0];
    this.selectedPlanTypeC = this.plans[0];
    this.getRetailerPartners();
    this.getContentProviders();
    this.audience = "RETAILER";
  }

  getContentProviders() {
    var cpList = JSON.parse(sessionStorage.getItem("CONTENT_PROVIDERS"));
    if(!cpList || cpList.length < 1) {
      this.contentProviderService.browseContentProviders().subscribe(
        res => {
          this.cpInfoList = res;
          sessionStorage.setItem("CONTENT_PROVIDERS", JSON.stringify(this.cpInfoList));
        },
        err => this.toastr.warning("Unable to fetch content providers. Please contact admin")
      )
    } 
  }

  tabClick(event) {
    
    if(event.tab.textLabel === "RETAILER") {
      this.audience="RETAILER";
      this.selectedPlanTypeR=this.plans[0];
      // this.getRetailerIncentivePlans();
      this.getRPlansSelectedPartnerPlanType(null);
    } else if(event.tab.textLabel === "CONSUMER"){
      this.audience="CONSUMER";
      this.selectedPlanTypeC=this.plans[0];
      this.getConsumerIncentivePlans();
    }
  }

  getRetailerPartners() {
    this.retailerService.getRetailerPartners().subscribe(
      res => {
        this.partners = this.getPartnerCodes(res);
        sessionStorage.setItem("RETAILER_PARTNERS", JSON.stringify(this.partners));
        this.selectedRetailerPartner = this.partners[0];
        this.getRetailerIncentivePlansForPartner(this.selectedRetailerPartner, this.selectedPlanTypeR);
      },
      err => this.toastr.error("Unable to load retailer Partner.")
    );
  }

  getRPlansSelectedPartnerPlanType(value) {
    this.filterValueR = "";
    this.selectedPlanTypeR = value ? value : this.selectedPlanTypeR;
    this.getRetailerIncentivePlansForPartner(this.selectedRetailerPartner, this.selectedPlanTypeR);
  }


  changeEndDate(incentive): void {
    const dialogRef = this.dialog.open(EditIncentiveEndDateComponent, {
      width: '300px',
      data: {
        incentive: incentive,
        audience: this.audience
      }
    });

    dialogRef.componentInstance.onDateChange.subscribe(data => {
      this.toastr.success(data);
      if(this.audience === "RETAILER") {
        this.getRetailerIncentivePlansForPartner(this.selectedRetailerPartner, this.selectedPlanTypeR);
      } else {
        this.getConsumerIncentivePlans();
      }
      dialogRef.close();
    })

    dialogRef.afterClosed().subscribe(result => {
      console.log('The dialog was closed');
    });
  }

  getRetailerIncentivePlansForPartner(partner, type){
    this.isErrorRetailer = false;
    this.incentiveService.getRetailerIncentivesByPartnerAndPlanType(partner, type).subscribe(res => {
      var resArr: any = res;
      resArr.sort((a, b) => (a.createdDate < b.createdDate ? 1: -1));
      this.dataSourceRetailers = this.createDataSource(resArr);
      this.dataSourceRetailers.paginator = this.paginator.toArray()[0];;
      this.dataSourceRetailers.sort = this.sort.toArray()[0]; 
      this.getMissingPlansPerPartner(res, type);
    }, err => {
      this.missingListPartner = {
        missingListRegular: [],
        missingListMilestone: []
      }
      this.isErrorRetailer = true;
      this.dataSourceRetailers = this.createDataSource([]);
      if(err === 'Not Found') {
        this.errMessage = "No data found";
      } else {
        this.toastr.error(err);
        this.errMessage = err;
      }
    })
  }

  getMissingPlansPerPartner(plans, type) {
    var regularPlans = [];
    var milestonePlans = [];
    var missingListRegular = [];
    var missingListMilestone = [];
    if(type === PlanType.REGULAR) {
      regularPlans = plans.filter(plan => (plan.planType === PlanType.REGULAR && plan.publishMode === PublishMode.PUBLISHED))
            .sort((p1, p2) => {
              return new Date(p1.startDate).getTime() - new Date(p2.startDate).getTime();
            });
      
      for(let i=0; i < regularPlans.length-1; i ++ ){
        if(new Date(regularPlans[i+1].startDate).getTime()- new Date(regularPlans[i].endDate).getTime() > 1000){
          var missingStartDate = new Date(regularPlans[i].endDate);
          missingStartDate.setDate(missingStartDate.getDate()+1);
          var missingEndDate = new Date(regularPlans[i+1].startDate)
          missingEndDate.setDate(missingEndDate.getDate()-1);
          var missingPlan = {
            startDate: missingStartDate,
            endDate: missingEndDate
          }
          missingListRegular.push(missingPlan);
        }
      }
     
    } else if(type === PlanType.MILESTONE){
      milestonePlans = plans.filter(plan => (plan.planType === PlanType.MILESTONE && plan.publishMode === PublishMode.PUBLISHED))
        .sort((p1, p2) => {
          return new Date(p1.startDate).getTime() - new Date(p2.startDate).getTime();
        });
  
      for(let i=0; i < milestonePlans.length-1; i ++ ){
        if(new Date(milestonePlans[i+1].startDate).getTime() - new Date(milestonePlans[i].endDate).getTime()    > 1000){
          var missingStartDate = new Date(milestonePlans[i].endDate);
          missingStartDate.setDate(missingStartDate.getDate()+1);
          var missingEndDate = new Date(milestonePlans[i+1].startDate)
          missingEndDate.setDate(missingEndDate.getDate()-1);
          var missingPlan = {
            startDate: missingStartDate,
            endDate: missingEndDate
          }
          missingListMilestone.push(missingPlan);
        }
      }
    }
   
  this.missingListPartner = {
    missingListRegular: missingListRegular,
    missingListMilestone: missingListMilestone
  }
}

  getMissingPlansConsumer(plans, type) {
    var regularPlans = [];
    var milestonePlans = [];
    var missingListRegular = [];
    var missingListMilestone = [];
    if(type === PlanType.REGULAR) {
      regularPlans =  plans.filter(plan => (plan.type === PlanType.REGULAR && plan.status === PublishMode.PUBLISHED))
      .sort((p1, p2) => {
        return new Date(p1.startDate).getTime() - new Date(p2.startDate).getTime();
      });
      for(let i=0; i < regularPlans.length-1; i ++ ){
        if(new Date(regularPlans[i+1].startDate).getTime()- new Date(regularPlans[i].endDate).getTime() > 1000){
          var missingStartDate = new Date(regularPlans[i].endDate);
          missingStartDate.setDate(missingStartDate.getDate()+1);
          var missingEndDate = new Date(regularPlans[i+1].startDate)
          missingEndDate.setDate(missingEndDate.getDate()-1);
          var missingPlan = {
            startDate: missingStartDate,
            endDate: missingEndDate
          }
          missingListRegular.push(missingPlan);
        }
      }
    } else if(type === PlanType.MILESTONE) {
      milestonePlans= plans.filter(plan => (plan.type === PlanType.MILESTONE && plan.status === PublishMode.PUBLISHED))
      .sort((p1, p2) => {
        return new Date(p1.startDate).getTime() - new Date(p2.startDate).getTime();
      });
      for(let i=0; i < milestonePlans.length-1; i ++ ){
        if(new Date(milestonePlans[i+1].startDate).getTime() - new Date(milestonePlans[i].endDate).getTime()    > 1000){
          var missingStartDate = new Date(milestonePlans[i].endDate);
          missingStartDate.setDate(missingStartDate.getDate()+1);
          var missingEndDate = new Date(milestonePlans[i+1].startDate)
          missingEndDate.setDate(missingEndDate.getDate()-1);
          var missingPlan = {
            startDate: missingStartDate,
            endDate: missingEndDate
          }
          missingListMilestone.push(missingPlan);
        }
      }
    }
    this.missingListConsumer = {
      missingListRegular: missingListRegular,
      missingListMilestone: missingListMilestone
    }
  }

  // getMissingPlansforPartner(event){
  //   this.missingPlansforPartner = this.missingListPartner.find( p => p.partner === event.value);
  // }

  getPartnerCodes(partners) {
    var partnerCodes = [];
    if(partners && Array.isArray(partners)){
      partners.forEach(partner => {
        partnerCodes.push(partner.partnerCode);
      })
    }
    return partnerCodes;
  }
  

  createDataSource(response) {
    var dataSource: Incentive[] =[];
    if(response && response.length > 0) {
      this.errMessage = "";
      this.errMessageCust = "";
      this.isErrorConsumer = false;
      this.isErrorRetailer = false;
      response.forEach( data => {
        var row: any = {};
        row.id = data.id;
        row.name = data.planName;
        row.status = data.publishMode;
        row.type = data.planType;
        row.partner = data.audience.subTypeName;
        row.startDateString = this.pipe.transform(data.startDate, 'short');
        row.endDateString =  this.pipe.transform(data.endDate, 'short');
        dataSource.push(row);
      });
    } else {
      this.errMessage = "No data found";
      this.errMessageCust = "No data found";
      this.isErrorConsumer = true;
      this.isErrorRetailer = true;
    }
    return new MatTableDataSource(dataSource);
  }


  addIncentiveRetailer($event) {
    if($event) {
      this.selectedRetailerPartner = $event.partner,
      this.selectedPlanTypeR = $event.planType
    }
    this.getContentProviders();
    this.selectedPlan = null;
    this.createRetailerIncentive = false;
    this.getRPlansSelectedPartnerPlanType(null);
    this.showRetailerIncentive = true;
    
  }

  addIncentiveConsumer($event) {
    if($event) {
      this.selectedPlanTypeC = $event
    }
    this.createConsumerIncentive = false;
    this.getConsumerIncentivePlans();
    this.showConsumerIncentive = true;
  }

  getConsumerIncentivePlans() {
    this.isErrorConsumer = false;
    this.incentiveService.getConsumerIncentivesByPlanType(this.selectedPlanTypeC).subscribe(
      res => { 
          // const validResult = res.filter((result) => Array.isArray(result));
          // var mergedResult = [].concat.apply([], validResult);
          var resArr: any = res.body;
          resArr.sort((a, b) => (a.createdDate < b.createdDate ? 1: -1));
          this.dataSourceConsumers = this.createDataSource(resArr);
          this.dataSourceConsumers.paginator = this.paginator.toArray()[1];;
          this.dataSourceConsumers.sort = this.sort.toArray()[1];   
          this.selectedStatusConsumer= this.plans[0];     
          this.applyFilterConsumer();  
          this.getMissingPlansConsumer(this.dataSourceConsumers.data, this.selectedPlanTypeC);  
        },
      err => {
        this.missingListConsumer = {
          missingListRegular: [],
          missingListMilestone: []
        }
        this.isErrorConsumer = true;
        this.dataSourceConsumers = this.createDataSource([]);
        if(err === 'Not Found') {
          this.errMessage = "No data found";
        } else {
          this.toastr.error(err);
          this.errMessage = err;
        }
      }
    );
  }

  editIncentivePlan(row, audience) {
    this.selectedPlan = row;
    this.openAddIncentivePage(audience);
  }

  openNewIncentivePage(audience) {
    this.selectedPlan = null;
    this.openAddIncentivePage(audience);
  }

  

  openAddIncentivePage(audience): void {
    if(audience === "RETAILER") {
      this.showRetailerIncentive = false;
      this.createRetailerIncentive = true;
    } else {
      this.showConsumerIncentive = false;
      this.createConsumerIncentive = true;
    }

  }

  showIncentivePage(audience): void {
    if(audience === "RETAILER") {
      this.showRetailerIncentive = true;
      this.createRetailerIncentive = false;
    } else {
      this.showConsumerIncentive = true;
      this.createConsumerIncentive = false;
    }
  }

  selectPlanTypeRetailer(event) {

  }
  applyFilterRetailer() {
    // var filterValue = "";
    // if(event && event.target) {
    //   filterValue = event.target ? (event.target as HTMLInputElement).value : this.selectedStatusRetailer;
    // } else if(event && event.value){
    //   filterValue = event.value ;
    // }
    this.dataSourceRetailers.filter = this.filterValueR.trim().toLowerCase();

    if (this.dataSourceRetailers.paginator) {
      this.dataSourceRetailers.paginator.firstPage();
    }
  }


  applyFilterConsumer() {
    // var filterValue = "";
    // if(event && event.target) {
    //    filterValue = event.target ? (event.target as HTMLInputElement).value : this.selectedStatusConsumer;
    // } else if(event && event.value){
    //   filterValue = event.value;
    // }
    this.dataSourceConsumers.filter = this.filterValueC.trim().toLowerCase();

    if (this.dataSourceConsumers.paginator) {
      this.dataSourceConsumers.paginator.firstPage();
    }
  }

  
  publishPlan(row, audience) {
    if(audience === "RETAILER") {
      this.publishRetailerIncentive(row.id, row.partner);
    } else {
      this.publishConsumerIncentive(row.id);
    }
  }

  openDeleteDialog(row, audience) {
    const dialogRef = this.dialog.open(CommonDialogComponent, {
      data: {
        heading: 'Confirm',
        message: "Please click confirm to delete the drafted incentive plan.",
        partner: row.partner,
        planId: row.id,
        audience: audience,
        action: "PROCESS",
        buttons: this.openSelectCPModalButtons()
      },
      maxHeight: '400px'
    });
  
  
    dialogRef.afterClosed().subscribe(result => {
      if (result === 'proceed') {
        this.deletePlan(row, audience);
      }
    });
  }

  deletePlan(row, audience) {
    if(audience === "RETAILER") {
      this.deleteDraftRetailerIncentive(row.id, row.partner);
    } else {
      this.deleteDraftConsumerIncentive(row.id);
    }
  }
  
openPublishDialog(row, audience) {
  const dialogRef = this.dialog.open(CommonDialogComponent, {
    data: {
      heading: 'Confirm',
      message: "Please click confirm to publish the incentive plan.",
      partner: row.partner,
      planId: row.id,
      audience: audience,
      action: "PROCESS",
      buttons: this.openSelectCPModalButtons()
    },
    maxHeight: '400px'
  });


  dialogRef.afterClosed().subscribe(result => {
    if (result === 'proceed') {
      this.publishPlan(row, audience);
    }
  });
}

openSelectCPModalButtons(): Array<any> {
  return [{
    label: 'Cancel',
    type: 'basic',
    value: 'cancel',
    class: 'discard-btn'
  },
  {
    label: 'Confirm',
    type: 'primary',
    value: 'submit',
    class: 'update-btn'
  }
  ]
}

deleteDraftRetailerIncentive(id, partner) {
  this.incentiveService.deleteDraftRetailerIncentivePlan(id, partner).subscribe(
    res => {
      this.toastr.success("Retailer Incentive plan deleted successfully");
      this.getRetailerIncentivePlansForPartner(this.selectedRetailerPartner, this.selectedPlanTypeR);
    },
    err =>  {
      console.log(err);
      this.toastr.error(err);
    }
  );
}

deleteDraftConsumerIncentive(id){
  this.incentiveService.deleteDraftConsumerIncentivePlan(id).subscribe(
    res => {
      this.toastr.success("Consumer Incentive plan deleted successfully");
      this.getConsumerIncentivePlans();
    },
    err =>  {
      console.log(err);
      this.toastr.error(err);
    }
  );
}


  publishRetailerIncentive(id, partner) {
    this.incentiveService.publishRetailerIncentivePlan(id, partner).subscribe(
      res => {
        this.toastr.success("Retailer Incentive plan published successfully");
        this.getRPlansSelectedPartnerPlanType(null);
      },
      err =>  {
        console.log(err);
        this.toastr.error(err);
      }
    );
  }

  publishConsumerIncentive(id){
    this.incentiveService.publishConsumerIncentivePlan(id).subscribe(
      res => {
        this.toastr.success("Consumer Incentive plan published successfully");
        this.getConsumerIncentivePlans();
      },
      err =>  {
        console.log(err);
        this.toastr.error(err);
      }
    );
  }

  
}
