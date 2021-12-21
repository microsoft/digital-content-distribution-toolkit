import { Component, EventEmitter, Inject, OnInit, Output } from '@angular/core';
import { FormControl, FormGroup, Validators} from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { ContentproviderAdmin } from '../models/contentprovider-admin';
import { Contentprovider } from '../models/contentprovider.model';
import { ContentProviderService } from '../services/content-provider.service';
import { ToastrService } from 'ngx-toastr';
import { UserService } from '../services/user.service';
import { environment } from 'src/environments/environment';


@Component({
  selector: 'app-add-content-provider',
  templateUrl: './add-content-provider.component.html',
  styleUrls: ['./add-content-provider.component.css']
})
export class AddContentProviderComponent implements OnInit {
  contentAdministrators: ContentproviderAdmin[] = [];
  selectable = true;
  removable = false;
  adminNotFoundError: string="";
  adminFoundSuccessMsg: string = "";
  adminWarnMsg: string ="";
  cp: Contentprovider;
  @Output() onCPUpdateOrCreate = new EventEmitter<any>();
  isSuperAdmin: boolean = false;



  cpForm = new FormGroup({
    cpname :  new FormControl('', [Validators.required, Validators.maxLength(20), Validators.pattern(/^[^\s].*[^\s]$/)]),
    logoUrl : new FormControl(''),
    admins: new FormControl(''),
    contentAdministrators : new FormControl(this.contentAdministrators),
    adminUpn: new FormControl('',  [Validators.maxLength(10), Validators.minLength(10), Validators.pattern(/^-?(0|[1-9]\d*)?$/)])
  });



  constructor(
    public dialogRef: MatDialogRef<AddContentProviderComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any,
    public contentProviderService: ContentProviderService,
    private toastr: ToastrService,
    private userService: UserService
  ) {

    this.isSuperAdmin = sessionStorage.getItem("roles")?.includes(environment.roles.SuperAdmin);
    if(this.isSuperAdmin) {
      this.removable = true;
    } else{
      this.cpForm.disable();
    }
    
  }

  ngOnInit(): void {
    this.cp = this.data.cp;
    this.cpForm.get("cpname").setValue(this.cp.name);
    this.cpForm.get("logoUrl").setValue(this.cp.logoUrl);
    this.contentAdministrators = this.cp.contentAdministrators ? this.cp.contentAdministrators :[];
    this.cpForm.get("admins").setValue(this.contentAdministrators);
  }

  get f() { 
    return this.cpForm.controls; 
  }


  getAdminSearchError() {
    this.adminNotFoundError= "";
    this.adminFoundSuccessMsg="";
    this.adminWarnMsg = "";
    if (this.cpForm.get("adminUpn").invalid) {
      return 'Please enter a valid Phone Number';
    }
  }
  searchAndAddAdmin() {
    var upn = this.cpForm.get("adminUpn").value;
    if(this.contentAdministrators.some(admin => admin.phoneNumber ===  upn)) {
      this.adminWarnMsg="This user is already added!";
    } else {
      this.userService.getUserDetails(upn).subscribe(res => {
          var newAdmin = {
            id : res.identityId,
            phoneNumber: res.phoneNumber,
            userId: res.userId
          }
          this.contentAdministrators.push(newAdmin);
          this.adminFoundSuccessMsg="Please click save to add the user as admin";
          this.cpForm.get("adminUpn").setValue("");
      },
      err => {
        if(err === "Not Found") {
          this.adminNotFoundError = "This is not a registered user!";
        } else {
          this.toastr.error(err);
        }
         
      })
    }
  }

  remove(admin: ContentproviderAdmin): void {
    const index = this.contentAdministrators.indexOf(admin);
    if (index >= 0) {
      this.contentAdministrators.splice(index, 1);
    }
  }



  saveOrUpdate() {
    var newUpdatedCP = {
      id: this.cp.id,
      name: this.cpForm.get("cpname").value,
      logoUrl: this.cpForm.get("logoUrl").value,
      contentAdministrators: this.contentAdministrators
    }
    if(newUpdatedCP.id) {
      this.contentProviderService.editContentProvider(newUpdatedCP).subscribe(res => {
        if(res) {
          this.showSuccess("Update Successful");
        }
        this.onCPUpdateOrCreate.emit("Content Provider Updated");
      },
      err => {
        this.showError(err);
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