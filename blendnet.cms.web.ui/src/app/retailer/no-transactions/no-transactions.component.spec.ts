import { ComponentFixture, TestBed } from '@angular/core/testing';

import { NoTransactionsComponent } from './no-transactions.component';

describe('NoTransactionsComponent', () => {
  let component: NoTransactionsComponent;
  let fixture: ComponentFixture<NoTransactionsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ NoTransactionsComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(NoTransactionsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
