using SnagIt.API.Core.Domain.Exceptions;
using SnagIt.API.Core.Domain.SeedWork;
using Newtonsoft.Json;
using SnagIt.API.Core.Domain.Aggregates.Shared;
using NodaTime;

namespace SnagIt.API.Core.Domain.Aggregates.SnagItTask
{
    public class TaskDetail : ValueObject
    {
        [JsonConstructor]
        private TaskDetail(
            string title,
            string area,
            string description,
            bool open,
            Instant dueDate,
            double estimatedCost,
            TaskCategory category,
            TaskPriority priority,
            LocationDetail? locationDetail,
            PropertyId property,
            UserId assignedUser)
        {
            Title = title; 
            Area = area; 
            Description = description;
            Open = open;
            DueDate = dueDate;
            EstimatedCost = estimatedCost;
            TaskCategory = category;
            TaskPriority = priority;
            LocationDetail = locationDetail;
            Property = property;
            AssignedUser = assignedUser;
        }

        public static TaskDetail Create(
            string title,
            string area,
            string description,
            bool open,
            Instant dueDate,
            double estimatedCost,
            TaskCategory category,
            TaskPriority priority,
            LocationDetail? locationDetail,
            PropertyId property,
            UserId assignedUser)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                throw new DomainException($"A value for {nameof(title)} was not supplied.");
            }

            if (string.IsNullOrWhiteSpace(area))
            {
                throw new DomainException($"A value for {nameof(area)} was not supplied.");
            }

            if (string.IsNullOrWhiteSpace(description))
            {
                throw new DomainException($"A value for {nameof(description)} was not supplied.");
            }

            if (double.IsNaN(estimatedCost))
            {
                throw new DomainException($"A value for {nameof(estimatedCost)} was not supplied.");
            }

            if (category is null)
            {
                throw new DomainException($"A value for {nameof(category)} was not supplied.");
            }

            if (priority is null)
            {
                throw new DomainException($"A value for {nameof(priority)} was not supplied.");
            }

            if (locationDetail is null)
            {
                throw new DomainException($"A value for {nameof(locationDetail)} was not supplied.");
            }

            if (property is null)
            {
                throw new DomainException($"A value for {nameof(property)} was not supplied.");
            }

            if (assignedUser is null)
            {
                throw new DomainException($"A value for {nameof(assignedUser)} was not supplied.");
            }

            return new TaskDetail(
                title,
                area,
                description,
                open,
                dueDate,
                estimatedCost,
                category,
                priority,
                locationDetail,
                property,
                assignedUser);
        }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Title;
            yield return Area;
            yield return Description;
            yield return Open;
            yield return DueDate;
            yield return EstimatedCost;
            yield return TaskCategory;
            yield return TaskPriority;
            yield return LocationDetail;
            yield return Property;
            yield return AssignedUser;
        }

        public string Title { get; set; }
        public string Area { get; set; }
        public string Description { get; set; }
        public bool Open { get; set; }
        public Instant DueDate { get; set; }
        public double EstimatedCost { get; set; }
        public TaskCategory TaskCategory { get; set; }
        public TaskPriority TaskPriority { get; set; }
        public LocationDetail? LocationDetail { get; private set; }
        public PropertyId Property { get; }
        public UserId AssignedUser { get; }
    }
}
