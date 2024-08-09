import { IResponseCollectionDTO } from "../Responses/IResponseCollectionDTO";
import { IResponseDTO } from "../Responses/IResponseDTO";


export interface IPropertyListGetDTO extends IResponseDTO<IResponseCollectionDTO<IPropertyListItemDTO>> {
    
}

export interface IPropertyListItemDTO {
    id: string;
    title: string;
    description: string;
    iconURL: string;
}