import { Component, OnInit } from '@angular/core';
import { KaizalaService } from '../services/kaizala.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {

  currentUser: any;
  userName: string = "";
  roles: string = "";

  constructor(
    private kaizalaService: KaizalaService
  ) {
    this.kaizalaService.currentUser.subscribe(x => this.currentUser = x);
    
   }

  ngOnInit(): void {
    this.roles = localStorage.getItem("roles");
  }

}
