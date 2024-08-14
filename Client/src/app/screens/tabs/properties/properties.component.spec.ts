import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { PropertiesComponent } from './properties.component';

describe('PropertiesComponent', () => {
  let component: PropertiesComponent;
  let fixture: ComponentFixture<PropertiesComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      imports: [PropertiesComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(PropertiesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  }));

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
