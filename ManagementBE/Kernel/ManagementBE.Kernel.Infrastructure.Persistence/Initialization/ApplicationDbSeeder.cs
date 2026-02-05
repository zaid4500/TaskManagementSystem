using ManagementBE.Kernel.Core.Authorization;
using ManagementBE.Kernel.Core.Constants;
using ManagementBE.Kernel.Core.Infrastructure;
using ManagementBE.Kernel.Core.Persistence;
using ManagementBE.Kernel.Domain.Common;
using ManagementBE.Kernel.Domain.Identity;
using ManagementBE.Kernel.Infrastructure.Persistence.UnitOfWorks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ManagementBE.Kernel.Infrastructure.Persistence.Initialization
{
    public class ApplicationDbSeeder
    {
        readonly RoleManager<IdentityRole> _roleManager;
        readonly UserManager<ApplicationUser> _userManager;
        readonly CustomSeederRunner _seederRunner;
        readonly IRepository<IdentityRoleClaim<string>> _roleClaimRepo;
        readonly IRepository<Lookup> _lookupRepository;
        readonly IRepository<LookupCategory> _lookupCategoryRepository;
        readonly IApplicationLoggerService _logger;

        public ApplicationDbSeeder(
            RoleManager<IdentityRole> roleManager,
            UserManager<ApplicationUser> userManager,
            CustomSeederRunner seederRunner,
            IRepository<IdentityRoleClaim<string>> roleClaimRepo,
            IApplicationLoggerService logger
,
            IRepository<Lookup> lookupRepository,
            IRepository<LookupCategory> lookupCategoryRepository)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _seederRunner = seederRunner;
            _roleClaimRepo = roleClaimRepo;
            _logger = logger;
            _lookupRepository = lookupRepository;
            _lookupCategoryRepository = lookupCategoryRepository;
        }

        private async Task SeedLookupCategoriesAsync()
        {
            var existingCategories = await _lookupCategoryRepository.GetAll().ToListAsync();

            if (existingCategories.Any())
            {
                await _logger.LogInformation("Lookup categories already exist. Skipping seeding.");
                return;
            }

            var categories = new List<LookupCategory>
            {
                new LookupCategory
                {
                    NameEn = "Gender",
                    NameAr = "الجنس",
                    Code = "GENDER"
                },
                new LookupCategory
                {
                    NameEn = "Task Status",
                    NameAr = "حالة المهمة",
                    Code = "TASK_STATUS"
                }
            };

            await _lookupCategoryRepository.InsertAsync(categories);
            await _lookupCategoryRepository.UnitOfWork.SaveChangesAsync();

            await _logger.LogInformation("Lookup categories seeded successfully");
        }

        private async Task SeedLookupsAsync()
        {
            var existingLookups = await _lookupRepository.GetAll().ToListAsync();

            if (existingLookups.Any())
            {
                await _logger.LogInformation("Lookups already exist. Skipping seeding.");
                return;
            }

            var genderCategory = await _lookupCategoryRepository.GetFirstOrDefaultAsync(
                predicate: c => c.Code == "GENDER"
            );

            var taskStatusCategory = await _lookupCategoryRepository.GetFirstOrDefaultAsync(
                predicate: c => c.Code == "TASK_STATUS"
            );

            if (genderCategory == null || taskStatusCategory == null)
            {
                await _logger.LogWarning("Lookup categories not found. Skipping lookups seeding.");
                return;
            }

            var lookups = new List<Lookup>
            {
                new Lookup
                {
                    LookupCategoryId = genderCategory.Id,
                    NameEn = "Male",
                    NameAr = "ذكر",
                    Code = "MALE"
                },
                new Lookup
                {
                    LookupCategoryId = genderCategory.Id,
                    NameEn = "Female",
                    NameAr = "أنثى",
                    Code = "FEMALE"
                },
                new Lookup
                {
                    LookupCategoryId = taskStatusCategory.Id,
                    NameEn = "New",
                    NameAr = "جديد",
                    Code = "NEW"
                },
                new Lookup
                {
                    LookupCategoryId = taskStatusCategory.Id,
                    NameEn = "Active",
                    NameAr = "نشط",
                    Code = "ACTIVE"
                },
                new Lookup
                {
                    LookupCategoryId = taskStatusCategory.Id,
                    NameEn = "Closed",
                    NameAr = "مغلق",
                    Code = "CLOSED"
                }
            };

            await _lookupRepository.InsertAsync(lookups);
            await _lookupRepository.UnitOfWork.SaveChangesAsync();

            await _logger.LogInformation("Lookups seeded successfully");
        }
        public async Task SeedDatabaseAsync(CancellationToken cancellationToken)
        {
            await SeedLookupCategoriesAsync();
            await SeedLookupsAsync();
            await SeedRolesAsync();
            await SeedAdminUserAsync();
            await SeedRegularUsersAsync();
            await _seederRunner.RunSeedersAsync(cancellationToken);
        }

        private async Task SeedRolesAsync()
        {
            string[] roleNames = { "Admin", "User" };

            foreach (var roleName in roleNames)
            {
                if (!await _roleManager.RoleExistsAsync(roleName))
                {
                    var result = await _roleManager.CreateAsync(new IdentityRole(roleName));
                    if (result.Succeeded)
                    {
                        _logger.LogInformation($"Role '{roleName}' created successfully");
                    }
                }
            }
        }

        private async Task SeedAdminUserAsync()
        {
            var adminEmail = "admin@gmail.com";
            var adminUser = await _userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    FirstName = "System",
                    LastName = "Administrator",
                    MiddleName = "",
                    FullNameEn = "System Administrator",
                    FirstNameAr = "مدير",
                    MiddleNameAr = "",
                    LastNameAr = "النظام",
                    FullNameAr = "مدير النظام",
                    DateOfBirth = new DateTime(1990, 1, 1),
                    Gender = 1,
                    NormalizedEmail = "ADMIN@GMAIL.COM",
                    Nationality = "Jordanian",
                    IsActive = true,
                    CreatedBy = "DB-Seeder",
                    CreatedOn = DateTime.UtcNow,
                    IsDeleted = false
                };

                var result = await _userManager.CreateAsync(adminUser, "Admin@123456");
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(adminUser, "Admin");
                    _logger.LogInformation("Admin user created successfully");
                }
            }
        }

        private async Task SeedRegularUsersAsync()
        {
            var user1Email = "user1@gmail.com";
            if (await _userManager.FindByEmailAsync(user1Email) == null)
            {
                var user1 = new ApplicationUser
                {
                    UserName = user1Email,
                    NormalizedUserName = user1Email.ToUpper(),
                    Email = user1Email,
                    NormalizedEmail = user1Email.ToUpper(),
                    EmailConfirmed = true,
                    FirstName = "Mohammed",
                    MiddleName = "Ahmed",
                    LastName = "Al-Ahmed",
                    FullNameEn = "Mohammed Ahmed Al-Ahmed",
                    FirstNameAr = "محمد",
                    MiddleNameAr = "أحمد",
                    LastNameAr = "الأحمد",
                    FullNameAr = "محمد أحمد الأحمد",
                    PhoneNumber = "+962791234567",
                    MobileNumber = "+962791234567",
                    DateOfBirth = new DateTime(1995, 5, 15),
                    Gender = 1,
                    Nationality = "Jordanian",
                    IsActive = true,
                    CreatedBy = "DB-Seeder",
                    CreatedOn = DateTime.UtcNow,
                    IsDeleted = false
                };

                var result = await _userManager.CreateAsync(user1, "User@123456");
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user1, "User");
                    _logger.LogInformation("User 1 (Mohammed) created successfully");
                }
                else
                {
                    _logger.LogWarning($"Failed to create User 1: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
            }

            var user2Email = "user2@gmail.com";
            if (await _userManager.FindByEmailAsync(user2Email) == null)
            {
                var user2 = new ApplicationUser
                {
                    UserName = user2Email,
                    NormalizedUserName = user2Email.ToUpper(),
                    Email = user2Email,
                    NormalizedEmail = user2Email.ToUpper(),
                    EmailConfirmed = true,
                    FirstName = "Fatima",
                    MiddleName = "Ahmed",
                    LastName = "Al-Ahmed",
                    FullNameEn = "Ahmad Ahmed Al-Ahmed",
                    FirstNameAr = "فاطمة",
                    MiddleNameAr = "أحمد",
                    LastNameAr = "الأحمد",
                    FullNameAr = "أحمد أحمد الأحمد",
                    PhoneNumber = "+962791234568",
                    MobileNumber = "+962791234568",
                    DateOfBirth = new DateTime(1997, 8, 22),
                    Gender = 2,
                    Nationality = "Jordanian",
                    IsActive = true,
                    CreatedBy = "DB-Seeder",
                    CreatedOn = DateTime.UtcNow,
                    IsDeleted = false
                };

                var result = await _userManager.CreateAsync(user2, "User@123456");
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user2, "User");
                    _logger.LogInformation("User 2 (Fatima) created successfully");
                }
                else
                {
                    _logger.LogWarning($"Failed to create User 2: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
            }

            var user3Email = "user3@gmail.com";
            if (await _userManager.FindByEmailAsync(user3Email) == null)
            {
                var user3 = new ApplicationUser
                {
                    UserName = user3Email,
                    NormalizedUserName = user3Email.ToUpper(),
                    Email = user3Email,
                    NormalizedEmail = user3Email.ToUpper(),
                    EmailConfirmed = true,
                    FirstName = "Khalid",
                    MiddleName = "Ahmed",
                    LastName = "Al-Ahmed",
                    FullNameEn = "Khalid Ahmed Al-Ahmed",
                    FirstNameAr = "خالد",
                    MiddleNameAr = "أحمد",
                    LastNameAr = "الأحمد",
                    FullNameAr = "خالد أحمد الأحمد",
                    PhoneNumber = "+962791234569",
                    MobileNumber = "+962791234569",
                    DateOfBirth = new DateTime(1999, 3, 10),
                    Gender = 1,
                    Nationality = "Jordanian",
                    IsActive = true,
                    CreatedBy = "DB-Seeder",
                    CreatedOn = DateTime.UtcNow,
                    IsDeleted = false
                };

                var result = await _userManager.CreateAsync(user3, "User@123456");
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user3, "User");
                    _logger.LogInformation("User 3 (Khalid) created successfully");
                }
                else
                {
                    _logger.LogWarning($"Failed to create User 3: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
            }
        }
    }
}
