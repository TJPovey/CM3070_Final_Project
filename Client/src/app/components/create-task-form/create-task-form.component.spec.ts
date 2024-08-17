import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { CreateTaskFormComponent } from './create-task-form.component';

describe('CreateTaskFormComponent', () => {
  let component: CreateTaskFormComponent;
  let fixture: ComponentFixture<CreateTaskFormComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      imports: [CreateTaskFormComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(CreateTaskFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  }));

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
