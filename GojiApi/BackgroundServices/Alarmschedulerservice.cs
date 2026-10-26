using GojiApi.Model.Alarm;

namespace GojiApi.BackgroundServices
{
    public class AlarmSchedulerService : BackgroundService
    {
        private const int BatchSize = 50;
        private static readonly TimeSpan LockDuration = TimeSpan.FromSeconds(30);

        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<AlarmSchedulerService> _log;
        private readonly TimeSpan _interval;

        public AlarmSchedulerService(
            IServiceScopeFactory scopeFactory,
            ILogger<AlarmSchedulerService> log,
            IConfiguration configuration)
        {
            _scopeFactory = scopeFactory;
            _log = log;

            var seconds = Math.Clamp(configuration.GetValue("AlarmScheduler:IntervalSeconds", 1), 1, 5);
            _interval = TimeSpan.FromSeconds(seconds);
        }

        protected override async Task ExecuteAsync(CancellationToken ct)
        {
            using var timer = new PeriodicTimer(_interval);

            try
            {
                while (await timer.WaitForNextTickAsync(ct))
                {
                    try
                    {
                        await TickAsync(ct);
                    }
                    catch (Exception ex) when (ex is not OperationCanceledException)
                    {
                        _log.LogError(ex, "Fallo el tick del scheduler de alarmas");
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // Apagado normal
            }
        }

        private async Task TickAsync(CancellationToken ct)
        {
            using var scope = _scopeFactory.CreateScope();
            var alarms = scope.ServiceProvider.GetRequiredService<IAlarm>();
            var observers = scope.ServiceProvider.GetServices<IAlarmObserver>().ToList();

            // Reclamo atomico
            var claimed = alarms.ClaimDue(BatchSize, LockDuration).ToList();

            foreach (var alarm in claimed)
            {
                foreach (var o in observers)
                {
                    try { await o.OnFiredAsync(alarm, ct); }
                    catch (Exception ex) when (ex is not OperationCanceledException)
                    {
                        _log.LogError(ex, "Observador {Observer} fallo en alarma {Id}", o.GetType().Name, alarm.Id);
                    }
                }

                alarm.MarkFired();
                alarms.Update(alarm);
            }
        }
    }
}