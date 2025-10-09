using Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

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
                    new PartnerType { TypeName = "Nhà cung cấp" },
                    new PartnerType { TypeName = "Nhà phân phối" },
                    new PartnerType { TypeName = "Đại lý" },
                    new PartnerType { TypeName = "Khách hàng lẻ" },
                    new PartnerType { TypeName = "Đối tác chiến lược" },
                    new PartnerType { TypeName = "Nhà thầu" },
                    new PartnerType { TypeName = "Cộng tác viên" }
                );
                context.SaveChanges();
            }

            // 2️⃣ Seed Partners
            if (!context.Partners.Any())
            {
                var supplierType = context.PartnerTypes.First(pt => pt.TypeName == "Nhà cung cấp");
                var distributorType = context.PartnerTypes.First(pt => pt.TypeName == "Nhà phân phối");
                var agentType = context.PartnerTypes.First(pt => pt.TypeName == "Đại lý");
                var customerType = context.PartnerTypes.First(pt => pt.TypeName == "Khách hàng lẻ");
                var strategicType = context.PartnerTypes.First(pt => pt.TypeName == "Đối tác chiến lược");
                var contractorType = context.PartnerTypes.First(pt => pt.TypeName == "Nhà thầu");
                var collaboratorType = context.PartnerTypes.First(pt => pt.TypeName == "Cộng tác viên");

                context.Partners.AddRange(
                    new Partner { PartnerName = "Công ty Gỗ Việt", PartnerTypeId = supplierType.PartnerTypeId, ContactEmail = "contact@goviet.vn", ContactPhone = "0903123456" },
                    new Partner { PartnerName = "Thép Hòa Phát", PartnerTypeId = supplierType.PartnerTypeId, ContactEmail = "info@hoaphatsteel.vn", ContactPhone = "0911222333" },
                    new Partner { PartnerName = "Nhựa Duy Tân", PartnerTypeId = distributorType.PartnerTypeId, ContactEmail = "sales@duytanplastic.vn", ContactPhone = "0988999777" },
                    new Partner { PartnerName = "Đại lý Minh Tâm", PartnerTypeId = agentType.PartnerTypeId, ContactEmail = "minhtam@agent.vn", ContactPhone = "0933444555" },
                    new Partner { PartnerName = "Khách hàng Lê Văn A", PartnerTypeId = customerType.PartnerTypeId, ContactEmail = "levana@customer.vn", ContactPhone = "0915666777" },
                    new Partner { PartnerName = "Công ty xây dựng Sài Gòn", PartnerTypeId = contractorType.PartnerTypeId, ContactEmail = "saigonbuild@contractor.vn", ContactPhone = "0909777888" },
                    new Partner { PartnerName = "Cộng tác viên Nguyễn Thị B", PartnerTypeId = collaboratorType.PartnerTypeId, ContactEmail = "nguyenb@collaborator.vn", ContactPhone = "0922333444" }
                );
                context.SaveChanges();
            }

            // 3️⃣ Seed Roles
            if (!context.Roles.Any())
            {
                context.Roles.AddRange(
                    new Role { RoleName = "Quản trị viên", Description = "Quản trị toàn bộ hệ thống" },
                    new Role { RoleName = "Quản lý kho", Description = "Quản lý kho và nhân sự" },
                    new Role { RoleName = "Nhân viên kho", Description = "Nhân viên nhập/xuất kho" },
                    new Role { RoleName = "Kế toán", Description = "Quản lý tài chính và hóa đơn" },
                    new Role { RoleName = "Nhân viên bán hàng", Description = "Xử lý đơn hàng và khách hàng" },
                    new Role { RoleName = "Nhân viên hỗ trợ", Description = "Hỗ trợ khách hàng và kho" },
                    new Role { RoleName = "Kiểm kho", Description = "Kiểm tra và báo cáo tồn kho" }
                );
                context.SaveChanges();
            }

            // 4️⃣ Seed Users
            if (!context.Users.Any())
            {
                context.Users.AddRange(
                    new User { UserName = "admin", Email = "admin@scmvlxd.vn", FullName = "Nguyễn Văn Admin", PasswordHash = "admin123", CreatedAt = DateTime.Now },
                    new User { UserName = "manager1", Email = "manager1@scmvlxd.vn", FullName = "Trần Thị Quản Lý", PasswordHash = "manager123", CreatedAt = DateTime.Now },
                    new User { UserName = "staff01", Email = "staff01@scmvlxd.vn", FullName = "Lê Văn Nhân Viên", PasswordHash = "staff123", CreatedAt = DateTime.Now },
                    new User { UserName = "accountant1", Email = "accountant1@scmvlxd.vn", FullName = "Phạm Thị Kế Toán", PasswordHash = "accountant123", CreatedAt = DateTime.Now },
                    new User { UserName = "sales1", Email = "sales1@scmvlxd.vn", FullName = "Ngô Văn Bán Hàng", PasswordHash = "sales123", CreatedAt = DateTime.Now },
                    new User { UserName = "support1", Email = "support1@scmvlxd.vn", FullName = "Vũ Thị Hỗ Trợ", PasswordHash = "support123", CreatedAt = DateTime.Now },
                    new User { UserName = "inventory1", Email = "inventory1@scmvlxd.vn", FullName = "Đỗ Văn Kiểm Kho", PasswordHash = "inventory123", CreatedAt = DateTime.Now }
                );
                context.SaveChanges();

                var adminRole = context.Roles.First(r => r.RoleName == "Quản trị viên");
                var managerRole = context.Roles.First(r => r.RoleName == "Quản lý kho");
                var staffRole = context.Roles.First(r => r.RoleName == "Nhân viên kho");
                var accountantRole = context.Roles.First(r => r.RoleName == "Kế toán");
                var salesRole = context.Roles.First(r => r.RoleName == "Nhân viên bán hàng");
                var supportRole = context.Roles.First(r => r.RoleName == "Nhân viên hỗ trợ");
                var inventoryRole = context.Roles.First(r => r.RoleName == "Kiểm kho");

                context.UserRoles.AddRange(
                    new UserRole { UserId = context.Users.First(u => u.UserName == "admin").UserId, RoleId = adminRole.RoleId, AssignedAt = DateTime.Now },
                    new UserRole { UserId = context.Users.First(u => u.UserName == "manager1").UserId, RoleId = managerRole.RoleId, AssignedAt = DateTime.Now },
                    new UserRole { UserId = context.Users.First(u => u.UserName == "staff01").UserId, RoleId = staffRole.RoleId, AssignedAt = DateTime.Now },
                    new UserRole { UserId = context.Users.First(u => u.UserName == "accountant1").UserId, RoleId = accountantRole.RoleId, AssignedAt = DateTime.Now },
                    new UserRole { UserId = context.Users.First(u => u.UserName == "sales1").UserId, RoleId = salesRole.RoleId, AssignedAt = DateTime.Now },
                    new UserRole { UserId = context.Users.First(u => u.UserName == "support1").UserId, RoleId = supportRole.RoleId, AssignedAt = DateTime.Now },
                    new UserRole { UserId = context.Users.First(u => u.UserName == "inventory1").UserId, RoleId = inventoryRole.RoleId, AssignedAt = DateTime.Now }
                );
                context.SaveChanges();
            }

            // 5️⃣ Seed Categories
            if (!context.Categories.Any())
            {
                context.Categories.AddRange(
                    new Category { CategoryName = "Gỗ", Description = "Vật liệu từ gỗ tự nhiên và công nghiệp" },
                    new Category { CategoryName = "Kim loại", Description = "Vật liệu kim loại xây dựng" },
                    new Category { CategoryName = "Nhựa", Description = "Vật liệu nhựa công nghiệp" },
                    new Category { CategoryName = "Xi măng", Description = "Vật liệu xi măng xây dựng" },
                    new Category { CategoryName = "Gạch", Description = "Gạch xây dựng và trang trí" },
                    new Category { CategoryName = "Sơn", Description = "Sơn xây dựng và công nghiệp" },
                    new Category { CategoryName = "Kính", Description = "Kính xây dựng và trang trí" }
                );
                context.SaveChanges();
            }

            // 6️⃣ Seed Warehouses
            if (!context.Warehouses.Any())
            {
                var manager = context.Users.First(u => u.UserName == "manager1");
                context.Warehouses.AddRange(
                    new Warehouse { WarehouseName = "Kho Hà Nội", Location = "Số 12 Nguyễn Trãi, Thanh Xuân, Hà Nội", ManagerId = manager.UserId },
                    new Warehouse { WarehouseName = "Kho TP.HCM", Location = "Số 98 Lê Văn Việt, Quận 9, TP.HCM", ManagerId = manager.UserId },
                    new Warehouse { WarehouseName = "Kho Đà Nẵng", Location = "Số 45 Nguyễn Văn Linh, Hải Châu, Đà Nẵng", ManagerId = manager.UserId },
                    new Warehouse { WarehouseName = "Kho Hải Phòng", Location = "Số 33 Lê Lợi, Ngô Quyền, Hải Phòng", ManagerId = manager.UserId },
                    new Warehouse { WarehouseName = "Kho Cần Thơ", Location = "Số 22 Nguyễn Văn Cừ, Ninh Kiều, Cần Thơ", ManagerId = manager.UserId },
                    new Warehouse { WarehouseName = "Kho Nha Trang", Location = "Số 15 Lê Hồng Phong, Phước Hải, Nha Trang", ManagerId = manager.UserId },
                    new Warehouse { WarehouseName = "Kho Vũng Tàu", Location = "Số 78 Nguyễn An Ninh, Phường 7, Vũng Tàu", ManagerId = manager.UserId }
                );
                context.SaveChanges();
            }

            // 7️⃣ Seed Materials
            if (!context.Materials.Any())
            {
                var woodCat = context.Categories.First(c => c.CategoryName == "Gỗ");
                var metalCat = context.Categories.First(c => c.CategoryName == "Kim loại");
                var plasticCat = context.Categories.First(c => c.CategoryName == "Nhựa");
                var cementCat = context.Categories.First(c => c.CategoryName == "Xi măng");
                var brickCat = context.Categories.First(c => c.CategoryName == "Gạch");
                var paintCat = context.Categories.First(c => c.CategoryName == "Sơn");
                var glassCat = context.Categories.First(c => c.CategoryName == "Kính");
                var goviet = context.Partners.First(p => p.PartnerName == "Công ty Gỗ Việt");
                var hoaphat = context.Partners.First(p => p.PartnerName == "Thép Hòa Phát");
                var duytan = context.Partners.First(p => p.PartnerName == "Nhựa Duy Tân");

                context.Materials.AddRange(
                    new Material { MaterialCode = "W001", MaterialName = "Gỗ thông tấm 2m", Unit = "tấm", PartnerId = goviet.PartnerId, CategoryId = woodCat.CategoryId },
                    new Material { MaterialCode = "M001", MaterialName = "Thép cây D20", Unit = "cây", PartnerId = hoaphat.PartnerId, CategoryId = metalCat.CategoryId },
                    new Material { MaterialCode = "P001", MaterialName = "Tấm nhựa PVC 1m x 2m", Unit = "tấm", PartnerId = duytan.PartnerId, CategoryId = plasticCat.CategoryId },
                    new Material { MaterialCode = "C001", MaterialName = "Xi măng PC40", Unit = "bao", PartnerId = hoaphat.PartnerId, CategoryId = cementCat.CategoryId },
                    new Material { MaterialCode = "B001", MaterialName = "Gạch đỏ 20x20", Unit = "viên", PartnerId = goviet.PartnerId, CategoryId = brickCat.CategoryId },
                    new Material { MaterialCode = "S001", MaterialName = "Sơn nước Dulux 20L", Unit = "thùng", PartnerId = duytan.PartnerId, CategoryId = paintCat.CategoryId },
                    new Material { MaterialCode = "G001", MaterialName = "Kính cường lực 8mm", Unit = "m2", PartnerId = goviet.PartnerId, CategoryId = glassCat.CategoryId }
                );
                context.SaveChanges();
            }

            // 8️⃣ Seed Inventories
            if (!context.Inventories.Any())
            {
                var wh1 = context.Warehouses.First(w => w.WarehouseName == "Kho Hà Nội");
                var wh2 = context.Warehouses.First(w => w.WarehouseName == "Kho TP.HCM");
                var wh3 = context.Warehouses.First(w => w.WarehouseName == "Kho Đà Nẵng");
                var wood = context.Materials.First(m => m.MaterialCode == "W001");
                var metal = context.Materials.First(m => m.MaterialCode == "M001");
                var plastic = context.Materials.First(m => m.MaterialCode == "P001");
                var cement = context.Materials.First(m => m.MaterialCode == "C001");
                var brick = context.Materials.First(m => m.MaterialCode == "B001");
                var paint = context.Materials.First(m => m.MaterialCode == "S001");
                var glass = context.Materials.First(m => m.MaterialCode == "G001");

                context.Inventories.AddRange(
                    new Inventory { WarehouseId = wh1.WarehouseId, MaterialId = wood.MaterialId, Quantity = 120, UnitPrice = 250000, CreatedAt = DateTime.Now },
                    new Inventory { WarehouseId = wh1.WarehouseId, MaterialId = metal.MaterialId, Quantity = 80, UnitPrice = 320000, CreatedAt = DateTime.Now },
                    new Inventory { WarehouseId = wh2.WarehouseId, MaterialId = plastic.MaterialId, Quantity = 200, UnitPrice = 180000, CreatedAt = DateTime.Now },
                    new Inventory { WarehouseId = wh2.WarehouseId, MaterialId = cement.MaterialId, Quantity = 150, UnitPrice = 90000, CreatedAt = DateTime.Now },
                    new Inventory { WarehouseId = wh3.WarehouseId, MaterialId = brick.MaterialId, Quantity = 5000, UnitPrice = 1200, CreatedAt = DateTime.Now },
                    new Inventory { WarehouseId = wh3.WarehouseId, MaterialId = paint.MaterialId, Quantity = 50, UnitPrice = 1500000, CreatedAt = DateTime.Now },
                    new Inventory { WarehouseId = wh1.WarehouseId, MaterialId = glass.MaterialId, Quantity = 100, UnitPrice = 200000, CreatedAt = DateTime.Now }
                );
                context.SaveChanges();
            }

            // 9️⃣ Seed Invoices & InvoiceDetails
            if (!context.Invoices.Any())
            {
                var manager = context.Users.First(u => u.UserName == "manager1");
                var goviet = context.Partners.First(p => p.PartnerName == "Công ty Gỗ Việt");
                var hoaphat = context.Partners.First(p => p.PartnerName == "Thép Hòa Phát");
                var duytan = context.Partners.First(p => p.PartnerName == "Nhựa Duy Tân");
                var minhtam = context.Partners.First(p => p.PartnerName == "Đại lý Minh Tâm");
                var saigonbuild = context.Partners.First(p => p.PartnerName == "Công ty xây dựng Sài Gòn");
                var levan = context.Partners.First(p => p.PartnerName == "Khách hàng Lê Văn A");
                var nguyenb = context.Partners.First(p => p.PartnerName == "Cộng tác viên Nguyễn Thị B");

                context.Invoices.AddRange(
                    new Invoice { InvoiceCode = "INV-001", InvoiceType = "Import", PartnerId = goviet.PartnerId, CreatedBy = manager.UserId, IssueDate = DateTime.Now.AddDays(-10), Status = "Pending", CreatedAt = DateTime.Now },
                    new Invoice { InvoiceCode = "INV-002", InvoiceType = "Import", PartnerId = hoaphat.PartnerId, CreatedBy = manager.UserId, IssueDate = DateTime.Now.AddDays(-15), Status = "Approved", CreatedAt = DateTime.Now },
                    new Invoice { InvoiceCode = "INV-003", InvoiceType = "Import", PartnerId = duytan.PartnerId, CreatedBy = manager.UserId, IssueDate = DateTime.Now.AddDays(-20), Status = "Success", CreatedAt = DateTime.Now },
                    new Invoice { InvoiceCode = "INV-004", InvoiceType = "Export", PartnerId = minhtam.PartnerId, CreatedBy = manager.UserId, IssueDate = DateTime.Now.AddDays(-5), Status = "Pending", CreatedAt = DateTime.Now },
                    new Invoice { InvoiceCode = "INV-005", InvoiceType = "Export", PartnerId = saigonbuild.PartnerId, CreatedBy = manager.UserId, IssueDate = DateTime.Now.AddDays(-7), Status = "Approved", CreatedAt = DateTime.Now },
                    new Invoice { InvoiceCode = "INV-006", InvoiceType = "Export", PartnerId = levan.PartnerId, CreatedBy = manager.UserId, IssueDate = DateTime.Now.AddDays(-3), Status = "Success", CreatedAt = DateTime.Now },
                    new Invoice { InvoiceCode = "INV-007", InvoiceType = "Import", PartnerId = nguyenb.PartnerId, CreatedBy = manager.UserId, IssueDate = DateTime.Now.AddDays(-2), Status = "Pending", CreatedAt = DateTime.Now }
                );
                context.SaveChanges();

                var wood = context.Materials.First(m => m.MaterialCode == "W001");
                var metal = context.Materials.First(m => m.MaterialCode == "M001");
                var plastic = context.Materials.First(m => m.MaterialCode == "P001");
                var cement = context.Materials.First(m => m.MaterialCode == "C001");
                var brick = context.Materials.First(m => m.MaterialCode == "B001");
                var paint = context.Materials.First(m => m.MaterialCode == "S001");
                var glass = context.Materials.First(m => m.MaterialCode == "G001");

                context.InvoiceDetails.AddRange(
                    new InvoiceDetail { InvoiceId = context.Invoices.First(i => i.InvoiceCode == "INV-001").InvoiceId, MaterialId = wood.MaterialId, Quantity = 50, UnitPrice = 250000, LineTotal = 50 * 250000 },
                    new InvoiceDetail { InvoiceId = context.Invoices.First(i => i.InvoiceCode == "INV-002").InvoiceId, MaterialId = metal.MaterialId, Quantity = 20, UnitPrice = 320000, LineTotal = 20 * 320000 },
                    new InvoiceDetail { InvoiceId = context.Invoices.First(i => i.InvoiceCode == "INV-003").InvoiceId, MaterialId = plastic.MaterialId, Quantity = 100, UnitPrice = 180000, LineTotal = 100 * 180000 },
                    new InvoiceDetail { InvoiceId = context.Invoices.First(i => i.InvoiceCode == "INV-004").InvoiceId, MaterialId = cement.MaterialId, Quantity = 80, UnitPrice = 90000, LineTotal = 80 * 90000 },
                    new InvoiceDetail { InvoiceId = context.Invoices.First(i => i.InvoiceCode == "INV-005").InvoiceId, MaterialId = brick.MaterialId, Quantity = 2000, UnitPrice = 1200, LineTotal = 2000 * 1200 },
                    new InvoiceDetail { InvoiceId = context.Invoices.First(i => i.InvoiceCode == "INV-006").InvoiceId, MaterialId = paint.MaterialId, Quantity = 10, UnitPrice = 1500000, LineTotal = 10 * 1500000 },
                    new InvoiceDetail { InvoiceId = context.Invoices.First(i => i.InvoiceCode == "INV-007").InvoiceId, MaterialId = glass.MaterialId, Quantity = 50, UnitPrice = 200000, LineTotal = 50 * 200000 }
                );
                context.SaveChanges();
            }

            // 🔟 Seed Imports & ImportDetails
            if (!context.Imports.Any())
            {
                var pendingInvoices = context.Invoices
                    .Where(i => i.Status == "Pending" && i.InvoiceType == "Import")
                    .ToList();
                var manager = context.Users.First(u => u.UserName == "manager1");
                var wh1 = context.Warehouses.First(w => w.WarehouseName == "Kho Hà Nội");

                foreach (var invoice in pendingInvoices)
                {
                    var import = new Import
                    {
                        ImportCode = "IMP-" + Guid.NewGuid().ToString("N").Substring(0, 8),
                        ImportDate = DateTime.Now,
                        WarehouseId = wh1.WarehouseId,
                        CreatedBy = invoice.CreatedBy,
                        Notes = $"Tự động nhập từ hóa đơn {invoice.InvoiceCode}",
                        Status = "Pending", // Đổi trạng thái sang Pending
                        CreatedAt = DateTime.Now
                    };
                    context.Imports.Add(import);
                    context.SaveChanges();

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
                    context.SaveChanges();
                }

                var wood = context.Materials.First(m => m.MaterialCode == "W001");
                var metal = context.Materials.First(m => m.MaterialCode == "M001");
                var plastic = context.Materials.First(m => m.MaterialCode == "P001");
                var cement = context.Materials.First(m => m.MaterialCode == "C001");
                var brick = context.Materials.First(m => m.MaterialCode == "B001");
                var paint = context.Materials.First(m => m.MaterialCode == "S001");
                var glass = context.Materials.First(m => m.MaterialCode == "G001");

                context.Imports.AddRange(
                    new Import { ImportCode = "IMP-001", ImportDate = DateTime.Now, WarehouseId = wh1.WarehouseId, CreatedBy = manager.UserId, Notes = "Nhập gỗ và thép", Status = "Pending", CreatedAt = DateTime.Now },
                    new Import { ImportCode = "IMP-002", ImportDate = DateTime.Now, WarehouseId = wh1.WarehouseId, CreatedBy = manager.UserId, Notes = "Nhập nhựa", Status = "Pending", CreatedAt = DateTime.Now },
                    new Import { ImportCode = "IMP-003", ImportDate = DateTime.Now, WarehouseId = wh1.WarehouseId, CreatedBy = manager.UserId, Notes = "Nhập xi măng", Status = "Pending", CreatedAt = DateTime.Now },
                    new Import { ImportCode = "IMP-004", ImportDate = DateTime.Now, WarehouseId = wh1.WarehouseId, CreatedBy = manager.UserId, Notes = "Nhập gạch", Status = "Pending", CreatedAt = DateTime.Now },
                    new Import { ImportCode = "IMP-005", ImportDate = DateTime.Now, WarehouseId = wh1.WarehouseId, CreatedBy = manager.UserId, Notes = "Nhập sơn", Status = "Pending", CreatedAt = DateTime.Now },
                    new Import { ImportCode = "IMP-006", ImportDate = DateTime.Now, WarehouseId = wh1.WarehouseId, CreatedBy = manager.UserId, Notes = "Nhập kính", Status = "Pending", CreatedAt = DateTime.Now },
                    new Import { ImportCode = "IMP-PENDING-001", ImportDate = DateTime.Now, WarehouseId = wh1.WarehouseId, CreatedBy = manager.UserId, Notes = "Phiếu nhập đang chờ duyệt", Status = "Pending", CreatedAt = DateTime.Now }
                );
                context.SaveChanges();

                context.ImportDetails.AddRange(
                    new ImportDetail { ImportId = context.Imports.First(i => i.ImportCode == "IMP-001").ImportId, MaterialId = wood.MaterialId, MaterialCode = wood.MaterialCode ?? "", MaterialName = wood.MaterialName, Unit = wood.Unit, UnitPrice = 250000, Quantity = 50, LineTotal = 250000 * 50 },
                    new ImportDetail { ImportId = context.Imports.First(i => i.ImportCode == "IMP-001").ImportId, MaterialId = metal.MaterialId, MaterialCode = metal.MaterialCode ?? "", MaterialName = metal.MaterialName, Unit = metal.Unit, UnitPrice = 320000, Quantity = 20, LineTotal = 320000 * 20 },
                    new ImportDetail { ImportId = context.Imports.First(i => i.ImportCode == "IMP-002").ImportId, MaterialId = plastic.MaterialId, MaterialCode = plastic.MaterialCode ?? "", MaterialName = plastic.MaterialName, Unit = plastic.Unit, UnitPrice = 180000, Quantity = 100, LineTotal = 180000 * 100 },
                    new ImportDetail { ImportId = context.Imports.First(i => i.ImportCode == "IMP-003").ImportId, MaterialId = cement.MaterialId, MaterialCode = cement.MaterialCode ?? "", MaterialName = cement.MaterialName, Unit = cement.Unit, UnitPrice = 90000, Quantity = 80, LineTotal = 90000 * 80 },
                    new ImportDetail { ImportId = context.Imports.First(i => i.ImportCode == "IMP-004").ImportId, MaterialId = brick.MaterialId, MaterialCode = brick.MaterialCode ?? "", MaterialName = brick.MaterialName, Unit = brick.Unit, UnitPrice = 1200, Quantity = 2000, LineTotal = 1200 * 2000 },
                    new ImportDetail { ImportId = context.Imports.First(i => i.ImportCode == "IMP-005").ImportId, MaterialId = paint.MaterialId, MaterialCode = paint.MaterialCode ?? "", MaterialName = paint.MaterialName, Unit = paint.Unit, UnitPrice = 1500000, Quantity = 10, LineTotal = 1500000 * 10 },
                    new ImportDetail { ImportId = context.Imports.First(i => i.ImportCode == "IMP-006").ImportId, MaterialId = glass.MaterialId, MaterialCode = glass.MaterialCode ?? "", MaterialName = glass.MaterialName, Unit = glass.Unit, UnitPrice = 200000, Quantity = 50, LineTotal = 200000 * 50 }
                );
                context.SaveChanges();
            }

            // 11️⃣ Seed Exports & ExportDetails
            if (!context.Exports.Any())
            {
                var manager = context.Users.First(u => u.UserName == "manager1");
                var wh1 = context.Warehouses.First(w => w.WarehouseName == "Kho Hà Nội");
                var minhtam = context.Partners.First(p => p.PartnerName == "Đại lý Minh Tâm");
                var saigonbuild = context.Partners.First(p => p.PartnerName == "Công ty xây dựng Sài Gòn");
                var levan = context.Partners.First(p => p.PartnerName == "Khách hàng Lê Văn A");

                context.Exports.AddRange(
                    new Export { ExportCode = "EXP-001", ExportDate = DateTime.Now.AddDays(-5), WarehouseId = wh1.WarehouseId, CreatedBy = manager.UserId, Notes = "Xuất cho đại lý Minh Tâm", Status = "Success", CreatedAt = DateTime.Now },
                    new Export { ExportCode = "EXP-002", ExportDate = DateTime.Now.AddDays(-4), WarehouseId = wh1.WarehouseId, CreatedBy = manager.UserId, Notes = "Xuất cho công ty xây dựng", Status = "Success", CreatedAt = DateTime.Now },
                    new Export { ExportCode = "EXP-003", ExportDate = DateTime.Now.AddDays(-3), WarehouseId = wh1.WarehouseId, CreatedBy = manager.UserId, Notes = "Xuất cho khách lẻ", Status = "Success", CreatedAt = DateTime.Now },
                    new Export { ExportCode = "EXP-004", ExportDate = DateTime.Now.AddDays(-2), WarehouseId = wh1.WarehouseId, CreatedBy = manager.UserId, Notes = "Xuất gạch và xi măng", Status = "Success", CreatedAt = DateTime.Now },
                    new Export { ExportCode = "EXP-005", ExportDate = DateTime.Now.AddDays(-1), WarehouseId = wh1.WarehouseId, CreatedBy = manager.UserId, Notes = "Xuất sơn và kính", Status = "Success", CreatedAt = DateTime.Now },
                    new Export { ExportCode = "EXP-006", ExportDate = DateTime.Now, WarehouseId = wh1.WarehouseId, CreatedBy = manager.UserId, Notes = "Xuất gỗ và thép", Status = "Success", CreatedAt = DateTime.Now },
                    new Export { ExportCode = "EXP-PENDING-001", ExportDate = DateTime.Now, WarehouseId = wh1.WarehouseId, CreatedBy = manager.UserId, Notes = "Phiếu xuất đang chờ duyệt", Status = "Pending", CreatedAt = DateTime.Now }
                );
                context.SaveChanges();

                var wood = context.Materials.First(m => m.MaterialCode == "W001");
                var metal = context.Materials.First(m => m.MaterialCode == "M001");
                var plastic = context.Materials.First(m => m.MaterialCode == "P001");
                var cement = context.Materials.First(m => m.MaterialCode == "C001");
                var brick = context.Materials.First(m => m.MaterialCode == "B001");
                var paint = context.Materials.First(m => m.MaterialCode == "S001");
                var glass = context.Materials.First(m => m.MaterialCode == "G001");

                context.ExportDetails.AddRange(
                    new ExportDetail { ExportId = context.Exports.First(e => e.ExportCode == "EXP-001").ExportId, MaterialId = wood.MaterialId, MaterialCode = wood.MaterialCode ?? "", MaterialName = wood.MaterialName, Unit = wood.Unit, UnitPrice = 250000, Quantity = 20, LineTotal = 250000 * 20 },
                    new ExportDetail { ExportId = context.Exports.First(e => e.ExportCode == "EXP-002").ExportId, MaterialId = metal.MaterialId, MaterialCode = metal.MaterialCode ?? "", MaterialName = metal.MaterialName, Unit = metal.Unit, UnitPrice = 320000, Quantity = 10, LineTotal = 320000 * 10 },
                    new ExportDetail { ExportId = context.Exports.First(e => e.ExportCode == "EXP-003").ExportId, MaterialId = plastic.MaterialId, MaterialCode = plastic.MaterialCode ?? "", MaterialName = plastic.MaterialName, Unit = plastic.Unit, UnitPrice = 180000, Quantity = 50, LineTotal = 180000 * 50 },
                    new ExportDetail { ExportId = context.Exports.First(e => e.ExportCode == "EXP-004").ExportId, MaterialId = cement.MaterialId, MaterialCode = cement.MaterialCode ?? "", MaterialName = cement.MaterialName, Unit = cement.Unit, UnitPrice = 90000, Quantity = 40, LineTotal = 90000 * 40 },
                    new ExportDetail { ExportId = context.Exports.First(e => e.ExportCode == "EXP-005").ExportId, MaterialId = brick.MaterialId, MaterialCode = brick.MaterialCode ?? "", MaterialName = brick.MaterialName, Unit = brick.Unit, UnitPrice = 1200, Quantity = 1000, LineTotal = 1200 * 1000 },
                    new ExportDetail { ExportId = context.Exports.First(e => e.ExportCode == "EXP-006").ExportId, MaterialId = paint.MaterialId, MaterialCode = paint.MaterialCode ?? "", MaterialName = paint.MaterialName, Unit = paint.Unit, UnitPrice = 1500000, Quantity = 5, LineTotal = 1500000 * 5 },
                    new ExportDetail { ExportId = context.Exports.First(e => e.ExportCode == "EXP-PENDING-001").ExportId, MaterialId = glass.MaterialId, MaterialCode = glass.MaterialCode ?? "", MaterialName = glass.MaterialName, Unit = glass.Unit, UnitPrice = 200000, Quantity = 25, LineTotal = 200000 * 25 }
                );
                context.SaveChanges();
            }
        }
    }
}