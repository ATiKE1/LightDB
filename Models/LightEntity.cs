﻿using System.Reflection;
using System.Collections.Generic;

namespace LightDatabase
{
    public class LightEntity<TEntity>
    {
        private TEntity _backendEntity;

        public TEntity Entity { get; private set; }

        public bool IsDeleted { get; private set; }

        public bool IsChanged
        {
            get
            {
                return GetChangedProperties().ChangedProperties.Count > 0;
            }
        }

        public bool IsOld { get; private set; } = false;

        internal void SetOld() => IsOld = true;


        internal TEntity GetBackendEntity() => _backendEntity;


        public void SetEntity(TEntity entity, TEntity backendEntity)
        {
            Entity = entity;
            _backendEntity = backendEntity;
        }

        public void Delete() =>
            IsDeleted = true;

        public void Restore() =>
            IsDeleted = false;

        public (Dictionary<string, object> ChangedProperties, Dictionary<string, object> AllProperties) GetChangedProperties()
        {
            var properties = _backendEntity.GetType().GetRuntimeProperties();

            Dictionary<string, object> changedProperties = new Dictionary<string, object>();
            Dictionary<string, object> allProperties = new Dictionary<string, object>();
            foreach (var property in properties)
            {
                var entityValue = Entity.GetType().GetProperty(property.Name).GetValue(Entity).ToString();
                var backendEntityValue = _backendEntity.GetType().GetProperty(property.Name).GetValue(_backendEntity).ToString();
                if (entityValue != backendEntityValue)
                    changedProperties.Add(property.Name, property.GetValue(Entity));

                allProperties.Add(property.Name, property.GetValue(_backendEntity));
            }

            return (changedProperties, allProperties);
        }
    }
}
