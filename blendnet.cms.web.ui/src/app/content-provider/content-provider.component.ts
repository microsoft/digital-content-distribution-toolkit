import { Component, OnInit} from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
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
  baseHref: string;

  constructor(public dialog: MatDialog,
    public contentProviderService: ContentProviderService,
    private toastr: ToastrService) { 
  }

  ngOnInit(): void {
    this.baseHref = this.contentProviderService.getBaseHref();
    this.getContentProviders();
  }

  getContentProviders(): void {
    this.cps$ = this.contentProviderService.getContentProviders()
    .pipe(map( cps => {
      if(cps.length >= 1 && (!sessionStorage.getItem("contentProviderId") || 
        !sessionStorage.getItem("contentProviderName"))) {
          this.selectedCP = cps[0];
          this.contentProviderService.changeDefaultCP(this.selectedCP);
          sessionStorage.setItem("contentProviderId", this.selectedCP.id);
          sessionStorage.setItem("contentProviderName", this.selectedCP.name);
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
    cps.forEach(cp => {
       if (cp.logoUrl !== "") {
        cpList.push(cp);
       } else {
         cp.logoUrl = "../../" + this.baseHref + "/assets/images/cp-default-logo/cp-default-logo.png"
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

  // openDeleteConfirmModal(selectedCp): void {
  //   const dialogRef = this.dialog.open(CPDeleteConfirmDialog, {
  //     data: {
  //       message: this.deleteMessage,
  //       cpId: selectedCp.id
  //     },
  //     width: '40%'
  //   });
  
  //   dialogRef.componentInstance.onCPDelete.subscribe(data => {
  //     this.cps$ = of([]);
  //     this.getContentProviders();
  //     dialogRef.close();
  //   })
  //   dialogRef.afterClosed().subscribe(result => {
  //     console.log('The dialog was closed');
  //   });
  // }

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



// @Component({
//   selector: 'cp-delete-confirm-dialog',
//   templateUrl: 'cp-delete-confirm-dialog.html',
//   styleUrls: ['./content-provider.component.css']
// })
// export class CPDeleteConfirmDialog {

//   @Output() onCPDelete= new EventEmitter<any>();

//   constructor(
//     public dialogRef: MatDialogRef<CPDeleteConfirmDialog>,
//     @Inject(MAT_DIALOG_DATA) public data: any,
//     public contentProviderService: ContentProviderService,
//     private toastr: ToastrService,
//     ) {}

//   onCancelDelete(): void {
//     this.dialogRef.close();
//   }

//   onConfirmDelete() {
//     console.log("Delete CP is called !!");
//     return this.contentProviderService.deleteContentProvider(this.data.cpId).subscribe(res => {
//       if(res.status === 204) {
//         this.toastr.success("Content Provider deleted successfully!");
//         this.onCPDelete.emit("Content Provider deleted successfully!");
//       } else {
//         this.toastr.error("Error deleting Content Provider. Please try again!")
//         this.onCPDelete.emit("Error deleting Content Provider. Please try again!");
//       }
//     });
//   }

// }
