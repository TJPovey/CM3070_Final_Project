export interface IPlaceFeature {
    place_name: string;
    longitude: number,
    latitude: number
  }
  
  export interface MapboxOutput {
    attribution: string;
    features: Feature[];
    query: [];
  }
  
  export interface Feature {
    place_name: string;
    center: number[];
  }