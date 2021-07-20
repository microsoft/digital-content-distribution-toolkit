import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SeamlessLoginComponent } from './seamless-login.component';

describe('SeamlessLoginComponent', () => {
  let component: SeamlessLoginComponent;
  let fixture: ComponentFixture<SeamlessLoginComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ SeamlessLoginComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(SeamlessLoginComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
