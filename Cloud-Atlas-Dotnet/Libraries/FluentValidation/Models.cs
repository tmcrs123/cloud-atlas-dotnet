using System.Text;

namespace Cloud_Atlas_Dotnet.Libraries.FluentValidation
{
    public class ValidationOutcome
    {
        public bool Valid => ValidationFailures is null || !ValidationFailures.Any();
        public IDictionary<string, List<ValidationFailure>?> ValidationFailures = new Dictionary<string, List<ValidationFailure>?>();

        public string PrintAllFailures()
        {
            if (!this.ValidationFailures.Any()) return string.Empty;

            StringBuilder sb = new StringBuilder();

            foreach (KeyValuePair<string, List<ValidationFailure>> pair in ValidationFailures)
            {
                sb.Append(string.Join(',', pair.Value));
                sb.AppendLine();
            }

            return sb.ToString();
        }
    }

    public class ValidationFailure
    {
        public string PropertyName { get; set; }
        public string ErrorMessage { get; set; }
        public object AttemptedValue { get; set; }
        public ValidationFailure(string propertyName, string errorMessage, object attemptedValue)
        {
            PropertyName = propertyName;
            ErrorMessage = errorMessage;
            AttemptedValue = attemptedValue;
        }

        public override string ToString()
        {
            return $"{PropertyName}: {ErrorMessage}; Attempted value: {(AttemptedValue == null ? "null" : AttemptedValue)}";
        }
    }
}
