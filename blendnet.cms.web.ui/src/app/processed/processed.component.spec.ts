// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { ProcessedComponent } from './processed.component';

describe('ProcessedComponent', () => {
  let component: ProcessedComponent;
  let fixture: ComponentFixture<ProcessedComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ ProcessedComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ProcessedComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
