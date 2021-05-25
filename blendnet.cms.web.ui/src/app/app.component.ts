import { Component, Inject, ViewChild } from '@angular/core';
import { Subject } from 'rxjs';
import { MatSidenav } from '@angular/material/sidenav';
import { Contentprovider } from './models/contentprovider.model';
import { ContentProviderService } from './services/content-provider.service';
import { Router } from '@angular/router';
import { KaizalaService } from './services/kaizala.service';
import { environment } from 'src/environments/environment';


@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  title = 'CMS';
  private readonly _destroying$ = new Subject<void>();

  @ViewChild('sidenav') sidenav: MatSidenav;
  isExpanded = true;
  showHomeSubmenu: boolean = true;
  showContentSubmenu: boolean = true;
  showDeviceSubmenu: boolean = true;
  selectedCP: Contentprovider;
  selectedCPName: string = "Not Selected";
  currentUser: any;
  currentUserName: string = '';
  hasMenuAccess: boolean = false;

  constructor(
    private cpService: ContentProviderService,
    private router: Router,
    private kaizalaService: KaizalaService
  ) {
    this.kaizalaService.currentUser.subscribe(user => {
      this.currentUser = user});
    this.kaizalaService.currentUserName.subscribe(userName => {
        this.currentUserName = userName});
    this.selectedCPName = localStorage.getItem("contentProviderName") ? 
      localStorage.getItem("contentProviderName") : "Not Selected";

  }

    ngOnInit(): void {
        this.cpService.sharedSelectedCP$.subscribe(selectedCP => {
          this.selectedCPName = selectedCP ? selectedCP.name : 
          localStorage.getItem("contentProviderName") ? 
          localStorage.getItem("contentProviderName") :"Not Selected";
        });
        
    }

    ngDoCheck() {
      this.hasMenuAccess = localStorage.getItem("roles")?.includes(environment.roles.SuperAdmin) ||
      localStorage.getItem("roles")?.includes(environment.roles.ContentAdmin);

      this.cpService.sharedSelectedCP$.subscribe(selectedCP => {
        this.selectedCPName = selectedCP ? selectedCP.name : 
        localStorage.getItem("contentProviderName") ? 
        localStorage.getItem("contentProviderName") :"Not Selected";      
      });
      this.kaizalaService.currentUser.subscribe(user => {
        this.currentUser = user});
    }

    logout() {
      this.selectedCPName = "Not Selected";
      this.kaizalaService.logout();
      this.router.navigate(['/login']);
    }

    ngOnDestroy(): void {
      this._destroying$.next(undefined);
      this._destroying$.complete();
    }
}

