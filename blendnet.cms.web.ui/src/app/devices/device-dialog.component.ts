import { Component, EventEmitter, Inject, OnInit, Output } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { ToastrService } from 'ngx-toastr';
import { DeviceService } from '../services/device.service';

@Component({
  selector: 'app-device-dialog',
  templateUrl: './device-dialog.component.html',
  styleUrls: ['./devices.component.css']
})
export class DeviceDialogComponent implements OnInit {
  @Output() onDeviceCreate = new EventEmitter<any>();
  deviceid;



  constructor(
    @Inject(MAT_DIALOG_DATA) public data: any,
    public dialogRef: MatDialogRef<DeviceDialogComponent>,
    private deviceService: DeviceService,
    private toastr: ToastrService

  ) { }

  ngOnInit(): void {
    }

  closeDialog(): void {
    this.dialogRef.close();
  }

  createDevice() {
    var device = this.getDeviceDetails();
    this.deviceService.createDevice(device).subscribe(
      res => {
        console.log(res);
        this.onDeviceCreate.emit("Device created successfully!");
       
      },
      err => this.toastr.error(err)
    );
  }


  getDeviceDetails() {
    var device: any = {
      id: this.deviceid
    }
    return device;
  }
    

  
  onNoClick(): void {
    this.dialogRef.close();
  }
}
