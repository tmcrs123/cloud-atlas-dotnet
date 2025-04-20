namespace Cloud_Atlas_Dotnet.Libraries.FluentValidation.Interfaces
{
    public interface IValidator<T>
    {
        public ValidationOutcome Validate(T instance);
    }

    public interface IPropertyRuleBuilder<T>
    {
        bool Validate(T instance);

        Dictionary<string, List<ValidationFailure>> Failures { get; }
    }
}
