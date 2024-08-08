import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, Component, inject, OnInit } from '@angular/core';
import { IonItem, IonLabel, IonList, IonThumbnail, IonNote } from '@ionic/angular/standalone';
import { BehaviorSubject, Observable, Subject, switchMap } from 'rxjs';
import { PropertyFacadeService } from 'src/app/facade/Property/property-facade.service';
import { IPropertyListItemDTO } from 'src/app/models/DTOs/Property/IPropertyListGetDTO';
import { IResponseCollectionDTO } from 'src/app/models/DTOs/Responses/IResponseCollectionDTO';
import { PaginationOptions } from 'src/app/models/Pagination/IPaginationOptions';

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
export class PropertyListComponent implements OnInit {

  private _propertyFacadeService = inject(PropertyFacadeService);

  protected propertyCollection$: Observable<IResponseCollectionDTO<IPropertyListItemDTO>>;

  constructor() {
    this.propertyCollection$ = this._propertyFacadeService.propertyCollection$;

    this.propertyCollection$.subscribe(res => {
      console.log(res);
    })
  }

  public ngOnInit() {
    this.handlePagingRequest();
  }

  protected handlePagingRequest(pagination?: PaginationOptions) {
    this._propertyFacadeService.updatePropertyPage(pagination);
  }

  public trackByPropertyId(index: number, property: IPropertyListItemDTO) {
    return property.id;
  }
  

}
