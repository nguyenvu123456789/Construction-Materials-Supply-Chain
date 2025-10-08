using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence
{
    public static class SeedData
    {
        public static void Initialize(ScmVlxdContext context)
        {
            context.Database.EnsureCreated();

            // 1️⃣ Seed PartnerTypes
            if (!context.PartnerTypes.Any())
            {
                context.PartnerTypes.AddRange(
                    new PartnerType { TypeName = "Supplier" },
                    new PartnerType { TypeName = "Distributor" },
                    new PartnerType { TypeName = "Agent" }
                );
                context.SaveChanges();
            }

            // 2️⃣ Seed Partners
            if (!context.Partners.Any())
            {
                var supplierType = context.PartnerTypes.First(pt => pt.TypeName == "Supplier");
                var distributorType = context.PartnerTypes.First(pt => pt.TypeName == "Distributor");

                context.Partners.AddRange(
                    new Partner
                    {
                        PartnerName = "Công ty Gỗ Việt",
                        PartnerTypeId = supplierType.PartnerTypeId,
                        ContactEmail = "contact@goviet.vn",
                        ContactPhone = "0903123456"
                    },
                    new Partner
                    {
                        PartnerName = "Thép Hòa Phát",
                        PartnerTypeId = supplierType.PartnerTypeId,
                        ContactEmail = "info@hoaphatsteel.vn",
                        ContactPhone = "0911222333"
                    },
                    new Partner
                    {
                        PartnerName = "Nhựa Duy Tân",
                        PartnerTypeId = distributorType.PartnerTypeId,
                        ContactEmail = "sales@duytanplastic.vn",
                        ContactPhone = "0988999777"
                    }
                );
                context.SaveChanges();
            }

            // 3️⃣ Seed Roles
            if (!context.Roles.Any())
            {
                context.Roles.AddRange(
                    new Role { RoleName = "Admin", Description = "Quản trị hệ thống" },
                    new Role { RoleName = "Manager", Description = "Quản lý kho và nhân sự" },
                    new Role { RoleName = "User", Description = "Nhân viên nhập/xuất kho" }
                );
                context.SaveChanges();
            }

            // 4️⃣ Seed Users
            if (!context.Users.Any())
            {
                var admin = new User
                {
                    UserName = "admin",
                    Email = "admin@scmvlxd.vn",
                    FullName = "Nguyễn Văn Admin",
                    PasswordHash = "admin123",
                    CreatedAt = DateTime.Now
                };

                var manager = new User
                {
                    UserName = "manager1",
                    Email = "manager1@scmvlxd.vn",
                    FullName = "Trần Thị Quản Lý",
                    PasswordHash = "manager123",
                    CreatedAt = DateTime.Now
                };

                var staff = new User
                {
                    UserName = "staff01",
                    Email = "staff01@scmvlxd.vn",
                    FullName = "Lê Văn Nhân Viên",
                    PasswordHash = "staff123",
                    CreatedAt = DateTime.Now
                };

                context.Users.AddRange(admin, manager, staff);
                context.SaveChanges();

                var adminRole = context.Roles.First(r => r.RoleName == "Admin");
                var managerRole = context.Roles.First(r => r.RoleName == "Manager");
                var userRole = context.Roles.First(r => r.RoleName == "User");

                context.UserRoles.AddRange(
                    new UserRole { UserId = admin.UserId, RoleId = adminRole.RoleId, AssignedAt = DateTime.Now },
                    new UserRole { UserId = manager.UserId, RoleId = managerRole.RoleId, AssignedAt = DateTime.Now },
                    new UserRole { UserId = staff.UserId, RoleId = userRole.RoleId, AssignedAt = DateTime.Now }
                );

                context.SaveChanges();
            }

            // 5️⃣ Seed Categories
            if (!context.Categories.Any())
            {
                context.Categories.AddRange(
                    new Category { CategoryName = "Gỗ", Description = "Vật liệu từ gỗ" },
                    new Category { CategoryName = "Kim loại", Description = "Vật liệu kim loại" },
                    new Category { CategoryName = "Nhựa", Description = "Vật liệu nhựa công nghiệp" }
                );
                context.SaveChanges();
            }

            // 6️⃣ Seed Warehouses
            if (!context.Warehouses.Any())
            {
                var manager = context.Users.First(u => u.UserName == "manager1");

                context.Warehouses.AddRange(
                    new Warehouse
                    {
                        WarehouseName = "Kho Hà Nội",
                        Location = "Số 12 Nguyễn Trãi, Thanh Xuân, Hà Nội",
                        ManagerId = manager.UserId
                    },
                    new Warehouse
                    {
                        WarehouseName = "Kho TP.HCM",
                        Location = "Số 98 Lê Văn Việt, Quận 9, TP.HCM",
                        ManagerId = manager.UserId
                    }
                );
                context.SaveChanges();
            }

            // 7️⃣ Seed Materials
            if (!context.Materials.Any())
            {
                var woodCat = context.Categories.First(c => c.CategoryName == "Gỗ");
                var metalCat = context.Categories.First(c => c.CategoryName == "Kim loại");
                var plasticCat = context.Categories.First(c => c.CategoryName == "Nhựa");

                var goviet = context.Partners.First(p => p.PartnerName == "Công ty Gỗ Việt");
                var hoaphat = context.Partners.First(p => p.PartnerName == "Thép Hòa Phát");
                var duytan = context.Partners.First(p => p.PartnerName == "Nhựa Duy Tân");

                context.Materials.AddRange(
                    new Material { MaterialCode = "W001", MaterialName = "Gỗ thông tấm 2m", Unit = "tấm", PartnerId = goviet.PartnerId, CategoryId = woodCat.CategoryId },
                    new Material { MaterialCode = "M001", MaterialName = "Thép cây D20", Unit = "cây", PartnerId = hoaphat.PartnerId, CategoryId = metalCat.CategoryId },
                    new Material { MaterialCode = "P001", MaterialName = "Tấm nhựa PVC 1m x 2m", Unit = "tấm", PartnerId = duytan.PartnerId, CategoryId = plasticCat.CategoryId }
                );
                context.SaveChanges();
            }

            // 8️⃣ Seed Inventories
            if (!context.Inventories.Any())
            {
                var wh1 = context.Warehouses.First(w => w.WarehouseName == "Kho Hà Nội");
                var wh2 = context.Warehouses.First(w => w.WarehouseName == "Kho TP.HCM");

                var wood = context.Materials.First(m => m.MaterialCode == "W001");
                var metal = context.Materials.First(m => m.MaterialCode == "M001");
                var plastic = context.Materials.First(m => m.MaterialCode == "P001");

                context.Inventories.AddRange(
                    new Inventory { WarehouseId = wh1.WarehouseId, MaterialId = wood.MaterialId, Quantity = 120, UnitPrice = 250000, CreatedAt = DateTime.Now },
                    new Inventory { WarehouseId = wh1.WarehouseId, MaterialId = metal.MaterialId, Quantity = 80, UnitPrice = 320000, CreatedAt = DateTime.Now },
                    new Inventory { WarehouseId = wh2.WarehouseId, MaterialId = plastic.MaterialId, Quantity = 200, UnitPrice = 180000, CreatedAt = DateTime.Now }
                );
                context.SaveChanges();
            }

            // 9️⃣ Seed Invoices & InvoiceDetails
            // 9️⃣ Seed Invoices và InvoiceDetails
            if (!context.Invoices.Any())
            {
                var manager = context.Users.First(u => u.UserName == "manager1");
                var goviet = context.Partners.First(p => p.PartnerName == "Công ty Gỗ Việt");
                var hoaphat = context.Partners.First(p => p.PartnerName == "Thép Hòa Phát");

                // Hai hóa đơn nhập
                var inv1 = new Invoice
                {
                    InvoiceCode = "INV-001",
                    InvoiceType = "Import",
                    PartnerId = goviet.PartnerId,
                    CreatedBy = manager.UserId,
                    IssueDate = DateTime.Now.AddDays(-10),
                    Status = "Pending",
                    CreatedAt = DateTime.Now
                };

                var inv2 = new Invoice
                {
                    InvoiceCode = "INV-002",
                    InvoiceType = "Import",
                    PartnerId = hoaphat.PartnerId,
                    CreatedBy = manager.UserId,
                    IssueDate = DateTime.Now.AddDays(-15),
                    Status = "Approved",
                    CreatedAt = DateTime.Now
                };

                context.Invoices.AddRange(inv1, inv2);
                context.SaveChanges();

                // Chi tiết hóa đơn
                var wood = context.Materials.First(m => m.MaterialCode == "W001");
                var metal = context.Materials.First(m => m.MaterialCode == "M001");
                var plastic = context.Materials.First(m => m.MaterialCode == "P001");

                context.InvoiceDetails.AddRange(
                    new InvoiceDetail
                    {
                        InvoiceId = inv1.InvoiceId,
                        MaterialId = wood.MaterialId,
                        Quantity = 50,
                        UnitPrice = 250000,
                        LineTotal = 50 * 250000
                    },
                    new InvoiceDetail
                    {
                        InvoiceId = inv1.InvoiceId,
                        MaterialId = metal.MaterialId,
                        Quantity = 20,
                        UnitPrice = 320000,
                        LineTotal = 20 * 320000
                    },
                    new InvoiceDetail
                    {
                        InvoiceId = inv2.InvoiceId,
                        MaterialId = plastic.MaterialId,
                        Quantity = 100,
                        UnitPrice = 180000,
                        LineTotal = 100 * 180000
                    }
                );
                context.SaveChanges();
            }

            // 🔟 Seed Imports & ImportDetails từ hóa đơn Pending
            if (!context.Imports.Any())
            {
                var pendingInvoices = context.Invoices
                    .Where(i => i.Status == "Pending" && i.InvoiceType == "Import")
                    .ToList();

                foreach (var invoice in pendingInvoices)
                {
                    // ✅ Tạo phiếu nhập tự động từ hóa đơn Pending
                    var import = new Import
                    {
                        ImportCode = "IMP-" + Guid.NewGuid().ToString("N").Substring(0, 8),
                        ImportDate = DateTime.Now,
                        WarehouseId = context.Warehouses.First().WarehouseId, // gán kho đầu tiên
                        CreatedBy = invoice.CreatedBy,
                        Notes = $"Tự động nhập từ hóa đơn {invoice.InvoiceCode}",
                        Status = "Success",
                        CreatedAt = DateTime.Now
                    };

                    context.Imports.Add(import);
                    context.SaveChanges();

                    // ✅ Copy chi tiết từ InvoiceDetail sang ImportDetail
                    var invoiceDetails = context.InvoiceDetails
                        .Where(d => d.InvoiceId == invoice.InvoiceId)
                        .ToList();

                    foreach (var detail in invoiceDetails)
                    {
                        var material = context.Materials.First(m => m.MaterialId == detail.MaterialId);

                        context.ImportDetails.Add(new ImportDetail
                        {
                            ImportId = import.ImportId,
                            MaterialId = material.MaterialId,
                            MaterialCode = material.MaterialCode ?? "",
                            MaterialName = material.MaterialName,
                            Unit = material.Unit,
                            UnitPrice = detail.UnitPrice,
                            Quantity = detail.Quantity,
                            LineTotal = detail.UnitPrice * detail.Quantity
                        });
                    }

                    // ✅ Cập nhật hóa đơn sang trạng thái Success
                    invoice.Status = "Success";
                    invoice.UpdatedAt = DateTime.Now;
                    context.Invoices.Update(invoice);

                    context.SaveChanges();
                }

                // ✅ Seed thêm một phiếu nhập Pending để test API tạo nhập kho thủ công
                var manager = context.Users.First(u => u.UserName == "manager1");
                var wh1 = context.Warehouses.First(w => w.WarehouseName == "Kho Hà Nội");
                var wood = context.Materials.First(m => m.MaterialCode == "W001");
                var metal = context.Materials.First(m => m.MaterialCode == "M001");

                var pendingImport = new Import
                {
                    ImportCode = "IMP-PENDING-001",
                    ImportDate = DateTime.Now,
                    WarehouseId = wh1.WarehouseId,
                    CreatedBy = manager.UserId,
                    Notes = "Phiếu nhập đang chờ duyệt (Pending)",
                    Status = "Pending",
                    CreatedAt = DateTime.Now
                };

                context.Imports.Add(pendingImport);
                context.SaveChanges();

                context.ImportDetails.AddRange(
                    new ImportDetail
                    {
                        ImportId = pendingImport.ImportId,
                        MaterialId = wood.MaterialId,
                        MaterialCode = wood.MaterialCode ?? "",
                        MaterialName = wood.MaterialName,
                        Unit = wood.Unit,
                        UnitPrice = 250000,
                        Quantity = 10,
                        LineTotal = 250000 * 10
                    },
                    new ImportDetail
                    {
                        ImportId = pendingImport.ImportId,
                        MaterialId = metal.MaterialId,
                        MaterialCode = metal.MaterialCode ?? "",
                        MaterialName = metal.MaterialName,
                        Unit = metal.Unit,
                        UnitPrice = 320000,
                        Quantity = 5,
                        LineTotal = 320000 * 5
                    }
                );
                context.SaveChanges();
            }



        }
    }
}
