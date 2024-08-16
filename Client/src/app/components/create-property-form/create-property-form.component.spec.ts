import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { CreatePropertyFormComponent } from './create-property-form.component';

describe('CreatePropertyFormComponent', () => {
  let component: CreatePropertyFormComponent;
  let fixture: ComponentFixture<CreatePropertyFormComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      imports: [CreatePropertyFormComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(CreatePropertyFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  }));

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
