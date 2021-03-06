﻿using ContentBlockService.Data.Helpers;
using ContentBlockService.Data.Model;
using System;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Threading.Tasks;

namespace ContentBlockService.Data
{
    public interface IContentBlockServiceContext
    {
        DbSet<User> Users { get; set; }
        DbSet<Role> Roles { get; set; }
        DbSet<Tenant> Tenants { get; set; }
        DbSet<DigitalAsset> DigitalAssets { get; set; }        
        DbSet<Account> Accounts { get; set; }
        DbSet<Profile> Profiles { get; set; }
        DbSet<RESTService> RESTServices { get; set; }
        DbSet<Resource> Resources { get; set; }
        DbSet<ContentBlock> ContentBlocks { get; set; }
        DbSet<QuintupleContentBlock> QuintupleContentBlocks { get; set; }
        DbSet<CallToActionContentBlock> CallToActionContentBlocks { get; set; }
        DbSet<HeadlineContentBlock> HeadlineContentBlocks { get; set; }
        DbSet<MegaHeaderContentBlock> MegaHeaderContentBlocks { get; set; }
        Task<int> SaveChangesAsync();
    }

    public class ContentBlockServiceContext: DbContext, IContentBlockServiceContext
    {
        public ContentBlockServiceContext()
            :base("ContentBlockServiceContext")
        {
            Configuration.ProxyCreationEnabled = false;
            Configuration.LazyLoadingEnabled = false;
            Configuration.AutoDetectChangesEnabled = true;
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Tenant> Tenants { get; set; }
        public DbSet<DigitalAsset> DigitalAssets { get; set; }        
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Profile> Profiles { get; set; }
        public DbSet<RESTService> RESTServices { get; set; }
        public DbSet<ContentBlock> ContentBlocks { get; set; }
        public DbSet<QuintupleContentBlock> QuintupleContentBlocks { get; set; }
        public DbSet<Resource> Resources { get; set; }
        public DbSet<CallToActionContentBlock> CallToActionContentBlocks { get; set; }
        public DbSet<HeadlineContentBlock> HeadlineContentBlocks { get; set; }
        public DbSet<MegaHeaderContentBlock> MegaHeaderContentBlocks { get; set; }
        public override int SaveChanges()
        {
            UpdateLoggableEntries();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync()
        {
            UpdateLoggableEntries();
            return base.SaveChangesAsync();
        }

        public void UpdateLoggableEntries()
        {
            foreach (var entity in ChangeTracker.Entries()
                .Where(e => e.Entity is ILoggable && ((e.State == EntityState.Added || (e.State == EntityState.Modified))))
                .Select(x => x.Entity as ILoggable))
            {
                entity.CreatedOn = entity.CreatedOn == default(DateTime) ? DateTime.UtcNow : entity.CreatedOn;
                entity.LastModifiedOn = DateTime.UtcNow;
            }
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().
                HasMany(u => u.Roles).
                WithMany(r => r.Users).
                Map(
                    m =>
                    {
                        m.MapLeftKey("User_Id");
                        m.MapRightKey("Role_Id");
                        m.ToTable("UserRoles");
                    });


            var convention = new AttributeToTableAnnotationConvention<SoftDeleteAttribute, string>(
                "SoftDeleteColumnName",
                (type, attributes) => attributes.Single().ColumnName);

            modelBuilder.Conventions.Add(convention);
        }
    }
}