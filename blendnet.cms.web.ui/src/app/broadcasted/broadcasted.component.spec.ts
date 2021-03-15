import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { BroadcastedComponent } from './broadcasted.component';

describe('BroadcastedComponent', () => {
  let component: BroadcastedComponent;
  let fixture: ComponentFixture<BroadcastedComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ BroadcastedComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(BroadcastedComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
