import { Component, EventEmitter, Inject, Output, OnInit, Input } from "@angular/core";
import { FormControl, FormGroup, Validators, FormBuilder } from "@angular/forms";
import { MatDialogRef, MAT_DIALOG_DATA } from "@angular/material/dialog";
import { ToastrService } from "ngx-toastr";
import { DialogData } from "../broadcast/broadcast.component";
import { Time } from "@angular/common";
import { SubscriptionService } from '../services/subscription.service';
import { UploaderService } from "../services/uploader.service";
import {MatChipInputEvent} from '@angular/material/chips';
import {COMMA, ENTER} from '@angular/cdk/keycodes';

export interface Fruit {
  name: string;
}

@Component({
    selector: 'app-notifications-dialog',
    templateUrl: 'notifications-dialog.html',
    styleUrls: ['./notifications.component.css']
  })
  
  export class NotificationsDialog implements OnInit {
    notif:string ="";
    expiresIn:string= "";
    title;
    text;
    sendDate; 
    sendTime;
    minStart: Date;
    minTime: Time;
    @Output() onNotifCreate = new EventEmitter<any>();
    // fileUploadError: string ="";
    file = File;
    progress: number;
    infoMessage: any;
    isUploading: boolean = false;
    subForm: FormGroup;
    notifFormGroup: FormGroup;
    // secondFormGroup: FormGroup;
    isEditable = false;
    @Input() scheduleChecked = true;
    tags: Array<string> = [];
    imageUrl: string | ArrayBuffer = "https://th.bing.com/th/id/OIP.xtB199njiKybNeU8z0bSWAHaCy?pid=ImgDet&rs=1"
    //"https://th.bing.com/th/id/OIP.MRd8X-X-GRdY3tcOKTLDEwHaHa?pid=ImgDet&rs=1";
    // fileName: string = "No file selected";
    selectable = true;
    removable = true;
    addOnBlur = true;
    readonly separatorKeysCodes = [ENTER, COMMA] as const;
    constructor(
        public uploader: UploaderService,
        public dialogRef: MatDialogRef<NotificationsDialog>,
        @Inject(MAT_DIALOG_DATA) public data: DialogData,
        private toastr: ToastrService,
        private _formBuilder: FormBuilder,
        private subscriptionService: SubscriptionService
    ) {
        const currentYear = new Date().getFullYear();
        const currentMonth = new Date().getMonth();
        const currentDay = new Date().getDate();
        this.minStart = new Date(currentYear, currentMonth, currentDay);
  
      }
  
    ngOnInit() {
      this.notifFormGroup = new FormGroup({
        title :  new FormControl('', [Validators.required]),
        body : new FormControl('', [Validators.required]),
        attachmentUrl :  new FormControl('', [Validators.required]),
        type :  new FormControl(0),
        topic: new FormControl('', [Validators.required]),
        tags: new FormControl(''),
      }),

      // this.secondFormGroup = this._formBuilder.group({
      //   sendDate : new FormControl(null, [Validators.required]),
      //   sendTime : new FormControl(null, [Validators.required])
      // }),

      this.uploader.progressSource.subscribe(progress => {
        this.progress = progress;
      });

    }

    onNoClick(): void {
      this.dialogRef.close();
    }
  
    
    public errorHandling = (control: string, error: string) => {
        return this.notifFormGroup.controls[control].hasError(error);
      }

      add(event: MatChipInputEvent): void {
        const value = (event.value || '').trim();
    
        // Add our fruit
        if (value) {
          this.tags.push(value);
        }
    
        // Clear the input value
        event.input.value='';
        // this.notifFormGroup.value.push(value);
        // this.courseIds.updateValueAndValidity();
        console.log(this.tags);
      }
    
      remove(tag: string): void {
        const index = this.tags.indexOf(tag);
    
        if (index >= 0) {
          this.tags.splice(index, 1);
        }
      }

      createNotif() {
        console.log(
        this.notifFormGroup.value);
        Object.keys(this.notifFormGroup.value).forEach(data => {
          console.log(data);
        });
      }

    // onChange(file: File) {
    //   if (file) {
    //     this.fileName = file.name;
    //     this.file = File;
  
    //     const reader = new FileReader();
    //     reader.readAsDataURL(file);
  
    //     reader.onload = event => {
    //       this.imageUrl = reader.result;
    //     };
    //   }
    // }
  
    // onUpload(file: File) {
    //   this.infoMessage = null;
    //   this.progress = 0;
    //   this.isUploading = true;
    //   this.file = File;
  
    //   this.uploader.upload(file).subscribe(message => {
    //     this.isUploading = false;
    //     this.infoMessage = message;
    //   });
    // }
   
  }
