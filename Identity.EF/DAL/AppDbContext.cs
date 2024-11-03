using Identity.Core.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.EF.DAL;
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions options) : base(options)
    {
    }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<User>(prop =>
        {
            prop.HasKey(x => x.UserId);
            prop.Property(x => x.Username).IsRequired().IsUnicode().HasMaxLength(150);
            prop.Property(x => x.Email).IsRequired().IsUnicode().HasMaxLength(150);
            prop.Property(x => x.PasswordHash).IsRequired().HasMaxLength(150);
            prop.Property(x => x.FullName).IsRequired().HasMaxLength(150);
            prop.Property(x => x.CreatedAt).IsRequired();
        });
        base.OnModelCreating(builder);
    }
}
