using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;

namespace RepositoryPattern
{
    public static class DbDataReaderExtension
    {
        public static IEnumerable<TEntity> Read<TEntity>(this DbDataReader reader) where TEntity : class, new()
        {
            if (reader != null && reader.HasRows)
            {
                var entity = typeof(TEntity);
                var entities = new List<TEntity>();
                var propertyDictionary = new Dictionary<string, PropertyInfo>();
                var properties = entity.GetProperties(BindingFlags.Instance | BindingFlags.Public);
                propertyDictionary = properties.ToDictionary(p => p.Name.ToUpper(), p => p);

                while (reader.Read())
                {
                    var newObject = new TEntity();
                    for (var i = 0; i < reader.FieldCount; i++)
                    {
                        if (propertyDictionary.ContainsKey(reader.GetName(i).ToUpper()))
                        {
                            var info = propertyDictionary[reader.GetName(i).ToUpper()];
                            if ((info != null) && info.CanWrite)
                            {
                                var val = reader.GetValue(i);
                                info.SetValue(newObject, (val == DBNull.Value) ? null : val, null);
                            }
                        }
                    }
                    entities.Add(newObject);
                }
                return entities;
            }

            return new List<TEntity>();
        }
    }
}
