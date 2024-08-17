import { CommonModule } from '@angular/common';
import { Component, Input, OnChanges, SimpleChanges, ViewChild } from '@angular/core';
import { IonButton, IonButtons, IonContent, IonHeader, IonIcon, IonImg, IonInput, IonItem, IonLabel, IonList, IonModal, IonNote, IonText, IonTitle, IonToggle, IonToolbar } from "@ionic/angular/standalone";
import { ITaskDetail } from 'src/app/models/DTOs/Tasks/ITaskDto';

@Component({
  selector: 'app-task-form',
  templateUrl: './task-form.component.html',
  styleUrls: ['./task-form.component.scss'],
  standalone: true,
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
    IonLabel
  ]
})
export class TaskFormComponent implements OnChanges {

  @ViewChild(IonModal) modal!: IonModal;
  @Input() taskDetail?: ITaskDetail | null;


  public ngOnChanges(changes: SimpleChanges): void {
    if (this.taskDetail) {
      this.modal.isOpen = true;
    }
  }

  protected close() {
    this.modal.dismiss(null, 'cancel');
    this.modal.isOpen = false;  
    this.taskDetail = null;
  }
}
