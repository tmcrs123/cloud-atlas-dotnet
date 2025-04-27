namespace Cloud_Atlas_Dotnet.Domain.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
    public class SensitiveDataAttribute : Attribute
    {
    }
}
