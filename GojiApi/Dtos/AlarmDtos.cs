namespace GojiApi.Dtos
{
    public record CreateAlarmRequest(
        Guid TaskItemId,
        Guid UserId,
        DateTimeOffset TriggerAt);

    public record AlarmResponse(
        Guid Id, Guid TaskItemId, Guid UserId, DateTime TriggerAtUtc,
        string Status, DateTime? FiredAtUtc, DateTime? CreatedAt)
    {
        public static AlarmResponse From(Model.Alarm.Alarm alarm) =>
            new(alarm.Id, alarm.TaskItemId, alarm.UserId, alarm.TriggerAtUtc,
                alarm.Status.ToString(), alarm.FiredAtUtc, alarm.CreatedAt);
    }
}