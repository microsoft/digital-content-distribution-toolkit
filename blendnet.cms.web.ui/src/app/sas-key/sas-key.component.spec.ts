// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { SasKeyComponent } from './sas-key.component';

describe('SasKeyComponent', () => {
  let component: SasKeyComponent;
  let fixture: ComponentFixture<SasKeyComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ SasKeyComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SasKeyComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
