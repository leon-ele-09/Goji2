namespace GojiApi.Model.Alarm
{
    public enum AlarmStatus { Pending, Processing, Fired, Cancelled }

    public class Alarm
    {
        public Guid Id { get; private set; }
        public Guid TaskItemId { get; set; }
        public Guid UserId { get; set; }
        public DateTime TriggerAtUtc { get; private set; }
        public AlarmStatus Status { get; private set; }
        public DateTime? FiredAtUtc { get; private set; }
        public DateTime? LockedUntilUtc { get; private set; }
        public DateTime? CreatedAt { get; private set; }

        public static Alarm Create(Guid taskItemId, Guid userId, DateTime triggerAtUtc) => new Alarm
        {
            Id = Guid.NewGuid(),
            Status = AlarmStatus.Pending,
            CreatedAt = DateTime.UtcNow
        }.WithTaskItem(taskItemId).WithUser(userId).WithTriggerAt(triggerAtUtc);

        public static Alarm Hydrate(
            Guid id, Guid taskItemId, Guid userId, DateTime triggerAtUtc,
            AlarmStatus status, DateTime? firedAtUtc, DateTime? lockedUntilUtc, DateTime createdAt) =>
            new Alarm
            {
                Id = id,
                TaskItemId = taskItemId,
                UserId = userId,
                TriggerAtUtc = triggerAtUtc,
                Status = status,
                FiredAtUtc = firedAtUtc,
                LockedUntilUtc = lockedUntilUtc,
                CreatedAt = createdAt
            };

        public Alarm WithTaskItem(Guid id)
        {
            if (id == Guid.Empty) { throw new ArgumentException("Invalid TaskItem"); }

            TaskItemId = id;
            return this;
        }

        public Alarm WithUser(Guid id)
        {
            if (id == Guid.Empty) { throw new ArgumentException("Invalid User"); }

            UserId = id;
            return this;
        }

        public Alarm WithTriggerAt(DateTime triggerAtUtc)
        {
            if (Status != AlarmStatus.Pending) { throw new InvalidOperationException("Only a pending alarm can be rescheduled"); }
            if (triggerAtUtc.Kind != DateTimeKind.Utc) { throw new ArgumentException("TriggerAt must be UTC"); }
            if (triggerAtUtc <= DateTime.UtcNow) { throw new ArgumentException("TriggerAt must be in the future"); }

            TriggerAtUtc = triggerAtUtc;
            return this;
        }

        public Alarm Cancel()
        {
            if (Status != AlarmStatus.Pending) { throw new InvalidOperationException("Only a pending alarm can be cancelled"); }

            Status = AlarmStatus.Cancelled;
            return this;
        }

        public Alarm Claim(DateTime lockedUntilUtc)
        {
            var now = DateTime.UtcNow;
            var lockExpired = Status == AlarmStatus.Processing && LockedUntilUtc <= now;

            if (Status != AlarmStatus.Pending && !lockExpired) { throw new InvalidOperationException("Alarm cannot be claimed"); }
            if (lockedUntilUtc <= now) { throw new ArgumentException("Invalid LockedUntil"); }

            Status = AlarmStatus.Processing;
            LockedUntilUtc = lockedUntilUtc;
            return this;
        }

        public Alarm MarkFired()
        {
            if (Status != AlarmStatus.Processing) { throw new InvalidOperationException("Only a processing alarm can be fired"); }

            Status = AlarmStatus.Fired;
            FiredAtUtc = DateTime.UtcNow;
            LockedUntilUtc = null;
            return this;
        }
    }
}