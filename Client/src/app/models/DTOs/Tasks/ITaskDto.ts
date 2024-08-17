import { IResponseDTO } from "../Responses/IResponseDTO";
import { ILocationDto } from "../Shared/ILocationDto";

export interface ITaskDto extends IResponseDTO<ITaskDetail> {

}

export interface ITaskDetail {
    id: string;
    propertyId: string;
    title: string;
    open: boolean;
    area: string;
    description: string;
    imageUri?: string;
    dueDate: string;
    estimatedCost: number;
    category: ICategory;
    priority: IPriority;
    assignedUser: IUserAssignment;
    location?: ILocationDto;
}

export interface IPriority {
    id: string;
    name: string;
}

export interface ICategory {
    id: string;
    name: string;
}

export interface IUserAssignment {
    id: string;
    userName: string;
}