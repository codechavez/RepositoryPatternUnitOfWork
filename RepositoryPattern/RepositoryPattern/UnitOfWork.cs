using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace RepositoryPattern
{
    public abstract class UnitOfWork<TDbContext> where TDbContext : DbContext, IDisposable
    {
        public TDbContext UoWContext { get; private set; }

        public UnitOfWork(TDbContext context)
        {
            UoWContext = context;
        }

        public virtual void Detach()
        {
            var entries = UoWContext.ChangeTracker.Entries();
            foreach (var entry in entries)
            {
                switch (entry.State)
                {
                    case EntityState.Deleted:
                    case EntityState.Modified:
                        entry.State = EntityState.Unchanged;
                        break;
                    case EntityState.Added:
                        entry.State = EntityState.Detached;
                        break;
                    default:
                        break;
                }
            }
        }

        public virtual void Refresh()
        {
            var refresEntries = UoWContext.ChangeTracker
                                          .Entries()
                                          .Where(x => x.State == EntityState.Deleted ||
                                                      x.State == EntityState.Modified ||
                                                      x.State == EntityState.Added)
                                          .ToArray();
            foreach (var entry in refresEntries)
                UoWContext.Entry(entry.Entity).Reload();
        }

        public virtual void Commit()
        {
            bool saveFailed;
            do
            {
                saveFailed = false;
                try
                {
                    _ = UoWContext.SaveChanges();
                }
                catch (DbUpdateConcurrencyException uEx)
                {
                    saveFailed = true;
                    uEx.Entries.Single().Reload();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            } while (saveFailed);
        }

        public virtual IEnumerable<TEntity> RawQuery<TEntity>(string query) where TEntity : class, new()
        {
            using var command = (SqlCommand)UoWContext.Database.GetDbConnection().CreateCommand();
            command.CommandText = query;
            UoWContext.Database.OpenConnection();

            using var reader = command.ExecuteReader();
            return reader.Read<TEntity>();
        }

        public virtual object RawQueryScalar(string query)
        {
            using var command = (SqlCommand)UoWContext.Database.GetDbConnection().CreateCommand();
            command.CommandText = query;
            UoWContext.Database.OpenConnection();

            return command.ExecuteScalar();
        }

        public virtual IEnumerable<TResult> StoreProcedure<TResult>(string sp, params SqlParameter[] parameters) where TResult : class, new()
        {
            using var command = UoWContext.Database.GetDbConnection().CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = sp;
            if (parameters.Any())
                command.Parameters.AddRange(parameters);

            UoWContext.Database.OpenConnection();

            using var reader = command.ExecuteReader();
            return reader.Read<TResult>();
        }


        public void Dispose() => UoWContext.Dispose();
    }
}
