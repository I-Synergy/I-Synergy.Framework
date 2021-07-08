namespace ISynergy.Framework.Mathematics.Optimization
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

#if !NET35
    internal static class ExpressionParser
    {
        public static void Parse(SortedSet<string> list, Expression expr)
        {
            if (expr == null)
                return;

            BinaryExpression eb = expr as BinaryExpression;
            MemberExpression em = expr as MemberExpression;
            UnaryExpression eu = expr as UnaryExpression;
            MethodCallExpression ec = expr as MethodCallExpression;
            if (em != null) // member expression
            {
                list.Add(em.Member.Name);
            }
            else if (eb != null) // binary expression
            {
                Parse(list, eb.Left);
                Parse(list, eb.Right);
            }
            else if (eu != null) // unary expression
            {
                Parse(list, eu.Operand);
            }
            else if (ec != null) // call expression
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

            if (expr == null)
                return null;

            BinaryExpression eb = expr as BinaryExpression;
            MemberExpression em = expr as MemberExpression;
            UnaryExpression eu = expr as UnaryExpression;
            MethodCallExpression ec = expr as MethodCallExpression;
            NewArrayExpression ea = expr as NewArrayExpression;

            if (em != null) // member expression
            {
                string varName = em.Member.Name;
                int index = variables[varName];
                ConstantExpression indexExpression = Expression.Constant(index);
                return Expression.ArrayAccess(parameter, indexExpression);
            }
            else if (eb != null) // binary expression
            {
                var left = Replace(parameter, eb.Left, variables);
                var right = Replace(parameter, eb.Right, variables);
                return eb.Update(left, eb.Conversion, right);
            }
            else if (eu != null) // unary expression
            {
                var op = Replace(parameter, eu.Operand, variables);
                return eu.Update(op);
            }
            else if (ec != null) // call expression
            {
                List<Expression> args = new List<Expression>();

                foreach (var a in ec.Arguments)
                    args.Add(Replace(parameter, a, variables));

                return ec.Update(ec.Object, args);
            }
            else if (ea != null) // new array expression
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
}

