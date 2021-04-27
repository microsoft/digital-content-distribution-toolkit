import { Component, OnInit, ViewChild } from '@angular/core';
import { MatSidenav } from '@angular/material/sidenav';
import { MsalBroadcastService, MsalService } from '@azure/msal-angular';
import { EventMessage, EventType } from '@azure/msal-browser';
import { filter } from 'rxjs/operators';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {
  loginDisplay = false;
  username ="";
  role:string[]= [];
  token;

  @ViewChild('sidenav') sidenav: MatSidenav;
  isExpanded = true;
  showHomeSubmenu: boolean = true;
  showContentSubmenu: boolean = true;
  showDeviceSubmenu: boolean = true;
  
  constructor(private authService: MsalService, private msalBroadcastService: MsalBroadcastService) { }

  ngOnInit(): void {
    this.msalBroadcastService.msalSubject$
      .pipe(
        filter((msg: EventMessage) => msg.eventType === EventType.LOGIN_SUCCESS),
      )
      .subscribe({
        next: (result: EventMessage) => {
          console.log(result);
          if (result?.payload?.account) {
            this.authService.instance.setActiveAccount(result.payload.account);
          }
        },
        error: (error) => console.log(error)
      });

    this.setLoginDisplay();

  }

  setLoginDisplay() {
    this.loginDisplay = this.authService.instance.getAllAccounts().length > 0;
    
    if(this.loginDisplay) {
      this.token = this.authService.instance.getAllAccounts()[0].idTokenClaims;
      this.token.groups.forEach(group => {
        this.role.push(group);
      });
      this.username = this.token.givenName;
      console.log(this.username);
      console.log(this.role);
    }
    
    
  }

}
