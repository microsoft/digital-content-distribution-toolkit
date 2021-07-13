import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RetailerCommissionsComponent } from './retailer-commissions.component';

describe('RetailerCommissionsComponent', () => {
  let component: RetailerCommissionsComponent;
  let fixture: ComponentFixture<RetailerCommissionsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ RetailerCommissionsComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(RetailerCommissionsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
