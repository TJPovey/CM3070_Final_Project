import { CommonModule } from '@angular/common';
import { AfterViewInit, ChangeDetectionStrategy, Component, EventEmitter, inject, Input, OnInit, Output, ViewChild } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, ReactiveFormsModule, ValidationErrors, Validators } from '@angular/forms';
import { Math as CesiumMath } from '@cesium/engine';
import { IonButton, IonButtons, IonContent, IonHeader, IonImg, IonInput, IonItem, IonLabel, IonModal, IonText, IonTitle, IonToolbar } from "@ionic/angular/standalone";
import { PropertyDtoExtensions } from 'src/app/extensions/PropertyDtoExtensions';
import { PropertyFacadeService } from 'src/app/facade/Property/property-facade.service';
import { IPropertyDto } from 'src/app/models/DTOs/Property/IPropertyDto';
import { IPropertyPostDto } from 'src/app/models/DTOs/Property/IPropertyPostDto';
import { ILocationDto } from 'src/app/models/DTOs/Shared/ILocationDto';
import { PhotoCaptureService } from 'src/app/services/photo-capture/photo-capture.service';


@Component({
  selector: 'app-create-property-form',
  templateUrl: './create-property-form.component.html',
  styleUrls: ['./create-property-form.component.scss'],
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [
    IonButtons,
    IonInput,
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
  ]
})
export class CreatePropertyFormComponent implements OnInit, AfterViewInit {

  @ViewChild(IonModal) modal!: IonModal;
  @ViewChild('PlaceholderFileInput') placeholderFileInput!: IonInput;

  @Output() addPropertyCancelled = new EventEmitter(); 
  @Output() addPropertyDetails = new EventEmitter<IPropertyDto>(); 

  @Input() latitudeRadians?: number;
  @Input() longitudeRadians?: number;
  @Input() elevation?: number;

  protected propertyCreateForm!: FormGroup;
  private _formBuilder = inject(FormBuilder);
  private _photoCaptureService = inject(PhotoCaptureService);
  private _propertyFacaeService = inject(PropertyFacadeService);


  public ngOnInit(): void {

    const latitude = this.latitudeRadians ? CesiumMath.toDegrees(this.latitudeRadians) : null;
    const longitude = this.longitudeRadians ? CesiumMath.toDegrees(this.longitudeRadians) : null;
    const elevation = this.elevation ?? null;

    this.propertyCreateForm = this._formBuilder.group({
      propertyName: ['', [Validators.required, Validators.minLength(4)]],
      reportTitle: ['', [Validators.required, Validators.minLength(4)]],
      imageName: ['', [Validators.required]],
      latitude: [latitude, [Validators.required, this.latitudeValidator]],
      longitude: [longitude, [Validators.required, this.longitudeValidator]],
      elevation: [elevation, [Validators.required]],
    });
  }

  public ngAfterViewInit() {
    this.modal.isOpen = true
  }

  protected cancel() {
    this.addPropertyCancelled.emit();
    this.modal.dismiss(null, 'cancel');
  }

  protected confirm() {

    if (this.propertyCreateForm.valid) {

      const propertyDto = PropertyDtoExtensions.ToPropertyPostDto(
        this.propertyCreateForm.controls["propertyName"].value,
        this.propertyCreateForm.controls["reportTitle"].value,
        this.propertyCreateForm.controls["latitude"].value,
        this.propertyCreateForm.controls["longitude"].value,
        this.propertyCreateForm.controls["elevation"].value
      );

      this._propertyFacaeService.createProperty(propertyDto, this.propertyCreateForm.controls["imageName"].value)
        .subscribe(res => {
          this.addPropertyDetails.emit(res[1]);
          this.modal.dismiss(null, 'confirm');
        });
    }
  }

  protected async selectPhoto() {
    const fileName = await this._photoCaptureService.addNewToGallery();
    if (fileName) {
      this.propertyCreateForm.controls["imageName"].setValue(fileName);
      this.placeholderFileInput.writeValue('Image Selected');
    }
  }

  private latitudeValidator(control: AbstractControl): ValidationErrors | null {
    const value = control.value;
    if (value < -90 || value > 90) {
      return { latitudeInvalid: true };
    }
    return null;
  }

  private longitudeValidator(control: AbstractControl): ValidationErrors | null {
    const value = control.value;
    if (value < -180 || value > 180) {
      return { longitudeInvalid: true };
    }
    return null;
  }
  
}
