using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using BugTrackingSystem.Data.Model;

namespace BugTrackingSystem.Data.Infrastructure
{
    public abstract class RepositoryBase<T> where T : class
    {
        private DBModel _dataContext;
        private readonly IDbSet<T> _dbSet;

        protected IDbFactory DbFactory { get; private set; }

        protected DBModel DbContext
        {
            get { return _dataContext ?? (_dataContext = DbFactory.Init()); }
        }

        protected RepositoryBase(IDbFactory dbFactory)
        {
            DbFactory = dbFactory;
            //var projects = new List<Project>
            //{
            //    new Project{Name = "The Financier", Prefix = "TDF"},
            //    new Project{Name = "The Titan", Prefix = "DTT"},
            //    new Project{Name = "The Stoic", Prefix = "TDS"},
            //};

            //var user = new User(){FirstName = "George", LastName = "Orwell", Email = "george.orwell@gmail.com", Login = "GeorgeOrwell", Password = "12345", Photo = "orwell.jpg", UserRoleID = 1};

            //var bugs = new List<Bug>
            //{
            //    new Bug(){Project = projects[0], User = user, CreationDate = DateTime.Now, ModificationDate = DateTime.Now, Description = "bla bla", Number = 1, Subject = "Font for password is different from other labels font on \"Login\" screen.", StatusID = 1, PriorityID = 1},
            //    new Bug(){Project = projects[1], User = user, CreationDate = DateTime.Now, ModificationDate = DateTime.Now, Description = "bla blaasd", Number = 2,Subject = "The error message is displayed after tapping on the \"Get Link\" button on the volume's \"Information\" screen.", StatusID = 1, PriorityID = 1},
            //    new Bug(){Project = projects[2], User = user, CreationDate = DateTime.Now, ModificationDate = DateTime.Now, Description = "bla blaadsad", Number = 3,Subject = "Account activation link in Registration email is invalid.", StatusID = 1, PriorityID = 1}
            //};

            //user.Projects.Add(projects[0]);
            //user.Projects.Add(projects[1]);
            //user.Projects.Add(projects[2]);

            //DbContext.Projects.AddRange(projects);
            //DbContext.Users.Add(user);
            //DbContext.Bugs.AddRange(bugs);
            //DbContext.SaveChanges();

            //user.Bugs.Add(bugs[0]);
            //user.Bugs.Add(bugs[1]);
            //user.Bugs.Add(bugs[2]);

            //projects[0].Users.Add(user);
            //projects[1].Users.Add(user);
            //projects[2].Users.Add(user);

            //projects[0].Bugs.Add(bugs[0]);
            //projects[1].Bugs.Add(bugs[1]);
            //projects[2].Bugs.Add(bugs[2]);

            _dbSet = DbContext.Set<T>();
        }

        public virtual void Add(T entity)
        {
            _dbSet.Add(entity);
        }

        public virtual void Update(T entity)
        {
            _dbSet.Attach(entity);
            _dataContext.Entry(entity).State = EntityState.Modified;
        }

        public virtual void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }

        public virtual void Delete(Expression<Func<T, bool>> where)
        {
            var objects = _dbSet.Where<T>(where).AsEnumerable();

            foreach (T obj in objects)
            {
                _dbSet.Remove(obj);
            }
        }

        public virtual T GetById(int id)
        {
            return _dbSet.Find(id);
        }

        public virtual IEnumerable<T> GetAll()
        {
            return _dbSet.ToList();
        }

        public virtual IEnumerable<T> GetMany(Expression<Func<T, bool>> where)
        {
            return _dbSet.Where(where).ToList();
        }

        public T Get(Expression<Func<T, bool>> where)
        {
            return _dbSet.Where(where).FirstOrDefault<T>();
        }
    }
}
