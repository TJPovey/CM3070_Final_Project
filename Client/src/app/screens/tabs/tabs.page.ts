import { Component } from '@angular/core';
import { IonIcon, IonLabel, IonTabBar, IonTabButton, IonTabs } from '@ionic/angular/standalone';
import { addIcons } from 'ionicons';
import { homeSharp, globe } from 'ionicons/icons';
import { AppRoute } from '../app-routes.enum';

@Component({
  selector: 'app-tabs',
  templateUrl: 'tabs.page.html',
  styleUrls: ['tabs.page.scss'],
  standalone: true,
  imports: [
    IonTabs, 
    IonTabBar, 
    IonTabButton, 
    IonIcon, 
    IonLabel
  ],
})
export class TabsPage {

  protected exploreRoute = `${AppRoute.Home}/${AppRoute.Explore}`;
  protected propertiesRoute = `${AppRoute.Home}/${AppRoute.Property_List}`;

  constructor() {
    addIcons({ homeSharp, globe });
  }
}
