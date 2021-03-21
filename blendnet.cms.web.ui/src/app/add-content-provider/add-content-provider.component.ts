import { Component, EventEmitter, Inject, OnInit, Output } from '@angular/core';
import {FormArray, FormControl, FormGroup, Validators} from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { ContentproviderAdmin } from '../models/contentprovider-admin';
import { Contentprovider } from '../models/contentprovider.model';
import { ContentProviderService } from '../services/content-provider.service';
import { ToastrService } from 'ngx-toastr';
import { UserService } from '../services/user.service';

@Component({
  selector: 'app-add-content-provider',
  templateUrl: './add-content-provider.component.html',
  styleUrls: ['./add-content-provider.component.css']
})
export class AddContentProviderComponent implements OnInit {
  admins: ContentproviderAdmin[] = [];
  selectable = true;
  removable = true;
  cp: Contentprovider;
  @Output() onCPUpdateOrCreate = new EventEmitter<any>();


  cpForm = new FormGroup({
    cpname :  new FormControl(' ', [Validators.required]),
    logoUrl : new FormControl(' '),
    isActive : new FormControl("inactive", [Validators.required]),
    activationDate : new FormControl(null, [Validators.required]),
    deactivationDate : new FormControl(null, [Validators.required]),
    admins : new FormControl(this.admins)
  });


  // aminDate: Date;
  // amaxDate: Date;
  // dminDate: Date;
  // dmaxDate: Date;

  constructor(
    public dialogRef: MatDialogRef<AddContentProviderComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any,
    public contentProviderService: ContentProviderService,
    private toastr: ToastrService,
    private userService: UserService
  ) {
    // const currentYear = new Date().getFullYear();
    // const currentDate= new Date().getDate();
    // const currentMonth= new Date().getMonth();
    // this.aminDate = new Date(currentYear, currentMonth, currentDate-5);
    // this.amaxDate = new Date(currentYear , currentMonth, currentDate+10);
    // this.dminDate = new Date(currentYear, currentMonth, currentDate+1);
    // this.dmaxDate = new Date(currentYear+1, 11, 31);
  }

  ngOnInit(): void {
    this.cp = this.data.cp;
    this.cpForm.get("cpname").setValue(this.cp.name);
    this.cpForm.get("logoUrl").setValue(this.cp.logoUrl);
    this.cpForm.get("isActive").setValue(this.cp.isActive? "active" : "inactive");
    this.cpForm.get("activationDate").setValue(this.cp.activationDate);
    this.cpForm.get("deactivationDate").setValue(this.cp.deactivationDate);
    this.admins= this.cp.admins
    this.cpForm.get("admins").setValue(this.admins);
  }


  remove(admin: ContentproviderAdmin): void {
    const index = this.admins.indexOf(admin);
    if (index >= 0) {
      this.admins.splice(index, 1);
    }
  }

  public errorHandling = (control: string, error: string) => {
    return this.cpForm.controls[control].hasError(error);
  }

  saveOrUpdate() {
    var newUpdatedCP = {
      id: this.cp.id,
      name: this.cpForm.get("cpname").value,
      logoUrl: this.cpForm.get("logoUrl").value,
      activationDate: this.cpForm.get("activationDate").value,
      deactivationDate: this.cpForm.get("deactivationDate").value,
      isActive: this.cpForm.get("isActive").value === "active" ? true : false,
      admins: []
    }
    if(newUpdatedCP.id) {
      this.contentProviderService.editContentProvider(newUpdatedCP).subscribe(res => {
        console.log(res);
        if(res) {
          this.showSuccess("Update Successful");
        }
        this.onCPUpdateOrCreate.emit("Content Provider Updated");
      });
    } else {
      this.contentProviderService.createContentProvider(newUpdatedCP).subscribe(res => {
        if(res.status === 201) {
          this.showSuccess("New content provider created Successful");
        } else {
          this.showError("Content Provider creation failed !")
        }
        this.onCPUpdateOrCreate.emit("Content Provider Created");
      });
    }
  }


  getUserDetails(upn) {
    this.userService.getUserDetails(upn);
  }
  showSuccess(message) {
    this.toastr.success(message);
  }

  showError(message) {
    this.toastr.error(message);
  }

  closeDialog(): void {
    this.dialogRef.close();
  }


}