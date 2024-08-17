import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { IonItem, IonLabel, IonList, IonNote, IonThumbnail } from '@ionic/angular/standalone';
import { Observable, Subject } from 'rxjs';
import { PropertyFacadeService } from 'src/app/facade/Property/property-facade.service';
import { IPropertyAssignment } from 'src/app/models/DTOs/User/IUserDTO';
import { PropertyFormComponent } from '../property-form/property-form.component';
import { IPropertyDetail } from 'src/app/models/DTOs/Property/IPropertyDto';

@Component({
  selector: 'app-property-list',
  templateUrl: './property-list.component.html',
  styleUrls: ['./property-list.component.scss'],
  imports: [
    CommonModule,
    IonList,
    IonItem,
    IonLabel,
    IonThumbnail,
    IonNote,
    PropertyFormComponent
  ],
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class PropertyListComponent {

  private _propertyFacadeService = inject(PropertyFacadeService);
  protected propertyCollection$: Observable<IPropertyAssignment[]>;

  private _selectedProperty$ = new Subject<IPropertyDetail>();
  protected selectedProperty$ = this._selectedProperty$.asObservable();

  constructor() {
    this.propertyCollection$ = this._propertyFacadeService.propertyCollection$;
  }


  public trackByPropertyId(index: number, property: IPropertyAssignment) {
    return property.property.id;
  }

  protected handlePropertySelected(property: IPropertyAssignment) {
    this._propertyFacadeService.getProperty(property.property.id, property.property.ownerId)
      .subscribe(res => this._selectedProperty$.next(res));
  }

}
