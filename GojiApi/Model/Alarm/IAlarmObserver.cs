namespace GojiApi.Model.Alarm
{
    public interface IAlarmObserver
    {
        Task OnFiredAsync(Alarm alarm, CancellationToken ct);
    }
}
