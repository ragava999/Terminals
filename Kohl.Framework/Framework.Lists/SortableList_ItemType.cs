using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Windows.Forms;

namespace Kohl.Framework.Lists
{
    public class SortableList<ItemType> : List<ItemType>
    {
        public SortableList()
        {
        }

        public SortableList(IEnumerable<ItemType> collection) : base(collection)
        {
        }

        public SortableList<ItemType> SortByProperty(string propertyName, SortOrder direction)
        {
            if (direction == SortOrder.None)
            {
                return this;
            }
            ParameterExpression parameterExpression = Expression.Parameter(typeof(ItemType), "item");
            MemberExpression memberExpression = Expression.Property(parameterExpression, propertyName);
            ParameterExpression[] parameterExpressionArray = new ParameterExpression[] { parameterExpression };
            Expression<Func<ItemType, object>> expression = Expression.Lambda<Func<ItemType, object>>(memberExpression, parameterExpressionArray);
            if (direction == SortOrder.Ascending)
            {
                return new SortableList<ItemType>(this.AsQueryable<ItemType>().OrderBy<ItemType, object>(expression));
            }
            return new SortableList<ItemType>(this.AsQueryable<ItemType>().OrderByDescending<ItemType, object>(expression));
        }
    }
}