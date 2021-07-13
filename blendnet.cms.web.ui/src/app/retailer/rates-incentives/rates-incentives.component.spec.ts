import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RatesIncentivesComponent } from './rates-incentives.component';

describe('RatesIncentivesComponent', () => {
  let component: RatesIncentivesComponent;
  let fixture: ComponentFixture<RatesIncentivesComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ RatesIncentivesComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(RatesIncentivesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
