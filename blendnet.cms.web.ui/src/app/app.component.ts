import { Component, ViewChild } from '@angular/core';
import { fromEvent, Observable, Subject, Subscription } from 'rxjs';
import { MatSidenav } from '@angular/material/sidenav';
import { Contentprovider } from './models/contentprovider.model';
import { ContentProviderService } from './services/content-provider.service';
import { Router } from '@angular/router';
import { KaizalaService } from './services/kaizala.service';
import { environment } from 'src/environments/environment';
import { CommonDialogComponent } from 'src/app/common-dialog/common-dialog.component';
import { MatDialog } from '@angular/material/dialog';
import { UserService } from './services/user.service';



@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  title = 'CMS';
  private readonly _destroying$ = new Subject<void>();

  @ViewChild('sidenav') sidenav: MatSidenav;
  isExpanded;
  isDesktop;
  showHomeSubmenu: boolean = true;
  showContentSubmenu: boolean = true;
  showDeviceSubmenu: boolean = true;
  selectedCP: Contentprovider;
  selectedCPName: string = "Not Selected";
  currentUser: any;
  currentUserName: string = '';
  hasMenuAccess: boolean = false;
  hasRetailerAccess: boolean = false;
  innerWidth = window.innerWidth;
  resizeObservable$: Observable<Event>
  resizeSubscription$: Subscription;
  constructor(
    private cpService: ContentProviderService,
    private router: Router,
    private kaizalaService: KaizalaService,
    public dialog: MatDialog,
    public userService: UserService
  ) {
    this.kaizalaService.currentUser.subscribe(user => {
      this.currentUser = user});
    this.kaizalaService.currentUserName.subscribe(userName => {
        this.currentUserName = userName});
    this.selectedCPName = sessionStorage.getItem("contentProviderName") ? 
      sessionStorage.getItem("contentProviderName") : "Not Selected";

  }

    ngOnInit(): void {
        this.isExpanded = (this.innerWidth > 768) ? true : false;
        this.isDesktop = (this.innerWidth > 1023) ? true : false;
        this.cpService.sharedSelectedCP$.subscribe(selectedCP => {
          this.selectedCPName = selectedCP ? selectedCP.name : 
          sessionStorage.getItem("contentProviderName") ? 
          sessionStorage.getItem("contentProviderName") :"Not Selected";
        });

        this.resizeObservable$ = fromEvent(window, 'resize')
        this.resizeSubscription$ = this.resizeObservable$.subscribe( evt => {
          this.innerWidth = window.innerWidth;
          this.isExpanded = (this.innerWidth > 768) ? true : false;
          this.isDesktop = (this.innerWidth > 1023) ? true : false;
        })
        
    }



    ngDoCheck() {
      this.hasRetailerAccess = sessionStorage.getItem("roles")?.includes(environment.roles.Retailer);
      this.hasMenuAccess = (sessionStorage.getItem("roles")?.includes(environment.roles.SuperAdmin) ||
      sessionStorage.getItem("roles")?.includes(environment.roles.ContentAdmin)) && !this.hasRetailerAccess;

      this.cpService.sharedSelectedCP$.subscribe(selectedCP => {
        this.selectedCPName = selectedCP ? selectedCP.name : 
        sessionStorage.getItem("contentProviderName") ? 
        sessionStorage.getItem("contentProviderName") :"Not Selected";      
      });
      this.userService.loggedInUser$.subscribe(user => {
        this.currentUserName = user ? user : 
        sessionStorage.getItem("currentUserName");
      })
    }

    logout() {
      const dialogRef = this.dialog.open(CommonDialogComponent, {
        disableClose: true,
        data: {message: "Are you sure you want to log out?", heading:'Confirm Logout',
          buttons: this.logoutButtons()
        },
      });

      dialogRef.afterClosed().subscribe(result => {
        if (result === 'proceed') {
          this.selectedCPName = "Not Selected";
          this.kaizalaService.logout();
          this.router.navigate(['/login']);
        }
      });
    }

    ngOnDestroy(): void {
      this._destroying$.next(undefined);
      this._destroying$.complete();
    }

    logoutButtons(): Array<any> {
      return [{
        label: 'Cancel',
        type: 'basic',
        value: 'cancel',
        class: 'discard-btn'
      },
      {
        label: 'Log Out',
        type: 'primary',
        value: 'submit',
        class: 'update-btn'
      }
      ]
    }

}

