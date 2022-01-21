﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Northwind.DataAccessLayer.Abstract.IRepository;
using Northwind.DataAccessLayer.Concrete.EntityFramework.GenericRepository;
using Northwind.EntityLayer.Concrete.Models;

namespace Northwind.DataAccessLayer.Concrete.EntityFramework.EfRepository
{
    public class EfUserRepository : EfGenericRepository<User>, IUserRepository
    {
        public EfUserRepository(DbContext context) : base(context)
        {
        }

        public User Login(User login)
        {
          


            var user = set.FirstOrDefault(x => x.Email == login.Email && x.PasswordHash == login.PasswordHash);
            return user;
        }

        public User Register(User register)
        {
            context.Entry(register).State = EntityState.Added;
            set.Add(register);
            return register;
        }
    }
}
