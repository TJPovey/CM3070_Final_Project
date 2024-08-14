import { TestBed } from '@angular/core/testing';

import { CanActivateJwtService } from './can-activate-jwt.service';

describe('CanActivateJwtService', () => {
  let service: CanActivateJwtService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(CanActivateJwtService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
