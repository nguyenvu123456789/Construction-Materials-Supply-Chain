using BusinessObjects;

namespace DataAccess
{
    public static class SeedData
    {
        public static void Initialize(ScmVlxdContext context)
        {
            context.Database.EnsureCreated();

            // 1. Seed PartnerTypes
            if (!context.PartnerTypes.Any())
            {
                context.PartnerTypes.AddRange(
                    new PartnerType { TypeName = "Supplier" },
                    new PartnerType { TypeName = "Distributor" },
                    new PartnerType { TypeName = "Agent" }
                );
                context.SaveChanges();
            }

            // 2. Seed Partners
            if (!context.Partners.Any())
            {
                var supplierType = context.PartnerTypes.First(pt => pt.TypeName == "Supplier");

                context.Partners.AddRange(
                    new Partner
                    {
                        PartnerName = "Default Supplier",
                        PartnerTypeId = supplierType.PartnerTypeId,
                        ContactEmail = "supplier@example.com",
                        ContactPhone = "0123456789"
                    },
                    new Partner
                    {
                        PartnerName = "Another Supplier",
                        PartnerTypeId = supplierType.PartnerTypeId,
                        ContactEmail = "supplier2@example.com",
                        ContactPhone = "0987654321"
                    }
                );
                context.SaveChanges();
            }

            // 3. Seed Roles
            if (!context.Roles.Any())
            {
                context.Roles.AddRange(
                    new Role { RoleName = "Admin", Description = "Administrator role" },
                    new Role { RoleName = "Manager", Description = "Manager role" },
                    new Role { RoleName = "User", Description = "Regular user role" }
                );
                context.SaveChanges();
            }

            // 4. Seed Users
            if (!context.Users.Any())
            {
                var adminUser = new User
                {
                    UserName = "admin",
                    Email = "admin@example.com",
                    PasswordHash = "admin123", // nên hash trong thực tế
                    CreatedAt = DateTime.Now
                };
                context.Users.Add(adminUser);
                context.SaveChanges();

                var adminRole = context.Roles.First(r => r.RoleName == "Admin");
                context.UserRoles.Add(new UserRole
                {
                    UserId = adminUser.UserId,
                    RoleId = adminRole.RoleId,
                    AssignedAt = DateTime.Now
                });
                context.SaveChanges();
            }

            // 5. Seed Categories
            if (!context.Categories.Any())
            {
                context.Categories.AddRange(
                    new Category { CategoryName = "Wood", Description = "Wood materials" },
                    new Category { CategoryName = "Metal", Description = "Metal materials" },
                    new Category { CategoryName = "Plastic", Description = "Plastic materials" }
                );
                context.SaveChanges();
            }

            // 6. Seed Warehouses
            if (!context.Warehouses.Any())
            {
                var manager = context.Users.FirstOrDefault();
                context.Warehouses.AddRange(
                    new Warehouse { WarehouseName = "Main Warehouse", Location = "Hanoi", ManagerId = manager?.UserId },
                    new Warehouse { WarehouseName = "Secondary Warehouse", Location = "Ho Chi Minh", ManagerId = manager?.UserId }
                );
                context.SaveChanges();
            }
            // 6. Seed Warehouses
            if (!context.Warehouses.Any())
            {
                var manager = context.Users.FirstOrDefault();

                context.Warehouses.AddRange(
                    new Warehouse { WarehouseName = "Main Warehouse", Location = "Hanoi", ManagerId = manager?.UserId },
                    new Warehouse { WarehouseName = "Secondary Warehouse", Location = "Ho Chi Minh", ManagerId = manager?.UserId }
                );
                context.SaveChanges();
            }

            // 7. Seed Inventories
            if (!context.Inventories.Any())
            {
                var mainWarehouse = context.Warehouses.First(w => w.WarehouseName == "Main Warehouse");
                var secondaryWarehouse = context.Warehouses.First(w => w.WarehouseName == "Secondary Warehouse");

                var wood = context.Materials.First(m => m.MaterialCode == "W001");
                var metal = context.Materials.First(m => m.MaterialCode == "M001");
                var plastic = context.Materials.First(m => m.MaterialCode == "P001");

                context.Inventories.AddRange(
                    new Inventory
                    {
                        WarehouseId = mainWarehouse.WarehouseId,
                        MaterialId = wood.MaterialId,
                        Quantity = 100,
                        UnitPrice = 20,
                        CreatedAt = DateTime.Now
                    },
                    new Inventory
                    {
                        WarehouseId = mainWarehouse.WarehouseId,
                        MaterialId = metal.MaterialId,
                        Quantity = 50,
                        UnitPrice = 50,
                        CreatedAt = DateTime.Now
                    },
                    new Inventory
                    {
                        WarehouseId = secondaryWarehouse.WarehouseId,
                        MaterialId = plastic.MaterialId,
                        Quantity = 200,
                        UnitPrice = 10,
                        CreatedAt = DateTime.Now
                    }
                );
                context.SaveChanges();
            }

            // 7. Seed Materials
            if (!context.Materials.Any())
            {
                var partner = context.Partners.First();

                var woodCategory = context.Categories.First(c => c.CategoryName == "Wood");
                var metalCategory = context.Categories.First(c => c.CategoryName == "Metal");
                var plasticCategory = context.Categories.First(c => c.CategoryName == "Plastic");

                context.Materials.AddRange(
                    new Material { MaterialCode = "W001", MaterialName = "Wood Board", Unit = "pcs", PartnerId = partner.PartnerId, CategoryId = woodCategory.CategoryId },
                    new Material { MaterialCode = "M001", MaterialName = "Steel Rod", Unit = "pcs", PartnerId = partner.PartnerId, CategoryId = metalCategory.CategoryId },
                    new Material { MaterialCode = "P001", MaterialName = "Plastic Sheet", Unit = "pcs", PartnerId = partner.PartnerId, CategoryId = plasticCategory.CategoryId }
                );
                context.SaveChanges();
            }

            // 9. Seed Invoices + InvoiceDetails
            if (!context.Invoices.Any())
            {
                var user = context.Users.First();
                var partner = context.Partners.First();

                var invoice1 = new Invoice
                {
                    InvoiceCode = "INV-001",
                    InvoiceNumber = "INV-001",
                    InvoiceType = "Import",
                    PartnerId = partner.PartnerId,
                    CreatedBy = user.UserId,
                    IssueDate = DateTime.Now.AddDays(-1),
                    Status = "Pending",
                    CreatedAt = DateTime.Now
                };

                var invoice2 = new Invoice
                {
                    InvoiceCode = "INV-002",
                    InvoiceNumber = "INV-002",
                    InvoiceType = "Import",
                    PartnerId = partner.PartnerId,
                    CreatedBy = user.UserId,
                    IssueDate = DateTime.Now.AddDays(-2),
                    Status = "Pending",
                    CreatedAt = DateTime.Now
                };

                context.Invoices.AddRange(invoice1, invoice2);
                context.SaveChanges();

                // InvoiceDetails cho invoice1
                var wood = context.Materials.First(m => m.MaterialCode == "W001");
                var metal = context.Materials.First(m => m.MaterialCode == "M001");

                context.InvoiceDetails.AddRange(
                    new InvoiceDetail
                    {
                        InvoiceId = invoice1.InvoiceId,
                        MaterialId = wood.MaterialId,
                        Quantity = 50,
                        UnitPrice = 20,
                        LineTotal = 50 * 20
                    },
                    new InvoiceDetail
                    {
                        InvoiceId = invoice1.InvoiceId,
                        MaterialId = metal.MaterialId,
                        Quantity = 30,
                        UnitPrice = 50,
                        LineTotal = 30 * 50
                    }
                );

                // InvoiceDetails cho invoice2
                var plastic = context.Materials.First(m => m.MaterialCode == "P001");

                context.InvoiceDetails.AddRange(
                    new InvoiceDetail
                    {
                        InvoiceId = invoice2.InvoiceId,
                        MaterialId = plastic.MaterialId,
                        Quantity = 100,
                        UnitPrice = 10,
                        LineTotal = 100 * 10
                    }
                );

                context.SaveChanges();
            }

        }
    }
}
