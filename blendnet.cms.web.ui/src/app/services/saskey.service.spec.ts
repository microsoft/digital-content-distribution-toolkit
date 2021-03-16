import { TestBed } from '@angular/core/testing';

import { SaskeyService } from './saskey.service';

describe('SaskeyService', () => {
  let service: SaskeyService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(SaskeyService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
