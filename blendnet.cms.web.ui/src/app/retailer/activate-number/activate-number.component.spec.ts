import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ActivateNumberComponent } from './activate-number.component';

describe('ActivateNumberComponent', () => {
  let component: ActivateNumberComponent;
  let fixture: ComponentFixture<ActivateNumberComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ActivateNumberComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ActivateNumberComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
