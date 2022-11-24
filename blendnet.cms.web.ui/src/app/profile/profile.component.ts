// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.css']
})
export class ProfileComponent implements OnInit {
 
  roles: string[];
  // currentUsername;
  constructor() { }

  ngOnInit(): void {
    this.roles = sessionStorage.getItem("roles")?.split(",");
    // this.currentUsername = sessionStorage.getItem("currentUserName");
  }

  // convertToSentence(role): string {
  //   var result = role.replace( /([A-Z])/g, " $1" );
  //   var finalResult = result.charAt(0).toUpperCase() + result.slice(1);
  //   console.log(finalResult);
  //   return finalResult;
  // }

}
