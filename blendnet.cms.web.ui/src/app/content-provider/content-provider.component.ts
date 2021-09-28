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
import { environment } from 'src/environments/environment';

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
      if(cps.length >= 1 && (!sessionStorage.getItem("contentProviderId") || 
        !sessionStorage.getItem("contentProviderName"))) {
          console.log("Setting the default Content Provider " + cps[0].name);
          this.selectedCP = cps[0];
          this.contentProviderService.changeDefaultCP(this.selectedCP);
          sessionStorage.setItem("contentProviderId", this.selectedCP.id);
          sessionStorage.setItem("contentProviderName", this.selectedCP.name);
        }
      // sessionStorage.setItem("CONTENT_PROVIDERS", JSON.stringify(cps));
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
       if (cp.logoUrl !== "") {
        cpList.push(cp);
       } else {
         console.log("changing to default logo");
         cp.logoUrl = "data:image/jpeg;base64,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"
         cpList.push(cp);
       }
      
    });

    if(sessionStorage.getItem("roles")?.includes(environment.roles.SuperAdmin)) {
      var emptyCP = {
        id: null,
        name: '',
        logoUrl:'',
        contentAdministrators: []
      }
      cpList.unshift(emptyCP);
    }
    return cpList;
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
      width: '40%',
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
        data: {message: "Please confirm to continue with your selection", heading:'Confirm',
          buttons: this.openSelectCPModalButtons()
        },
      });

      dialogRef.afterClosed().subscribe(result => {
        if (result === 'proceed') {
          console.log('proceed');
          sessionStorage.setItem("contentProviderId", selectedCp.id);
          sessionStorage.setItem("contentProviderName", selectedCp.name);
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
