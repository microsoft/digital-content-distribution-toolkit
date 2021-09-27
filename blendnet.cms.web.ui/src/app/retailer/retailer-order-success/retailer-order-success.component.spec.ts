import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RetailerOrderSuccessComponent } from './retailer-order-success.component';

describe('RetailerOrderSuccessComponent', () => {
  let component: RetailerOrderSuccessComponent;
  let fixture: ComponentFixture<RetailerOrderSuccessComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ RetailerOrderSuccessComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(RetailerOrderSuccessComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
