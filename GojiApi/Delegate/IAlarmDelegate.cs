using GojiApi.Model.Alarm;

namespace GojiApi.Delegate
{
    public interface IAlarmDelegate
    {
        Alarm CreateAlarm(Guid taskItemId, Guid userId, DateTime triggerAtUtc);
        Alarm? GetAlarm(Guid id);
        IEnumerable<Alarm> GetAlarms(Guid? taskItemId, Guid? userId);
        Alarm CancelAlarm(Guid id);
    }
}