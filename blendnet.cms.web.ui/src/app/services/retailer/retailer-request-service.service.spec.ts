import { TestBed } from '@angular/core/testing';

import { RetailerRequestServiceService } from './retailer-request-service.service';

describe('RetailerRequestServiceService', () => {
  let service: RetailerRequestServiceService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(RetailerRequestServiceService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
