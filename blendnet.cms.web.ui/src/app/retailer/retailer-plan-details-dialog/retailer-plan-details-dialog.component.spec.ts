// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RetailerPlanDetailsDialogComponent } from './retailer-plan-details-dialog.component';

describe('RetailerPlanDetailsDialogComponent', () => {
  let component: RetailerPlanDetailsDialogComponent;
  let fixture: ComponentFixture<RetailerPlanDetailsDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ RetailerPlanDetailsDialogComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(RetailerPlanDetailsDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
