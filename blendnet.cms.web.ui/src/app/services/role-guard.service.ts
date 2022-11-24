// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, Router } from '@angular/router';
import { KaizalaService } from './kaizala.service';
@Injectable({
    providedIn: 'root'
  })
export class RoleGuardService implements CanActivate {

  constructor(private kaizalaService: KaizalaService, private router: Router) {}
  
  canActivate(route: ActivatedRouteSnapshot): boolean {
    
    const userRoles = sessionStorage.getItem("roles")?.split(",");
  
    if(!userRoles) {
      window.alert('Please ensure that your account is assigned to an app role and then sign-out and sign-in again.');
      this.kaizalaService.logout();
      return false;
    } else if (this.validateRole(route) || this.validateFeatureAccess(route)) {
      window.alert('You do not have access as expected role is missing. Please ensure that your account is assigned to an app role and then sign-out and sign-in again.');
      // this.kaizalaService.logout();
      return false;
    } else if((route.routeConfig.path === 'sas-key' || route.routeConfig.path === 'unprocessed-content'
      || route.routeConfig.path === 'unprocessed-content' ||  route.routeConfig.path === 'processed-content'
      || route.routeConfig.path === 'broadcast-content' || route.routeConfig.path === 'subscriptions')
      && !sessionStorage.getItem("contentProviderId")) {
      window.alert("Please select a Content Provider to access the management services");
      this.router.navigate(['/admin/content-providers'])
    }
    return true;
  }

  validateRole(route: ActivatedRouteSnapshot) {
    const expectedRoles = route.data.expectedRole;
    const userRoles = sessionStorage.getItem("roles")?.split(",");
    return (expectedRoles && !expectedRoles.some(expectedRole => userRoles.includes(expectedRole)));
  }

  validateFeatureAccess(route: ActivatedRouteSnapshot) {
    if(!route.data.featureName || !sessionStorage.getItem("supportedFeatures")) {
      return false;
    }
    const featureName = route.data.featureName;
    const supportedFeatures = JSON.parse(sessionStorage.getItem("supportedFeatures"));
    return (featureName && !supportedFeatures[featureName]);
  }

}