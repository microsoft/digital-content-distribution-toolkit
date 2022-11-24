// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RetailerMilestonesComponent } from './retailer-milestones.component';

describe('RetailerMilestonesComponent', () => {
  let component: RetailerMilestonesComponent;
  let fixture: ComponentFixture<RetailerMilestonesComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ RetailerMilestonesComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(RetailerMilestonesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
