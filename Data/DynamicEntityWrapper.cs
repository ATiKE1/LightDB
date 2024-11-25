using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;

namespace LightDB
{
    public interface INotifyPropertyChangedDynamic
    {
        event PropertyChangedEventHandler PropertyChanged;
    }

    public class DynamicEntityWrapper<TEntity> : INotifyPropertyChangedDynamic
    {
        private readonly TEntity _entity;

        public DynamicEntityWrapper(TEntity entity)
        {
            _entity = entity;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void SetProperty<T>(Expression<Func<TEntity, T>> propertyExpression, T value)
        {
            var memberExpression = propertyExpression.Body as MemberExpression;
            if (memberExpression == null)
                throw new ArgumentException("Invalid expression", nameof(propertyExpression));

            var propertyInfo = memberExpression.Member as PropertyInfo;
            if (propertyInfo == null)
                throw new ArgumentException("Invalid property", nameof(propertyExpression));

            var currentValue = (T)propertyInfo.GetValue(_entity);
            if (!EqualityComparer<T>.Default.Equals(currentValue, value))
            {
                propertyInfo.SetValue(_entity, value);
                OnPropertyChanged(propertyInfo.Name);
            }
        }

        public T GetProperty<T>(Expression<Func<TEntity, T>> propertyExpression)
        {
            var memberExpression = propertyExpression.Body as MemberExpression;
            if (memberExpression == null)
                throw new ArgumentException("Invalid expression", nameof(propertyExpression));

            var propertyInfo = memberExpression.Member as PropertyInfo;
            if (propertyInfo == null)
                throw new ArgumentException("Invalid property", nameof(propertyExpression));

            return (T)propertyInfo.GetValue(_entity);
        }
    }
}