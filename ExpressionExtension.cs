using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace sla
{
    public static class ExpressionExtension
    {
        public static string ToCustomString(this Expression f) =>
            ConstantEvaluator.Instance.Visit(f).ToString();

        private static NewArrayExpression GenerateExpressionList(object obj)
        {
            var listExpression = new List<Expression>();
            Type typeElementExpressionList = null;

            if (obj is Array)
            {
                typeElementExpressionList = obj.GetType().GetElementType();

                if (typeElementExpressionList == typeof(decimal))
                    foreach (var item in (decimal[])obj)
                        listExpression.Add(Expression.Constant(item));
                else if (typeElementExpressionList == typeof(int))
                    foreach (var item in (int[])obj)
                        listExpression.Add(Expression.Constant(item));
                else if (typeElementExpressionList == typeof(DateTime))
                    foreach (var item in (DateTime[])obj)
                        listExpression.Add(Expression.Constant(item));
            }

            if (obj.GetType().IsGenericType && obj.GetType().GetGenericTypeDefinition() == typeof(List<>))
            {
                typeElementExpressionList = obj.GetType().GetGenericArguments().Single();

                if(typeElementExpressionList == typeof(decimal))
                    foreach (var item in (List<decimal>)obj)
                        listExpression.Add(Expression.Constant(item));
                else if(typeElementExpressionList == typeof(int))
                    foreach (var item in (List<int>)obj)
                        listExpression.Add(Expression.Constant(item));
                else if(typeElementExpressionList == typeof(DateTime))
                    foreach (var item in (List<DateTime>)obj)
                        listExpression.Add(Expression.Constant(item));
            }

            if(typeElementExpressionList is null)
                throw new Exception("Mapeamento de DataType pendente em Extension");

            return Expression.NewArrayInit(typeElementExpressionList, listExpression);
        }

        internal class ConstantEvaluator : ExpressionVisitor
        {
            public static ConstantEvaluator Instance { get; } = new ConstantEvaluator();
            private ConstantEvaluator() { }

            protected override Expression VisitMember(MemberExpression node)
            {
                var target = Visit(node.Expression);
                if (target is ConstantExpression c)
                {
                    switch (node.Member)
                    {
                        case FieldInfo field:
                            if ((field.GetValue(c.Value) is Array || field.GetValue(c.Value).GetType().IsGenericType && field.GetValue(c.Value).GetType().GetGenericTypeDefinition() == typeof(List<>)) &&
                            field.GetValue(c.Value).GetType() != typeof(string))
                                return GenerateExpressionList(field.GetValue(c.Value));
                            else
                                return Expression.Constant(field.GetValue(c.Value), field.FieldType);
                        case PropertyInfo prop:
                            return Expression.Constant(prop.GetValue(c.Value), prop.PropertyType);
                    }
                }

                return node;
            }
        }
    }
}