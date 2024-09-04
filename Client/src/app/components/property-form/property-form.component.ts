import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, Component, inject, Input, OnChanges, SimpleChanges, ViewChild } from '@angular/core';
import { IonButton, IonButtons, LoadingController, IonContent, IonHeader, IonIcon, IonImg, IonInput, IonItem, IonLabel, IonList, IonLoading, IonModal, IonNote, IonText, IonTitle, IonToggle, IonToolbar } from "@ionic/angular/standalone";
import { finalize, forkJoin, Subject, take } from 'rxjs';
import { PropertyFacadeService } from 'src/app/facade/Property/property-facade.service';
import { IPropertyDetail, ITaskAssignment } from 'src/app/models/DTOs/Property/IPropertyDto';
import { ITaskDetail } from 'src/app/models/DTOs/Tasks/ITaskDto';
import { PdfGeneratorService } from 'src/app/services/pdf-generator/pdf-generator.service';
import { AssignUserFormComponent } from '../assign-user-form/assign-user-form.component';
import { TaskFormComponent } from '../task-form/task-form.component';


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
    AssignUserFormComponent,
    IonLoading
  ]
})
export class PropertyFormComponent implements OnChanges {

  @ViewChild(IonModal) modal!: IonModal;
  @Input() propertyDetail?: IPropertyDetail | null;

  private _propertyFacade = inject(PropertyFacadeService)
  private _pdfGenerator = inject(PdfGeneratorService)

  private _loadingController = inject(LoadingController);

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


  protected async export() {

    if (!this.propertyDetail) {
      return;
    }

    const loading = await this._loadingController.create({message: "Generating report..."});
    loading.present();

    const propertyBlobResponse = await this._propertyFacade.getPropertyImageBlob(this.propertyDetail);
    const propertyBlob = await propertyBlobResponse?.blobBody;
    const propertyArr = new Uint8Array(await propertyBlob!.arrayBuffer());

    const taskImages = new Map<string, Uint8Array>();
    const taskIds = this.propertyDetail.taskAssignments.map(res => {
      return res.id
    });

    const taskRequests = taskIds.map(id => this._propertyFacade.getTask(
      id, 
      this.propertyDetail!.id, 
      this.propertyDetail!.ownerId.id))
    const tasks = forkJoin(taskRequests);

    tasks.pipe(
      take(1),
      finalize(async () => await this._loadingController.dismiss())
    ).subscribe(async tasks => {

        const taskPromises = tasks.map(async (res) => {
          const blobResponse = await this._propertyFacade.getTaskImageBlob(this.propertyDetail?.writeToken!, res);
          const blob = await blobResponse?.blobBody;
          const arr = new Uint8Array(await blob!.arrayBuffer());
          taskImages.set(res.id, arr);
        });
    
        await Promise.all(taskPromises);

        this._pdfGenerator.generateReport(
          this.propertyDetail!,
          propertyArr,
          tasks,
          taskImages
        );
      }
    );
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
