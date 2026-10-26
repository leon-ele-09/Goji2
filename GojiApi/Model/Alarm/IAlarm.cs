namespace GojiApi.Model.Alarm
{
    public interface IAlarm
    {
        void Add(Alarm alarm);
        Alarm? GetById(Guid id);
        IEnumerable<Alarm> GetByTaskItemId(Guid taskItemId);
        IEnumerable<Alarm> GetByUserId(Guid userId);
        void Update(Alarm alarm);
        void Delete(Alarm alarm);

        // Solo lo usa el scheduler: reclama de forma atómica las alarmas vencidas
        IEnumerable<Alarm> ClaimDue(int batchSize, TimeSpan lockDuration);
    }
}