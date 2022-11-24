// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RetailerDownloadComponent } from './retailer-download.component';

describe('RetailerDownloadComponent', () => {
  let component: RetailerDownloadComponent;
  let fixture: ComponentFixture<RetailerDownloadComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ RetailerDownloadComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(RetailerDownloadComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
