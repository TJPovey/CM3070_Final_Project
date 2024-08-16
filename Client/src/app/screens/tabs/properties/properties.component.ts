import { Component } from '@angular/core';
import { IonContent, IonHeader, IonTitle, IonToolbar, IonImg } from '@ionic/angular/standalone';
import { PropertyListComponent } from '../../../components/property-list/property-list.component';
@Component({
  selector: 'app-properties',
  templateUrl: './properties.component.html',
  styleUrls: ['./properties.component.scss'],
  standalone: true,
  imports: [
    IonHeader, 
    IonToolbar, 
    IonTitle, 
    IonContent,
    IonImg, 
    PropertyListComponent
  ]
})
export class PropertiesComponent {

}
