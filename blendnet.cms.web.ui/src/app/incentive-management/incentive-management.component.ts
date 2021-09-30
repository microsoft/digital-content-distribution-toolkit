import { Component, OnInit, QueryList, ViewChildren } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { ToastrService } from 'ngx-toastr';
import { CommonDialogComponent } from '../common-dialog/common-dialog.component';
import { Incentive, PlanType, PublishMode } from '../models/incentive.model';
import { ContentProviderService } from '../services/content-provider.service';
import { IncentiveService } from '../services/incentive.service';
import { RetailerService } from '../services/retailer.service';

@Component({
  selector: 'app-incentive-management',
  templateUrl: './incentive-management.component.html',
  styleUrls: ['./incentive-management.component.css']
})
export class IncentiveManagementComponent implements OnInit {
  displayedColumnsRetailers: string[] = ['name', 'type', 'partner', 'startDate', 'endDate', 'status' , 'edit', 'publish', 'delete'];
  dataSourceRetailers: MatTableDataSource<Incentive>;

  dataSourceConsumers: MatTableDataSource<Incentive>;
  displayedColumnsConsumers: string[] = ['name', 'type', 'startDate', 'endDate', 'status' , 'edit', 'publish', 'delete'];


  @ViewChildren(MatPaginator) paginator = new QueryList<MatPaginator>();
  @ViewChildren(MatSort) sort = new QueryList<MatSort>();

  showRetailerIncentive = true;
  createRetailerIncentive = false;
  showConsumerIncentive = true;
  createConsumerIncentive = false;

  statuses= [];
  selectedStatusRetailer;
  selectedStatusConsumer;
  selectedPlan = null;
  // missingListPartner: any[] = [];
  missingListConsumer: any ={};
  missingListPartner: any = {};
  partners = [];
  selectedPartner;
  missingPlansforPartner;

  errMessage;
  error= false;
  isErrorRetailer = false;
  isErrorConsumer=false;
  selectedRetailerPartner;
  selectedPlanType="REGULAR";

  constructor( private incentiveService: IncentiveService,
    public dialog: MatDialog,
    private toastr: ToastrService,
    private contentProviderService: ContentProviderService,
    private retailerService: RetailerService
    ) { }

  ngOnInit(): void {
    this.getRetailerPartners();
    this.getContentProviders()
    this.statuses = ['REGULAR', 'MILESTONE'];
    //this.getRetailerIncentivePlans();
  }

  getContentProviders() {
    var cpList = JSON.parse(sessionStorage.getItem("CONTENT_PROVIDERS"));
    if(!cpList || cpList.length < 1) {
      this.contentProviderService.browseContentProviders().subscribe(
        res => {
          sessionStorage.setItem("CONTENT_PROVIDERS", JSON.stringify(res));
        },
        err => this.toastr.warning("Unable to fetch content providers. Please contact admin")
      )
    } 
  }

  tabClick(event) {
    this. selectedPlanType="REGULAR";
    if(event.tab.textLabel === "RETAILER") {
      // this.getRetailerIncentivePlans();
      this.getRPlansSelectedPartnerPlanType();
    } else if(event.tab.textLabel === "CONSUMER"){
      this.getConsumerIncentivePlans();
    }
  }

  // getRetailerIncentivePlans() {
  //   this.incentiveService.getRetailerIncentives().subscribe(
  //     res => {
  //       const validResult = res.filter((result) => Array.isArray(result));
  //       var mergedResult = [].concat.apply([], validResult);
  //       this.dataSourceRetailers = this.createDataSource(mergedResult);
  //       this.dataSourceRetailers.paginator = this.paginator.toArray()[0];;
  //       this.dataSourceRetailers.sort = this.sort.toArray()[0];  
  //       this.selectedStatusRetailer= this.statuses[0]; 
  //       this.applyFilterRetailer(null);   
  //       this.getMissingPlans(this.dataSourceRetailers.data);
  //       },
  //     err => {
  //       this.error = true;
  //       this.dataSourceRetailers = this.createDataSource([]);
  //       if(err === 'Not Found') {
  //         this.errMessage = "No data found";
  //       } else {
  //         this.toastr.error(err);
  //         this.errMessage = err;
  //       }
  //     }
  //   );
  // }

  getRetailerPartners() {
    this.retailerService.getRetailerPartners().subscribe(
      res => {
        this.partners = this.getPartnerCodes(res);
        sessionStorage.setItem("RETAILER_PARTNERS", JSON.stringify(this.partners));
        this.selectedRetailerPartner = this.partners[0];
        this.getRetailerIncentivePlansForPartner(this.selectedRetailerPartner, this.selectedPlanType);
      },
      err => this.toastr.error("Unable to load retailer Partner.")
    );
  }

  getRPlansSelectedPartnerPlanType() {
    this.getRetailerIncentivePlansForPartner(this.selectedRetailerPartner, this.selectedPlanType);
  }


