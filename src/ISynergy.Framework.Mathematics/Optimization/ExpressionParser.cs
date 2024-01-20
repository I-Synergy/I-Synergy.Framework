using System.Linq.Expressions;
namespace ISynergy.Framework.Mathematics.Optimization;

#if !NET35
internal static class ExpressionParser
{
    public static void Parse(SortedSet<string> list, Expression expr)
    {
        if (expr is null)
            return;

        BinaryExpression eb = expr as BinaryExpression;
        MemberExpression em = expr as MemberExpression;
        UnaryExpression eu = expr as UnaryExpression;
        MethodCallExpression ec = expr as MethodCallExpression;
        if (em is not null) // member expression
        {
            list.Add(em.Member.Name);
        }
        else if (eb is not null) // binary expression
        {
            Parse(list, eb.Left);
            Parse(list, eb.Right);
        }
        else if (eu is not null) // unary expression
        {
            Parse(list, eu.Operand);
        }
        else if (ec is not null) // call expression
        {
            foreach (var a in ec.Arguments)
                Parse(list, a);
        }
        return;
    }

    public static Expression<Func<double[], double[]>> Replace(Expression<Func<double[]>> expr,
        IDictionary<string, int> variables)
    {
        int variableCount = variables.Count;

        ParameterExpression parameter = Expression.Parameter(typeof(double[]), "input");

        var newBody = Replace(parameter, expr.Body, variables);

        return Expression.Lambda<Func<double[], double[]>>(newBody, parameter);
    }

    public static Expression<Func<double[], double>> Replace(Expression<Func<double>> expr,
        IDictionary<string, int> variables)
    {
        int variableCount = variables.Count;

        ParameterExpression parameter = Expression.Parameter(typeof(double[]), "input");

        var newBody = Replace(parameter, expr.Body, variables);

        return Expression.Lambda<Func<double[], double>>(newBody, parameter);
    }

    public static Expression Replace(ParameterExpression parameter,
        Expression expr, IDictionary<string, int> variables)
    {

        if (expr is null)
            return null;

        BinaryExpression eb = expr as BinaryExpression;
        MemberExpression em = expr as MemberExpression;
        UnaryExpression eu = expr as UnaryExpression;
        MethodCallExpression ec = expr as MethodCallExpression;
        NewArrayExpression ea = expr as NewArrayExpression;

        if (em is not null) // member expression
        {
            string varName = em.Member.Name;
            int index = variables[varName];
            ConstantExpression indexExpression = Expression.Constant(index);
            return Expression.ArrayAccess(parameter, indexExpression);
        }
        else if (eb is not null) // binary expression
        {
            var left = Replace(parameter, eb.Left, variables);
            var right = Replace(parameter, eb.Right, variables);
            return eb.Update(left, eb.Conversion, right);
        }
        else if (eu is not null) // unary expression
        {
            var op = Replace(parameter, eu.Operand, variables);
            return eu.Update(op);
        }
        else if (ec is not null) // call expression
        {
            List<Expression> args = new List<Expression>();

            foreach (var a in ec.Arguments)
                args.Add(Replace(parameter, a, variables));

            return ec.Update(ec.Object, args);
        }
        else if (ea is not null) // new array expression
        {
            List<Expression> values = new List<Expression>();

            foreach (var v in ea.Expressions)
                values.Add(Replace(parameter, v, variables));

            return ea.Update(values);
        }
        else
        {
            return expr;
        }
    }
}
#endif

