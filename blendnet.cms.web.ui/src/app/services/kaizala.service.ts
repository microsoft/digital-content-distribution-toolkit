import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { last, map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { ContentProviderService } from './content-provider.service';
import { LogService } from './log.service';

@Injectable({
  providedIn: 'root'
})
export class KaizalaService {
  private currentUserSubject: BehaviorSubject<any>;
  public currentUser: Observable<any>;
  private currentUserNameSubject: BehaviorSubject<any>;
  public currentUserName: Observable<any>;

  
  baseUrl0 = environment.kaizalaApi0;
  baseUrl1 = environment.kaizalaApi1;
  baseUrl2 = environment.kaizalaApi2;

  constructor(
    private logger: LogService,
    private http: HttpClient,
    private contentProviderService: ContentProviderService
  ) {
    this.currentUserSubject = new BehaviorSubject<any>(JSON.parse(sessionStorage.getItem('currentUser')));
    this.currentUser = this.currentUserSubject.asObservable();
    this.currentUserNameSubject = new BehaviorSubject<any>(sessionStorage.getItem('currentUserName'));
    this.currentUserName = this.currentUserNameSubject.asObservable();
   }

  public get currentUserValue() {
    return this.currentUserSubject.value;
  }

  public get currentUserNameValue() {
    return this.currentUserNameSubject.value;
  }

  getURLSuffix (lastDigit) {
    var url = "";
    switch(lastDigit) {
      case '0' : 
      case '4' :
      case '6' :
        url = this.baseUrl0;
        break;
      case '1' :
      case '3' :
      case '5' :
        url = this.baseUrl1;
        break;
      case '2' :
      case '7' :
      case '8' :
      case '9' :
        url = this.baseUrl2;
        break;
    }
    return url;
  }

  getOTP(contact: string) {
    const lastDigit = contact.charAt(contact.length-1);
    var url = this.getURLSuffix(lastDigit).concat(environment.kaizalaSignUpSignIn);
    this.logger.log(`Calling Authentication by OTP`);
    var request = {
      "phoneNumber": contact,
      "UseVoice" : false
    }

    return this.http.post(url, request, {
      headers: {
        'AppName': environment.kaizalaAppName},
      observe: 'response'
    });
    // var myHeaders = new Headers();
    // myHeaders.append("Content-Type", "application/json");
    // myHeaders.append("appName", environment.appName);
    // var raw = JSON.stringify({"phoneNumber":contact,"useVoice":false});
    // var requestOptions = {
    //   mode: "no-cors",      
    //   method: 'POST',
    //   headers: myHeaders,
    //   body: raw,
    //   redirect: 'follow'
    // };
    // fetch("https://api-alpha2.kaiza.la/api/Authentication/LoginWithPhoneForPartners", 
    // {
    //   mode: "no-cors",      
    //   method: 'POST',
    //   headers: myHeaders,
    //   body: raw,
    //   redirect: 'follow'
    // })
    // .then(response => {
    //   return response.text()}
    //   )
    // .then(result => console.log(result))
    // .catch(error => console.log('error', error));
  }

  verifyOTP(otp: string, countryCode: string, contact: string) {
    const lastDigit = contact.charAt(contact.length-1);
    var url = this.getURLSuffix(lastDigit).concat(environment.kaizalaVerifyOTP);
    this.logger.log(`Calling Verify OTP`);
    var request = {
        "phoneNumber": countryCode.concat(contact),
        "pin": otp,
        "permissions": ["1.1"]
      }
      
    return this.http.post(url, request, 
      {
        headers: {'AppName': environment.kaizalaAppName}
      }).pipe(map(user => {
        sessionStorage.setItem('currentUserName', contact);
        sessionStorage.setItem('currentUser', JSON.stringify(user));
        this.currentUserSubject.next(user);
        return user;
      }));
  }

  getUserRoles(contact: string) {
    const lastDigit = contact.charAt(contact.length-1);
    var url = this.getURLSuffix(lastDigit).concat(environment.kaizalaGetUserRoles);
    let params = new HttpParams().set(environment.kaizalaAppNameParam, environment.kaizalaAppName);
    //.concat(environment.appName);
    var user = this.currentUserValue;
    return this.http.get(url, 
      {
        params: params,
        headers: {'accessToken': user.authenticationToken},
        observe: 'response'
      });
  }

  logout() {
    // remove the selected Content Provider from the local storage
    sessionStorage.removeItem("contentProviderId");
    sessionStorage.removeItem("contentProviderName");
    sessionStorage.removeItem("roles");

    this.contentProviderService.changeDefaultCP(null);
    // remove user from local storage and set current user to null
    sessionStorage.removeItem('currentUser');
    sessionStorage.removeItem('currentUserName');
    this.currentUserSubject.next(null);
  }
}
