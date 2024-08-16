import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { map, take, tap } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { IPlaceFeature, MapboxOutput } from './geocode.model';


@Injectable({
  providedIn: 'root'
})
export class GeocodeService {

  private _geocodeCache = new Map<string, IPlaceFeature[]>();
  private _geocodeUrl = 'https://api.mapbox.com/geocoding/v5/mapbox.places';

  private http = inject(HttpClient);

  public searchPlace(query: string): Observable<IPlaceFeature[]> {

    if (this._geocodeCache.has(query)) {
      const features = this._geocodeCache.get(query);
      if (features) {
        return this.getPlaceFromCache(features);
      }
    }

    return this.getPlaceFromEndpoint(query);
  }


  private getPlaceFromCache(features: IPlaceFeature[]): Observable<IPlaceFeature[]> {
    return of(features);
  }

  private getPlaceFromEndpoint(query: string): Observable<IPlaceFeature[]> {

    const params = {
      types: "place,postcode,address,poi",
      access_token: environment.mapBoxToken,
      autocomplete: true,
      limit: 7
    };

    const url = `${this._geocodeUrl}/${query}.json`;

    return this.http.get(url, { params: params })
      .pipe(
        take(1),
        map(res => {
          const output = res as MapboxOutput;
          const features = output.features?.map(feature => {
            return {
              place_name: feature?.place_name,
              longitude: feature?.center[0],
              latitude: feature?.center[1]
            } as IPlaceFeature;
          });
          return features;
        }),
        tap(res => this._geocodeCache.set(query, res)));
  }
}