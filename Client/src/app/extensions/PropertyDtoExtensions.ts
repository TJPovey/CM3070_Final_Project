import { IPropertyPostDto } from "../models/DTOs/Property/IPropertyPostDto";
import { ILocationDto } from "../models/DTOs/Shared/ILocationDto";


export class PropertyDtoExtensions {

    static ToPropertyPostDto(
        propertyName: string, 
        reportTitle: string, 
        latitude: number, 
        longitude: number,
        elevation: number): IPropertyPostDto {

        const location = <ILocationDto> {
            latitude,
            longitude,
            elevation
        }

        const dto = <IPropertyPostDto> {
            propertyName,
            reportTitle,
            location
        };

        return dto;
    }
}