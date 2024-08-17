import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { AssignUserFormComponent } from './assign-user-form.component';

describe('AssignUserFormComponent', () => {
  let component: AssignUserFormComponent;
  let fixture: ComponentFixture<AssignUserFormComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      imports: [AssignUserFormComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(AssignUserFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  }));

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
