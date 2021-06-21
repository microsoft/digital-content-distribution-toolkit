import { TestBed } from '@angular/core/testing';

import { IncentiveService } from './incentive.service';

describe('IncentiveService', () => {
  let service: IncentiveService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(IncentiveService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
