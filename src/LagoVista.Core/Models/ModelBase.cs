using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace LagoVista.Core.Models
{
    public class ModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;


        private bool CheckExpressionForMemberAccess(System.Linq.Expressions.Expression propertyExpression)
        {
            return propertyExpression.NodeType == ExpressionType.MemberAccess;
        }

        public string GetPropertyNameFromExpression<TResult>(Expression<Func<TResult>> propertyExpression)
        {
            if (propertyExpression == null)
                throw new ArgumentNullException(nameof(propertyExpression));

            System.Linq.Expressions.MemberExpression memberExpression = (System.Linq.Expressions.MemberExpression)propertyExpression.Body;

            if (memberExpression != null)
                return memberExpression.Member.Name;

            throw new ArgumentException(nameof(propertyExpression));
        }


        protected void OnPropertyChanged<TResult>(Expression<Func<TResult>> propertyExpression)
        {
            if (propertyExpression == null)
                throw new ArgumentNullException(nameof(propertyExpression));

            if (!this.CheckExpressionForMemberAccess(propertyExpression.Body))
                throw new ArgumentException(nameof(propertyExpression),
                        string.Format("The expected expression is no 'MemberAccess'; its a '{0}'", propertyExpression.Body.NodeType));

            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(this.GetPropertyNameFromExpression(propertyExpression)));
        }

        protected bool Set<T>(ref T storage, T value, string columnName = null, [CallerMemberName] string propertyName = null)
        {
            if (object.Equals(storage, value)) return false;

            storage = value;
            if (PropertyChanged != null && !String.IsNullOrEmpty(propertyName))
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));

            return true;
        }

        public void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (PropertyChanged != null && !String.IsNullOrEmpty(propertyName))
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }        
    }
}
