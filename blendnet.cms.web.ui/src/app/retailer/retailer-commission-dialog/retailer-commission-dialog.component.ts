import { Component, Inject, Output, EventEmitter } from '@angular/core';
import {MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';


@Component({
  selector: 'app-retailer-commission-dialog',
  templateUrl: './retailer-commission-dialog.component.html',
  styleUrls: ['./retailer-commission-dialog.component.css']
})
export class RetailerCommissionDialogComponent {
  constructor(
    public dialogRef: MatDialogRef<RetailerCommissionDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any
  ) { 

  }

  submit(button): void {

  }

  closeDialog() {
    this.dialogRef.close();
  }

}

