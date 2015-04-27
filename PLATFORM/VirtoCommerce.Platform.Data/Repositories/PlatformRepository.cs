﻿using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using VirtoCommerce.Platform.Data.Infrastructure;
using VirtoCommerce.Platform.Data.Infrastructure.Interceptors;
using VirtoCommerce.Platform.Data.Model;

namespace VirtoCommerce.Platform.Data.Repositories
{
    public class PlatformRepository : EFRepositoryBase, IPlatformRepository
    {
        public PlatformRepository()
        {
            Database.SetInitializer<PlatformRepository>(null);
            Configuration.LazyLoadingEnabled = false;
        }

        public PlatformRepository(string nameOrConnectionString, params IInterceptor[] interceptors)
            : base(nameOrConnectionString, null, interceptors)
        {
            Database.SetInitializer<PlatformRepository>(null);
            Configuration.LazyLoadingEnabled = false;
        }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            #region Settings

            modelBuilder.Entity<SettingEntity>("PlatformSetting", "SettingId");
            modelBuilder.Entity<SettingValueEntity>("PlatformSettingValue", "SettingValueId");

            modelBuilder.Entity<SettingValueEntity>()
                .HasRequired(x => x.Setting)
                .WithMany(x => x.SettingValues)
                .HasForeignKey(x => x.SettingId);

            #endregion

            #region Security

            // Tables
            modelBuilder.Entity<AccountEntity>("PlatformAccount", "AccountId");
            modelBuilder.Entity<ApiAccountEntity>("PlatformApiAccount", "ApiAccountId");
            modelBuilder.Entity<RoleEntity>("PlatformRole", "RoleId");
            modelBuilder.Entity<PermissionEntity>("PlatformPermission", "PermissionId");
            modelBuilder.Entity<RoleAssignmentEntity>("PlatformRoleAssignment", "RoleAssignmentId");
            modelBuilder.Entity<RolePermissionEntity>("PlatformRolePermission", "RolePermissionId");

            // Properties
            modelBuilder.Entity<AccountEntity>().Property(x => x.StoreId).HasMaxLength(128);
            modelBuilder.Entity<AccountEntity>().Property(x => x.MemberId).HasMaxLength(64);
            modelBuilder.Entity<AccountEntity>().Property(x => x.UserName).IsRequired().HasMaxLength(128);

            modelBuilder.Entity<ApiAccountEntity>().Property(x => x.AppId)
                .IsRequired()
                .HasMaxLength(128)
                .HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("IX_AppId") { IsUnique = true }));

            modelBuilder.Entity<RoleEntity>().Property(x => x.Name).IsRequired().HasMaxLength(128);

            modelBuilder.Entity<PermissionEntity>().Property(x => x.Name).IsRequired().HasMaxLength(256);

            modelBuilder.Entity<RoleAssignmentEntity>().Property(x => x.OrganizationId).HasMaxLength(64);

            // Relations
            modelBuilder.Entity<ApiAccountEntity>()
                .HasRequired(x => x.Account)
                .WithMany(x => x.ApiAccounts)
                .HasForeignKey(x => x.AccountId);

            modelBuilder.Entity<RoleAssignmentEntity>()
                .HasRequired(x => x.Account)
                .WithMany(x => x.RoleAssignments)
                .HasForeignKey(x => x.AccountId);

            modelBuilder.Entity<RoleAssignmentEntity>()
                .HasRequired(x => x.Role)
                .WithMany(x => x.RoleAssignments)
                .HasForeignKey(x => x.RoleId);

            modelBuilder.Entity<RolePermissionEntity>()
                .HasRequired(x => x.Permission)
                .WithMany(x => x.RolePermissions)
                .HasForeignKey(x => x.PermissionId);

            modelBuilder.Entity<RolePermissionEntity>()
                .HasRequired(x => x.Role)
                .WithMany(x => x.RolePermissions)
                .HasForeignKey(x => x.RoleId);

            #endregion
        }

        #region IPlatformRepository Members

        public IQueryable<SettingEntity> Settings { get { return GetAsQueryable<SettingEntity>(); } }

        public IQueryable<AccountEntity> Accounts { get { return GetAsQueryable<AccountEntity>(); } }
        public IQueryable<ApiAccountEntity> ApiAccounts { get { return GetAsQueryable<ApiAccountEntity>(); } }
        public IQueryable<RoleEntity> Roles { get { return GetAsQueryable<RoleEntity>(); } }
        public IQueryable<PermissionEntity> Permissions { get { return GetAsQueryable<PermissionEntity>(); } }
        public IQueryable<RoleAssignmentEntity> RoleAssignments { get { return GetAsQueryable<RoleAssignmentEntity>(); } }
        public IQueryable<RolePermissionEntity> RolePermissions { get { return GetAsQueryable<RolePermissionEntity>(); } }

        public AccountEntity GetAccountByName(string userName, UserDetails detailsLevel)
        {
            var query = Accounts;

            if (detailsLevel == UserDetails.Full)
            {
                query = query
                    .Include(a => a.RoleAssignments.Select(ra => ra.Role.RolePermissions.Select(rp => rp.Permission)))
                    .Include(a => a.ApiAccounts);
            }

            return query.FirstOrDefault(a => a.UserName == userName);
        }

        #endregion
    }
}