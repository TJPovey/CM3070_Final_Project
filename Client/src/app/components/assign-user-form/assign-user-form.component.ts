import { CommonModule } from '@angular/common';
import { AfterViewInit, ChangeDetectionStrategy, Component, EventEmitter, inject, Input, OnInit, Output, ViewChild } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, ReactiveFormsModule, ValidationErrors, Validators } from '@angular/forms';
import { IonButton, IonButtons, IonContent, IonHeader, IonImg, IonItem, IonLabel, IonModal, IonSelect, IonSelectOption, IonText, IonTitle, IonToolbar } from "@ionic/angular/standalone";
import { PropertyFacadeService } from 'src/app/facade/Property/property-facade.service';
import { IPropertyDetail } from 'src/app/models/DTOs/Property/IPropertyDto';


@Component({
  selector: 'app-assign-user-form',
  templateUrl: './assign-user-form.component.html',
  styleUrls: ['./assign-user-form.component.scss'],
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [
    IonButtons,
    IonModal,
    CommonModule,
    IonHeader,
    IonToolbar,
    IonTitle,
    IonItem,
    IonContent,
    IonLabel,
    IonButton,
    IonText,
    IonImg,
    ReactiveFormsModule,
    IonSelect,
    IonSelectOption,
  ]
})
export class AssignUserFormComponent implements OnInit,  AfterViewInit {

  @ViewChild(IonModal) modal!: IonModal;
  @Input() propertyId!: string;
  @Input() ownerId!: string;

  @Output() assignUserCancelled = new EventEmitter(); 
  @Output() assignedUserDetails = new EventEmitter<IPropertyDetail>(); 

  protected userAssignForm!: FormGroup;
  private _formBuilder = inject(FormBuilder);
  private _propertyFacaeService = inject(PropertyFacadeService);

  public ngOnInit(): void {
    this.userAssignForm = this._formBuilder.group({
      username: ['', [Validators.required, Validators.minLength(3)]],
      role: ['', [Validators.required, this.roleValidator]],
    });
  }


  public ngAfterViewInit() {
    this.modal.isOpen = true
  }

  protected cancel() {
    this.assignUserCancelled.emit();
    this.modal.dismiss(null, 'cancel');
  }

  protected confirm() {
    if (this.userAssignForm.valid) {

      this._propertyFacaeService.assignUserToProperty(this.propertyId, this.ownerId, {
        userRole: this.userAssignForm.controls["role"].value,
        assignedToUserName: this.userAssignForm.controls["username"].value
      }).subscribe(res => {
          this.assignedUserDetails.emit(res);
          this.modal.dismiss(null, 'confirm');
      });
    }
  }

  private roleValidator(control: AbstractControl): ValidationErrors | null {
    const value = control.value;
    if (value != "Contributer" && value != "Reader") {
      return { roleInvalid: "Role must equal either Contributer or Reader." };
    }
    return null;
  }

}
