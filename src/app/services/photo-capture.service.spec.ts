import { TestBed } from '@angular/core/testing';

import { PhotoCaptureService } from './photo-capture.service';

describe('PhotoCaptureService', () => {
  let service: PhotoCaptureService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(PhotoCaptureService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
