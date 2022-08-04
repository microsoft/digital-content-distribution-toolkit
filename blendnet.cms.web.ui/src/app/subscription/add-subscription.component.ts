import {Component, Input, OnInit, Output, EventEmitter, ViewChild, Inject, LOCALE_ID} from '@angular/core';
import { FormGroup, FormBuilder, FormControl, Validators } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
import { MatTableDataSource } from '@angular/material/table';
import { ToastrService } from 'ngx-toastr';
import { CommonDialogComponent } from '../common-dialog/common-dialog.component';
import { MatSort } from '@angular/material/sort';
import { lengthConstants } from '../constants/length-constants';
import { CustomValidator } from '../custom-validator/custom-validator';
import { ContentService } from '../services/content.service';
import { ContentDetailsDialog } from '../unprocessed/unprocessed.component';
import { SubscriptionService } from '../services/subscription.service';
import { broadcastCompleteContentFilters } from '../constants/content-status-filters';
import { ContentView } from '../models/content-view.model';
import { AddContentListDialog } from './add-content-list-dialog';
import { DatePipe } from '@angular/common';

@Component({
    selector: 'app-add-subscription-page',
    templateUrl: './add-subscription.component.html',
    styleUrls: ['./add-subscription.component.css']
})
export class AddSubscriptionComponent implements OnInit {

    @Input() subscriptionPlan: any;
    @Output() newSubscriptionEvent = new EventEmitter<any>();
    subscriptionFormGroup: FormGroup;
    isPublished = false;
    isDraft = false;
    dataSource: MatTableDataSource<ContentView>;
    @ViewChild(MatPaginator) paginator: MatPaginator;
    @ViewChild(MatSort) sort: MatSort;
    minStart: Date;
    minEnd: Date;
    displayedColumns: string[] = ['title', 'status', 'broadcastStartDate', 'broadcastEndDate', 'view', 'delete'];
    errMessage;
    error = false;
    pipe;

    constructor(
        public dialog: MatDialog,
        private formBuilder: FormBuilder,
        public contentService: ContentService,
        public toastr: ToastrService,
        private subscriptionService: SubscriptionService,
        @Inject(LOCALE_ID) locale: string,
    )
    {
        this.minStart = new Date();
        this.minEnd  = new Date();
        this.pipe = new DatePipe(locale);
    }

    ngOnInit(): void {
        this.createEmptyFormDraft("SVOD");
        this.initializeValue();
    }

    createEmptyFormDraft(subscriptionType) {
        this.subscriptionFormGroup = this.formBuilder.group({
            type: new FormControl(subscriptionType),
            name :  new FormControl('', [Validators.required, 
                Validators.maxLength(lengthConstants.titleMaxLength),
                Validators.minLength(lengthConstants.titleMinLength),
                CustomValidator.alphaNumericSplChar]),
            price : new FormControl('', [Validators.required, CustomValidator.float, Validators.min(1)]),
            durationDays : new FormControl('', [Validators.required, Validators.max(365), Validators.min(1)]),
            startDate : new FormControl(null, [Validators.required]),
            endDate : new FormControl(null, [Validators.required]),
            isRedeemable : new FormControl("Yes", [Validators.required]),
            redemptionValue: new FormControl('')
        });
        this.dataSource = this.createDataSource([]);
    }

    initializeValue() {
        if(this.subscriptionPlan) {
            this.subscriptionFormGroup.get('type').setValue(this.subscriptionPlan.subscriptionType);
            this.subscriptionFormGroup.get('name').setValue(this.subscriptionPlan.title);
            this.subscriptionFormGroup.get('price').setValue(this.subscriptionPlan.price);
            this.subscriptionFormGroup.get('durationDays').setValue(this.subscriptionPlan.durationDays);
            this.subscriptionFormGroup.get('startDate').setValue(this.subscriptionPlan.startDate);
            this.subscriptionFormGroup.get('endDate').setValue(this.subscriptionPlan.endDate);
            this.subscriptionFormGroup.get('isRedeemable').setValue(this.subscriptionPlan.isRedeemable? "Yes" : "No");
            this.subscriptionFormGroup.get('redemptionValue').setValue(this.subscriptionPlan.redemptionValue);

            if(this.subscriptionPlan.subscriptionType === "TVOD") {
                this.setContentData();
            }
            
            if (this.subscriptionPlan.publishMode === "DRAFT") {
                this.isDraft = true;
            }
            else {
                this.isPublished = true;
                this.subscriptionFormGroup.disable();
            }
        }
    }

    setContentData() {
        this.contentService.getContentByCpIdAndFilters(broadcastCompleteContentFilters).subscribe(
            res => {
                let data: ContentView[];
                data = res.filter(item =>{
                    return this.subscriptionPlan.contentIds.includes(item.contentId);
                });
                data.forEach(item => {
                    item.displayCreatedDate =  this.pipe.transform(item.createdDate, 'short');
                    item.displayModifiedDate =  this.pipe.transform(item.modifiedDate, 'short');
                })
                this.dataSource = this.createDataSource(data);
                this.dataSource.paginator = this.paginator;
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
            }
        );
    }

    createDataSource(rawDataList) {
        return new MatTableDataSource<ContentView>(rawDataList)
    }

