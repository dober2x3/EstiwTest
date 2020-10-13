using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Lx.Data.Repository
{
    public sealed class EntityStateManager
    {
        private readonly IEntity _entity;
        private readonly IDictionary<string, EntityProperty> _properties = new Dictionary<string, EntityProperty>();

        public EntityStateManager(IEntity entity)
        {
            _entity = entity;
        }

        public void Initialize()
        {
            foreach (PropertyInfo propertyInfo in _entity.GetType().GetProperties())
            {
                if (propertyInfo.GetCustomAttributes(typeof(ChangeTrackerAttribute), true).Length != 0)
                {
                    EntityProperty property = new EntityProperty(_entity, propertyInfo);
                    _properties.Add(property.Name, property);
                }
            }
        }

        public bool ContainsProperty(string propertyName)
        {
            return _properties.ContainsKey(propertyName);
        }
        public void AcceptChanges()
        {
            foreach (EntityProperty property in _properties.Values)
            {
                if (property.HasChanges)
                {
                    property.AcceptChanges();
                }
            }
        }
        public void RejectChanges()
        {
            foreach (EntityProperty property in _properties.Values)
            {
                if (property.HasChanges)
                {
                    property.RejectChanges();
                }
            }
        }
        public bool HasChanges
        {
            get
            {
                return _properties.Values.Where(p => p.HasChanges).Count() > 0;
            }
        }

        private class EntityProperty
        {
            private readonly IEntity _entity;
            private readonly PropertyInfo _propertyInfo;
            
            private object _initialValue;

            public EntityProperty(IEntity entity, PropertyInfo propertyInfo)
            {
                _entity = entity;
                _propertyInfo = propertyInfo;

                AcceptChanges();
            }

            public object CurrentValue
            {
                get
                {
                    return _propertyInfo.GetValue(_entity, null);
                }
            }

            public string Name
            {
                get
                {
                    return _propertyInfo.Name;
                }
            }

           // private readonly CloneManager _cloneManager = new CloneManager();
            public void AcceptChanges()
            {
				// TODO: Support for collections
                _initialValue = CurrentValue;
            }
            public void RejectChanges()
            {
                if (_propertyInfo.GetSetMethod(true) != null)
                {
                    _propertyInfo.SetValue(_entity, _initialValue, null);
                }
            }

            public bool HasChanges
            {
                get
                {
                    if (_initialValue == null)
                    {
                        return CurrentValue != null;
                    }

                    if (object.ReferenceEquals(_initialValue, CurrentValue))
                    {
                        return false;
                    }

                    // TODO: Support for collections

                    return !_initialValue.Equals(CurrentValue);
                }
            }
        }
    }
}
