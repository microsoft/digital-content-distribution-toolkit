// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { UnprocessedComponent } from './unprocessed.component';

describe('UnprocessedComponent', () => {
  let component: UnprocessedComponent;
  let fixture: ComponentFixture<UnprocessedComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ UnprocessedComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(UnprocessedComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
