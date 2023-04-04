﻿using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;
        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            Category = new CategoryRepository(_db);
            CoverType = new CoverTypeRepository(_db);
            Product = new ProductRepository(_db);
            Company = new CompanyRepository(_db);
            ShoppingCart = new ShoppingCartRepository(_db);
            ApplicationUser = new ApplicationUserRepository(_db);
			OrderHeader = new OrderHeaderRepository(_db);
			OrderDetail = new OrderDetailRepository(_db);



		}
		public ICategoryRepository Category { get; private set; } = null!;

        public ICoverTypeRepository CoverType { get; private set; } = null!;
        public IProductRepository Product { get; private set; } = null!;
        public ICompanyRepository Company { get; private set; } = null!;
        public IShoppingCartRepository ShoppingCart { get; private set; } = null!;
        public IApplicationUserRepository ApplicationUser { get; private set; } = null!;
		public IOrderHeaderRepository OrderHeader { get; private set; } = null!;
		public IOrderDetailRepository OrderDetail { get; private set; } = null!;


		public void Save()
        {
            _db.SaveChanges();
        }
    }


}
