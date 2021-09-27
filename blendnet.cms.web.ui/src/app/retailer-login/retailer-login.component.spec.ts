import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RetailerLoginComponent } from './retailer-login.component';

describe('RetailerLoginComponent', () => {
  let component: RetailerLoginComponent;
  let fixture: ComponentFixture<RetailerLoginComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ RetailerLoginComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(RetailerLoginComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
