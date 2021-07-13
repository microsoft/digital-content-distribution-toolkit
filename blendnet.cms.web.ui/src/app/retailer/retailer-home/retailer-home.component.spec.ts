import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RetailerHomeComponent } from './retailer-home.component';

describe('RetailerHomeComponent', () => {
  let component: RetailerHomeComponent;
  let fixture: ComponentFixture<RetailerHomeComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ RetailerHomeComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(RetailerHomeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
