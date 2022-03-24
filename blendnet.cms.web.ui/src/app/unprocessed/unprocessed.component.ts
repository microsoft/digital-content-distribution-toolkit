import { SelectionModel } from '@angular/cdk/collections';
import { Component, Inject, LOCALE_ID, ViewChild} from '@angular/core';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import {MatPaginator} from '@angular/material/paginator';
import {MatSort} from '@angular/material/sort';
import {MatTableDataSource} from '@angular/material/table';
import { environment } from '../../environments/environment';
import { Content } from '../models/content.model';
import { ContentService } from '../services/content.service';
import { of} from 'rxjs';
import {catchError, map, } from 'rxjs/operators';
import {  HttpEventType } from '@angular/common/http';
import { ToastrService } from 'ngx-toastr';
import { ContentStatus } from '../models/content-status.enum';
import { CommonDialogComponent } from '../common-dialog/common-dialog.component';
import { DatePipe } from '@angular/common';
import { ContentView } from '../models/content-view.model';
import { unprocessedContentFilters } from '../constants/content-status-filters';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { lengthConstants } from "../constants/length-constants";
import { CustomValidator } from "../custom-validator/custom-validator";
import { COMMA, ENTER, P } from '@angular/cdk/keycodes';


@Component({
  selector: 'app-unprocessed',
  styleUrls: ['unprocessed.component.css'],
  templateUrl: 'unprocessed.component.html',
})
export class UnprocessedComponent {
  displayedColumns: string[] = ['select', 'title', 'status', 'createdDate', 'modifiedDate', 'view', 'edit', 'isDeletable', 'isProcessable'];
  dataSource: MatTableDataSource<ContentView>;
  fileUploadError: string ="";
  showDialog: boolean = false;
  deleteConfirmMessage: string = "Content once deleted can not be restored. Please press Continue to begin the deletion.";
  processConfirmMessage: string = "Please press Continue to begin the transformation.";
  initialSelection = [];
  allowMultiSelect = true;
  selection = new SelectionModel<Content>(this.allowMultiSelect, this.initialSelection);
  file;
  selectedContents: number =0;
  allowedMaxSelection: number = environment.allowedMaxSelection;
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  @ViewChild('jsonFileInput') jsonFileInput;
  filterValue;
  myfilename = 'Choose a meta-data file to upload content';
  errMessage;
  error= false;
  fileName = '';
  pipe;
  contentList: ContentView[] = [];

  constructor(public dialog: MatDialog,
    public contentService: ContentService,
    private toastr: ToastrService,
    @Inject(LOCALE_ID) locale: string
    ) {
      this.pipe = new DatePipe(locale);
  }

  

  ngOnInit(): void {
    this.getUnprocessedContent();
  }

  refreshPage() {
    this.filterValue = '';
    this.getUnprocessedContent();
  }

  getUnprocessedContent() { 
    this.contentService.getContentByCpIdAndFilters(unprocessedContentFilters).subscribe(
      res => {
        this.contentList = res;
        this.dataSource = this.createDataSource(res);
        this.dataSource.paginator = this.paginator;
        this.dataSource.sort = this.sort;
        this.selectedContents = 0;
      },
      err => {
        this.error = true;
        this.dataSource = this.createDataSource([]);
        if(err === 'Not Found') {
          this.errMessage = "No data found";
        } else {
          this.toastr.error(err);
          this.errMessage = err;
        }
      });
  }

  toggleSelection(event, row) {
    if(event.checked){
        this.selectedContents++;
        row.isSelected = true;
      }else{
        this.selectedContents--;
        row.isSelected = false;
      }
  }

  allowSelection(row) {
    return (!row.isSelected && this.selectedContents >= this.allowedMaxSelection);
  }

  createDataSource(rawData: ContentView[]) {
    var dataSource: ContentView[] =[];
    if(rawData && rawData.length > 0) {
      this.errMessage = "";
      this.error = false;
      rawData.forEach( data => {
        data.displayStatus = data.contentTransformStatus !== ContentStatus.TRANSFORM_NOT_INITIALIZED ? 
        data.contentTransformStatus : data.contentUploadStatus;
        data.isSelected = false;
        data.displayCreatedDate =  this.pipe.transform(data.createdDate, 'short');
        data.displayModifiedDate=  this.pipe.transform(data.modifiedDate, 'short');
        dataSource.push(data);
      });
    } else {
      this.errMessage = "No data found";
      this.error = true;
    }
    return new MatTableDataSource(dataSource);
  }

