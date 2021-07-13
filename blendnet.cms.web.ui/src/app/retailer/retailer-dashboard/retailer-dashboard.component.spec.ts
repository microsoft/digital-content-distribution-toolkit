import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RetailerDashboardComponent } from './retailer-dashboard.component';

describe('RetailerDashboardComponent', () => {
  let component: RetailerDashboardComponent;
  let fixture: ComponentFixture<RetailerDashboardComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ RetailerDashboardComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(RetailerDashboardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
