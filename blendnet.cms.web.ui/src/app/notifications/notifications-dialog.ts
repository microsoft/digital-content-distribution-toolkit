import { Component, EventEmitter, Inject, Output, OnInit, Input } from "@angular/core";
import { FormControl, FormGroup, Validators } from "@angular/forms";
import { MatDialogRef, MAT_DIALOG_DATA } from "@angular/material/dialog";
import { ToastrService } from "ngx-toastr";
import { DialogData } from "../broadcast/broadcast.component";
import {MatChipInputEvent} from '@angular/material/chips';
import {COMMA, ENTER} from '@angular/cdk/keycodes';
import { NotificationService } from "../services/notification.service";
import { lengthConstants } from "../constants/length-constants";
import { CustomValidator } from "../custom-validator/custom-validator";

@Component({
    selector: 'app-notifications-dialog',
    templateUrl: 'notifications-dialog.html',
    styleUrls: ['./notifications.component.css']
  })
  
  export class NotificationsDialog implements OnInit {
    notif:string ="";
    
    notifFormGroup: FormGroup;
    isEditable = false;
    @Input() scheduleChecked = true;
    tags: Array<string> = [];

    selectable = true;
    removable = true;
    addOnBlur = true;
    readonly separatorKeysCodes = [ENTER, COMMA] as const;
    @Output() onNotificationBroadcast = new EventEmitter<any>();

    constructor(
        public dialogRef: MatDialogRef<NotificationsDialog>,
        @Inject(MAT_DIALOG_DATA) public data: DialogData,
        private toastr: ToastrService,
        private notificationService: NotificationService
    ) {
  
      }
  
    ngOnInit() {
      this.notifFormGroup = new FormGroup({
        title :  new FormControl('', [Validators.maxLength(lengthConstants.titleMaxLength),
          Validators.minLength(lengthConstants.titleMinLength), 
          CustomValidator.alphaNumericSplChar]),
        body : new FormControl('', [Validators.maxLength(lengthConstants.longDescriptionMaxLength),
          Validators.minLength(lengthConstants.longDescriptionMinLength),
          CustomValidator.alphaNumericSplChar]),
        attachmentUrl :  new FormControl(''),
        // type :  new FormControl(0),
        // topic: new FormControl('', [Validators.required]),
        tags: new FormControl('', [Validators.required]),
      })

    }

    onNoClick(): void {
      this.dialogRef.close();
    }
  
    
    public errorHandling = (control: string, error: string) => {
        return this.notifFormGroup.controls[control].hasError(error);
      }

      add(event: MatChipInputEvent): void {
        const value = (event.value || '').trim();
        if (value) {
          this.tags.push(value);
        }
        // Clear the input value
        event.input.value='';
        // this.notifFormGroup.value.push(value);
        // this.courseIds.updateValueAndValidity();
      }
    
      remove(tag: string): void {
        const index = this.tags.indexOf(tag);
    
        if (index >= 0) {
          this.tags.splice(index, 1);
        }
      }

      createNotif() {
        var notif = this.getNotifDetails();
        this.notificationService.sendBroadcast(notif).subscribe(
          res => this.onNotificationBroadcast.emit("Notification broadcast successfully!"),
          err => this.toastr.error(err)
        );
    
      }

      getNotifDetails() {
        var notification = 
          {
            "title": this.notifFormGroup.get('title').value,
            "body": this.notifFormGroup.get('body').value,
            "attachmentUrl": this.notifFormGroup.get('attachmentUrl').value,
            "type": 1,
            "topic": "mishtu_entertainment",
            "tags": this.tags?.join()
          }
        return notification;

      }

   
  }
