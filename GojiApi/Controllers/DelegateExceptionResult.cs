using GojiApi.Delegate;

namespace GojiApi.Endpoints
{
    // esto lit nomas es para que arroje errores web een base a las cosas
    // tambien es buena practica aparentemente, pero no es de awiwi
    public static class DelegateExceptionResult
    {
        public static IResult ToResult(this DelegateException ex)
        {
            var statusCode = ex switch
            {
                NotFoundException => StatusCodes.Status404NotFound,
                ConflictException => StatusCodes.Status409Conflict,
                ForbiddenException => StatusCodes.Status403Forbidden,
                _ => StatusCodes.Status400BadRequest
            };

            return Results.Json(new { error = ex.Message }, statusCode: statusCode);
        }
    }
}