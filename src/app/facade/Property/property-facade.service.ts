import { inject, Injectable } from '@angular/core';
import { BehaviorSubject, combineLatest, distinctUntilChanged, map, Observable, switchMap } from 'rxjs';
import { PropertyService } from 'src/app/api/Property/property.service';
import { IPropertyListItemDTO } from 'src/app/models/DTOs/Property/IPropertyListGetDTO';
import { IResponseCollectionDTO } from 'src/app/models/DTOs/Responses/IResponseCollectionDTO';
import { PaginationOptions } from 'src/app/models/Pagination/IPaginationOptions';

@Injectable({
  providedIn: 'root'
})
export class PropertyFacadeService {

  private _propertyAPIService = inject(PropertyService);

  public static readonly DefaultPagingOptions: PaginationOptions = {offset: 0, limit: 20};
  private _paginationUpdate$ = new BehaviorSubject<PaginationOptions>(PropertyFacadeService.DefaultPagingOptions);
  private _paginationUpdateObs$ = this._paginationUpdate$.asObservable().pipe(distinctUntilChanged(this.paginationDistinction));

  public propertyCollection$: Observable<IResponseCollectionDTO<IPropertyListItemDTO>>;

  constructor() {
    this.propertyCollection$ = combineLatest([
      this._paginationUpdateObs$
    ]).pipe(
      switchMap(([pagination]) => 
        this.updatePropertyPageData(pagination))); 
  }


  public updatePropertyPage(paginationOptions: PaginationOptions = PropertyFacadeService.DefaultPagingOptions) {
    this._paginationUpdate$.next({...paginationOptions});
  }


  private updatePropertyPageData(paginationOptions: PaginationOptions): Observable<IResponseCollectionDTO<IPropertyListItemDTO>> {
    return this._propertyAPIService.getProperties(paginationOptions.offset, paginationOptions.limit)
      .pipe(map(res => res.data));
  }

  private paginationDistinction(prev: PaginationOptions, curr: PaginationOptions) {
    return prev.offset === curr.offset && prev.limit === curr.limit;
  }

}
