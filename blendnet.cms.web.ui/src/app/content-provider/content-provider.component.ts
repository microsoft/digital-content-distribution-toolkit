import { Component, Inject, OnInit } from '@angular/core';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { AddContentProviderComponent } from '../add-content-provider/add-content-provider.component';
import { Contentprovider } from '../models/contentprovider.model';
import { ContentProviderService } from '../services/content-provider.service';
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

  
  constructor(public dialog: MatDialog,
    public contentProviderService: ContentProviderService) { 
  }

  ngOnInit(): void {
    this.getContentProviders();
  }

  getContentProviders(): void {
    this.contentProviderService.getContentProviders()
      .subscribe(cps => {
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
      activationDate: null,
      deactivationDate: null,
      isActive: false,
      admins: []
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

  }



@Component({
  selector: 'cp-delete-confirm-dialog',
  templateUrl: 'cp-delete-confirm-dialog.html',
  styleUrls: ['./content-provider.component.css']
})
export class CPDeleteConfirmDialog {

  constructor(
    public dialogRef: MatDialogRef<CPDeleteConfirmDialog>,
    @Inject(MAT_DIALOG_DATA) public data: any,
    public contentProviderService: ContentProviderService
    ) {}

  onCancelDelete(): void {
    this.dialogRef.close();
  }

  onConfirmDelete() {
    console.log("Delete CP is called !!");
    return this.contentProviderService.deleteContentProvider(this.data.cpId);
  }

}



