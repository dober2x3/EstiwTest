using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Lx.Data.Repository
{
    public abstract class EntityBase : IEntity
    {
        private readonly EntityStateManager _changeTracker;

        protected EntityBase()
        {
            _changeTracker = new EntityStateManager(this);
        }

        protected void EnableChangeTracker()
        {
            _changeTracker.Initialize();

            AcceptChanges();
        }

        protected void OnEntityValueChanged<TEntity>(Expression<Func<TEntity>> property)
        {
            if (property == null)
            {
                throw new ArgumentNullException("property");
            }

            MemberExpression member = property.Body as MemberExpression;
            if (member == null || member.Member == null || member.Member.MemberType != MemberTypes.Property)
            {
                throw new ArgumentException("Provide a property", "property");
            }

            OnEntityValueChanged(member.Member.Name);
        }

        protected virtual void OnEntityValueChanged(string propertyName)
        {
            if (propertyName != null && _changeTracker.ContainsProperty(propertyName))
            {
                NotifyHasChangesChangedListeners();
            }
        }

        #region Validation Members

        protected ICollection<ValidationResult> GetValidationResults<TEntity>(Expression<Func<TEntity>> property)
        {
            if (property == null)
            {
                throw new ArgumentNullException("property");
            }

            MemberExpression memberExpression = property.Body as MemberExpression;
            if (memberExpression == null || memberExpression.Member == null || memberExpression.Member.MemberType != MemberTypes.Property)
            {
                throw new ArgumentException("Provide a property", "property");
            }

            return GetValidationResults(memberExpression.Member.Name);
        }
        public ICollection<ValidationResult> GetValidationResults(string propertyName)
        {
            if (propertyName == null)
            {
                throw new ArgumentNullException("propertyName");
            }

            ICollection<ValidationResult> result = new List<ValidationResult>();

            PropertyInfo propertyInfo = this.GetType().GetProperty(propertyName);

            object value = null;
            try
            {
                value = propertyInfo.GetValue(this, null);
            }
            catch (Exception)
            { }

            Validator.TryValidateProperty(
                value,
                new ValidationContext(this, null, null)
                {
                    MemberName = propertyName
                },
                result
            );

            return result;
        }

        public virtual bool Validate()
        {
            foreach (PropertyInfo propertyInfo in this.GetType().GetProperties().Where(p => p.GetCustomAttributes(typeof(ValidationAttribute), true).Count() > 0))
            {
                if (!IsPropertyValid(propertyInfo.Name))
                {
                    return false;
                }
            }

            return true;
        }

        protected bool IsPropertyValid<TEntity>(Expression<Func<TEntity>> property)
        {
            if (property == null)
            {
                throw new ArgumentNullException("property");
            }

            MemberExpression memberExpression = property.Body as MemberExpression;
            if (memberExpression == null || memberExpression.Member == null || memberExpression.Member.MemberType != MemberTypes.Property)
            {
                throw new ArgumentException("Provide a property", "property");
            }

            return IsPropertyValid(memberExpression.Member.Name);
        }
        public virtual bool IsPropertyValid(string propertyName)
        {
            if (propertyName == null)
            {
                throw new ArgumentNullException("propertyName");
            }

            return GetValidationResults(propertyName).Count == 0;
        }

        #endregion

        #region IEntity Members
        
        public event EventHandler HasChangesChanged;
        protected void NotifyHasChangesChangedListeners()
        {
            EventHandler handler = HasChangesChanged;
            if (handler != null)
            {
                handler(this, new EventArgs());
            }
        }

        public virtual bool HasChanges
        {
            get 
            {
                return _changeTracker.HasChanges;
            }
        }

        public virtual void AcceptChanges()
        {
            _changeTracker.AcceptChanges();

            NotifyHasChangesChangedListeners();
        }

        public virtual void RejectChanges()
        {
            _changeTracker.RejectChanges();

            NotifyHasChangesChangedListeners();
        }

        #endregion
    }
}
