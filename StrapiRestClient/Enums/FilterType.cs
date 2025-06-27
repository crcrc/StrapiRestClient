namespace StrapiRestClient.Enums
{
    /// <summary>
    /// Defines the types of filters that can be applied to Strapi API requests.
    /// </summary>
    public enum FilterType
    {
        /// <summary>
        /// Matches if the field's value is in the provided list.
        /// </summary>
        In,
        /// <summary>
        /// Matches if the field's value is not in the provided list.
        /// </summary>
        NotIn,
        /// <summary>
        /// Matches if the field's value is exactly equal to the provided value.
        /// </summary>
        EqualTo,
        /// <summary>
        /// Matches if the field's value is not equal to the provided value.
        /// </summary>
        NotEqualTo,
        /// <summary>
        /// Matches if the field's value is less than the provided value.
        /// </summary>
        LessThan,
        /// <summary>
        /// Matches if the field's value is less than or equal to the provided value.
        /// </summary>
        LessThanOrEqualTo,
        /// <summary>
        /// Matches if the field's value is greater than the provided value.
        /// </summary>
        GreaterThan,
        /// <summary>
        /// Matches if the field's value is greater than or equal to the provided value.
        /// </summary>
        GreaterThanOrEqualTo,
        /// <summary>
        /// Matches if the field's value contains the provided substring.
        /// </summary>
        Contains,
        /// <summary>
        /// Matches if the field's value does not contain the provided substring.
        /// </summary>
        DoesNotContain,
        /// <summary>
        /// Matches if the field's value starts with the provided substring.
        /// </summary>
        StartsWith,
        /// <summary>
        /// Matches if the field's value ends with the provided substring.
        /// </summary>
        EndsWith,
        /// <summary>
        /// Matches if the field's value is null.
        /// </summary>
        IsNull,
        /// <summary>
        /// Matches if the field's value is not null.
        /// </summary>
        IsNotNull,
    }
}
