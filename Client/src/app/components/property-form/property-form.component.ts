import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, Component, inject, Input, OnChanges, SimpleChanges, ViewChild } from '@angular/core';
import { IonIcon, IonText, IonLabel, IonItem, IonImg, IonContent, IonHeader, IonModal, IonTitle, IonToolbar, IonButton, IonButtons, IonList, IonInput, IonToggle, IonNote } from "@ionic/angular/standalone";
import { IPropertyDetail, ITaskAssignment } from 'src/app/models/DTOs/Property/IPropertyDto';
import { TaskFormComponent } from '../task-form/task-form.component';
import { PropertyFacadeService } from 'src/app/facade/Property/property-facade.service';
import { BehaviorSubject, Subject, take } from 'rxjs';
import { ITaskDetail } from 'src/app/models/DTOs/Tasks/ITaskDto';
import { AssignUserFormComponent } from '../assign-user-form/assign-user-form.component';

@Component({
  selector: 'app-property-form',
  templateUrl: './property-form.component.html',
  styleUrls: ['./property-form.component.scss'],
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [
    IonNote, 
    IonText,
    IonIcon,
    IonToggle, 
    IonInput,  
    CommonModule,
    IonButtons, 
    IonButton, 
    IonHeader,
    IonModal,
    IonToolbar,
    IonTitle,
    IonContent,
    IonImg,
    IonList,
    IonItem,
    IonLabel,
    TaskFormComponent,
    AssignUserFormComponent
  ]
})
export class PropertyFormComponent implements OnChanges {

  @ViewChild(IonModal) modal!: IonModal;
  @Input() propertyDetail?: IPropertyDetail | null;

  private _propertyFacade = inject(PropertyFacadeService)

  private _selectedTask$ = new Subject<ITaskDetail>();
  protected selectedTask$ = this._selectedTask$.asObservable();

  private _assigningUser$ = new Subject<boolean>();
  protected assigningUser$ = this._assigningUser$.asObservable();

  public ngOnChanges(changes: SimpleChanges): void {
    if (this.propertyDetail) {
      this.modal.isOpen = true;
    }
  }

  protected close() {
    this.modal.dismiss(null, 'cancel');
    this.modal.isOpen = false;  
    this.propertyDetail = null;
  }

  protected handleTaskClick(taskAssignment: ITaskAssignment) {
    if (this.propertyDetail) {
      this._propertyFacade.getTask(
        taskAssignment.id, 
        this.propertyDetail?.id, 
        this.propertyDetail?.ownerId.id)
        .pipe(take(1))
        .subscribe(res => this._selectedTask$.next({...res}));
    }
  }

  protected handleAssignUserClick() {
    console.log("assign user click");
    this._assigningUser$.next(true);
  }

  protected handleAssignUserCancel() {
    this._assigningUser$.next(false);
  }

  protected handleAssignedUser(property: IPropertyDetail) {
    this._assigningUser$.next(false);
    this.propertyDetail = property;
  }
  
}
