namespace GojiApi.Delegate
{
    
    public abstract class DelegateException : Exception
    {
        protected DelegateException(string message) : base(message) { }
    }

    // No existe
    public class NotFoundException : DelegateException
    {
        public NotFoundException(string message) : base(message) { }
    }

    // Duplicados
    public class ConflictException : DelegateException
    {
        public ConflictException(string message) : base(message) { }
    }

    
    public class ForbiddenException : DelegateException
    {
        public ForbiddenException(string message) : base(message) { }
    }

    // Reglas de formato
    public class BusinessRuleException : DelegateException
    {
        public BusinessRuleException(string message) : base(message) { }
    }

    // en general todo esto vi que era buena practica hacerlo
    // - lena
}


