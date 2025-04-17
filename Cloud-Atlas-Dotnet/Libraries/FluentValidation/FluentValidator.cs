using Cloud_Atlas_Dotnet.Libraries.FluentValidation.Interfaces;
using System.Linq.Expressions;

namespace Cloud_Atlas_Dotnet.Libraries.FluentValidation
{
    public class PropertyRuleBuilder<T, TProperty> : IPropertyRuleBuilder<T>
    {
        private readonly Expression<Func<T, TProperty>> _propertySelectorExpression;
        private readonly Func<T, TProperty> _propertySelector;
        
        internal readonly string _propertyName;
        internal List<Func<TProperty, bool>> validationFns = new();
        public List<Failure> Failures { get; set; } = new();

        public PropertyRuleBuilder(Expression<Func<T, TProperty>> propertySelectorExpression)
        {
            _propertySelectorExpression = propertySelectorExpression;
            _propertySelector = _propertySelectorExpression.Compile();
            _propertyName = GetPropertyDetails(propertySelectorExpression);
        }

        private static string GetPropertyDetails(Expression<Func<T, TProperty>> propertySelector)
        {
            var body = propertySelector.Body;

            if (propertySelector.Body is UnaryExpression unary && unary.NodeType == ExpressionType.Convert)
            {
                body = unary.Operand;
            }

            if (body is MemberExpression memberExpression)
            {
                return memberExpression.Member.Name;
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        public bool Validate(T objectToValidate)
        {
            var propertyValue = _propertySelector(objectToValidate);

            IEnumerable<bool> executedValidatorsResults = validationFns.Select(fn => fn(propertyValue));

            return executedValidatorsResults.Distinct().Count() == 1 && executedValidatorsResults.First();
        }
    }

    public abstract class FluentValidator<T> : IValidator<T>
    {
        public List<IPropertyRuleBuilder<T>> _propertyRuleBuilderList = new();

        public PropertyRuleBuilder<T, TProperty> RuleFor<TProperty>(Expression<Func<T, TProperty>> propertySelectorExpression)
        {
            var propertyRuleBuilder = new PropertyRuleBuilder<T, TProperty>(propertySelectorExpression);
            _propertyRuleBuilderList.Add(propertyRuleBuilder);
            return propertyRuleBuilder;
        }
        public ValidationOutcome Validate(T objectToValidate)
        {
            //ALL validators for ALL properties
            var executedValidatorsResult = _propertyRuleBuilderList.Select(rb => rb.Validate(objectToValidate));

            var allValid = executedValidatorsResult.Distinct().Count() == 1 && executedValidatorsResult.First();

            if (allValid) return new ValidationOutcome() { Failures = null };
            else
            {
                var allFailures = _propertyRuleBuilderList.Select(x => x.Failures).SelectMany(x => x).ToList();

                return new ValidationOutcome() { Failures = allFailures };
            } 
                
        }
    }
}
