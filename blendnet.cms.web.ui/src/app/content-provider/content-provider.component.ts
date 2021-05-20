import { Component, Inject, OnInit, Output, EventEmitter } from '@angular/core';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { AddContentProviderComponent } from '../add-content-provider/add-content-provider.component';
import { Contentprovider } from '../models/contentprovider.model';
import { ContentProviderService } from '../services/content-provider.service';
import { ToastrService } from 'ngx-toastr';
import { Observable } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { of } from 'rxjs';
import { CommonDialogComponent } from '../common-dialog/common-dialog.component';

export interface DialogData {
  message: string;
}
@Component({
  selector: 'app-content-provider',
  templateUrl: './content-provider.component.html',
  styleUrls: ['./content-provider.component.css']
})

export class ContentProviderComponent implements OnInit {
  cps$: Observable<Contentprovider[]>;
  deleteMessage: string = "Please press OK to continue.";
  selectedCP: Contentprovider;
  errorMsg: string;

  constructor(public dialog: MatDialog,
    public contentProviderService: ContentProviderService,
    private toastr: ToastrService) { 
  }

  ngOnInit(): void {
    this.getContentProviders();
  }

  getContentProviders(): void {
    this.cps$ = this.contentProviderService.getContentProviders()
    .pipe(map( cps => {
      if(cps.length >= 1 && (!localStorage.getItem("contentProviderId") || 
        !localStorage.getItem("contentProviderName"))) {
          console.log("Setting the default Content Provider " + cps[0].name);
          this.selectedCP = cps[0];
          this.contentProviderService.changeDefaultCP(this.selectedCP);
          localStorage.setItem("contentProviderId", this.selectedCP.id);
          localStorage.setItem("contentProviderName", this.selectedCP.name);
        }
      return this.createCPList(cps);
    }))
    .pipe(
      catchError(error => {
        this.toastr.error(error);        
        return of([]);
      })
    )
  }

  createCPList(cps) {
    var cpList = [];
    console.log("Create CP list call");
    cps.forEach(cp => {
      // TODO: uncomment when proper data starts flowing in
      //  if (!cp.logoUrl || cp.logoUrl !== "") {
      //    this.cps.push(cp);
      //  } else {
         console.log("changing to default logo");
         //cp.logoUrl = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQJhjsbdgpgib0753nQenK7-PNPWdbaa4Xjrw&usqp=CAU";
         cp.logoUrl = "data:image/png;base64,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";
         cpList.push(cp);
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
    cpList.unshift(emptyCP);
    return cpList;
;
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
      this.cps$ = of([]);
      this.getContentProviders();
      dialogRef.close();
    })
    dialogRef.afterClosed().subscribe(result => {
      console.log('The dialog was closed');
    });
  }

  openEditConfirmModal(selectedCp, edit = true): void {
    const heading = edit ? 'Edit ': 'Add '
    const dialogRef = this.dialog.open(AddContentProviderComponent, {
      width: '60%',disableClose: true,
      data: {cp: selectedCp, heading: heading + 'Content Provider'}
    });
  
    dialogRef.componentInstance.onCPUpdateOrCreate.subscribe(data => {
      this.cps$ = of([]);
      this.getContentProviders();
      dialogRef.close();
    })
    dialogRef.afterClosed().subscribe(result => {
      console.log('The dialog was closed');
    });
  }

    openSelectCPModal(selectedCp): void {
      const dialogRef = this.dialog.open(CommonDialogComponent, {
        disableClose: true,
        data: {message: "Please confirm to continue with your selection", heading:'Confirm',
          buttons: this.openSelectCPModalButtons()
        },
      });

      dialogRef.afterClosed().subscribe(result => {
        if (result === 'proceed') {
          console.log('proceed');
          localStorage.setItem("contentProviderId", selectedCp.id);
          localStorage.setItem("contentProviderName", selectedCp.name);
          this.toastr.success("Your have selected " + selectedCp.name);
          this.contentProviderService.changeDefaultCP(selectedCp);
        }
      });

    }

    openSelectCPModalButtons(): Array<any> {
      return [{
        label: 'Cancel',
        type: 'basic',
        value: 'cancel',
        class: 'discard-btn'
      },
      {
        label: 'Continue',
        type: 'primary',
        value: 'submit',
        class: 'update-btn'
      }
      ]
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
