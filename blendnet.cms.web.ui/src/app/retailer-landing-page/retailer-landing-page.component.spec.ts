import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RetailerLandingPageComponent } from './retailer-landing-page.component';

describe('RetailerLandingPageComponent', () => {
  let component: RetailerLandingPageComponent;
  let fixture: ComponentFixture<RetailerLandingPageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ RetailerLandingPageComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(RetailerLandingPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
