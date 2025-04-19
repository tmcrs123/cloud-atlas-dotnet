namespace Cloud_Atlas_Dotnet.Libraries.FluentValidation
{
    public class ValidationOutcome
    {
        public bool Valid => Failures is null || !Failures.Any();
        public List<Failure> Failures = new();
    }

    public class Failure
    {
        public string PropertyName { get; set; }
        public string ErrorMessage { get; set; }
        public object AttemptedValue { get; set; }
        public Failure(string propertyName, string errorMessage, object attemptedValue)
        {
            PropertyName = propertyName;
            ErrorMessage = errorMessage;
            AttemptedValue = attemptedValue;
        }
    }
}
