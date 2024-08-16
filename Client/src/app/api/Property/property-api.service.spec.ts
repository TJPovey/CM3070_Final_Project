import { TestBed } from '@angular/core/testing';

import { PropertyApiService } from './property-api.service';

describe('PropertyService', () => {
  let service: PropertyApiService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(PropertyApiService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
