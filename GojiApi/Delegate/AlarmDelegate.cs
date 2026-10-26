using GojiApi.Model.Alarm;
using GojiApi.Model.Project;
using GojiApi.Model.TaskItem;
using GojiApi.Model.User;

namespace GojiApi.Delegate
{
    public class AlarmDelegate : IAlarmDelegate
    {
        private readonly IAlarm _alarms;
        private readonly ITaskItem _tasks;
        private readonly IProject _projects;
        private readonly IUser _users;

        public AlarmDelegate(IAlarm alarms, ITaskItem tasks, IProject projects, IUser users)
        {
            _alarms = alarms;
            _tasks = tasks;
            _projects = projects;
            _users = users;
        }

        public Alarm CreateAlarm(Guid taskItemId, Guid userId, DateTime triggerAtUtc)
        {
            var task = _tasks.GetById(taskItemId)
                ?? throw new NotFoundException($"Tarea '{taskItemId}' no existe.");

            var user = _users.GetById(userId)
                ?? throw new NotFoundException($"Usuario '{userId}' no existe.");

            if (!user.Active)
            {
                throw new BusinessRuleException("Usuario no esta activo.");
            }

            // Regla opcional: el usuario debe pertenecer al proyecto de la tarea
            var project = _projects.GetById(task.ProjectId)
                ?? throw new NotFoundException($"Proyecto '{task.ProjectId}' no existe.");

            if (!project.HasUser(user.Id))
            {
                throw new BusinessRuleException("Usuario no esta en proyecto.");
            }

            var alarm = Alarm.Create(task.Id, user.Id, triggerAtUtc);

            _alarms.Add(alarm);
            return alarm;
        }

        public Alarm? GetAlarm(Guid id) => _alarms.GetById(id);

        public IEnumerable<Alarm> GetAlarms(Guid? taskItemId, Guid? userId)
        {
            IEnumerable<Alarm> results;

            if (taskItemId is not null)
            {
                results = _alarms.GetByTaskItemId(taskItemId.Value);
            }
            else if (userId is not null)
            {
                results = _alarms.GetByUserId(userId.Value);
            }
            else
            {
                throw new BusinessRuleException("Falta tarea y/o usuario.");
            }

            if (taskItemId is not null && userId is not null)
            {
                results = results.Where(a => a.UserId == userId.Value);
            }

            return results;
        }

        public Alarm CancelAlarm(Guid id)
        {
            var alarm = _alarms.GetById(id)
                ?? throw new NotFoundException($"Alarma '{id}' no existe.");

            try
            {
                alarm.Cancel();
            }
            catch (InvalidOperationException ex)
            {
                throw new ConflictException(ex.Message);
            }

            _alarms.Update(alarm);
            return alarm;
        }
    }
}