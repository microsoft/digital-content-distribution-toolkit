import { Component, OnInit, AfterViewInit, OnDestroy } from '@angular/core';
import { RetailerDashboardService } from 'src/app/services/retailer/retailer-dashboard.service'
import { ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot } from '@angular/router';
import { UserService } from '../../services/user.service';
import { EventType } from 'src/app/models/incentive.model';

@Component({
  selector: 'app-retailer-milestones',
  templateUrl: './retailer-milestones.component.html',
  styleUrls: ['./retailer-milestones.component.css']
})
export class RetailerMilestonesComponent implements OnInit, AfterViewInit, OnDestroy {
  totalMilestoneEarnings = 0
  milestonesCarouselArr: Array<any> = [];
  partnerCode = sessionStorage.getItem('partnerCode');
  retailerPartnerProvidedId = sessionStorage.getItem('partnerProvidedId');
  baseHref = this.retailerDashboardService.getBaseHref();
  constructor(
    private retailerDashboardService: RetailerDashboardService,
    public router: Router,
    public userService: UserService
  ) { }

  ngOnInit(): void {
    this.partnerCode = this.retailerDashboardService.getpartnerCode();
    this.retailerPartnerProvidedId = this.retailerDashboardService.getRetailerPartnerProvidedId();
    this.getMilestoneTotal();

    this.getRegularRatesIncentives()
  }

  getMilestoneTotal() {
    let totalMilestoneEarnings = 0;
    let milestonesCarouselArr = []
    this.retailerDashboardService.getMileStonesHome(this.partnerCode, this.retailerPartnerProvidedId).subscribe( res => {
      
      if(res.planDetails) {
        res.planDetails.forEach(planDetail => {
          if(planDetail.formula && planDetail.formula.formulaType === 'DIVIDE_AND_MULTIPLY') {
            if(planDetail.result && planDetail.result.value) {
              const value = planDetail.result.value;
              totalMilestoneEarnings+=value;
            }
            if(!planDetail.result) {
              planDetail.result = {
                residualValue: 0,
                value: 0
              }
            }
            if(planDetail.formula.firstOperand && planDetail.formula.secondOperand && planDetail.result) {
              milestonesCarouselArr.push({
                firstOperand: planDetail.formula.firstOperand,
                secondOperand: planDetail.formula.secondOperand,
                value : planDetail.result.value ? planDetail.result.value : 0,
                residualValue : planDetail.result.residualValue ? planDetail.result.residualValue : 0,
                progress: ((planDetail.result.residualValue ? planDetail.result.residualValue : 0))*100/planDetail.formula.firstOperand,
                referral: planDetail.eventType === EventType['Retailer Referral']
              });
            } 
          }
        });
        this.totalMilestoneEarnings = totalMilestoneEarnings;
        this.milestonesCarouselArr = milestonesCarouselArr;
        console.log(this.milestonesCarouselArr);
      }
    } ,err => {
      console.log('error in milestone fetch');
      this.totalMilestoneEarnings = totalMilestoneEarnings;
    });
  }

  getRegularRatesIncentives() {
    this.retailerDashboardService.getRegularRatesIncentives(this.partnerCode, this.retailerPartnerProvidedId).subscribe(
      res => {

      },
      err => {
        
      }
    )
  }

  
  navigateToHome() {
    this.router.navigate(['retailer/home']);
  }

  ngAfterViewInit() {
    console.log('setting routed to' + true);
    this.userService.setRetailerRouted(true);
  }

  ngOnDestroy() {
    this.userService.setRetailerRouted(false);
  }


}
