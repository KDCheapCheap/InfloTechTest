using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UserManagement.Data.Entities;
using UserManagement.Data.Interceptors;
using UserManagement.Models;

namespace UserManagement.Data;

public class DataContext : DbContext, IDataContext
{
    private readonly DataContextSaveChangesInterceptor _auditInterceptor = new();

    public DataContext(DbContextOptions<DataContext> options) : base(options){}

    protected override void OnConfiguring(DbContextOptionsBuilder options) =>
        options.AddInterceptors(_auditInterceptor);

    protected override void OnModelCreating(ModelBuilder model)
    {
        model
            .Entity<User>()
            .HasData(
                new[]
                {
                    new User
                    {
                        Id = 1,
                        Forename = "Peter",
                        Surname = "Loew",
                        Email = "ploew@example.com",
                        DateOfBirth = new System.DateTime(1962, 1, 15),
                        IsActive = true
                    },
                    new User
                    {
                        Id = 2,
                        Forename = "Benjamin Franklin",
                        Surname = "Gates",
                        Email = "bfgates@example.com",
                        DateOfBirth = new System.DateTime(1970, 5, 4),
                        IsActive = true,
                    },
                    new User
                    {
                        Id = 3,
                        Forename = "Castor",
                        Surname = "Troy",
                        Email = "ctroy@example.com",
                        DateOfBirth = new System.DateTime(1965, 9, 22),
                        IsActive = false,
                    },
                    new User
                    {
                        Id = 4,
                        Forename = "Memphis",
                        Surname = "Raines",
                        Email = "mraines@example.com",
                        DateOfBirth = new System.DateTime(1974, 3, 11),
                        IsActive = true,
                    },
                    new User
                    {
                        Id = 5,
                        Forename = "Stanley",
                        Surname = "Goodspeed",
                        Email = "sgodspeed@example.com",
                        DateOfBirth = new System.DateTime(1968, 11, 8),
                        IsActive = true,
                    },
                    new User
                    {
                        Id = 6,
                        Forename = "H.I.",
                        Surname = "McDunnough",
                        Email = "himcdunnough@example.com",
                        DateOfBirth = new System.DateTime(1972, 6, 19),
                        IsActive = true,
                    },
                    new User
                    {
                        Id = 7,
                        Forename = "Cameron",
                        Surname = "Poe",
                        Email = "cpoe@example.com",
                        DateOfBirth = new System.DateTime(1976, 2, 27),
                        IsActive = false,
                    },
                    new User
                    {
                        Id = 8,
                        Forename = "Edward",
                        Surname = "Malus",
                        Email = "emalus@example.com",
                        DateOfBirth = new System.DateTime(1981, 12, 3),
                        IsActive = false,
                    },
                    new User
                    {
                        Id = 9,
                        Forename = "Damon",
                        Surname = "Macready",
                        Email = "dmacready@example.com",
                        DateOfBirth = new System.DateTime(1970, 8, 14),
                        IsActive = false,
                    },
                    new User
                    {
                        Id = 10,
                        Forename = "Johnny",
                        Surname = "Blaze",
                        Email = "jblaze@example.com",
                        DateOfBirth = new System.DateTime(1985, 4, 30),
                        IsActive = true,
                    },
                    new User
                    {
                        Id = 11,
                        Forename = "Robin",
                        Surname = "Feld",
                        Email = "rfeld@example.com",
                        DateOfBirth = new System.DateTime(1990, 10, 17),
                        IsActive = true,
                    },
                }
            );

            model.Entity<User>().HasQueryFilter(u => !u.IsDeleted);
    }

    public DbSet<User>? Users { get; set; }
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    public IQueryable<TEntity> GetAll<TEntity>() where TEntity : class
    {
        return base.Set<TEntity>();
    }

    public async Task<IQueryable<TEntity>> GetAllAsync<TEntity>() where TEntity : class
    {
        return await Task.FromResult(base.Set<TEntity>());
    }

    public TEntity Get<TEntity>(long id) where TEntity : class
    {
        var entity = Set<TEntity>().Find(id);

        if (entity == null)
        {
            throw new Exception("Cannot find Entity for ID: " + id);
        }

        return entity;
    }

    public async Task<TEntity> GetAsync<TEntity>(long id) where TEntity : class
    {
        var entity = await Set<TEntity>().FindAsync(id);

        if (entity == null)
        {
            throw new Exception("Cannot find Entity for ID: " + id);
        }

        return entity;
    }

    public void Create<TEntity>(TEntity entity) where TEntity : class
    {
        base.Add(entity);
        SaveChanges();
    }

    public async Task CreateAsync<TEntity>(TEntity entity) where TEntity : class
    {
        await base.AddAsync(entity);
        await SaveChangesAsync();
    }

    public new void Update<TEntity>(TEntity entity) where TEntity : class
    {
        base.Update(entity);
        SaveChanges();
    }

    public async Task UpdateAsync<TEntity>(TEntity entity) where TEntity : class
    {
        base.Update(entity);
        await SaveChangesAsync();
    }

    public void Delete<TEntity>(TEntity entity) where TEntity : class
    {
        base.Remove(entity);
        SaveChanges();
    }

    public async Task DeleteAsync<TEntity>(TEntity entity) where TEntity : class
    {
        base.Remove(entity);
        await SaveChangesAsync();
    }
}
