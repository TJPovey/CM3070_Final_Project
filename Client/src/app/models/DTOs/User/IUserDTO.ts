import { UserRole } from "../../UserRoles";
import { IResponseDTO } from "../Responses/IResponseDTO";
import { ILocationDto } from "../Shared/ILocationDto";

export interface IUserDTO extends IResponseDTO<IUserDetail> {

}

export interface IUserDetail {
    id: string;
    fullName: string;
    firstName: string;
    lastName: string;
    userName: string;
    email: string;
    propertyAssignments: IPropertyAssignment[];
}

export interface IPropertyAssignment {
    property: IPropertyId;
    role: UserRole;
    imageUri?: string;
    location: ILocationDto;
}

export interface IPropertyId {
    id: string;
    ownerId: string;
    name: string;
}