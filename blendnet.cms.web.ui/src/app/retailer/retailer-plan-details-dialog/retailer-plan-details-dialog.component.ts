// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

import { Component, Inject } from '@angular/core';
import {MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';

@Component({
  selector: 'app-retailer-plan-details-dialog',
  templateUrl: './retailer-plan-details-dialog.component.html',
  styleUrls: ['./retailer-plan-details-dialog.component.css']
})
export class RetailerPlanDetailsDialogComponent  {

  constructor(
    public dialogRef: MatDialogRef<RetailerPlanDetailsDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any
  ) { }

  ngOnInit(): void {
  }

  closeDialog() {
    this.dialogRef.close();
  }

}
