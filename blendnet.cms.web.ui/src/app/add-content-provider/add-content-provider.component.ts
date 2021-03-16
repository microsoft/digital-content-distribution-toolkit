import { Component, Inject, OnInit } from '@angular/core';
import {FormArray, FormControl, FormGroup, Validators} from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { ContentproviderAdmin } from '../models/contentprovider-admin';
import { Contentprovider } from '../models/contentprovider.model';
import { ContentProviderService } from '../services/content-provider.service';
import { ToastrService } from 'ngx-toastr';

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

  cpForm = new FormGroup({
    cpname :  new FormControl(' ', [Validators.required]),
    logoUrl : new FormControl(' '),
    status : new FormControl(false, [Validators.required]),
    activationDate : new FormControl(null, [Validators.required]),
    deactivationDate : new FormControl(null, [Validators.required]),
    admins : new FormControl(this.admins)
  });


  aminDate: Date;
  amaxDate: Date;
  dminDate: Date;
  dmaxDate: Date;

  constructor(
    public dialogRef: MatDialogRef<AddContentProviderComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any,
    public contentProviderService: ContentProviderService,
    private toastr: ToastrService
  ) {
    const currentYear = new Date().getFullYear();
    const currentDate= new Date().getDate();
    const currentMonth= new Date().getMonth();
    this.aminDate = new Date(currentYear, currentMonth, currentDate-5);
    this.amaxDate = new Date(currentYear , currentMonth, currentDate+10);
    this.dminDate = new Date(currentYear, currentMonth, currentDate+1);
    this.dmaxDate = new Date(currentYear+1, 11, 31);
  }

  ngOnInit(): void {
    console.log('modal dialog opened:'+this.data.cp.name);
    this.cp = this.data.cp;
    this.cpForm.get("cpname").setValue(this.cp.name);
    this.cpForm.get("logoUrl").setValue(this.cp.logoUrl);
    this.cpForm.get("status").setValue(this.cp.status? "active" : "inactive");
    this.cpForm.get("activationDate").setValue(null);
    this.cpForm.get("deactivationDate").setValue(null);
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
    if(this.cp.id) {
      this.contentProviderService.editContentProvider(this.cp).subscribe(res => {
        if(res) {
          this.showSuccess("Update Successful");
        }
      });
    } else {
      this.contentProviderService.createContentProvider(this.cp).subscribe(res => {
        this.showSuccess("New content provider created Successful");
      });
    }
  }


  showSuccess(message) {
    this.toastr.success(message);
  }

  closeDialog(): void {
    this.dialogRef.close();
  }


}