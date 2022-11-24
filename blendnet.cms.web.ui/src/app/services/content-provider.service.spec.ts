// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

import { TestBed } from '@angular/core/testing';

import { ContentProviderService } from './content-provider.service';

describe('ContentProviderService', () => {
  let service: ContentProviderService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ContentProviderService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
