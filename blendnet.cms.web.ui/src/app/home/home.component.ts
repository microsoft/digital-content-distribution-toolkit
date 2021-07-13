import { Component, OnInit } from '@angular/core';
import { environment } from 'src/environments/environment';
import { KaizalaService } from '../services/kaizala.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {

  // currentUser: any;
  // userName: string = "";
  // roles: string = "";
  // hasNoAdminRoles: boolean = false;
  constructor(
    // private kaizalaService: KaizalaService
  ) {
    // this.kaizalaService.currentUser.subscribe(user => this.currentUser = user);
    // this.userName = sessionStorage.getItem("currentUserName");
   }

  ngOnInit(): void {
    // this.roles = sessionStorage.getItem("roles");
    // this.hasNoAdminRoles = !(sessionStorage.getItem("roles")?.includes(environment.roles.SuperAdmin) ||
    // sessionStorage.getItem("roles")?.includes(environment.roles.ContentAdmin));
  }

}
