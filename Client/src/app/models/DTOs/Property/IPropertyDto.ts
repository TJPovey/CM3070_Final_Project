import { IResponseDTO } from "../Responses/IResponseDTO";
import { ILocationDto } from "../Shared/ILocationDto";

export interface IPropertyDto extends IResponseDTO<IPropertyDetail> {

}

export interface IPropertyDetail {
    id: string;
    propertyName: string;
    reportTitle: string;
    imageUri?: string;
    writeToken?: string;
    location: ILocationDto;
    userAssignments: IUserAssignment[];
    taskAssignments: ITaskAssignment[];
    ownerId: IOwnerId;
}

export interface ITaskAssignment {
    id: string;
    open: boolean;
    name: string;
    taskCategory: ITaskCategory;
    taskPriority: ITaskPriority;
    location?: ILocationDto;
}

export interface ITaskCategory {
    id: string;
    name: string;
}

export interface ITaskPriority {
    id: string;
    name: string;
}

export interface IUserAssignment {
    id: string;
    userName: string;
    role: IUserRole;
}

export interface IOwnerId {
    id: string;
    name: string;
}

export interface IUserRole {
    id: string;
    name: string;
}
