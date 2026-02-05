using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementBE.Kernel.Core.Authorization
{
    public static class ManagementBERoles
    {
        public const string SystemAdmin = nameof(SystemAdmin);
        public const string Individual = nameof(Individual);
        public const string Internal = nameof(Internal);
        public const string Employee = nameof(Employee);
        public const string Manager = nameof(Manager);
        public const string SecretaryGeneral = nameof(SecretaryGeneral);

        public const string HeadTheaterDepartment = nameof(HeadTheaterDepartment);
        public const string ArtistsSyndicate = nameof(ArtistsSyndicate);
        public const string DirectorDirectorateSurveyingArts = nameof(DirectorDirectorateSurveyingArts);
        public const string DirectorTheaterSeason = nameof(DirectorTheaterSeason); 
        public const string EvaluateCommittees = nameof(EvaluateCommittees);
        public const string MinisterOfCulture = nameof(MinisterOfCulture);

        public const string Guest = nameof(Guest);
        public const string AuditSecretary = nameof(AuditSecretary);
        public const string AuditManager = nameof(AuditManager);
        public const string AuditMember = nameof(AuditMember);
        public const string ArtisticDirection = nameof(ArtisticDirection);
        public const string LanguageAuditor = nameof(LanguageAuditor);
        public const string TechnicalProduct = nameof(TechnicalProduct);
        public const string DataEntry = nameof(DataEntry);
        public const string Company = nameof(Company);
        public const string FinanceDepartment = nameof(FinanceDepartment);
        public const string GovernmentAgencies = nameof(GovernmentAgencies);

        public const string Foreign = nameof(Foreign);
        public const string WarehouseDepartmentHead = nameof(WarehouseDepartmentHead);





        public static IReadOnlyList<string> DefaultRoles { get; } = new ReadOnlyCollection<string>(new[]
        {
            SystemAdmin,
            Individual,
            Internal,
            Guest,
            Employee,
            Manager,
            SecretaryGeneral,

            HeadTheaterDepartment,
            ArtistsSyndicate,
            DirectorDirectorateSurveyingArts,
            DirectorTheaterSeason,
            MinisterOfCulture,

            AuditSecretary,
            AuditManager,
            AuditMember,
            ArtisticDirection,
            LanguageAuditor,
            TechnicalProduct,
            DataEntry,
            Company
         });

        public static bool IsDefault(string roleName) => DefaultRoles.Any(r => r == roleName);
    }
}
