import { TestBed } from '@angular/core/testing';

import { PropertyFacadeService } from './property-facade.service';

describe('PropertyFacadeService', () => {
  let service: PropertyFacadeService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(PropertyFacadeService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
