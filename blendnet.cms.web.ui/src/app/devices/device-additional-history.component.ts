// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

import { Component, Inject, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';

@Component({
  selector: 'app-device-additional-history',
  templateUrl: './device-additional-history.component.html',
  styleUrls: ['./devices.component.css']
})
export class AdditionalHistoryDialog implements OnInit {
  content;
 

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: any

  ) { 
  }

  ngOnInit(): void {
   this.content =this.data.content;
   
  }

 

}
