using System.ComponentModel.DataAnnotations;


namespace SnagIt.API.Core.Application.Extensions.Exceptions
{
    public static class ExceptionExtensions
    {
        public static string GetTitle(this Exception ex)
        {
            if (ex is ValidationException)
            {
                return (ex as ValidationException).GetTitle();
            }

            return $"{ex.GetType()}";
        }

        public static string GetErrorDetail(this Exception ex)
        {
            if (ex is ValidationException)
            {
                return (ex as ValidationException).GetErrorDetail();
            }

            return $"{ex.Message} {ex.InnerException?.Message}";
        }
    }
}
