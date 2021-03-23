import { Component, Inject, OnInit, Output, EventEmitter } from '@angular/core';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { AddContentProviderComponent } from '../add-content-provider/add-content-provider.component';
import { Contentprovider } from '../models/contentprovider.model';
import { ContentProviderService } from '../services/content-provider.service';
import { ToastrService } from 'ngx-toastr';

export interface DialogData {
  message: string;
}
@Component({
  selector: 'app-content-provider',
  templateUrl: './content-provider.component.html',
  styleUrls: ['./content-provider.component.css']
})

export class ContentProviderComponent implements OnInit {
  cps: Contentprovider[] = [];
  deleteMessage: string = "Please press OK to continue.";
  data: Contentprovider;
  
  constructor(public dialog: MatDialog,
    public contentProviderService: ContentProviderService) { 
  }

  ngOnInit(): void {
    this.getContentProviders();
  }

  getContentProviders(): void {
    this.contentProviderService.getContentProviders()
      .subscribe(cps => {
        if(!localStorage.getItem("contentProviderId") || 
            !localStorage.getItem("contentProviderName")) {
              this.data = cps[0];
              this.contentProviderService.changeDefaultCP(this.data);
              this.contentProviderService.data$.subscribe(res => this.data = res);
            }
        this.createCPList(cps);
      });
  }

  createCPList(cps) {
    cps.forEach(cp => {
      // TODO: uncomment when proper data starts flowing in
      //  if (!cp.logoUrl || cp.logoUrl !== "") {
      //    this.cps.push(cp);
      //  } else {
         console.log("changing to default logo");
         cp.logoUrl = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQJhjsbdgpgib0753nQenK7-PNPWdbaa4Xjrw&usqp=CAU";
         this.cps.push(cp);
      //  }
      
    });
    var emptyCP = {
      id: null,
      name: '',
      logoUrl:'',
      // activationDate: null,
      // deactivationDate: null,
      // isActive: false,
      contentAdministrators: []
    }
    this.cps.unshift(emptyCP);
  }

  openDeleteConfirmModal(selectedCp): void {
    const dialogRef = this.dialog.open(CPDeleteConfirmDialog, {
      data: {
        message: this.deleteMessage,
        cpId: selectedCp.id
      },
      width: '40%'
    });
  
    dialogRef.componentInstance.onCPDelete.subscribe(data => {
      this.cps = [];
      this.getContentProviders();
      dialogRef.close();
    })
    dialogRef.afterClosed().subscribe(result => {
      console.log('The dialog was closed');
    });
  }

  openEditConfirmModal(selectedCp): void {
    const dialogRef = this.dialog.open(AddContentProviderComponent, {
      width: '60%',disableClose: true,
      data: {cp: selectedCp}
    });
  
    dialogRef.componentInstance.onCPUpdateOrCreate.subscribe(data => {
      this.cps = [];
      this.getContentProviders();
      dialogRef.close();
    })
    dialogRef.afterClosed().subscribe(result => {
      console.log('The dialog was closed');
    });
  }

  openSelectCPModal(selectedCp): void {
    const dialogRef = this.dialog.open(CPSelectConfirmDialog, {
      width: '60%',disableClose: true,
      data: {cp: selectedCp, message: "Please confirm to continue with your selection"}
    });
  
    dialogRef.componentInstance.onCPSelect.subscribe(data => {
      this.contentProviderService.changeDefaultCP(data);
      dialogRef.close();
    })
    dialogRef.afterClosed().subscribe(result => {
      console.log('The dialog was closed');
    });
  }

  }



@Component({
  selector: 'cp-delete-confirm-dialog',
  templateUrl: 'cp-delete-confirm-dialog.html',
  styleUrls: ['./content-provider.component.css']
})
export class CPDeleteConfirmDialog {

  @Output() onCPDelete= new EventEmitter<any>();

  constructor(
    public dialogRef: MatDialogRef<CPDeleteConfirmDialog>,
    @Inject(MAT_DIALOG_DATA) public data: any,
    public contentProviderService: ContentProviderService,
    private toastr: ToastrService,
    ) {}

  onCancelDelete(): void {
    this.dialogRef.close();
  }

  onConfirmDelete() {
    console.log("Delete CP is called !!");
    return this.contentProviderService.deleteContentProvider(this.data.cpId).subscribe(res => {
      if(res.status === 204) {
        this.toastr.success("Content Provider deleted successfully!");
        this.onCPDelete.emit("Content Provider deleted successfully!");
      } else {
        this.toastr.error("Error deleting Content Provider. Please try again!")
        this.onCPDelete.emit("Error deleting Content Provider. Please try again!");
      }
    });
  }

}



@Component({
  selector: 'cp-select-confirm-dialog',
  templateUrl: 'cp-select-confirm-dialog.html',
  styleUrls: ['./content-provider.component.css']
})
export class CPSelectConfirmDialog {

  @Output() onCPSelect= new EventEmitter<any>();

  constructor(
    public dialogRef: MatDialogRef<CPSelectConfirmDialog>,
    @Inject(MAT_DIALOG_DATA) public data: any,
    // public contentProviderService: ContentProviderService,
    private toastr: ToastrService,
    ) {}

    onCancelSelect(): void {
    this.dialogRef.close();
  }

  onConfirmSelect() {
    console.log("Select a CP is called !!");
    localStorage.setItem("contentProviderId", this.data.cp.id);
    localStorage.setItem("contentProviderName", this.data.cp.name);
    this.toastr.success("Your have selected " + this.data.cp.name);
    this.onCPSelect.emit(this.data.cp);
  }

}




