import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot } from '@angular/router';
@Injectable({
    providedIn: 'root'
  })
export class RoleGuardService implements CanActivate {

  constructor() {}
  
  canActivate(route: ActivatedRouteSnapshot): boolean {
    const expectedRole = route.data.expectedRole;
    const userRoles = sessionStorage.getItem("roles")?.split(",");
    var hasExpectedRole = false;
    userRoles?.forEach( role =>{
      if(expectedRole?.includes(role)) {
        hasExpectedRole = true;
      }
    });
  
    if(!userRoles) {
      window.alert('Please ensure that your account is assigned to an app role and then sign-out and sign-in again.');
      return false;
    } else if (!hasExpectedRole) {
      window.alert('You do not have access as expected role is missing. Please ensure that your account is assigned to an app role and then sign-out and sign-in again.');
      return false;
    } else if(!sessionStorage.getItem("contentProviderId") 
      && route.routeConfig.path !== 'incentive-management' 
      && route.routeConfig.path !== 'content-providers' && route.routeConfig.path !== 'notifications') {
      window.alert("Please select a Content Provider to access the management services");
      return false;
    }
    return true;
  }
}