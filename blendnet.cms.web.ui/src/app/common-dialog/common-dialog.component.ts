import { Component, Inject, Output, EventEmitter } from '@angular/core';
import {MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';


@Component({
  selector: 'app-common-dialog',
  templateUrl: './common-dialog.component.html',
  styleUrls: ['./common-dialog.component.css']
})
export class CommonDialogComponent {
  @Output() onCPSelect= new EventEmitter<any>();

  constructor(
    public dialogRef: MatDialogRef<CommonDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any
  ) { 

  }

  submit(button): void {
    if (button.value === 'submit') {
      this.dialogRef.close('proceed');
    } else if (button.value === 'cancel') {
      this.dialogRef.close();
    }
  }

}
