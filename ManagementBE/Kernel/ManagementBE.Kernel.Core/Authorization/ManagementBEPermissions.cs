using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementBE.Kernel.Core.Authorization
{
    public static class ManagementBEAction
    {
        public const string View = nameof(View);
        public const string Search = nameof(Search);
        public const string Create = nameof(Create);
        public const string Update = nameof(Update);
        public const string Delete = nameof(Delete);
        public const string Export = nameof(Export);
        public const string Generate = nameof(Generate);
        public const string Clean = nameof(Clean);
        public const string UpgradeSubscription = nameof(UpgradeSubscription);
    }


    public static class ManagementBEResource
    {
        public const string Dashboard = nameof(Dashboard);
        public const string Hangfire = nameof(Hangfire);
        public const string Users = nameof(Users);
        public const string UserRoles = nameof(UserRoles);
        public const string Roles = nameof(Roles);
        public const string RoleClaims = nameof(RoleClaims);
        public const string Forms = nameof(Forms);
        public const string Services = nameof(Services);
        public const string Departments = nameof(Departments);
    }


    public static class ManagementBEPermissions
    {
        private static readonly ManagementBEPermission[] _all = new ManagementBEPermission[]
        {
            //Dashboard
            new("View Dashboard", ManagementBEAction.View, ManagementBEResource.Dashboard,true,true,true),
            
            //Users
            new("View Users", ManagementBEAction.View, ManagementBEResource.Users,true,false,false),
            new("Search Users", ManagementBEAction.Search, ManagementBEResource.Users,true,false,false),
            new("Create Users", ManagementBEAction.Create, ManagementBEResource.Users, true, false, false),
            new("Update Users", ManagementBEAction.Update, ManagementBEResource.Users, true, false, false),
            new("Delete Users", ManagementBEAction.Delete, ManagementBEResource.Users,true,false,false),
            new("Export Users", ManagementBEAction.Export, ManagementBEResource.Users, true, false, false),
            
            //User Roles
            new("View UserRoles", ManagementBEAction.View, ManagementBEResource.UserRoles,true,false,false),
            new("Update UserRoles", ManagementBEAction.Update, ManagementBEResource.UserRoles, true, false, false),

            new("View Roles", ManagementBEAction.View, ManagementBEResource.Roles,true,false,false),
            new("Create Roles", ManagementBEAction.Create, ManagementBEResource.Roles, true, false, false),
            new("Update Roles", ManagementBEAction.Update, ManagementBEResource.Roles, true, false, false),
            new("Delete Roles", ManagementBEAction.Delete, ManagementBEResource.Roles, true, false, false),
            
            //UserClaims
            new("View RoleClaims", ManagementBEAction.View, ManagementBEResource.RoleClaims,true,false,false),
            new("Update RoleClaims", ManagementBEAction.Update, ManagementBEResource.RoleClaims, true, false, false),

            //Forms
            new("View Forms", ManagementBEAction.View, ManagementBEResource.Forms,true,true,true),
            new("Search Forms", ManagementBEAction.Search, ManagementBEResource.Forms, true, true, true),
            new("Create Forms", ManagementBEAction.Create, ManagementBEResource.Forms, true, true, true),
            new("Update Forms", ManagementBEAction.Update, ManagementBEResource.Forms, true, false, false),
            new("Delete Forms", ManagementBEAction.Delete, ManagementBEResource.Forms, true, false, false),

            //Services
            new("View Services", ManagementBEAction.View, ManagementBEResource.Services, true, true, true),
            new("Search Services", ManagementBEAction.Search, ManagementBEResource.Services, true, true, true),
            new("Create Services", ManagementBEAction.Create, ManagementBEResource.Services,true,false,false),
            new("Update Services", ManagementBEAction.Update, ManagementBEResource.Services, true, false, false),
            new("Delete Services", ManagementBEAction.Delete, ManagementBEResource.Services, true, false, false),
            new("Export Services", ManagementBEAction.Export, ManagementBEResource.Services, true, true, true),

            //Departments
            new("View Departments", ManagementBEAction.View, ManagementBEResource.Departments,true,true,true),
            new("Search Departments", ManagementBEAction.Search, ManagementBEResource.Departments, true, true, true),
            new("Create Departments", ManagementBEAction.Create, ManagementBEResource.Departments, true, false, false),
            new("Update Departments", ManagementBEAction.Update, ManagementBEResource.Departments, true, false, false),
            new("Delete Departments", ManagementBEAction.Delete, ManagementBEResource.Departments, true, false, false),
        };

        public static IReadOnlyList<ManagementBEPermission> All { get; } = new ReadOnlyCollection<ManagementBEPermission>(_all);
        public static IReadOnlyList<ManagementBEPermission> SystemAdmin { get; } = new ReadOnlyCollection<ManagementBEPermission>(_all.Where(p => p.IsAdmin).ToArray());
        public static IReadOnlyList<ManagementBEPermission> Internal { get; } = new ReadOnlyCollection<ManagementBEPermission>(_all.Where(p => p.IsInternal).ToArray());
        public static IReadOnlyList<ManagementBEPermission> Individual { get; } = new ReadOnlyCollection<ManagementBEPermission>(_all.Where(p => p.IsIndividual).ToArray());
    }

    public record ManagementBEPermission(string Description, string Action, string Resource,
        bool IsAdmin,
        bool IsInternal,
        bool IsIndividual)
    {
        public string Name => NameFor(Action, Resource);
        public static string NameFor(string action, string resource) => $"Permissions.{resource}.{action}";
    }

}
