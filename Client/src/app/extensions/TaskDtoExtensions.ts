import { ILocationDto } from "../models/DTOs/Shared/ILocationDto";
import { ITaskPostDto } from "../models/DTOs/Tasks/ITaskPostDto";


export class TaskDtoExtensions {

    static ToTaskPostDto(
        propertyId: string,
        userPropertyOwnerId: string,
        title: string,
        area: string,
        description: string,
        dueDate: string,
        estimatedCost: number,
        category: string,
        priority: string,
        assignedToUserId: string,
        assignedToUserUserName: string,
        latitude: number, 
        longitude: number,
        elevation: number): ITaskPostDto {

        const location = <ILocationDto> {
            latitude,
            longitude,
            elevation
        }

        const dto = <ITaskPostDto> {
            propertyId,
            userPropertyOwnerId,
            title,
            area,
            description,
            dueDate,
            estimatedCost,
            category,
            priority,
            assignedToUserId,
            assignedToUserUserName,
            location
        };

        return dto;
    }
}