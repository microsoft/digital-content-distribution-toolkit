// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

import { Component, Inject, Output, EventEmitter } from '@angular/core';
import {MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';


@Component({
  selector: 'app-retailer-commission-dialog',
  templateUrl: './retailer-commission-dialog.component.html',
  styleUrls: ['./retailer-commission-dialog.component.css']
})
export class RetailerCommissionDialogComponent {
  mobile: string;
  constructor(
    public dialogRef: MatDialogRef<RetailerCommissionDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any
  ) { 
    this.data.properties.forEach(obj => {
      if(obj.name === "UserPhone") {
        this.mobile = obj.value
      }
    });
  }

  submit(button): void {

  }

  closeDialog() {
    this.dialogRef.close();
  }

}

