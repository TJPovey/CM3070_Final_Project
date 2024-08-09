import { HttpContextToken } from "@angular/common/http";
import { CACHE_URL, cacheURL } from "./CACHE_URL";

describe("cacheURL", () => {

  it("should set CACHE_URL token on HttpContext", () => {
    const httpContext = cacheURL();
    expect(httpContext.has(CACHE_URL)).toBe(true);
  });


  it("should set CACHE_URL to have value set to true", () => {
    const httpContext = cacheURL();
    expect(httpContext.get(CACHE_URL)).toBe(true);
  });


  it("should create a new token instance for CACHE_URL", () => {
    const cacheToken = new HttpContextToken<boolean>(() => false);
    expect(CACHE_URL).not.toBe(cacheToken);
  });

});
