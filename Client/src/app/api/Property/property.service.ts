import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { IPropertyListGetDTO } from 'src/app/models/DTOs/Property/IPropertyListGetDTO';
import { mockPropertyList } from '../Mocks/mock-property-data';
import { GUID } from 'src/app/helpers/Guid';

@Injectable({
  providedIn: 'root'
})
export class PropertyService {

  private _mockPropertyList = [...mockPropertyList];


  getProperties(offset: number = 0, limit: number = 10): Observable<IPropertyListGetDTO> {
    /// Calculate start and end indices for pagination
    const startIndex = offset;
    const endIndex = startIndex + limit;

    // Get the paginated items
    const paginatedItems = this._mockPropertyList.slice(startIndex, endIndex);

    // Calculate total pages
    const totalPages = Math.ceil(this._mockPropertyList.length / limit);

    // Create the response DTO
    const response: IPropertyListGetDTO = {
      apiVersion: '1.0',
      context: GUID.GeneratGuid(),
      id: GUID.GeneratGuid(),
      method: 'propertcollection.get',
      data: {
        kind: 'PropertyCollection',
        currentItemCount: paginatedItems.length,
        items: paginatedItems,
        pageIndex: Math.floor(offset / limit) + 1,
        startIndex: startIndex,
        totalItems: mockPropertyList.length,
        totalPages: totalPages
      },
      error: undefined
    };

    return of(response);
  }
}