    displaySubscription() {
        this.subscriptionPlan = null;
        this.newSubscriptionEvent.emit(null);
    }

    disableTypeChange() {
        return this.isPublished;
    }

    onSubscriptionTypeChange($event) {
        if(this.isPublished) {
            $event.stopPropagation();
        }

        const dialogRef = this.dialog.open(CommonDialogComponent, {
            data: {
                heading: 'Confirm',
                message: "WARNING!!! All progress will be removed. Please click confirm to change the subscription type.",
                action: "PROCESS",
                buttons: this.openSelectCPModalButtons()
            },
            maxHeight: '400px'
        });

        dialogRef.afterClosed().subscribe(result => {
            if (result === "proceed") {
                this.changeSubscriptionType($event.value);
            }

            else if (result === "cancel") {
                if($event.value==="SVOD") {
                    this.subscriptionFormGroup.get('type').setValue("TVOD");
                }
                else {
                    this.subscriptionFormGroup.get('type').setValue("SVOD");
                }
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
          label: 'Confirm',
          type: 'primary',
          value: 'submit',
          class: 'update-btn'
        }
        ]
    }

    changeSubscriptionType(subscriptionType) {
        this.createEmptyFormDraft(subscriptionType);
        this.dataSource._updateChangeSubscription();
        this.dataSource.paginator = this.paginator;
    }

    isTvodSubscription() {
        return (this.subscriptionFormGroup.get('type').value == "TVOD");
    }

    openContentListDialog() {
        const dialogRef = this.dialog.open(AddContentListDialog, {
            width: '60%',
            height: '90%',
            data: {  
                selectedItem: this.dataSource.data
            }
        });

        dialogRef.componentInstance.onEventCreate.subscribe(event => {
            
            event.forEach(e => {
                this.dataSource.data.push(e);
            })
            
            this.dataSource._updateChangeSubscription();
            dialogRef.close();
        });

        dialogRef.afterClosed().subscribe(result => {
            console.log('closing the dialog');
        })
    }

    openContentList() {
        this.openContentListDialog();
        
    }

    get f() { 
        return this.subscriptionFormGroup.controls; 
    }

    public errorHandling = (control: string, error: string) => {
        return this.subscriptionFormGroup.controls[control].hasError(error);
    }

    isNotRedeemable() {
        return (this.isPublished || this.subscriptionFormGroup.get('isRedeemable').value !== "Yes");
    }

    createSubscription() {
        if (this.subscriptionPlan) {
            this.updateSubscription();
        }
        else {
            this.createNewSubscription();
        }
    }

    createNewSubscription() {
        let newPlan = this.getFormDetails();
        this.subscriptionService.createSubscription(newPlan).subscribe(
            res => {
                this.toastr.success("New Subscription drafted successfully");
                this.newSubscriptionEvent.emit(newPlan);
            },
            err => this.toastr.error(err)
        );
    }

    updateSubscription() {
        let newPlan = this.getFormDetails();
        newPlan.id = this.subscriptionPlan.id;
        this.subscriptionService.editSubscription(newPlan).subscribe(
            res => {
                this.toastr.success("Subscription updated successfully");
                this.newSubscriptionEvent.emit(newPlan);
            },
            err => this.toastr.error(err)
        );
    }

    getFormDetails() {
        var selectedEndDate = this.subscriptionFormGroup.get('endDate').value;
        if(Object.prototype.toString.call(selectedEndDate) !== "[object Date]") {
            selectedEndDate = new Date(selectedEndDate);
        } else {
            selectedEndDate.setHours(selectedEndDate.getHours() + 23);
            selectedEndDate.setMinutes(selectedEndDate.getMinutes() + 59);
            selectedEndDate.setSeconds(selectedEndDate.getSeconds() + 59);
        }

        var sub: any = {
            contentProviderId : sessionStorage.getItem("contentProviderId"),
            title: this.subscriptionFormGroup.get('name').value,
            durationDays: this.subscriptionFormGroup.get('durationDays').value,
            price: this.subscriptionFormGroup.get('price').value,
            startDate: this.subscriptionFormGroup.get('startDate').value,
            endDate: selectedEndDate,
            isRedeemable:  this.subscriptionFormGroup.get('isRedeemable').value === "Yes",
            redemptionValue :  this.subscriptionFormGroup.get('isRedeemable').value === "Yes" ? 
                this.subscriptionFormGroup.get('redemptionValue').value : 0,
            publishMode: "DRAFT",
            subscriptionType: this.subscriptionFormGroup.get('type').value
        }

        if(this.subscriptionFormGroup.get('type').value === "TVOD") {
            sub.contentIds = this.dataSource.data.map(val => {return val.contentId});
        }

        return sub;
    }

    isSubscriptionFormNotValid() {
        return !this.subscriptionFormGroup.valid;
    }

    isEventFormNotValid() {
        if(this.subscriptionFormGroup.get('type').value === "TVOD") {
            return this.dataSource.data.length < 1;
        }
        return false;
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
            }
        );
    }

    deleteItem(row) {
        this.dataSource.data = this.dataSource.data.filter(e => e.id !=  row.id);
    }

}