
namespace Cloud_Atlas_Dotnet.Libraries.FluentValidation
{
    public static class PropertyRuleBuilderGenericExtensions
    {

        private static void AddFailureToList<T, TProperty>(PropertyRuleBuilder<T, TProperty> propertyRuleBuilder, ValidationFailure failure)
        {
            if (propertyRuleBuilder.Failures.ContainsKey(propertyRuleBuilder._propertyName))
            {
                // is list instantiated?
                propertyRuleBuilder.Failures[propertyRuleBuilder._propertyName].Add(failure);
            }
            else
            {
                List<ValidationFailure> failuresList = new List<ValidationFailure>()
                        {
                    failure
                };
                propertyRuleBuilder.Failures.Add(propertyRuleBuilder._propertyName, failuresList);
            }
        }

        public static PropertyRuleBuilder<T, TProperty> NotNull<T, TProperty>(this PropertyRuleBuilder<T, TProperty> propertyRuleBuilder)
        {
            propertyRuleBuilder.validationFns.Add(propertyValue =>
            {
                if (propertyValue == null)
                {
                    ValidationFailure failure = new ValidationFailure(propertyRuleBuilder._propertyName, $"Null value not allowed for property {propertyRuleBuilder._propertyName}", null);

                    AddFailureToList(propertyRuleBuilder, failure);

                    return false;
                }

                return true;
            });

            return propertyRuleBuilder;
        }

        public static PropertyRuleBuilder<T, int> NumberLessThan<T>(this PropertyRuleBuilder<T, int> propertyRuleBuilder, int target)
        {
            propertyRuleBuilder.validationFns.Add(propertyValue =>
            {
                if (propertyValue > target)
                {
                    AddFailureToList(propertyRuleBuilder, new ValidationFailure(propertyRuleBuilder._propertyName, $"Property {propertyRuleBuilder._propertyName} needs to be less than {target}", propertyValue));
                    return false;
                }
                return true;
            });
            return propertyRuleBuilder;
        }

        public static PropertyRuleBuilder<T,int> Even<T>(this PropertyRuleBuilder<T, int> propertyRuleBuilder)
        {
            propertyRuleBuilder.validationFns.Add(propertyValue =>
            {
                if (propertyValue % 2 == 0)
                {
                    AddFailureToList(propertyRuleBuilder, new ValidationFailure(propertyRuleBuilder._propertyName, $"{propertyRuleBuilder._propertyName} needs to be an even number.", propertyValue));
                    return false;
                }

                return true;

            });

            return propertyRuleBuilder;
        }

        public static PropertyRuleBuilder<T, string> DifferentFrom<T>(this PropertyRuleBuilder<T, string> propertyRuleBuilder, string target)
        {
            propertyRuleBuilder.validationFns.Add(propertyValue =>
            {
                if (propertyValue.Split("").SequenceEqual(target.Split()))
                {
                    AddFailureToList(propertyRuleBuilder, new ValidationFailure(propertyRuleBuilder._propertyName, $"{propertyRuleBuilder._propertyName} needs to be different from ${target}", propertyValue));
                    return false;
                }
                return true;
            });
            return propertyRuleBuilder;
        }

        public static PropertyRuleBuilder<T, string> MinLength<T>(this PropertyRuleBuilder<T, string> propertyRuleBuilder, int target)
        {
            propertyRuleBuilder.validationFns.Add(propertyValue =>
            {
                if (propertyValue.Length < target)
                {
                    AddFailureToList(propertyRuleBuilder, new ValidationFailure(propertyRuleBuilder._propertyName, $"{propertyRuleBuilder._propertyName} needs to be at least ${target} characters long", propertyValue));
                    return false;
                }
                return true;
            });
            return propertyRuleBuilder;
        }

        public static PropertyRuleBuilder<T, string> MustBeEmailFormat<T>(this PropertyRuleBuilder<T, string> propertyRuleBuilder)
        {
            propertyRuleBuilder.validationFns.Add(propertyValue =>
            {
                if (!propertyValue.ToCharArray().Contains('@'))
                {
                    AddFailureToList(propertyRuleBuilder, new ValidationFailure(propertyRuleBuilder._propertyName, $"{propertyRuleBuilder._propertyName} needs an @ character", propertyValue));
                    return false;
                }
                return true;
            });
            return propertyRuleBuilder;
        }
    }
}
