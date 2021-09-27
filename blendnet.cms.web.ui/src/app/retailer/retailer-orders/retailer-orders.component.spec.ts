import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RetailerOrdersComponent } from './retailer-orders.component';

describe('RetailerOrdersComponent', () => {
  let component: RetailerOrdersComponent;
  let fixture: ComponentFixture<RetailerOrdersComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ RetailerOrdersComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(RetailerOrdersComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