  getRetailerIncentivePlansForPartner(partner, type){
    this.isErrorRetailer = false;
    this.incentiveService.getRetailerIncentivesByPartnerAndPlanType(partner, type).subscribe(res => {
      this.dataSourceRetailers = this.createDataSource(res.body);
      this.dataSourceRetailers.paginator = this.paginator.toArray()[0];;
      this.dataSourceRetailers.sort = this.sort.toArray()[0]; 
      this.getMissingPlansPerPartner(res.body, type);
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

  // getMissingPlans(plans) {
  //   var missingListPartner: any [] = [];
  //   this.retailerService.getRetailerPartners().subscribe(
  //     res => {
  //       this.partners = this.getPartnerCodes(res);
  //       sessionStorage.setItem("RETAILER_PARTNERS", JSON.stringify(this.partners));
  //       var partnerwisePlanList: any[] = [];
  //       this.partners.forEach( partner => {
           
  //         var partnerwisePlan = {
  //           partner : partner,
  //           regularPlans: plans.filter(plan => (plan.partner === partner && plan.type === PlanType.REGULAR && plan.status === PublishMode.PUBLISHED))
  //           .sort((p1, p2) => {
  //             return new Date(p1.startDate).getTime() - new Date(p2.startDate).getTime();
  //           }),
  //           milestonePlans: plans.filter(plan => (plan.partner === partner && plan.type === PlanType.MILESTONE && plan.status === PublishMode.PUBLISHED))
  //           .sort((p1, p2) => {
  //             return new Date(p1.startDate).getTime() - new Date(p2.startDate).getTime();
  //           })
  //         }
  //         partnerwisePlanList.push(partnerwisePlan);
  //       })

  //       partnerwisePlanList.forEach(partner => {
  //         var missingListRegular = [];
  //         var missingListMilestone = [];
  //         for(let i=0; i < partner.regularPlans.length-1; i ++ ){
  //           if(new Date(partner.regularPlans[i+1].startDate).getTime()- new Date(partner.regularPlans[i].endDate).getTime() > 1000){
  //             var missingStartDate = new Date(partner.regularPlans[i].endDate);
  //             missingStartDate.setDate(missingStartDate.getDate()+1);
  //             var missingEndDate = new Date(partner.regularPlans[i+1].startDate)
  //             missingEndDate.setDate(missingEndDate.getDate()-1);
  //             var missingPlan = {
  //               startDate: missingStartDate,
  //               endDate: missingEndDate
  //             }
  //             missingListRegular.push(missingPlan);
  //           }
  //         }

  //         for(let i=0; i < partner.milestonePlans.length-1; i ++ ){
  //           if(new Date(partner.milestonePlans[i+1].startDate).getTime() - new Date(partner.milestonePlans[i].endDate).getTime()    > 1000){
  //             var missingStartDate = new Date(partner.milestonePlans[i].endDate);
  //             missingStartDate.setDate(missingStartDate.getDate()+1);
  //             var missingEndDate = new Date(partner.milestonePlans[i+1].startDate)
  //             missingEndDate.setDate(missingEndDate.getDate()-1);
  //             var missingPlan = {
  //               startDate: missingStartDate,
  //               endDate: missingEndDate
  //             }
  //             missingListMilestone.push(missingPlan);
  //           }
  //         }
  //         var partnerList ={
  //           partner: partner.partner,
  //           missingListRegular: missingListRegular,
  //           missingListMilestone: missingListMilestone
  //         }
  //         missingListPartner.push(partnerList);
  //       });
    
  //       this.missingListPartner =  missingListPartner;
  //     },
  //     err => console.log(err)
  //   );
  // }

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
    if(response) {
      response.forEach( data => {
        var row: any = {};
        row.id = data.id;
        row.name = data.planName;
        row.status = data.publishMode;
        row.type = data.planType;
        row.partner = data.audience.subTypeName;
        row.startDate = data.startDate;
        row.endDate =  data.endDate;
        dataSource.push(row);
      });
    }
    return new MatTableDataSource(dataSource);
  }


  addIncentiveRetailer($event) {
    if($event) {
      this.selectedRetailerPartner = $event.partner,
      this.selectedPlanType = $event.planType
    }
    this.getContentProviders();
    this.selectedPlan = null;
    this.createRetailerIncentive = false;
    this.getRPlansSelectedPartnerPlanType();
    this.showRetailerIncentive = true;
    
  }

  addIncentiveConsumer($event) {
    if($event) {
      this.selectedPlanType = $event
    }
    this.createConsumerIncentive = false;
    this.getConsumerIncentivePlans();
    this.showConsumerIncentive = true;
  }

  getConsumerIncentivePlans() {
    this.isErrorConsumer = false;
    this.incentiveService.getConsumerIncentivesByPlanType(this.selectedPlanType).subscribe(
      res => { 
          // const validResult = res.filter((result) => Array.isArray(result));
          // var mergedResult = [].concat.apply([], validResult);
          this.dataSourceConsumers = this.createDataSource(res.body);
          this.dataSourceConsumers.paginator = this.paginator.toArray()[1];;
          this.dataSourceConsumers.sort = this.sort.toArray()[1];   
          this.selectedStatusConsumer= this.statuses[0];     
          this.applyFilterConsumer(null);  
          this.getMissingPlansConsumer(this.dataSourceConsumers.data, this.selectedPlanType);  
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
  applyFilterRetailer(event) {
    var filterValue = "";
    if(event && event.target) {
      filterValue = event.target ? (event.target as HTMLInputElement).value : this.selectedStatusRetailer;
    } else if(event && event.value){
      filterValue = event.value ;
    }
    this.dataSourceRetailers.filter = filterValue.trim().toLowerCase();

    if (this.dataSourceRetailers.paginator) {
      this.dataSourceRetailers.paginator.firstPage();
    }
  }


  applyFilterConsumer(event) {
    var filterValue = "";
    if(event && event.target) {
       filterValue = event.target ? (event.target as HTMLInputElement).value : this.selectedStatusConsumer;
    } else if(event && event.value){
      filterValue = event.value;
    }
    this.dataSourceConsumers.filter = filterValue.trim().toLowerCase();

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
      // this.getRetailerIncentivePlans();
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
        this.getRPlansSelectedPartnerPlanType();
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
