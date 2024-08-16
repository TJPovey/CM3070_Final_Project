import { HttpClient, HttpHeaders, HttpParams } from "@angular/common/http";
import { Inject, Injectable } from '@angular/core';
import { Observable } from "rxjs";
import { IResponseDTO } from "src/app/models/DTOs/Responses/IResponseDTO";
import { cacheURL } from "src/app/services/storage/models/CACHE_URL";
import { environment } from "src/environments/environment.local";

//! TODO - move to app-extensions lib? will make sense as that lib is for typical angular lib services

@Injectable({
    providedIn: 'root'
})
export class BackendService {

    private _rootUrl!: string

    constructor(private http: HttpClient) {
        this._rootUrl = environment.gatewayOrigin
    }


    createUrl(path: string) {
        path = path.startsWith('/') ? path.substring(1) : path
        return `${this._rootUrl}/${path}/`;
    }

    get<T>(path: string, params?: HttpParams, cache?: boolean): Observable<T> {
        const url = this.createUrl(path);
        return this.http.get<T>(url, {
            params: params || {},
            context: cache ? cacheURL() : undefined
        });
    }

    post<T, V>(path: string, body: T, params?: HttpParams, headers?: HttpHeaders): Observable<V> {
        const url = this.createUrl(path);
        return this.http.post<V>(url, body, {
            params: params || {},
            headers: headers || {}
        });
    }

    put<T, V>(path: string, body: T, params?: HttpParams, headers?: HttpHeaders): Observable<V> {
        const url = this.createUrl(path);
        return this.http.put<V>(url, body, {
            params: params || {},
            headers: headers || {}
        });
    }


    patch<T>(path: string, body: T, params?: HttpParams): Observable<IResponseDTO<T>> {
        const url = this.createUrl(path);
        return this.http.patch<IResponseDTO<T>>(url, body, {
            params: params || {}
        });
    }

    delete<T>(path: string, params?: HttpParams): Observable<T> {
        const url = this.createUrl(path);
        return this.http.delete<T>(url, {
            params: params || {}
        });
    }
}
