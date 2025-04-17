using System.Linq.Expressions;
using System.Reflection;

namespace Cloud_Atlas_Dotnet.Libraries
{
    public interface IRule<T>
    {
        public bool Validate(T instance);
    }
    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public List<ValidationFailure> ValidationFailures = new();
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
    }
    public class Rules<T, TProperty>
    {
        internal string PropertyName { get; set; }
        internal Func<T, TProperty> PropertyAccessor { get; set; }
        internal List<Func<TProperty, bool>> validationFns = new();
        internal List<ValidationFailure> validationFailures = new();

        internal Rules<T, TProperty> AddRule(Func<TProperty, bool> rule)
        {
            validationFns.Add(rule);
            return this;
        }

        internal bool Validate(T instance)
        {
            if (validationFns == null) return true;
            var value = PropertyAccessor(instance);

            var valid = true;
                var hasFailedValidations = validationFns.Select(fn => fn(value)).Distinct().Where(x => !x).Any();

            return hasFailedValidations ? false : true;
        }
    }

    public abstract class BaseFluentValidator<T> : IValidator<T>
    {
        private dynamic _rules;
        public dynamic rules
        {
            get { return _rules; }
            set { _rules = value; }
        }

        public Rules<T, TProperty> RuleFor<TProperty>(Expression<Func<T, TProperty>> expression)
        {
            var body = expression.Body;

            if (expression.Body is UnaryExpression unary && unary.NodeType == ExpressionType.Convert)
            {
                body = unary.Operand;
            }

            if (body is MemberExpression memberExpression)
            {
                var propName = memberExpression.Member.Name;
                var propertyType = ((PropertyInfo)memberExpression.Member).PropertyType;

                rules = new Rules<T, TProperty>(); //turn an expression tree into a delegate/lambda
                rules.PropertyName = propName;
                rules.PropertyAccessor = expression.Compile();
                return rules;
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        public ValidationResult Validate(T instance)
        {
            rules.Validate(instance);

            return new ValidationResult() { IsValid = rules.validationFailures.Count == 0, ValidationFailures = rules.validationFailures};
        }
    }

    public static class NumericRuleExtensions
    {
        public static Rules<T, int> LessThan<T>(this Rules<T, int> builder, int target)
        {
            return builder.AddRule(x =>
            {
                if (x < target)
                {
                    return true;
                }

                builder.validationFailures.Add(new ValidationFailure(builder.PropertyName, $"{x} needs to be less than {target}", target));

                return false;
            }
            );
        }

        public static Rules<T, int> BiggerThan<T>(this Rules<T, int> builder, int target)
        {
            return builder.AddRule(x =>
            {
                if (x > target)
                {
                    return true;
                }

                builder.validationFailures.Add(new ValidationFailure(builder.PropertyName, $"{x} needs to be bigger than {target}", target));

                return false;
            }
            );
        }
    }

    public static class StringRuleExtensions
    {
        public static Rules<T, string> MustBeDifferentFrom<T>(this Rules<T, string> builder, string target)
        {
            return builder.AddRule(x =>
            {
                if (x.Split("").SequenceEqual(target.Split("")))
                {
                    builder.validationFailures.Add(new ValidationFailure(builder.PropertyName, $"{x} cannot equal string", target));
                    return false;
                }

                return true;

            });
        }

        public static Rules<T, string> NotNullOrEmpty<T>(this Rules<T, string> builder)
        {
            return builder.AddRule(x =>
            {
                if (string.IsNullOrEmpty(x))
                {
                    builder.validationFailures.Add(new ValidationFailure(builder.PropertyName, $"{x} cannot be null or empty", string.Empty));

                    return false;
                }
                return true;
            });
        }

        public static Rules<T, string> AtLeastXCharactersLong<T>(this Rules<T, string> builder, int target)
        {
            return builder.AddRule(x =>
            {
                if (x.Length < target)
                {
                    builder.validationFailures.Add(new ValidationFailure(builder.PropertyName, $"{x} must be at least ${target} characters long", x.Length));

                    return false;
                }
                return true;
            });
        }
    }

    public interface IValidator<T>
    {
        public ValidationResult Validate(T instance);
    }
}
