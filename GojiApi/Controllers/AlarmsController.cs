using GojiApi.Delegate;
using GojiApi.Dtos;

namespace GojiApi.Endpoints
{
    public static class AlarmsEndpoints
    {
        public static IEndpointRouteBuilder MapAlarmsEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/alarms").WithTags("Alarms");

            // POST /alarms
            // Crea una alarma para una tarea y un usuario
            group.MapPost("/", Create);

            // GET /alarms/{id}
            group.MapGet("/{id:guid}", GetById)
                 .WithName("GetAlarmById");

            // GET /alarms?taskItemId=...&userId=...
            group.MapGet("/", GetMany);

            // POST /alarms/{id}/cancel
            group.MapPost("/{id:guid}/cancel", Cancel);

            return app;
        }

        private static IResult Create(CreateAlarmRequest request, IAlarmDelegate alarmDelegate)
        {
            try
            {
                var alarm = alarmDelegate.CreateAlarm(
                    request.TaskItemId, request.UserId, request.TriggerAt.UtcDateTime);

                return Results.CreatedAtRoute("GetAlarmById", new { id = alarm.Id }, AlarmResponse.From(alarm));
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
            catch (DelegateException ex)
            {
                return ex.ToResult();
            }
        }

        private static IResult GetById(Guid id, IAlarmDelegate alarmDelegate)
        {
            var alarm = alarmDelegate.GetAlarm(id);
            return alarm is null ? Results.NotFound() : Results.Ok(AlarmResponse.From(alarm));
        }

        private static IResult GetMany(Guid? taskItemId, Guid? userId, IAlarmDelegate alarmDelegate)
        {
            try
            {
                var results = alarmDelegate.GetAlarms(taskItemId, userId);
                return Results.Ok(results.Select(AlarmResponse.From));
            }
            catch (DelegateException ex)
            {
                return ex.ToResult();
            }
        }

        private static IResult Cancel(Guid id, IAlarmDelegate alarmDelegate)
        {
            try
            {
                var alarm = alarmDelegate.CancelAlarm(id);
                return Results.Ok(AlarmResponse.From(alarm));
            }
            catch (DelegateException ex)
            {
                return ex.ToResult();
            }
        }
    }
}