  isContentNotProcessable(row) {
    return row.contentUploadStatus !== ContentStatus.UPLOAD_COMPLETE
    || (row.contentTransformStatus !== ContentStatus.TRANSFORM_NOT_INITIALIZED && row.contentTransformStatus !== ContentStatus.TRANSFORM_FAILED);
  }

  isContentNotDeletable(row) {
    return (row.contentUploadStatus !== ContentStatus.UPLOAD_COMPLETE
    && row.contentUploadStatus !== ContentStatus.UPLOAD_FAILED)
    || (row.contentTransformStatus !== ContentStatus.TRANSFORM_NOT_INITIALIZED);  
  }


  applyFilter() { 
    this.dataSource.filter = this.filterValue.trim().toLowerCase();

    if (this.dataSource.paginator) {
      this.dataSource.paginator.firstPage();
    }
  }

  editContent(id): void {
    this.contentService.getContentById(id).subscribe(
      res => {
        const dialogRef = this.dialog.open(EditContentMetadataDialog, {
          data: {content: res}
        });
      
        dialogRef.afterClosed().subscribe(result => {
          console.log('The dialog was closed');
        });
      },
      err => {
        this.toastr.error(err);
      });
   }
  

  uploadFile(file) {
    const formData = new FormData(); 
    formData.append('file', file.data);  
    file.inProgress = true;
    this.contentService.uploadContent(formData).pipe(
      map(event => {
        switch (event.type) {
          case HttpEventType.UploadProgress:
            file.progress = Math.round(event.loaded * 100 / event.total);
            break;
          case HttpEventType.Response:
            return event;
        }  
      }),  
      catchError((error) => {
        file.inProgress = false;
        this.toastr.error(error);
        // this.fileName ='';
        return of(`Upload failed: ${file.data.name}`);
      })).subscribe((event: any) => {
        if (typeof (event) === 'object') {
          this.toastr.success(file.data.name + " Content uploaded successfully.");
          this.getUnprocessedContent();
          this.fileName ='';
        }  
      });  
      this.jsonFileInput.nativeElement.value = '';
     
  }

