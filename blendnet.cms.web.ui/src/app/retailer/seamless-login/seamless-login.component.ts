import { Component, OnInit, AfterViewInit } from '@angular/core';
import {KaizalaService } from 'src/app/services/kaizala.service'
import { ActivatedRoute, Router } from '@angular/router';
import { UserService } from '../../services/user.service';
import jwt_decode from 'jwt-decode';

@Component({
  selector: 'app-seamless-login',
  templateUrl: './seamless-login.component.html',
  styleUrls: ['./seamless-login.component.css']
})
export class SeamlessLoginComponent implements OnInit, AfterViewInit {
  partnerCode: string;
  parnterProvidedId: string;
  authenticationToken: string;
  roles: string;
  isCountryCodeSection: boolean = true;
  isContactOnlySection: boolean = true;
  isOTPSection: boolean = false;
  otpVerifyErrorMessage: string;

  constructor(
    private kaizalaService: KaizalaService,
    private route: ActivatedRoute,
    private userService: UserService,
    private router: Router
  ) {
    console.log('Called Constructor');
    this.route.queryParams.subscribe(params => {
        this.partnerCode = params['partnerCode'];
        this.parnterProvidedId = params['parnterProvidedId'];
        this.authenticationToken = params['authenticationToken'];
        sessionStorage.setItem('partnerCode', this.partnerCode);
        sessionStorage.setItem('parnterProvidedId', this.parnterProvidedId);
        sessionStorage.setItem('authenticationToken', this.authenticationToken);
        let currentUser = {
          authenticationToken: this.authenticationToken
        }
        sessionStorage.setItem('currentUser', JSON.stringify(currentUser));
        if(sessionStorage.getItem('retailerLogged')){

        } else {
          sessionStorage.setItem('retailerLogged', 'true');
          location.reload();
        }
    });
   }

  ngOnInit(): void {
    this.fetchRetailer();
  }

  ngAfterViewInit() {
  }

  fetchRetailer() {
    console.log('fetch retailer');
    debugger;
    this.kaizalaService.validateRetailerRole(this.authenticationToken).subscribe( 
      res => {
        var response: any = res.body;
        sessionStorage.setItem('roles', response.userRole);
        this.roles =  sessionStorage.getItem('roles');

        const decodedToken = jwt_decode(this.authenticationToken);
        const urnCreds = JSON.parse(decodedToken['urn:microsoft:credentials'])
        const phoneNumber = urnCreds['phoneNumber'];
        sessionStorage.setItem('currentUserName', phoneNumber);

        this.userService.setLoggedInUser(phoneNumber);
        this.router.navigate(['retailer/home']);
      },
      err => {
        this.otpVerifyErrorMessage = err;
      }
    ) 

  }
}
