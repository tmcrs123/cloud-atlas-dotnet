namespace Cloud_Atlas_Dotnet.Libraries.FluentValidation
{
    public static class PropertyRuleBuilderGenericExtensions
    {
        public static PropertyRuleBuilder<T, TProperty> NotNull<T, TProperty>(this PropertyRuleBuilder<T, TProperty> propertyRuleBuilder)
        {
            propertyRuleBuilder.validationFns.Add(propertyValue =>
            {
                if (propertyValue == null)
                {
                    propertyRuleBuilder.Failures.Add(new Failure(propertyRuleBuilder._propertyName, $"Null value not allowed for property {propertyRuleBuilder._propertyName}", null));
                    return false;
                }

                return true;
            });

            return propertyRuleBuilder;
        }
    }

    public static class PropertyRuleBuilderNumberExtensions
    {
        public static PropertyRuleBuilder<T, int> NumberLessThan<T>(this PropertyRuleBuilder<T, int> propertyRuleBuilder, int target)
        {
            propertyRuleBuilder.validationFns.Add(propertyValue =>
            {
                if (propertyValue > target)
                {
                    propertyRuleBuilder.Failures.Add(new Failure(propertyRuleBuilder._propertyName, $"Property {propertyRuleBuilder._propertyName} needs to be less than {target}", propertyValue));
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
                    propertyRuleBuilder.Failures.Add(new Failure(propertyRuleBuilder._propertyName, $"{propertyRuleBuilder._propertyName} needs to be an even number.", propertyValue));
                    return false;
                }

                return true;

            });

            return propertyRuleBuilder;
        }
    }

    public static class PropertyRuleBuilderStringExtensions
    {
        public static PropertyRuleBuilder<T, string> DifferentFrom<T>(this PropertyRuleBuilder<T, string> propertyRuleBuilder, string target)
        {
            propertyRuleBuilder.validationFns.Add(propertyValue =>
            {
                if (propertyValue.Split("").SequenceEqual(target.Split()))
                {
                    propertyRuleBuilder.Failures.Add(new Failure(propertyRuleBuilder._propertyName, $"{propertyRuleBuilder._propertyName} needs to be different from ${target}", propertyValue));
                    return false;
                }
                return true;
            });
            return propertyRuleBuilder;
        }
    }
}