  onFileSelected(event) {

    this.fileUploadError = null;
    if (event.target.files && event.target.files[0]) {
      if(event.target.files[0].size > environment.maxFileUploadSize) {
        this.fileUploadError="Max file size allowed is : " +  environment.maxFileUploadSize;
        return false;
      }
      if(event.target.files[0].type !== environment.fileAllowedType) {
        this.fileUploadError="Only " + environment.fileAllowedType + " files are allowed";
        return false;
      }
      this.fileName = event.target.files[0].name;
      this.file = {
        data: event.target.files[0], 
        inProgress: false, 
        progress: 0
      }
      this.uploadFile(this.file);
      }
  }

openProcessConfirmModal(row): void {
  var rows = [];
  if(row) {
    rows.push(row);
    this.openProcessDialog(rows);
  } else {
    rows = this.dataSource ? this.dataSource.data.filter(d => d.isSelected) : [];
    if(rows.length <1) {
      this.toastr.warning("Please select one or more content/s for processing");
    } else {
      var notEligibleForTransform = rows.filter(d => 
        (d.contentUploadStatus !== ContentStatus.UPLOAD_COMPLETE 
        || (d.contentTransformStatus !== ContentStatus.TRANSFORM_NOT_INITIALIZED
        && d.contentTransformStatus!== ContentStatus.TRANSFORM_FAILED)));
        if(notEligibleForTransform.length > 0) {
          this.toastr.warning("One or more selected content/s cannot be processed");
        } else {
            this.openProcessDialog(rows);          
        }
    }    
  }
}

openProcessDialog(rows) {
  const dialogRef = this.dialog.open(CommonDialogComponent, {
    data: {
      heading: 'Confirm',
      message: this.processConfirmMessage,
      contents: rows,
      action: "PROCESS",
      buttons: this.openSelectCPModalButtons()
    },
    maxHeight: '400px'
  });


  dialogRef.afterClosed().subscribe(result => {
    if (result === 'proceed') {
      this.onConfirmProcess(rows);
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

onConfirmProcess(contents): void {
  var selectedIds = contents.map(content => 
    {return content.id});
  var contentIds ={
    "contentIds": selectedIds
  }
  this.contentService.processContent(contentIds).subscribe(
    res => //this.toastr.success("Content/s submitted for transformation sucessfully!!"),
    this.successEmit(res),
    err => this.toastr.error(err));
}

successEmit(res) {
  if(res.length == 0) {
    this.toastr.success("Content/s submitted successfully for transformation");
  } else {
    this.toastr.warning(res[0]);
  }
  this.getUnprocessedContent();
}

openDeleteConfirmModal(row): void {
  const dialogRef = this.dialog.open(CommonDialogComponent, {
    data: {
      heading: 'Confirm',
      message: this.deleteConfirmMessage,
      action: "DELETE",
      contents: row,
      buttons: this.openSelectCPModalButtons()
    },
    maxWidth: '400px'
  });

  dialogRef.afterClosed().subscribe(result => {
    if (result === 'proceed') {
      this.onConfirmDelete(row.id);
    }
  });

}

  onConfirmDelete(contentId): void {
    this.contentService.deleteContent(contentId).subscribe(
      res => this.onDeleteSuccess(),
      err => this.toastr.error(err));
  }

  onDeleteSuccess() {
    this.toastr.success("Content submitted for deletion successfully");
    this.getUnprocessedContent();
  }

viewContent(id) : void {
  this.contentService.getContentById(id).subscribe(
    res => {
      const dialogRef = this.dialog.open(ContentDetailsDialog, {
        data: {content: res}
      });
    
      dialogRef.afterClosed().subscribe(result => {
        console.log('The dialog was closed');
      });
    },
    err => {
      this.toastr.error(err);
    });
 }
}

@Component({
  selector: 'content-detail-dialog',
  templateUrl: 'content-detail-dialog.html',
  styleUrls: ['unprocessed.component.css']
})
export class ContentDetailsDialog {

  constructor(
    public dialogRef: MatDialogRef<ContentDetailsDialog>,
    @Inject(MAT_DIALOG_DATA) public data: any) {}
    content: Content;
    people: string = '';
    attachments: string = '';

  ngOnInit(): void {
    this.content = this.data.content;
    this.data.content.people.forEach(each => 
      this.people += each.role+ " : " +each.name + " | "
    );
    this.data.content.attachments.filter( each => {
      this.attachments +=  each.name + ' ';
    })
  }


}


@Component({
  selector: 'edit-content-metadata',
  templateUrl: 'edit-content-metadata.html',
  styleUrls: ['unprocessed.component.css']
})
export class EditContentMetadataDialog {

  content: Content;
    attachments: string = '';
    metadataForm: FormGroup;
    genres: Array<string> = [];
    peopleType: Array<string> = [];
    peopleList: Array<string> = [];
    peoples: Array<any> = [];
    selectable = true;
    removable = true;
    addOnBlur = true;
    readonly separatorKeysCodes = [ENTER, COMMA] as const;


  constructor(
    public dialogRef: MatDialogRef<EditContentMetadataDialog>,
    @Inject(MAT_DIALOG_DATA) public data: any,
    private contentService: ContentService,
    private toastr: ToastrService) {
      this.genres = environment.genres;
      this.peopleType = environment.peopleType;
    }
    


  ngOnInit(): void {
    this.content = this.data.content;
    this.data.content.people.filter(each =>  {
      this.peopleList.push(each.role + " | " + each.name);
    }      
    );
    this.data.content.attachments.filter( each => {
      this.attachments +=  each.name + ' ';
    })
    var isHeaderContent = this.data.content.isHeaderContent ? "Yes": "No";
    var isFreeContent = this.data.content.isFreeContent ? "Yes": "No";
  
    this.metadataForm = new FormGroup({
      contentId :  new FormControl({value: this.data.content.contentId, disabled: true}),
      contentProviderContentId :  new FormControl({value:this.data.content.contentProviderContentId, disabled: true}),
      shortDescription :  new FormControl(this.data.content.shortDescription,  [ Validators.maxLength(lengthConstants.shortDescriptionMaxLength), 
        Validators.minLength(lengthConstants.shortDescriptionMinLength),
        CustomValidator.alphaNumericSplChar]),
      longDescription :  new FormControl(this.data.content.longDescription,  [ Validators.maxLength(lengthConstants.longDescriptionMaxLength), 
        Validators.minLength(lengthConstants.longDescriptionMinLength),
        CustomValidator.alphaNumericSplChar]),
      additionalDescription1 : new FormControl(this.data.content.additionalDescription1,  [ Validators.maxLength(lengthConstants.additionalDescriptionMaxLength), 
        Validators.minLength(lengthConstants.additionalDescriptionMinLength),
        CustomValidator.alphaNumericSplChar]),
      additionalDescription2 : new FormControl(this.data.content.additionalDescription2,  [ Validators.maxLength(lengthConstants.additionalDescriptionMaxLength), 
        Validators.minLength(lengthConstants.additionalDescriptionMinLength),
        CustomValidator.alphaNumericSplChar]),
      genre :  new FormControl(this.data.content.genre),
      yearOfRelease :  new FormControl(this.data.content.yearOfRelease),
      language :  new FormControl(this.data.content.language),
      durationInMts :  new FormControl(this.data.content.durationInMts),
      rating :  new FormControl(this.data.content.rating),
      mediaFileName :  new FormControl({value: this.data.content.mediaFileName, disabled: true}),
      hierarchy :  new FormControl(this.data.content.hierarchy),
      isHeaderContent :  new FormControl(isHeaderContent),
      isFreeContent :  new FormControl(isFreeContent),
      attachments :  new FormControl({value: this.attachments, disabled: true}),
      ageAppropriateness :  new FormControl(this.data.content.ageAppropriateness),
      contentAdvisory:  new FormControl(this.data.content.contentAdvisory),
      people:new FormControl('Actor'),
      name: new FormControl('', [ Validators.maxLength(lengthConstants.titleMaxLength), 
        Validators.minLength(lengthConstants.titleMinLength),
        CustomValidator.alphaNumericSplChar])
      });
  }


  addPeople() {
    if(this.metadataForm.get('name').value.trim() != "") {
      this.peopleList.push(this.metadataForm.get('people').value + " | " +this.metadataForm.get('name').value);
      this.metadataForm.get('name').setValue('');
    }
    
  }

  removePerson(person: string) {
    const index = this.peopleList.indexOf(person);
    if (index >= 0) {
      this.peopleList.splice(index, 1);
    }
  }


  updateMetadata() {
    this.contentService.updateMetaData(this.data.content.contentId, this.getUpdateMetadata()).subscribe(res => {
      this.toastr.success("Content metadata updated successfully!!")
    },
    err=> this.toastr.error(err)); 
  }

  getUpdateMetadata() {
    this.peopleList.forEach(p =>  {
      var token = p.split(" | ");
      this.peoples.push({"role": token[0], "name": token[1]});
    });

    var contentMetadata = this.data.content;
    contentMetadata.shortDescription = this.metadataForm.get('shortDescription').value;
    contentMetadata.longDescription = this.metadataForm.get('longDescription').value;
    contentMetadata.additionalDescription1 = this.metadataForm.get('additionalDescription1').value;
    contentMetadata.additionalDescription2 = this.metadataForm.get('additionalDescription2').value;
    contentMetadata.genre = this.metadataForm.get('genre').value;
    contentMetadata.yearOfRelease = this.metadataForm.get('yearOfRelease').value;
    contentMetadata.language = this.metadataForm.get('language').value;
    contentMetadata.durationInMts = this.metadataForm.get('durationInMts').value;
    contentMetadata.rating = this.metadataForm.get('rating').value;
    contentMetadata.isHeaderContent = this.metadataForm.get('isHeaderContent').value === "Yes";
    contentMetadata.isFreeContent = this.metadataForm.get('isFreeContent').value === "Yes";
    contentMetadata.ageAppropriateness = this.metadataForm.get('ageAppropriateness').value;
    contentMetadata.contentAdvisory = this.metadataForm.get('contentAdvisory').value;
    contentMetadata.people = this.peoples;
    return contentMetadata;

  }
}



