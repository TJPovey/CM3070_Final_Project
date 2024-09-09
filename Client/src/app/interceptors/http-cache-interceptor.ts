import { HttpInterceptorFn, HttpParams, HttpRequest, HttpResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { filter, of, tap } from 'rxjs';
import { HttpCacheStorageService } from '../services/storage/http-cache-storage.service';
import { CACHE_URL } from '../services/storage/models/CACHE_URL';

export const httpCacheInterceptor: HttpInterceptorFn = (req, next) => {

    const cacheStorage = inject(HttpCacheStorageService);

    // changes have been made to the db, delete the cache
    // if this app ever gets bigger - we should have seperate instances 
    // of this interceptor to only remove relevant caches.
    if (isModifyRequest(req)) {
      cacheStorage.removeAll();
      return next(req);
    }

    // get requests have a http context param added to let us know if the request is to be cached
    if(!isRequestCachable(req)) {
      return next(req);
    }

    // sort the query params alphabetically, as the same query may have params in a different order
    const urlWithSortedQueries = buildURLWithSortedQueries(req);
    const cachedResponse = cacheStorage.getItem<HttpResponse<unknown>>(urlWithSortedQueries, false);

    if (cachedResponse) {
      return of(cachedResponse.clone());
    }

    // cache the response
    return next(req).pipe(
      filter(event => event instanceof HttpResponse),
      tap(res => cacheStorage.setItem(urlWithSortedQueries, (res as HttpResponse<unknown>).clone()))
    );
}

const isModifyRequest = (req: HttpRequest<unknown>) => {
  return  req.method === 'POST' || 
          req.method === 'PUT' ||
          req.method === 'PATCH' ||
          req.method === 'DELETE';
}


const isRequestCachable = (req: HttpRequest<unknown>) =>
  req.method === 'GET' && req.context.get(CACHE_URL);  


const buildURLWithSortedQueries = (req: HttpRequest<unknown>): string => {
  const sortedParams = sortQueryParams(req.params);
  const sortedUrl = req.clone({params: sortedParams});
  return sortedUrl.urlWithParams;
}

const sortQueryParams = (params: HttpParams) => {
  let newParams = new HttpParams(); 
  const keys = params.keys().sort();
  keys.forEach(key => {
    const value =  params.get(key);
    // Some queries accept an empty string as their value
    // or should we always set null/undefined values to an empty string?
    // value = value ?? ""
    if (value || value === "") {
      newParams = newParams.append(key, value);
    }
  });
  return newParams;
}