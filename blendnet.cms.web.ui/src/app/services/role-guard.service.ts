import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot } from '@angular/router';
import { KaizalaService } from './kaizala.service';
@Injectable({
    providedIn: 'root'
  })
export class RoleGuardService implements CanActivate {

  constructor(private kaizalaService: KaizalaService) {}
  
  canActivate(route: ActivatedRouteSnapshot): boolean {
    const expectedRoles = route.data.expectedRole;
    const userRoles = sessionStorage.getItem("roles")?.split(",");
  
    if(!userRoles) {
      window.alert('Please ensure that your account is assigned to an app role and then sign-out and sign-in again.');
      this.kaizalaService.logout();
      return false;
    } else if (expectedRoles && !expectedRoles.some(expectedRole => userRoles.includes(expectedRole))) {
      window.alert('You do not have access as expected role is missing. Please ensure that your account is assigned to an app role and then sign-out and sign-in again.');
      this.kaizalaService.logout();
      return false;
    } else if(!sessionStorage.getItem("contentProviderId") 
      && route.routeConfig.path !== 'incentive-management'  &&  !route.routeConfig.path.includes('devices')
      &&  route.routeConfig.path !== 'manage-content' && route.routeConfig.path !== 'profile'
      && route.routeConfig.path !== 'content-providers' && route.routeConfig.path !== 'notifications') {
      window.alert("Please select a Content Provider to access the management services");
      return false;
    }
    return true;
  }

}