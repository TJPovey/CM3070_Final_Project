import { HttpContext, HttpContextToken } from "@angular/common/http";

export const CACHE_URL = new HttpContextToken<boolean>(() => false);

export function cacheURL() {
    return new HttpContext().set(CACHE_URL, true);
}