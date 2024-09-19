using System.Linq.Expressions;
using System.Text;

namespace Infrastructure.Config
{
    public class SqlWhereClause : ExpressionVisitor
    {
        private readonly StringBuilder _sb;

        public SqlWhereClause()
        {
            _sb = new StringBuilder();
        }

        public string Get()
        {
            return string.Format(@"WHERE {0}", _sb.ToString());
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            Visit(node.Left);

            switch (node.NodeType)
            {
                case ExpressionType.AndAlso:
                    _sb.Append(" AND ");
                    break;
                case ExpressionType.OrElse:
                    _sb.Append(" OR ");
                    break;
                case ExpressionType.Equal:
                    if (node.Right.ToString().Equals("null")) _sb.Append(" IS ");
                    else _sb.Append(" = ");
                    break;
                case ExpressionType.NotEqual:
                    if (node.Right.ToString().Equals("null")) _sb.Append(" IS NOT ");
                    else _sb.Append(" <> ");
                    break;
                case ExpressionType.LessThan:
                    _sb.Append(" < ");
                    break;
                case ExpressionType.GreaterThan:
                    _sb.Append(" > ");
                    break;
            }

            Visit(node.Right);

            return node;
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            if (node.Expression is ConstantExpression)
            {
                object value = GetValueFromExpression(node);

                _sb.Append($"'{value}'");
            }
            else
            {
                _sb.Append(node.Member.Name);
            }

            return node;
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            if (node.Value is null)
            {
                _sb.Append("NULL");
            }
            else
            {
                _sb.Append($"'{node.Value}'");
            }

            return node;
        }

        private object GetValueFromExpression(MemberExpression member)
        {
            UnaryExpression objectMember = Expression.Convert(member, typeof(object));
            Expression<Func<object>> getterLambda = Expression.Lambda<Func<object>>(objectMember);
            Func<object> getter = getterLambda.Compile();
            return getter();
        }
    }
}
