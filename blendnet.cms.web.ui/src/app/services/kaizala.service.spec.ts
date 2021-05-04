import { TestBed } from '@angular/core/testing';

import { KaizalaService } from './kaizala.service';

describe('KaizalaService', () => {
  let service: KaizalaService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(KaizalaService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
