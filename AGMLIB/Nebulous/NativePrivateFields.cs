using System.Linq.Expressions;
using System.Reflection;

internal static class NativePrivateFields
{
    internal static Func<object, T> Getter<T>(Type owner, string nestedType, string fieldName)
    {
        FieldInfo field = Find(owner, nestedType, fieldName);
        ParameterExpression target = Expression.Parameter(typeof(object), "target");
        MemberExpression value = Expression.Field(Expression.Convert(target, field.DeclaringType!), field);
        ConditionalExpression body = Expression.Condition(Expression.ReferenceEqual(target, Expression.Constant(null)),
            Expression.Default(typeof(T)), Expression.Convert(value, typeof(T)));
        return Expression.Lambda<Func<object, T>>(body, target).Compile();
    }

    internal static Action<object, T> Setter<T>(Type owner, string nestedType, string fieldName)
    {
        FieldInfo field = Find(owner, nestedType, fieldName);
        ParameterExpression target = Expression.Parameter(typeof(object), "target");
        ParameterExpression value = Expression.Parameter(typeof(T), "value");
        BinaryExpression assignment = Expression.Assign(Expression.Field(Expression.Convert(target, field.DeclaringType!), field),
            Expression.Convert(value, field.FieldType));
        return Expression.Lambda<Action<object, T>>(Expression.Block(assignment, Expression.Empty()), target, value).Compile();
    }

    private static FieldInfo Find(Type owner, string nestedType, string fieldName)
    {
        Type type = AccessTools.Inner(owner, nestedType) ?? throw new MissingMemberException(owner.FullName, nestedType);
        return AccessTools.Field(type, fieldName) ?? throw new MissingFieldException(type.FullName, fieldName);
    }
}
