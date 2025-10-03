namespace BusinessObjects
{
    public static class SeedData
    {
        public static void Initialize(ScmVlxdContext context)
        {
            context.Database.EnsureCreated();

            // Seed Roles
            if (!context.Roles.Any())
            {
                context.Roles.AddRange(
                    new Role { RoleName = "Admin", Description = "Administrator role" },
                    new Role { RoleName = "Manager", Description = "Manager role" },
                    new Role { RoleName = "User", Description = "Regular user role" }
                );
                context.SaveChanges();
            }

            // Seed Users
            if (!context.Users.Any())
            {
                var adminUser = new User
                {
                    UserName = "admin",
                    Email = "admin@example.com",
                    PasswordHash = "admin123", // nên hash sau này
                    CreatedAt = DateTime.Now
                };
                context.Users.Add(adminUser);
                context.SaveChanges();

                // Gán role cho admin
                var adminRole = context.Roles.First(r => r.RoleName == "Admin");
                context.UserRoles.Add(new UserRole
                {
                    UserId = adminUser.UserId,
                    RoleId = adminRole.RoleId,
                    AssignedAt = DateTime.Now
                });
                context.SaveChanges();
            }

            // Seed Categories
            if (!context.Categories.Any())
            {
                context.Categories.AddRange(
                    new Category { CategoryName = "Wood", Description = "Wood materials" },
                    new Category { CategoryName = "Metal", Description = "Metal materials" },
                    new Category { CategoryName = "Plastic", Description = "Plastic materials" }
                );
                context.SaveChanges();
            }

            // Seed Warehouses
            if (!context.Warehouses.Any())
            {
                var manager = context.Users.FirstOrDefault();
                context.Warehouses.AddRange(
                    new Warehouse { WarehouseName = "Main Warehouse", Location = "Hanoi", ManagerId = manager?.UserId },
                    new Warehouse { WarehouseName = "Secondary Warehouse", Location = "Ho Chi Minh", ManagerId = manager?.UserId }
                );
                context.SaveChanges();
            }
        }
    }
}
