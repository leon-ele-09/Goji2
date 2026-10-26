namespace GojiApi.Model.Alarm
{
    public class AlarmObserver : IAlarmObserver
    {
        private readonly ILogger<AlarmObserver> _log;
        public AlarmObserver(ILogger<AlarmObserver> log) => _log = log;

        public Task OnFiredAsync(Alarm alarm, CancellationToken ct)
        {
            _log.LogInformation("Alarma {Id} disparada", alarm.Id);
            return Task.CompletedTask;
        }
    }
}
