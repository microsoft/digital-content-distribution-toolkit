import { TestBed } from '@angular/core/testing';

import { ActivateNumberService } from './activate-number.service';

describe('AcvtivateNumberService', () => {
  let service: ActivateNumberService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ActivateNumberService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
