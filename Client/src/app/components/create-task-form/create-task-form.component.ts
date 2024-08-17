import { CommonModule } from '@angular/common';
import { AfterViewInit, ChangeDetectionStrategy, Component, EventEmitter, inject, Input, OnInit, Output, ViewChild } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, ReactiveFormsModule, ValidationErrors, Validators } from '@angular/forms';
import { Math as CesiumMath } from '@cesium/engine';
import { IonButton, IonButtons, IonContent, IonHeader, IonImg, IonInput, IonItem, IonLabel, IonModal, IonSelect, IonSelectOption, IonText, IonTitle, IonToolbar } from "@ionic/angular/standalone";
import { BehaviorSubject } from 'rxjs';
import { TaskDtoExtensions } from 'src/app/extensions/TaskDtoExtensions';
import { PropertyFacadeService } from 'src/app/facade/Property/property-facade.service';
import { IPropertyDetail, IUserAssignment } from 'src/app/models/DTOs/Property/IPropertyDto';
import { ITaskDto } from 'src/app/models/DTOs/Tasks/ITaskDto';
import { IPropertyAssignment } from 'src/app/models/DTOs/User/IUserDTO';
import { PhotoCaptureService } from 'src/app/services/photo-capture/photo-capture.service';

@Component({
  selector: 'app-create-task-form',
  templateUrl: './create-task-form.component.html',
  styleUrls: ['./create-task-form.component.scss'],
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
    IonSelectOption,
    IonSelect
  ]
})
export class CreateTaskFormComponent implements OnInit, AfterViewInit {

  @ViewChild(IonModal) modal!: IonModal;
  @ViewChild('PlaceholderFileInput') placeholderFileInput!: IonInput;

  @Output() addTaskCancelled = new EventEmitter<void>(); 
  @Output() addTaskDetails = new EventEmitter<IPropertyDetail>(); 

  @Input() latitudeRadians?: number;
  @Input() longitudeRadians?: number;
  @Input() elevation?: number;
  @Input() propertyDetail!: IPropertyDetail;

  protected taskCreateForm!: FormGroup;
  private _formBuilder = inject(FormBuilder);
  private _photoCaptureService = inject(PhotoCaptureService);
  private _propertyFacaeService = inject(PropertyFacadeService);

  private _formReady$ = new BehaviorSubject<boolean>(false);
  protected formReady$ = this._formReady$.asObservable();

  protected availableUsers!: IUserAssignment[];
  private _writeToken!: string;

  public async ngOnInit() {

    const latitude = this.latitudeRadians ? CesiumMath.toDegrees(this.latitudeRadians) : null;
    const longitude = this.longitudeRadians ? CesiumMath.toDegrees(this.longitudeRadians) : null;
    const elevation = this.elevation ?? null;

    this.availableUsers = this.propertyDetail.userAssignments;
    this._writeToken = this.propertyDetail.writeToken as string;

    this.taskCreateForm = this._formBuilder.group({
      propertyId: [this.propertyDetail.id, [Validators.required, Validators.minLength(4)]],
      userPropertyOwnerId: [this.propertyDetail.ownerId.id, [Validators.required, Validators.minLength(4)]],
      title: ['', [Validators.required, Validators.minLength(4)]],
      area: ['', [Validators.required, Validators.minLength(4)]],
      dueDate: ['', [Validators.required, Validators.minLength(4)]],
      description: ['', [Validators.required, Validators.minLength(4)]],
      category: ['', [Validators.required]],
      priority: ['', [Validators.required]],
      assignedTo: [null, [Validators.required]],
      estimatedCost: [null, [Validators.required]],
      latitude: [latitude, [Validators.required, this.latitudeValidator]],
      longitude: [longitude, [Validators.required, this.longitudeValidator]],
      elevation: [elevation, [Validators.required]],
      imageName: ['', [Validators.required]]
    });

    this._formReady$.next(true);
  }

  public ngAfterViewInit() {
    this.modal.isOpen = true
  }

  protected cancel() {
    this.addTaskCancelled.emit();
    this.modal.dismiss(null, 'cancel');
  }

  protected confirm() {

    if (this.taskCreateForm.valid) {

      const assignedUser = this.taskCreateForm.controls["assignedTo"].value as IUserAssignment;
      const date = new Date(this.taskCreateForm.controls["dueDate"].value);
      const isoDate = date.toISOString();

      const taskDto = TaskDtoExtensions.ToTaskPostDto(
        this.taskCreateForm.controls["propertyId"].value,
        this.taskCreateForm.controls["userPropertyOwnerId"].value,
        this.taskCreateForm.controls["title"].value,
        this.taskCreateForm.controls["area"].value,
        this.taskCreateForm.controls["description"].value,
        isoDate,
        this.taskCreateForm.controls["estimatedCost"].value,
        this.taskCreateForm.controls["category"].value,
        this.taskCreateForm.controls["priority"].value,
        assignedUser.id,
        assignedUser.userName,
        this.taskCreateForm.controls["latitude"].value,
        this.taskCreateForm.controls["longitude"].value,
        this.taskCreateForm.controls["elevation"].value
      );

      this._propertyFacaeService.createTask(
          taskDto, 
          this.taskCreateForm.controls["imageName"].value,
          this._writeToken,
          this.propertyDetail.ownerId.id)
        .subscribe((res) => {
          this.addTaskDetails.emit(res);
          this.modal.dismiss(null, 'confirm');
        });
    }
  }

  protected async selectPhoto() {
    const fileName = await this._photoCaptureService.addNewToGallery();
    if (fileName) {
      this.taskCreateForm.controls["imageName"].setValue(fileName);
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
