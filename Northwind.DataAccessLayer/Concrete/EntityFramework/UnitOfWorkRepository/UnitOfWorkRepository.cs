using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Northwind.DataAccessLayer.Abstract.GenericRepository;
using Northwind.DataAccessLayer.Abstract.UnitOfWorkRepository;
using Northwind.DataAccessLayer.Concrete.EntityFramework.GenericRepository;
using Northwind.EntityLayer.Concrete.Bases;

namespace Northwind.DataAccessLayer.Concrete.EntityFramework.UnitOfWorkRepository
{
    public class UnitOfWorkRepository : IUnitOfWorkRepository
    {
        #region Variables

        private DbContext context;
        private IDbContextTransaction transaction;
        bool dispose;

        public UnitOfWorkRepository(DbContext context)
        {
            this.context = context;
        }

        #endregion
        public IGenericRepository<T> GetRepository<T>() where T : EntityBase
        {
            return new EfGenericRepository<T>(context);
        }

        public bool BeginTransaction()
        {
            try
            {
                transaction = context.Database.BeginTransaction();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool RollBackTransaction()
        {
            try
            {
                transaction.Rollback();
                transaction = null;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public int SaveChanges()
        {
            var _transaction = this.transaction != null ? this.transaction : context.Database.BeginTransaction();
            using (_transaction)
            {
                try
                {
                    if (context == null)
                    {
                        throw new ArgumentException("Context is null");
                    }

                    int result = context.SaveChanges();
                    _transaction.Commit();
                    return result;
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw new Exception("Error on save changes", e);
                }
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.dispose) 
            {
                if (disposing) 
                {
                    context.Dispose();
                }
            }

            this.dispose = true;
        }
        public void Dispose()
        {

            Dispose(true);

            GC.SuppressFinalize(this);
        }
    }
}
