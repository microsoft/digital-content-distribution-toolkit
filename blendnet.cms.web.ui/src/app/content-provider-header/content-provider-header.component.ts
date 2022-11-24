// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

import { Component, OnInit } from '@angular/core';
import { Contentprovider } from '../models/contentprovider.model';
import { ContentProviderService } from '../services/content-provider.service';

@Component({
  selector: 'app-content-provider-header',
  templateUrl: './content-provider-header.component.html',
  styleUrls: ['./content-provider-header.component.css']
})
export class ContentProviderHeaderComponent implements OnInit {

  selectedCP: Contentprovider;
  selectedCPName: string = "Not Selected";
  contentProviderId: string = "";

  constructor(
    private cpService: ContentProviderService
  ) { 
    this.selectedCPName = sessionStorage.getItem("contentProviderName") ? 
      sessionStorage.getItem("contentProviderName") : "Not Selected";

    this.contentProviderId = sessionStorage.getItem("contentProviderId") ?
      sessionStorage.getItem("contentProviderId") : "";
  }

  ngOnInit(): void {
    this.cpService.sharedSelectedCP$.subscribe(selectedCP => {
      this.selectedCPName = selectedCP ? selectedCP.name :
        sessionStorage.getItem("contentProviderName") ?
        sessionStorage.getItem("contentProviderName") : "Not Selected";
      
      this.contentProviderId = sessionStorage.getItem("contentProviderId") ?
      sessionStorage.getItem("contentProviderId") : "";
    });
  }

  ngDoCheck() {
    this.cpService.sharedSelectedCP$.subscribe(selectedCP => {
      this.selectedCPName = selectedCP ? selectedCP.name :
        sessionStorage.getItem("contentProviderName") ?
        sessionStorage.getItem("contentProviderName") : "Not Selected";

      this.contentProviderId = sessionStorage.getItem("contentProviderId") ?
      sessionStorage.getItem("contentProviderId") : "";
    });
  }

}
