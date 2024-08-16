import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { IonItem, IonLabel, IonList, IonNote, IonThumbnail } from '@ionic/angular/standalone';
import { Observable } from 'rxjs';
import { PropertyFacadeService } from 'src/app/facade/Property/property-facade.service';
import { IPropertyAssignment } from 'src/app/models/DTOs/User/IUserDTO';

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
    IonNote
  ],
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class PropertyListComponent {

  private _propertyFacadeService = inject(PropertyFacadeService);
  protected propertyCollection$: Observable<IPropertyAssignment[]>;

  constructor() {
    this.propertyCollection$ = this._propertyFacadeService.propertyCollection$;
  }


  public trackByPropertyId(index: number, property: IPropertyAssignment) {
    return property.property.id;
  }

}
