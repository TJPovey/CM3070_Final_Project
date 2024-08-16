import { CommonModule } from '@angular/common';
import { Component, Input, OnChanges, SimpleChanges, ViewChild } from '@angular/core';
import { IonLabel, IonItem, IonImg, IonContent, IonHeader, IonModal, IonTitle, IonToolbar, IonButton, IonButtons, IonList, IonInput, IonToggle, IonNote } from "@ionic/angular/standalone";
import { IPropertyDetail } from 'src/app/models/DTOs/Property/IPropertyDto';

@Component({
  selector: 'app-property-form',
  templateUrl: './property-form.component.html',
  styleUrls: ['./property-form.component.scss'],
  standalone: true,
  imports: [
    IonNote, 
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
export class PropertyFormComponent implements OnChanges {

  @ViewChild(IonModal) modal!: IonModal;
  @Input() propertyDetail?: IPropertyDetail | null;


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
}
