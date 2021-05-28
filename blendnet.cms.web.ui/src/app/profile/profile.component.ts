import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.css']
})
export class ProfileComponent implements OnInit {
 
  roles: string[];
  currentUsername;
  constructor() { }

  ngOnInit(): void {
    this.roles = localStorage.getItem("roles").split(",");
    this.currentUsername = localStorage.getItem("currentUserName");
  }

}
