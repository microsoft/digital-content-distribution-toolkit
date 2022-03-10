import { Component, Inject, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { ToastrService } from 'ngx-toastr';
import { lengthConstants } from '../constants/length-constants';
import { CustomValidator } from '../custom-validator/custom-validator';
import { RetailerService } from '../services/retailer.service';

@Component({
  selector: 'app-device-assign',
  templateUrl: './device-assign.component.html',
  styleUrls: ['./devices.component.css']
})
export class DeviceAssignComponent implements OnInit {

  deviceDeploymentStatus:boolean = false;

  retailerForm = new FormGroup({
    retailerID: new FormControl('',  [Validators.maxLength(lengthConstants.titleMaxLength), 
      Validators.minLength(lengthConstants.titleMinLength), 
      CustomValidator.alphaNumericSplChar]),
    partnerCode: new FormControl('',  [Validators.maxLength(lengthConstants.titleMaxLength), 
      Validators.minLength(lengthConstants.titleMinLength), 
      CustomValidator.alphaNumericSplChar]),
    // startDate: new FormControl()
  });
  // minStart: Date;

  isUpdate= false;
  // minEnd: Date;
  constructor( @Inject(MAT_DIALOG_DATA) public data: any,
  public dialogRef: MatDialogRef<any>,
  private retailerService: RetailerService,
  private toastr: ToastrService) { 
    // this.minStart = new Date();
  }

  ngOnInit(): void {
    var retailerId = '';
    var partnerCode = '';
    this.retailerService.getRetailerByDeviceId(this.data.deviceId).subscribe(res => {
      if(res.status === 200) {
        var retailerList: any = res.body;
        var retailer  = retailerList.find(r => r.deviceAssignments.find(d => 
          d.isActive ));
        if(retailer) {
          this.isUpdate = true;
          retailerId = retailer.partnerProvidedId;
          partnerCode = retailer.partnerCode;
          var activeDevice = retailer.deviceAssignments.find(d=> d.isActive);
          this.deviceDeploymentStatus = activeDevice.isDeployed;
        }
      } else if(res.status === 404){
        //
      } else {
        this.toastr.error("Something went wrong...!!");
      }
      this.retailerForm = new FormGroup({
        retailerID: new FormControl(retailerId, [Validators.maxLength(lengthConstants.titleMaxLength), 
          Validators.minLength(lengthConstants.titleMinLength), 
          CustomValidator.alphaNumericSplChar]),
        partnerCode: new FormControl(partnerCode, [Validators.maxLength(lengthConstants.titleMaxLength), 
          Validators.minLength(lengthConstants.titleMinLength), 
          CustomValidator.alphaNumericSplChar])
      });
    })
  }

  get f() { 
    return this.retailerForm.controls; 
  }

  saveOrUpdate() {
    var request:any = {
      "partnerCode": this.retailerForm.get('partnerCode').value,
      "partnerProvidedRetailerId": this.retailerForm.get('retailerID').value,
      "deviceId": this.data.deviceId
    }
    if(this.isUpdate) {
      this.retailerService.unassignDeviceToRetailer(request).subscribe(res => {
        if(res.status === 204) {
          this.toastr.success("Device unassigned sucessfully");
          this.closeDialog();
        } 
      },
      err => { this.toastr.error(err);}
      );
    } else {
      this.retailerService.assignDeviceToRetailer(request).subscribe(res => {
        if(res.status === 204) {
          this.toastr.success("Device assigned to retailer sucessfully");
          this.closeDialog();
        } 
      },
      err => { this.toastr.error(err);}
      );
    }
    
  }
  deployDevice() {
    var request:any = {
      "partnerCode": this.retailerForm.get('partnerCode').value,
      "partnerProvidedRetailerId": this.retailerForm.get('retailerID').value,
      "deviceId": this.data.deviceId
    }
    this.retailerService.deployDeviceToRetailer(request).subscribe(res =>{
      if(res.status === 204) {
        this.toastr.success("Device deployed sucessfully");
        this.closeDialog();
      } else {
        this.toastr.error("Device deployed failed!");
      }
    }, err => {
      this.toastr.error(err);
    }

    )

  }

  closeDialog(): void {
    this.dialogRef.close();
  }

}
