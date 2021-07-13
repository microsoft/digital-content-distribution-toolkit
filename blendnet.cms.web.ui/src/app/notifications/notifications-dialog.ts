import { Component, EventEmitter, Inject, Output, OnInit, Input } from "@angular/core";
import { FormControl, FormGroup, Validators, FormBuilder } from "@angular/forms";
import { MatDialogRef, MAT_DIALOG_DATA } from "@angular/material/dialog";
import { ToastrService } from "ngx-toastr";
import { DialogData } from "../broadcast/broadcast.component";
import { Time } from "@angular/common";
import { SubscriptionService } from '../services/subscription.service';
import { UploaderService } from "../services/uploader.service";


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
    fileUploadError: string ="";
    file = File;
    progress: number;
    infoMessage: any;
    isUploading: boolean = false;
    subForm: FormGroup;
    firstFormGroup: FormGroup;
    secondFormGroup: FormGroup;
    isEditable = false;
    @Input() scheduleChecked = true;

    imageUrl: string | ArrayBuffer = "https://th.bing.com/th/id/OIP.xtB199njiKybNeU8z0bSWAHaCy?pid=ImgDet&rs=1"
    //"https://th.bing.com/th/id/OIP.MRd8X-X-GRdY3tcOKTLDEwHaHa?pid=ImgDet&rs=1";
    fileName: string = "No file selected";

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
      this.subForm = new FormGroup({
        title :  new FormControl('', [Validators.required]),
        text : new FormControl('')
      }),
      
      this.firstFormGroup = this._formBuilder.group({
        //firstCtrl: ['', Validators.required],
        title :  new FormControl('', [Validators.required]),
        text : new FormControl('')
      }),

      this.secondFormGroup = this._formBuilder.group({
        sendDate : new FormControl(null, [Validators.required]),
        sendTime : new FormControl(null, [Validators.required])
      }),

      this.uploader.progressSource.subscribe(progress => {
        this.progress = progress;
      });

    }

    onNoClick(): void {
      this.dialogRef.close();
    }
  
    
    public errorHandling = (control: string, error: string) => {
        return this.firstFormGroup.controls[control].hasError(error);
      }

    onChange(file: File) {
      if (file) {
        this.fileName = file.name;
        this.file = File;
  
        const reader = new FileReader();
        reader.readAsDataURL(file);
  
        reader.onload = event => {
          this.imageUrl = reader.result;
        };
      }
    }
  
    onUpload(file: File) {
      this.infoMessage = null;
      this.progress = 0;
      this.isUploading = true;
      this.file = File;
  
      this.uploader.upload(file).subscribe(message => {
        this.isUploading = false;
        this.infoMessage = message;
      });
    }
   
  }
