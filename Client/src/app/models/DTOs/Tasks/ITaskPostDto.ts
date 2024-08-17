import { ILocationDto } from "../Shared/ILocationDto";

export interface ITaskPostDto {
    propertyId: string;
    userPropertyOwnerId: string;
    title: string;
    area: string;
    description: string;
    dueDate: string;
    estimatedCost: number;
    category: string;
    priority: string;
    assignedToUserId: string;
    assignedToUserUserName: string;
    location: ILocationDto; 
}