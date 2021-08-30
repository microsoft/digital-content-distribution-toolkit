import { Component, Inject, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';

@Component({
  selector: 'app-device-dialog',
  templateUrl: './device-dialog.component.html',
  styleUrls: ['./devices.component.css']
})
export class DeviceDialogComponent implements OnInit {
  deviceForm = new FormGroup({
    devicename :  new FormControl('', [Validators.required, Validators.maxLength(20)]),
    deviceid:  new FormControl('', [Validators.required, Validators.maxLength(20)]),
  });


  constructor(
    @Inject(MAT_DIALOG_DATA) public data: any,
    public dialogRef: MatDialogRef<DeviceDialogComponent>,

  ) { }

  ngOnInit(): void {
  }

  get f() { 
    return this.deviceForm.controls; 
  }

  closeDialog(): void {
    this.dialogRef.close();
  }

  saveOrUpdate() {}
}
