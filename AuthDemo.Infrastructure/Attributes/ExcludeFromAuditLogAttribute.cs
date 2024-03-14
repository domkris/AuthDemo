namespace AuthDemo.Infrastructure.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    internal class ExcludeFromAuditLogAttribute : Attribute
    {
    }
}
