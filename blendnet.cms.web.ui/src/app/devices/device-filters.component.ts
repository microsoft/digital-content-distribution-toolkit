import { SelectionModel } from '@angular/cdk/collections';
import { Component, EventEmitter, Inject, OnInit, Output } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { ToastrService } from 'ngx-toastr';
import { environment } from 'src/environments/environment';
import { DeviceService } from '../services/device.service';

@Component({
  selector: 'app-device-filters',
  templateUrl: './device-filters.component.html',
  styleUrls: ['./devices.component.css']
})
export class DeviceFiltersComponent implements OnInit {
  @Output() onDeviceFilterUpdate = new EventEmitter<any>();
  deviceid;
  filters;
  selectedContents;
  allowMultiSelect = true;
  initialSelection = [];
  selection = new SelectionModel<any>(this.allowMultiSelect, this.initialSelection);
  appliedFilters =[];


  constructor(
    @Inject(MAT_DIALOG_DATA) public data: any,
    public dialogRef: MatDialogRef<DeviceFiltersComponent>,
    private deviceService: DeviceService,
    private toastr: ToastrService

  ) { 
    this.filters = environment.filters;
  }

  ngOnInit(): void {
    this.selectedContents=0;
    }

  closeDialog(): void {
    this.dialogRef.close();
  }

  applyFilters() {
    var deviceData = {
        "deviceIds": [
            this.data.device.id
        ],
        "filters": this.appliedFilters
      }
    this.deviceService.filterUpdate(deviceData).subscribe(
      res => {
        console.log(res);
        this.onDeviceFilterUpdate.emit("Device filters updated successfully!");
       
      },
      err => this.toastr.error(err)
    );
  }


  getDeviceDetails() {
    var device: any = {
      id: this.deviceid
    }
    return device;
  }
    

  
  onNoClick(): void {
    this.dialogRef.close();
  }

  
  toggleSelection(event, f) {
    if(event.checked){
      this.appliedFilters.push(f)
    }else{
      this.appliedFilters =  this.appliedFilters.filter(filter => {
        return filter !== f;
      });
    }
  }

}
