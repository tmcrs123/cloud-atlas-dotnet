using System.Linq.Expressions;
using System.Reflection;

namespace Cloud_Atlas_Dotnet.Libraries
{
    public class ValidationOutcome
    {
        public bool valid => !Failures.Any();
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

    public class PropertyRuleBuilder<T, TProperty>
    {
        private readonly Expression<Func<T, TProperty>> _propertySelectorExpression;
        public readonly Func<T, TProperty> propertySelector;
        public readonly string _propertyName;
        public readonly Type propertyType;
        public List<Func<TProperty, bool>> validationFns = new();
        public List<Failure> failures = new();

        public PropertyRuleBuilder(Expression<Func<T, TProperty>> propertySelectorExpression)
        {
            _propertySelectorExpression = propertySelectorExpression;
            propertySelector = _propertySelectorExpression.Compile();
            propertyType = GetPropertyName(propertySelectorExpression).Item1;
            _propertyName = GetPropertyName(propertySelectorExpression).Item2;
        }

        public PropertyRuleBuilder<T, TProperty> RuleFor(Expression<Func<T, TProperty>> propertySelectorExpression)
        {
            return new PropertyRuleBuilder<T, TProperty>(propertySelectorExpression);
        }

        private static (Type,string) GetPropertyName(Expression<Func<T, TProperty>> propertySelector)
        {
            var body = propertySelector.Body;

            if (propertySelector.Body is UnaryExpression unary && unary.NodeType == ExpressionType.Convert)
            {
                body = unary.Operand;
            }

            if (body is MemberExpression memberExpression)
            {
                return (((PropertyInfo)memberExpression.Member).PropertyType, memberExpression.Member.Name);
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        public PropertyRuleBuilder<T, TProperty> NotNull()
        {
            validationFns.Add(propertyValue =>
            {
                if (propertyValue == null)
                {
                    failures.Add(new Failure(_propertyName, "null", null));
                    return false;
                }

                return true;
            });

            return this;
        }

        public bool Validate(T objectToValidate)
        {
            var propertyValue = propertySelector(objectToValidate);

            var allValidatorsExecuted = validationFns.Select(fn => fn(propertyValue));

            var anyFailures = allValidatorsExecuted.Distinct().Contains(false);

            return anyFailures ? false : true;
        }
    }

    public static class PropertyRuleBuilderExtensions
    {
        public static PropertyRuleBuilder<T, int> NumberLessThanTarget<T>(this PropertyRuleBuilder<T, int> propertyRuleBuilder, int target )
        {
            propertyRuleBuilder.validationFns.Add(propertyValue =>
            {
                if (propertyValue > target)
                {
                    propertyRuleBuilder.failures.Add(new Failure(propertyRuleBuilder._propertyName, "100", propertyValue));
                    return false;
                }

                return true;

            });

            return propertyRuleBuilder;
        }

        public static PropertyRuleBuilder<T, int> Even<T>(this PropertyRuleBuilder<T, int> propertyRuleBuilder)
        {
            propertyRuleBuilder.validationFns.Add(propertyValue =>
            {
                if (propertyValue % 2 == 0)
                {
                    propertyRuleBuilder.failures.Add(new Failure(propertyRuleBuilder._propertyName, "Not even", propertyValue));
                    return false;
                }

                return true;

            });

            return propertyRuleBuilder;
        }

        public static PropertyRuleBuilder<T, string> Not<T>(this PropertyRuleBuilder<T, string> propertyRuleBuilder, string target)
        {
            propertyRuleBuilder.validationFns.Add(propertyValue =>
            {
                if (propertyValue.Split("").SequenceEqual(target.Split()))
                {
                    propertyRuleBuilder.failures.Add(new Failure(propertyRuleBuilder._propertyName, "Cannot use this string", propertyValue));
                    return false;
                }

                return true;

            });

            return propertyRuleBuilder;
        }

    }

    public abstract class SecondBaseValidator<T> : ISecondValidator<T>
    {

        public readonly ValidationOutcome validationOutcome;
        public List<dynamic> _propertyRuleBuilderList = new();


        public PropertyRuleBuilder<T, TProperty> RuleFor<TProperty>(Expression<Func<T, TProperty>> propertySelectorExpression)
        {
            var propertyRuleBuilder = new PropertyRuleBuilder<T, TProperty>(propertySelectorExpression);
            _propertyRuleBuilderList.Add(propertyRuleBuilder);
            return propertyRuleBuilder;
        }


        public ValidationOutcome Validate(T objectToValidate)
        {
            var allValidatorsExecuted = _propertyRuleBuilderList.Select(rb => rb.Validate(objectToValidate));

            var anyFailures = allValidatorsExecuted.Distinct().Contains(false);

            if (anyFailures) return new ValidationOutcome() { Failures = null };
            else return new ValidationOutcome() { Failures = null };
        }
    }

    public interface ISecondValidator<T>
    {
        public ValidationOutcome Validate(T instance);
    }

    public interface IPropertyRuleBuilder { }
       
}
