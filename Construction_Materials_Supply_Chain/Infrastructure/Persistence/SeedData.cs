using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence
{
    public static class SeedData
    {
        public static void Initialize(ScmVlxdContext context)
        {
            context.Database.EnsureCreated();

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

            // Seed Regions
            if (!context.Regions.Any())
            {
                context.Regions.AddRange(
                    new Region { RegionName = "Tây Hồ" },
                    new Region { RegionName = "Ba Đình" },
                    new Region { RegionName = "Thanh Xuân" },
                    new Region { RegionName = "Cầu Giấy" },
                    new Region { RegionName = "Hà Nội" },
                    new Region { RegionName = "Hồ Chí Minh" },
                    new Region { RegionName = "Đà Nẵng" },
                    new Region { RegionName = "Hải Phòng" },
                    new Region { RegionName = "Cần Thơ" },
                    new Region { RegionName = "Bình Dương" },
                    new Region { RegionName = "Đồng Nai" },
                    new Region { RegionName = "Quảng Ninh" },
                    new Region { RegionName = "Thừa Thiên Huế" },
                    new Region { RegionName = "Khánh Hòa" },
                    new Region { RegionName = "Lâm Đồng" },
                    new Region { RegionName = "An Giang" },
                    new Region { RegionName = "Kiên Giang" },
                    new Region { RegionName = "Nghệ An" },
                    new Region { RegionName = "Thanh Hóa" }
                );

                context.SaveChanges();
            }


            var tayHo = context.Regions.First(r => r.RegionName == "Tây Hồ");
            var baDinh = context.Regions.First(r => r.RegionName == "Ba Đình");
            var thanhXuan = context.Regions.First(r => r.RegionName == "Thanh Xuân");
            var cauGiay = context.Regions.First(r => r.RegionName == "Cầu Giấy");

            // Seed Partners
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
                    new Partner { PartnerCode = "P001", PartnerName = "Công ty Gỗ Việt", PartnerTypeId = supplierType.PartnerTypeId, ContactEmail = "contact@goviet.vn", ContactPhone = "0903123456", Status = "Active", PartnerRegions = new List<PartnerRegion> { new PartnerRegion { Region = tayHo }, new PartnerRegion { Region = baDinh }, new PartnerRegion { Region = thanhXuan } } },
                    new Partner { PartnerCode = "P002", PartnerName = "Thép Hòa Phát", PartnerTypeId = supplierType.PartnerTypeId, ContactEmail = "info@hoaphatsteel.vn", ContactPhone = "0911222333", Status = "Active", PartnerRegions = new List<PartnerRegion> { new PartnerRegion { Region = tayHo }, new PartnerRegion { Region = cauGiay } } },
                    new Partner { PartnerCode = "P003", PartnerName = "Nhựa Duy Tân", PartnerTypeId = distributorType.PartnerTypeId, ContactEmail = "sales@duytanplastic.vn", ContactPhone = "0988999777", Status = "Active", PartnerRegions = new List<PartnerRegion> { new PartnerRegion { Region = tayHo } } }, // lấy region của partner 1
                    new Partner { PartnerCode = "P004", PartnerName = "Đại lý Minh Tâm", PartnerTypeId = agentType.PartnerTypeId, ContactEmail = "minhtam@agent.vn", ContactPhone = "0933444555", Status = "Active", PartnerRegions = new List<PartnerRegion> { new PartnerRegion { Region = tayHo }, new PartnerRegion { Region = baDinh } } },
                    new Partner { PartnerCode = "P001", PartnerName = "Công ty Gỗ Việt", PartnerTypeId = supplierType.PartnerTypeId, ContactEmail = "contact@goviet.vn", ContactPhone = "0903123456", Status = "Active", PartnerRegions = new List<PartnerRegion> { new PartnerRegion { Region = tayHo } } },
                    new Partner { PartnerCode = "P002", PartnerName = "Thép Hòa Phát", PartnerTypeId = supplierType.PartnerTypeId, ContactEmail = "info@hoaphatsteel.vn", ContactPhone = "0911222333", Status = "Active", PartnerRegions = new List<PartnerRegion> { new PartnerRegion { Region = tayHo } } },
                    new Partner { PartnerCode = "P003", PartnerName = "Nhựa Duy Tân", PartnerTypeId = distributorType.PartnerTypeId, ContactEmail = "sales@duytanplastic.vn", ContactPhone = "0988999777", Status = "Active", PartnerRegions = new List<PartnerRegion> { new PartnerRegion { Region = tayHo } } },
                    new Partner { PartnerCode = "P004", PartnerName = "Đại lý Minh Tâm", PartnerTypeId = agentType.PartnerTypeId, ContactEmail = "minhtam@agent.vn", ContactPhone = "0933444555", Status = "Active", PartnerRegions = new List<PartnerRegion> { new PartnerRegion { Region = tayHo } } },
                    new Partner { PartnerCode = "P005", PartnerName = "Khách hàng Lê Văn A", PartnerTypeId = customerType.PartnerTypeId, ContactEmail = "levana@customer.vn", ContactPhone = "0915666777", Status = "Active", PartnerRegions = new List<PartnerRegion> { new PartnerRegion { Region = baDinh } } },
                    new Partner { PartnerCode = "P006", PartnerName = "Công ty xây dựng Sài Gòn", PartnerTypeId = contractorType.PartnerTypeId, ContactEmail = "saigonbuild@contractor.vn", ContactPhone = "0909777888", Status = "Active", PartnerRegions = new List<PartnerRegion> { new PartnerRegion { Region = thanhXuan } } },
                    new Partner { PartnerCode = "P007", PartnerName = "Cộng tác viên Nguyễn Thị B", PartnerTypeId = collaboratorType.PartnerTypeId, ContactEmail = "nguyenb@collaborator.vn", ContactPhone = "0922333444", Status = "Active" },
                    new Partner { PartnerCode = "P008", PartnerName = "Admin Nguyễn Văn", PartnerTypeId = strategicType.PartnerTypeId, ContactEmail = "admin@scmvlxd.vn", ContactPhone = "0901234567", Status = "Active", PartnerRegions = new List<PartnerRegion> { new PartnerRegion { Region = tayHo } } },
                    new Partner { PartnerCode = "P009", PartnerName = "Quản lý Trần Thị", PartnerTypeId = strategicType.PartnerTypeId, ContactEmail = "manager1@scmvlxd.vn", ContactPhone = "0912345678", Status = "Active", PartnerRegions = new List<PartnerRegion> { new PartnerRegion { Region = baDinh } } },
                    new Partner { PartnerCode = "P010", PartnerName = "Nhân viên Lê Văn", PartnerTypeId = collaboratorType.PartnerTypeId, ContactEmail = "staff01@scmvlxd.vn", ContactPhone = "0923456789", Status = "Active", PartnerRegions = new List<PartnerRegion> { new PartnerRegion { Region = baDinh } } },
                    new Partner { PartnerCode = "P011", PartnerName = "Kế toán Phạm Thị", PartnerTypeId = collaboratorType.PartnerTypeId, ContactEmail = "accountant1@scmvlxd.vn", ContactPhone = "0934567890", Status = "Active", PartnerRegions = new List<PartnerRegion> { new PartnerRegion { Region = thanhXuan } } },
                    new Partner { PartnerCode = "P012", PartnerName = "Bán hàng Ngô Văn", PartnerTypeId = collaboratorType.PartnerTypeId, ContactEmail = "sales1@scmvlxd.vn", ContactPhone = "0945678901", Status = "Active", PartnerRegions = new List<PartnerRegion> { new PartnerRegion { Region = thanhXuan } } },
                    new Partner { PartnerCode = "P013", PartnerName = "Hỗ trợ Vũ Thị", PartnerTypeId = collaboratorType.PartnerTypeId, ContactEmail = "support1@scmvlxd.vn", ContactPhone = "0956789012", Status = "Active", PartnerRegions = new List<PartnerRegion> { new PartnerRegion { Region = thanhXuan } } },
                    new Partner { PartnerCode = "P014", PartnerName = "Kiểm kho Đỗ Văn", PartnerTypeId = collaboratorType.PartnerTypeId, ContactEmail = "inventory1@scmvlxd.vn", ContactPhone = "0967890123", Status = "Active", PartnerRegions = new List<PartnerRegion> { new PartnerRegion { Region = thanhXuan } } }
                );

                context.SaveChanges();
            }

            // Seed thêm 10 partner thuộc các tỉnh
            if (!context.Partners.Any(p => p.PartnerCode.StartsWith("PX")))
            {
                var allRegions = context.Regions.ToList();
                var rnd = new Random();

                var supplierType = context.PartnerTypes.First(pt => pt.TypeName == "Nhà cung cấp");
                var agentType = context.PartnerTypes.First(pt => pt.TypeName == "Đại lý");

                var newPartners = new List<Partner>();

                for (int i = 1; i <= 10; i++)
                {
                    // Chọn 1–3 tỉnh ngẫu nhiên
                    var randomRegions = allRegions
                        .OrderBy(_ => rnd.Next())
                        .Take(rnd.Next(1, 4))
                        .Select(r => new PartnerRegion { Region = r })
                        .ToList();

                    newPartners.Add(new Partner
                    {
                        PartnerCode = $"PX{i:000}",
                        PartnerName = $"Đối tác Tỉnh Số {i}",
                        PartnerTypeId = supplierType.PartnerTypeId,
                        ContactEmail = $"province{i}@partner.vn",
                        ContactPhone = $"090{i:000000}",
                        Status = "Active",
                        PartnerRegions = randomRegions
                    });
                }

                context.Partners.AddRange(newPartners);
                context.SaveChanges();
            }

            if (!context.Roles.Any())
            {
                context.Roles.AddRange(
                    new Role { RoleName = "Quản trị viên", Description = "Quản trị toàn bộ hệ thống" },
                    new Role { RoleName = "Quản lý kho", Description = "Quản lý kho và nhân sự" },
                    new Role { RoleName = "Nhân viên kho", Description = "Nhân viên nhập/xuất kho" },
                    new Role { RoleName = "Kế toán", Description = "Quản lý tài chính và hóa đơn" },
                    new Role { RoleName = "Nhân viên bán hàng", Description = "Xử lý đơn hàng và khách hàng" },
                    new Role { RoleName = "Nhân viên hỗ trợ", Description = "Hỗ trợ khách hàng và kho" },
                    new Role { RoleName = "Kiểm kho", Description = "Kiểm tra và báo cáo tồn kho" },
                    new Role { RoleName = "Phân tích viên", Description = "Phân tích viên của công ty mẹ" }
                );
                context.SaveChanges();
            }

            if (!context.Users.Any())
            {
                context.Users.AddRange(
                    new User { UserName = "admin", Email = "admin@scmvlxd.vn", FullName = "Nguyễn Văn Admin", PasswordHash = "73l8gRjwLftklgfdXT+MdiMEjJwGPVMsyVxe16iYpk8=", Phone = "0901234567", Status = "Active", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now, PartnerId = 1 },
                    new User { UserName = "manager1", Email = "manager1@scmvlxd.vn", FullName = "Trần Thị Quản Lý", PasswordHash = "73l8gRjwLftklgfdXT+MdiMEjJwGPVMsyVxe16iYpk8=", Phone = "0912345678", Status = "Active", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now, PartnerId = 1 },
                    new User { UserName = "staff01", Email = "staff01@scmvlxd.vn", FullName = "Lê Văn Nhân Viên", PasswordHash = "73l8gRjwLftklgfdXT+MdiMEjJwGPVMsyVxe16iYpk8=", Phone = "0923456789", Status = "Active", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now, PartnerId = 1 },
                    new User { UserName = "accountant1", Email = "accountant1@scmvlxd.vn", FullName = "Phạm Thị Kế Toán", PasswordHash = "73l8gRjwLftklgfdXT+MdiMEjJwGPVMsyVxe16iYpk8=", Phone = "0934567890", Status = "Active", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now, PartnerId = 1 },
                    new User { UserName = "sales1", Email = "sales1@scmvlxd.vn", FullName = "Ngô Văn Bán Hàng", PasswordHash = "73l8gRjwLftklgfdXT+MdiMEjJwGPVMsyVxe16iYpk8=", Phone = "0945678901", Status = "Active", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now, PartnerId = 3 },
                    new User { UserName = "support1", Email = "support1@scmvlxd.vn", FullName = "Vũ Thị Hỗ Trợ", PasswordHash = "73l8gRjwLftklgfdXT+MdiMEjJwGPVMsyVxe16iYpk8=", Phone = "0956789012", Status = "Active", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now, PartnerId = 3 },
                    new User { UserName = "inventory1", Email = "inventory1@scmvlxd.vn", FullName = "Đỗ Văn Kiểm Kho", PasswordHash = "73l8gRjwLftklgfdXT+MdiMEjJwGPVMsyVxe16iYpk8=", Phone = "0967890123", Status = "Active", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now, PartnerId = 3 },
                    new User { UserName = "customer1", Email = "levana@customer.vn", FullName = "Lê Văn A", PasswordHash = "73l8gRjwLftklgfdXT+MdiMEjJwGPVMsyVxe16iYpk8=", Phone = "0915666777", Status = "Active", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now, PartnerId = 4 },
                    new User { UserName = "collaborator1", Email = "nguyenb@collaborator.vn", FullName = "Nguyễn Thị B", PasswordHash = "73l8gRjwLftklgfdXT+MdiMEjJwGPVMsyVxe16iYpk8=", Phone = "0922333444", Status = "Active", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now, PartnerId = 5 }
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
                    new UserRole { UserId = context.Users.First(u => u.UserName == "sales1").UserId, RoleId = 2, AssignedAt = DateTime.Now },
                    new UserRole { UserId = context.Users.First(u => u.UserName == "support1").UserId, RoleId = 3, AssignedAt = DateTime.Now },
                    new UserRole { UserId = context.Users.First(u => u.UserName == "inventory1").UserId, RoleId = 4, AssignedAt = DateTime.Now }, new UserRole { UserId = context.Users.First(u => u.UserName == "customer1").UserId, RoleId = salesRole.RoleId, AssignedAt = DateTime.Now },
                    new UserRole { UserId = context.Users.First(u => u.UserName == "collaborator1").UserId, RoleId = supportRole.RoleId, AssignedAt = DateTime.Now }
                );
                context.SaveChanges();
            }

            if (!context.Categories.Any())
            {
                var now = DateTime.Now;
                context.Categories.AddRange(
                    new Category { CategoryName = "Gỗ", Description = "Vật liệu từ gỗ tự nhiên và công nghiệp", Status = "Active", CreatedAt = now },
                    new Category { CategoryName = "Kim loại", Description = "Vật liệu kim loại xây dựng", Status = "Active", CreatedAt = now },
                    new Category { CategoryName = "Nhựa", Description = "Vật liệu nhựa công nghiệp", Status = "Active", CreatedAt = now },
                    new Category { CategoryName = "Xi măng", Description = "Vật liệu xi măng xây dựng", Status = "Active", CreatedAt = now },
                    new Category { CategoryName = "Gạch", Description = "Gạch xây dựng và trang trí", Status = "Active", CreatedAt = now },
                    new Category { CategoryName = "Sơn", Description = "Sơn xây dựng và công nghiệp", Status = "Active", CreatedAt = now },
                    new Category { CategoryName = "Kính", Description = "Kính xây dựng và trang trí", Status = "Active", CreatedAt = now }
                );
                context.SaveChanges();
            }

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

            if (!context.Materials.Any())
            {
                var now = DateTime.Now;

                var woodCat = context.Categories.First(c => c.CategoryName == "Gỗ");
                var metalCat = context.Categories.First(c => c.CategoryName == "Kim loại");
                var plasticCat = context.Categories.First(c => c.CategoryName == "Nhựa");
                var cementCat = context.Categories.First(c => c.CategoryName == "Xi măng");
                var brickCat = context.Categories.First(c => c.CategoryName == "Gạch");
                var paintCat = context.Categories.First(c => c.CategoryName == "Sơn");
                var glassCat = context.Categories.First(c => c.CategoryName == "Kính");
                var materials = new List<Material>
{
    new Material { MaterialCode = "W001", MaterialName = "Gỗ thông tấm 2m", Unit = "tấm", CategoryId = woodCat.CategoryId, Status = "Active", CreatedAt = now, CreatedByPartnerId = 1 },
    new Material { MaterialCode = "M001", MaterialName = "Thép cây D20", Unit = "cây", CategoryId = metalCat.CategoryId, Status = "Active", CreatedAt = now, CreatedByPartnerId = 1 },
    new Material { MaterialCode = "P001", MaterialName = "Tấm nhựa PVC 1m x 2m", Unit = "tấm", CategoryId = plasticCat.CategoryId, Status = "Active", CreatedAt = now, CreatedByPartnerId = 1 },
    new Material { MaterialCode = "C001", MaterialName = "Xi măng PC40", Unit = "bao", CategoryId = cementCat.CategoryId, Status = "Active", CreatedAt = now, CreatedByPartnerId = 1 },
    new Material { MaterialCode = "B001", MaterialName = "Gạch đỏ 20x20", Unit = "viên", CategoryId = brickCat.CategoryId, Status = "Active", CreatedAt = now, CreatedByPartnerId = 1 },
    new Material { MaterialCode = "S001", MaterialName = "Sơn nước Dulux 20L", Unit = "thùng", CategoryId = paintCat.CategoryId, Status = "Active", CreatedAt = now, CreatedByPartnerId = 1 },
    new Material { MaterialCode = "G001", MaterialName = "Kính cường lực 8mm", Unit = "m2", CategoryId = glassCat.CategoryId, Status = "Active", CreatedAt = now, CreatedByPartnerId = 1 },
    new Material { MaterialCode = "T001", MaterialName = "Tôn lạnh 1ly", Unit = "tấm", CategoryId = metalCat.CategoryId, Status = "Active", CreatedAt = now, CreatedByPartnerId = 1 },
    new Material { MaterialCode = "A001", MaterialName = "Ống nhựa PVC D60", Unit = "ống", CategoryId = plasticCat.CategoryId, Status = "Active", CreatedAt = now, CreatedByPartnerId = 1 },
    new Material { MaterialCode = "F001", MaterialName = "Cát xây dựng hạt vừa", Unit = "m3", CategoryId = cementCat.CategoryId, Status = "Active", CreatedAt = now, CreatedByPartnerId = 1 },
    new Material { MaterialCode = "M002", MaterialName = "Thép hộp 40x40", Unit = "cây", CategoryId = metalCat.CategoryId, Status = "Active", CreatedAt = now, CreatedByPartnerId = 1 },
    new Material { MaterialCode = "W002", MaterialName = "Gỗ xoan tự nhiên 3m", Unit = "tấm", CategoryId = woodCat.CategoryId, Status = "Active", CreatedAt = now, CreatedByPartnerId = 1 },
    new Material { MaterialCode = "P002", MaterialName = "Ống nhựa PVC D90", Unit = "ống", CategoryId = plasticCat.CategoryId, Status = "Active", CreatedAt = now, CreatedByPartnerId = 1 },
    new Material { MaterialCode = "C002", MaterialName = "Xi măng Hà Tiên", Unit = "bao", CategoryId = cementCat.CategoryId, Status = "Active", CreatedAt = now, CreatedByPartnerId = 1 },
    new Material { MaterialCode = "B002", MaterialName = "Gạch ống 4 lỗ", Unit = "viên", CategoryId = brickCat.CategoryId, Status = "Active", CreatedAt = now, CreatedByPartnerId = 1 },
    new Material { MaterialCode = "S002", MaterialName = "Sơn dầu Maxilite 5L", Unit = "thùng", CategoryId = paintCat.CategoryId, Status = "Active", CreatedAt = now, CreatedByPartnerId = 1 },
    new Material { MaterialCode = "G002", MaterialName = "Gương soi 5mm", Unit = "m2", CategoryId = glassCat.CategoryId, Status = "Active", CreatedAt = now, CreatedByPartnerId = 1 },
    new Material { MaterialCode = "T002", MaterialName = "Tôn sóng vuông màu xanh", Unit = "tấm", CategoryId = metalCat.CategoryId, Status = "Active", CreatedAt = now, CreatedByPartnerId = 1 },
    new Material { MaterialCode = "A002", MaterialName = "Ống nhựa PVC D27", Unit = "ống", CategoryId = plasticCat.CategoryId, Status = "Active", CreatedAt = now, CreatedByPartnerId = 1 },
    new Material { MaterialCode = "F002", MaterialName = "Đá 1x2 xây dựng", Unit = "m3", CategoryId = cementCat.CategoryId, Status = "Active", CreatedAt = now, CreatedByPartnerId = 1 }
};

                context.Materials.AddRange(materials);
                context.SaveChanges();

                var goviet = context.Partners.First(p => p.PartnerCode == "P001");
                var hoaphat = context.Partners.First(p => p.PartnerCode == "P002");
                var duytan = context.Partners.First(p => p.PartnerCode == "P003");

                var materialPartners = new List<MaterialPartner>
                {
                    new MaterialPartner { Id = 1, MaterialId = 1, PartnerId = hoaphat.PartnerId, BuyerId = goviet.PartnerId },
                    new MaterialPartner { Id = 2, MaterialId = 2, PartnerId = duytan.PartnerId, BuyerId = goviet.PartnerId },
                    new MaterialPartner { Id = 3, MaterialId = 3, PartnerId = hoaphat.PartnerId, BuyerId = goviet.PartnerId },
                    new MaterialPartner { Id = 4, MaterialId = 4, PartnerId = duytan.PartnerId, BuyerId = hoaphat.PartnerId },
                    new MaterialPartner { Id = 5, MaterialId = 5, PartnerId = goviet.PartnerId, BuyerId = hoaphat.PartnerId },
                    new MaterialPartner { Id = 6, MaterialId = 6, PartnerId = hoaphat.PartnerId, BuyerId = duytan.PartnerId },
                    new MaterialPartner { Id = 7, MaterialId = 7, PartnerId = goviet.PartnerId, BuyerId = duytan.PartnerId },
                    new MaterialPartner { Id = 8, MaterialId = 1, PartnerId = duytan.PartnerId, BuyerId = goviet.PartnerId }
                };

                context.MaterialPartners.AddRange(materialPartners);
                context.SaveChanges();
            }

            if (!context.Warehouses.Any(w => w.WarehouseName == "Kho Nhựa Duy Tân"))
            {
                var userPartner3 = context.Users.FirstOrDefault(u => u.PartnerId == 3);
                var managerId = userPartner3 != null ? userPartner3.UserId : context.Users.First().UserId;

                context.Warehouses.Add(new Warehouse
                {
                    WarehouseName = "Kho Nhựa Duy Tân",
                    Location = "Hồ Chí Minh",
                    ManagerId = managerId
                });

                context.SaveChanges();
            }

            if (!context.Inventories.Any())
            {
                var wh1 = context.Warehouses.First(w => w.WarehouseName == "Kho Hà Nội");
                var wh2 = context.Warehouses.First(w => w.WarehouseName == "Kho TP.HCM");
                var wh3 = context.Warehouses.First(w => w.WarehouseName == "Kho Đà Nẵng");
                var whPartner3 = context.Warehouses.FirstOrDefault(w => w.WarehouseName == "Kho Nhựa Duy Tân");

                var wood = context.Materials.First(m => m.MaterialCode == "W001");
                var metal = context.Materials.First(m => m.MaterialCode == "M001");
                var plastic = context.Materials.First(m => m.MaterialCode == "P001");
                var cement = context.Materials.First(m => m.MaterialCode == "C001");
                var brick = context.Materials.First(m => m.MaterialCode == "B001");
                var paint = context.Materials.First(m => m.MaterialCode == "S001");
                var glass = context.Materials.First(m => m.MaterialCode == "G001");

                var inventories = new List<Inventory>
                {
                    new Inventory { WarehouseId = wh1.WarehouseId, MaterialId = wood.MaterialId, Quantity = 120, CreatedAt = DateTime.Now },
                    new Inventory { WarehouseId = wh1.WarehouseId, MaterialId = metal.MaterialId, Quantity = 80, CreatedAt = DateTime.Now },
                    new Inventory { WarehouseId = wh2.WarehouseId, MaterialId = plastic.MaterialId, Quantity = 200, CreatedAt = DateTime.Now },
                    new Inventory { WarehouseId = wh2.WarehouseId, MaterialId = cement.MaterialId, Quantity = 150, CreatedAt = DateTime.Now },
                    new Inventory { WarehouseId = wh3.WarehouseId, MaterialId = brick.MaterialId, Quantity = 5000, CreatedAt = DateTime.Now },
                    new Inventory { WarehouseId = wh3.WarehouseId, MaterialId = paint.MaterialId, Quantity = 50, CreatedAt = DateTime.Now },
                    new Inventory { WarehouseId = wh1.WarehouseId, MaterialId = glass.MaterialId, Quantity = 100, CreatedAt = DateTime.Now }
                };

                if (whPartner3 != null)
                {
                    inventories.Add(new Inventory
                    {
                        WarehouseId = whPartner3.WarehouseId,
                        MaterialId = plastic.MaterialId,
                        Quantity = 100,
                        CreatedAt = DateTime.Now
                    });
                }

                var uniqueInventories = inventories
                    .GroupBy(i => new { i.WarehouseId, i.MaterialId })
                    .Select(g => g.First())
                    .ToList();

                context.Inventories.AddRange(uniqueInventories);
                context.SaveChanges();
            }

            // ==================== SEED PRICE MATERIAL PARTNER ====================
            if (!context.PriceMaterialPartners.Any())
            {
                var partners = context.Partners.ToList();
                var materials = context.Materials.ToList();

                var rnd = new Random();
                var list = new List<PriceMaterialPartner>();

                foreach (var partner in partners)
                {
                    foreach (var material in materials)
                    {
                        var sellPrice = rnd.Next(50, 200) * 1000; 

                        list.Add(new PriceMaterialPartner
                        {
                            PartnerId = partner.PartnerId,
                            MaterialId = material.MaterialId,
                            SellPrice = sellPrice,
                            Status = "Active"
                        });
                    }
                }

                context.PriceMaterialPartners.AddRange(list);
                context.SaveChanges();
            }

            if (!context.RelationTypes.Any())
            {
                var relationTypes = new List<RelationType>
            {
                new RelationType { Name = "VIP", DiscountPercent = 10, DiscountAmount = 0, Status = "Active" },
                new RelationType { Name = "Preferred", DiscountPercent = 5, DiscountAmount = 0, Status = "Active" },
                new RelationType { Name = "Regular", DiscountPercent = 0, DiscountAmount = 0, Status = "Active" }
            };

                context.RelationTypes.AddRange(relationTypes);
                context.SaveChanges();
            }

            // ==================== SEED PARTNER RELATION ====================
            if (!context.PartnerRelations.Any())
            {
                var partners = context.Partners.ToList();
                var relationTypes = context.RelationTypes.ToList();

                var rnd = new Random();
                var partnerRelations = new List<PartnerRelation>();

                for (int i = 0; i < partners.Count; i++)
                {
                    for (int j = 0; j < partners.Count; j++)
                    {
                        if (i == j) continue; 

                        var buyer = partners[i];
                        var seller = partners[j];
                        var type = relationTypes[rnd.Next(relationTypes.Count)];

                        partnerRelations.Add(new PartnerRelation
                        {
                            BuyerPartnerId = buyer.PartnerId,
                            SellerPartnerId = seller.PartnerId,
                            RelationTypeId = type.RelationTypeId,
                            CooperationDate = DateTime.Now.AddDays(-rnd.Next(0, 365)), 
                            Status = "Active"
                        });
                    }
                }

                context.PartnerRelations.AddRange(partnerRelations);
                context.SaveChanges();
            }

            if (!context.Orders.Any())
            {
                var customer = context.Users.First(u => u.UserName == "customer1");

                var wood = context.Materials.First(m => m.MaterialCode == "W001");
                var brick = context.Materials.First(m => m.MaterialCode == "B001");
                var metal = context.Materials.First(m => m.MaterialCode == "M001");
                var cement = context.Materials.First(m => m.MaterialCode == "C001");
                var plastic = context.Materials.First(m => m.MaterialCode == "P001");
                var paint = context.Materials.First(m => m.MaterialCode == "S001");
                var glass = context.Materials.First(m => m.MaterialCode == "G001");

                var supplier = context.Partners.First(p => p.PartnerCode == "P001");

                var wh1 = context.Warehouses.First(w => w.WarehouseId == 1);
                var wh2 = context.Warehouses.First(w => w.WarehouseId == 2);
                var wh3 = context.Warehouses.First(w => w.WarehouseId == 3);

                var orders = new List<Order>
    {
        new Order { OrderCode = "ORD-001", CustomerName = "Lê Văn A", PhoneNumber = "0123456789", DeliveryAddress = "123 Đường Láng, Hà Nội", Status = "Pending", Note = "Urgent delivery", CreatedBy = customer.UserId, SupplierId = supplier.PartnerId, WarehouseId = wh1.WarehouseId, CreatedAt = DateTime.Now.AddDays(-10), UpdatedAt = DateTime.Now.AddDays(-10) },

        new Order { OrderCode = "ORD-002", CustomerName = "Công ty xây dựng Sài Gòn", PhoneNumber = "0987654321", DeliveryAddress = "456 Nguyễn Trãi, TP.HCM", Status = "Approved", Note = "Bulk order", CreatedBy = customer.UserId, SupplierId = supplier.PartnerId, WarehouseId = wh2.WarehouseId, CreatedAt = DateTime.Now.AddDays(-9), UpdatedAt = DateTime.Now.AddDays(-9) },

        new Order { OrderCode = "ORD-003", CustomerName = "Nguyễn Thị B", PhoneNumber = "0912345678", DeliveryAddress = "789 Lê Lợi, Đà Nẵng", Status = "Pending", Note = "Check quality", CreatedBy = customer.UserId, SupplierId = supplier.PartnerId, WarehouseId = wh3.WarehouseId, CreatedAt = DateTime.Now.AddDays(-8), UpdatedAt = DateTime.Now.AddDays(-8) },

        new Order { OrderCode = "ORD-004", CustomerName = "Đại lý Minh Tâm", PhoneNumber = "0945678901", DeliveryAddress = "101 Trần Phú, Cần Thơ", Status = "Approved", Note = "Regular client", CreatedBy = customer.UserId, SupplierId = supplier.PartnerId, WarehouseId = wh1.WarehouseId, CreatedAt = DateTime.Now.AddDays(-7), UpdatedAt = DateTime.Now.AddDays(-7) },

        new Order { OrderCode = "ORD-005", CustomerName = "Trần Văn C", PhoneNumber = "0967890123", DeliveryAddress = "202 Hai Bà Trưng, Hà Nội", Status = "Success", Note = "Completed", CreatedBy = customer.UserId, SupplierId = supplier.PartnerId, WarehouseId = wh2.WarehouseId, CreatedAt = DateTime.Now.AddDays(-6), UpdatedAt = DateTime.Now.AddDays(-6) },

        new Order { OrderCode = "ORD-006", CustomerName = "Phạm Thị D", PhoneNumber = "0932109876", DeliveryAddress = "303 Phạm Văn Đồng, TP.HCM", Status = "Pending", Note = "Partial delivery", CreatedBy = customer.UserId, SupplierId = supplier.PartnerId, WarehouseId = wh3.WarehouseId, CreatedAt = DateTime.Now.AddDays(-5), UpdatedAt = DateTime.Now.AddDays(-5) },

        new Order { OrderCode = "ORD-007", CustomerName = "Công ty Gỗ Việt", PhoneNumber = "0976543210", DeliveryAddress = "404 Nguyễn Huệ, Huế", Status = "Approved", Note = "Wood-specific", CreatedBy = customer.UserId, SupplierId = supplier.PartnerId, WarehouseId = wh1.WarehouseId, CreatedAt = DateTime.Now.AddDays(-4), UpdatedAt = DateTime.Now.AddDays(-4) },

        new Order { OrderCode = "ORD-008", CustomerName = "Ngô Văn E", PhoneNumber = "0923456789", DeliveryAddress = "505 Lê Văn Sỹ, TP.HCM", Status = "Success", Note = "Fast delivery", CreatedBy = customer.UserId, SupplierId = supplier.PartnerId, WarehouseId = wh2.WarehouseId, CreatedAt = DateTime.Now.AddDays(-3), UpdatedAt = DateTime.Now.AddDays(-3) },

        new Order { OrderCode = "ORD-009", CustomerName = "Vũ Thị F", PhoneNumber = "0956789012", DeliveryAddress = "606 Nguyễn Văn Cừ, Hà Nội", Status = "Pending", Note = "Customer review", CreatedBy = customer.UserId, SupplierId = supplier.PartnerId, WarehouseId = wh3.WarehouseId, CreatedAt = DateTime.Now.AddDays(-2), UpdatedAt = DateTime.Now.AddDays(-2) },

        new Order { OrderCode = "ORD-010", CustomerName = "Đỗ Văn G", PhoneNumber = "0990123456", DeliveryAddress = "707 Tô Hiến Thành, Đà Nẵng", Status = "Approved", Note = "Final order", CreatedBy = customer.UserId, SupplierId = supplier.PartnerId, WarehouseId = wh1.WarehouseId, CreatedAt = DateTime.Now.AddDays(-1), UpdatedAt = DateTime.Now.AddDays(-1) }
    };

                context.Orders.AddRange(orders);
                context.SaveChanges();

                // ----- ORDER DETAIL -----

                context.OrderDetails.AddRange(
                    new OrderDetail { OrderId = orders[0].OrderId, MaterialId = wood.MaterialId, Quantity = 20, UnitPrice = 255000m, Status = "Pending" },
                    new OrderDetail { OrderId = orders[0].OrderId, MaterialId = brick.MaterialId, Quantity = 500, UnitPrice = 1200m, Status = "Pending" },

                    new OrderDetail { OrderId = orders[1].OrderId, MaterialId = metal.MaterialId, Quantity = 15, UnitPrice = 320000m, Status = "Approved" },
                    new OrderDetail { OrderId = orders[1].OrderId, MaterialId = cement.MaterialId, Quantity = 30, UnitPrice = 90000m, Status = "Approved" },

                    new OrderDetail { OrderId = orders[2].OrderId, MaterialId = plastic.MaterialId, Quantity = 25, UnitPrice = 180000m, Status = "Pending" },
                    new OrderDetail { OrderId = orders[2].OrderId, MaterialId = paint.MaterialId, Quantity = 5, UnitPrice = 1500000m, Status = "Pending" },

                    new OrderDetail { OrderId = orders[3].OrderId, MaterialId = glass.MaterialId, Quantity = 10, UnitPrice = 200000m, Status = "Approved" },
                    new OrderDetail { OrderId = orders[3].OrderId, MaterialId = wood.MaterialId, Quantity = 18, UnitPrice = 255000m, Status = "Approved" },

                    new OrderDetail { OrderId = orders[4].OrderId, MaterialId = brick.MaterialId, Quantity = 600, UnitPrice = 1200m, Status = "Delivered" },
                    new OrderDetail { OrderId = orders[4].OrderId, MaterialId = metal.MaterialId, Quantity = 12, UnitPrice = 320000m, Status = "Delivered" },

                    new OrderDetail { OrderId = orders[5].OrderId, MaterialId = cement.MaterialId, Quantity = 40, UnitPrice = 90000m, Status = "Pending" },
                    new OrderDetail { OrderId = orders[5].OrderId, MaterialId = plastic.MaterialId, Quantity = 20, UnitPrice = 180000m, Status = "Pending" },

                    new OrderDetail { OrderId = orders[6].OrderId, MaterialId = paint.MaterialId, Quantity = 8, UnitPrice = 1500000m, Status = "Approved" },
                    new OrderDetail { OrderId = orders[6].OrderId, MaterialId = glass.MaterialId, Quantity = 15, UnitPrice = 200000m, Status = "Approved" },

                    new OrderDetail { OrderId = orders[7].OrderId, MaterialId = wood.MaterialId, Quantity = 22, UnitPrice = 255000m, Status = "Delivered" },
                    new OrderDetail { OrderId = orders[7].OrderId, MaterialId = brick.MaterialId, Quantity = 400, UnitPrice = 1200m, Status = "Delivered" },

                    new OrderDetail { OrderId = orders[8].OrderId, MaterialId = metal.MaterialId, Quantity = 10, UnitPrice = 320000m, Status = "Pending" },
                    new OrderDetail { OrderId = orders[8].OrderId, MaterialId = cement.MaterialId, Quantity = 35, UnitPrice = 90000m, Status = "Pending" },

                    new OrderDetail { OrderId = orders[9].OrderId, MaterialId = plastic.MaterialId, Quantity = 30, UnitPrice = 180000m, Status = "Approved" },
                    new OrderDetail { OrderId = orders[9].OrderId, MaterialId = paint.MaterialId, Quantity = 6, UnitPrice = 1500000m, Status = "Approved" }
                );

                context.SaveChanges();
            }

            // 9️ Seed Invoices & InvoiceDetails

            if (!context.Invoices.Any())
            {
                var manager = context.Users.First(u => u.UserName == "manager1");

                var partners = context.Partners.ToList();
                var orders = context.Orders.ToList();
                var materials = context.Materials.ToList();

                // Danh sách hóa đơn seed
                var invoiceData = new[]
                {
        new { Code = "INV-001", OrderCode = "ORD-001", PartnerCode = "P001", MaterialCode = "W001", Quantity = 50, UnitPrice = 250000m },
        new { Code = "INV-002", OrderCode = "ORD-002", PartnerCode = "P002", MaterialCode = "M001", Quantity = 20, UnitPrice = 320000m },
        new { Code = "INV-003", OrderCode = "ORD-003", PartnerCode = "P003", MaterialCode = "P001", Quantity = 100, UnitPrice = 180000m },
        new { Code = "INV-004", OrderCode = "ORD-004", PartnerCode = "P004", MaterialCode = "C001", Quantity = 80, UnitPrice = 90000m },
        new { Code = "INV-005", OrderCode = "ORD-005", PartnerCode = "P006", MaterialCode = "B001", Quantity = 2000, UnitPrice = 1200m },
        new { Code = "INV-006", OrderCode = "ORD-006", PartnerCode = "P005", MaterialCode = "S001", Quantity = 10, UnitPrice = 1500000m },
        new { Code = "INV-007", OrderCode = "ORD-007", PartnerCode = "P007", MaterialCode = "G001", Quantity = 50, UnitPrice = 200000m }
    };

                var invoices = new List<Invoice>();

                foreach (var data in invoiceData)
                {
                    var partner = partners.First(p => p.PartnerCode == data.PartnerCode);
                    var order = orders.First(o => o.OrderCode == data.OrderCode);
                    var material = materials.First(m => m.MaterialCode == data.MaterialCode);

                    var relation = context.PartnerRelations
                        .Include(pr => pr.RelationType)
                        .FirstOrDefault(pr => pr.BuyerPartnerId == partner.PartnerId
                                           && pr.SellerPartnerId == manager.PartnerId);

                    decimal totalLine = data.Quantity * data.UnitPrice;
                    decimal discount = relation != null
                        ? totalLine * relation.RelationType.DiscountPercent / 100 + relation.RelationType.DiscountAmount
                        : 0;

                    if (discount > totalLine)
                        discount = totalLine;

                    var invoice = new Invoice
                    {
                        InvoiceCode = data.Code,
                        InvoiceType = "Export",
                        PartnerId = partner.PartnerId,
                        WarehouseId = order.WarehouseId,         
                        Warehouse = order.Warehouse,
                        CreatedBy = manager.UserId,
                        OrderId = order.OrderId,
                        IssueDate = DateTime.Now,
                        ExportStatus = "Pending",
                        Address = order.DeliveryAddress,
                        TotalAmount = totalLine,
                        DiscountAmount = discount,
                        PayableAmount = totalLine - discount,
                        CreatedAt = DateTime.Now
                    };

                    invoice.InvoiceDetails.Add(new InvoiceDetail
                    {
                        MaterialId = material.MaterialId,
                        Quantity = data.Quantity,
                        UnitPrice = data.UnitPrice,
                        LineTotal = totalLine,
                        DiscountAmount = discount
                    });

                    invoices.Add(invoice);
                }

                context.Invoices.AddRange(invoices);
                context.SaveChanges();
            }

            if (!context.Imports.Any())
            {
                var pendingInvoices = context.Invoices
                    .Where(i => i.InvoiceType == "Import" && i.ImportStatus == "Pending")
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
                        Status = "Pending",
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

            if (!context.Exports.Any())
            {
                var manager = context.Users.First(u => u.UserName == "manager1");
                var wh1 = context.Warehouses.First(w => w.WarehouseName == "Kho Hà Nội");
                var minhtam = context.Partners.First(p => p.PartnerCode == "P004");
                var saigonbuild = context.Partners.First(p => p.PartnerCode == "P006");
                var levan = context.Partners.First(p => p.PartnerCode == "P005");

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

                //material check

                if (!context.MaterialChecks.Any())
                {
                    var invUser = context.Users.First(u => u.UserName == "inventory1");
                    var mats = context.Materials.ToList();
                    var wh = context.Warehouses.First();

                    context.MaterialChecks.AddRange(
                        new MaterialCheck { UserId = invUser.UserId, WarehouseId = wh.WarehouseId, CheckDate = DateTime.Now.AddHours(-6), Notes = "Đợt kiểm kê đột xuất", Status = "Approved", Details = { new MaterialCheckDetail { MaterialId = mats.First(m => m.MaterialCode == "W001").MaterialId, SystemQty = 120, ActualQty = 115, Reason = "Mất 5 tấm" }, new MaterialCheckDetail { MaterialId = mats.First(m => m.MaterialCode == "M001").MaterialId, SystemQty = 80, ActualQty = 80, Reason = "Khớp số liệu" } } },
                        new MaterialCheck { UserId = invUser.UserId, WarehouseId = wh.WarehouseId, CheckDate = DateTime.Now.AddHours(-20), Notes = "Kiểm kê theo lịch", Status = "Pending", Details = { new MaterialCheckDetail { MaterialId = mats.First(m => m.MaterialCode == "P001").MaterialId, SystemQty = 210, ActualQty = 210, Reason = "Nhập bù chưa hạch toán" }, new MaterialCheckDetail { MaterialId = mats.First(m => m.MaterialCode == "C001").MaterialId, SystemQty = 150, ActualQty = 140, Reason = "Hao hụt / đổ vỡ" } } }
                    );

                    context.SaveChanges();
                }



                var staff = context.Users.FirstOrDefault(u => u.UserName == "staff01")
                    ?? throw new InvalidOperationException("User staff01 not found.");
                var whHN = context.Warehouses.FirstOrDefault(w => w.WarehouseName == "Kho Hà Nội")
                    ?? throw new InvalidOperationException("Warehouse Kho Hà Nội not found.");

                var prj1 = context.Exports.FirstOrDefault(e => e.ExportCode == "PRJ-001");
                var prj2 = context.Exports.FirstOrDefault(e => e.ExportCode == "PRJ-002");

                if (prj1 == null)
                {
                    prj1 = new Export
                    {
                        ExportCode = "PRJ-001",
                        ExportDate = DateTime.Now.AddDays(-10),
                        WarehouseId = whHN.WarehouseId,
                        CreatedBy = staff.UserId,
                        Notes = "Xuất công trình A",
                        Status = "Success",
                        CreatedAt = DateTime.Now.AddDays(-10)
                    };
                    context.Exports.Add(prj1);
                    context.SaveChanges();
                }

                if (prj2 == null)
                {
                    prj2 = new Export
                    {
                        ExportCode = "PRJ-002",
                        ExportDate = DateTime.Now.AddDays(-4),
                        WarehouseId = whHN.WarehouseId,
                        CreatedBy = staff.UserId,
                        Notes = "Xuất công trình B",
                        Status = "Success",
                        CreatedAt = DateTime.Now.AddDays(-4)
                    };
                    context.Exports.Add(prj2);
                    context.SaveChanges();
                }

                prj1.ExportDate = DateTime.Now.AddDays(-12);
                prj2.ExportDate = DateTime.Now.AddDays(-6);
                context.Exports.Update(prj1);
                context.Exports.Update(prj2);
                context.SaveChanges();

                if (!context.ExportDetails.Any(d => d.ExportId == prj1.ExportId))
                {
                    context.ExportDetails.AddRange(
                        new ExportDetail
                        {
                            ExportId = prj1.ExportId,
                            MaterialId = wood.MaterialId,
                            MaterialCode = wood.MaterialCode ?? "",
                            MaterialName = wood.MaterialName,
                            Unit = wood.Unit,
                            UnitPrice = 250000m,
                            Quantity = 18m,
                            LineTotal = 18m * 250000m
                        },
                        new ExportDetail
                        {
                            ExportId = prj1.ExportId,
                            MaterialId = cement.MaterialId,
                            MaterialCode = cement.MaterialCode ?? "",
                            MaterialName = cement.MaterialName,
                            Unit = cement.Unit,
                            UnitPrice = 90000m,
                            Quantity = 35m,
                            LineTotal = 35m * 90000m
                        }
                    );
                }

                if (!context.ExportDetails.Any(d => d.ExportId == prj2.ExportId))
                {
                    context.ExportDetails.AddRange(
                        new ExportDetail
                        {
                            ExportId = prj2.ExportId,
                            MaterialId = brick.MaterialId,
                            MaterialCode = brick.MaterialCode ?? "",
                            MaterialName = brick.MaterialName,
                            Unit = brick.Unit,
                            UnitPrice = 1200m,
                            Quantity = 800m,
                            LineTotal = 800m * 1200m
                        },
                        new ExportDetail
                        {
                            ExportId = prj2.ExportId,
                            MaterialId = wood.MaterialId,
                            MaterialCode = wood.MaterialCode ?? "",
                            MaterialName = wood.MaterialName,
                            Unit = wood.Unit,
                            UnitPrice = 250000m,
                            Quantity = 10m,
                            LineTotal = 10m * 250000m
                        }
                    );
                }
                context.SaveChanges();
            }



            if (!context.Invoices.Any(i => i.InvoiceType == "Import" || i.InvoiceType == "Export"))
            {
                var manager = context.Users.First(u => u.UserName == "manager1");
                var pGoviet = context.Partners.First(p => p.PartnerCode == "P001");
                var pHoaPhat = context.Partners.First(p => p.PartnerCode == "P002");

                var invI1 = new Invoice
                {
                    InvoiceCode = "IMP-001",
                    InvoiceType = "Import",
                    PartnerId = pGoviet.PartnerId,
                    CreatedBy = manager.UserId,
                    IssueDate = DateTime.Now.AddDays(-12),
                    DueDate = DateTime.Now.AddDays(-2),
                    ImportStatus = "Approved",
                    CreatedAt = DateTime.Now.AddDays(-12),
                    TotalAmount = 0m
                };

                var invI2 = new Invoice
                {
                    InvoiceCode = "IMP-002",
                    InvoiceType = "Import",
                    PartnerId = pGoviet.PartnerId,
                    CreatedBy = manager.UserId,
                    IssueDate = DateTime.Now.AddDays(-9),
                    DueDate = DateTime.Now.AddDays(+2),
                    ImportStatus = "Pending",
                    CreatedAt = DateTime.Now.AddDays(-9),
                    TotalAmount = 0m
                };

                var invE1 = new Invoice
                {
                    InvoiceCode = "EXP-001",
                    InvoiceType = "Export",
                    PartnerId = pHoaPhat.PartnerId,
                    CreatedBy = manager.UserId,
                    IssueDate = DateTime.Now.AddDays(-6),
                    DueDate = DateTime.Now.AddDays(+4),
                    ExportStatus = "Success",
                    CreatedAt = DateTime.Now.AddDays(-6),
                    TotalAmount = 0m
                };

                context.Invoices.AddRange(invI1, invI2, invE1);
                context.SaveChanges();

                var wood = context.Materials.First(m => m.MaterialCode == "W001");
                var metal = context.Materials.First(m => m.MaterialCode == "M001");

                context.InvoiceDetails.AddRange(
                    new InvoiceDetail { InvoiceId = invI1.InvoiceId, MaterialId = wood.MaterialId, Quantity = 40, UnitPrice = 240000m, LineTotal = 40m * 240000m },
                    new InvoiceDetail { InvoiceId = invI1.InvoiceId, MaterialId = metal.MaterialId, Quantity = 10, UnitPrice = 310000m, LineTotal = 10m * 310000m },
                    new InvoiceDetail { InvoiceId = invI2.InvoiceId, MaterialId = wood.MaterialId, Quantity = 60, UnitPrice = 255000m, LineTotal = 60m * 255000m },
                    new InvoiceDetail { InvoiceId = invI2.InvoiceId, MaterialId = metal.MaterialId, Quantity = 12, UnitPrice = 315000m, LineTotal = 12m * 315000m },
                    new InvoiceDetail { InvoiceId = invE1.InvoiceId, MaterialId = metal.MaterialId, Quantity = 25, UnitPrice = 325000m, LineTotal = 25m * 325000m }
                );
                context.SaveChanges();

                var ids = new[] { invI1.InvoiceId, invI2.InvoiceId, invE1.InvoiceId };
                var sums = context.InvoiceDetails
                    .Where(d => ids.Contains(d.InvoiceId))
                    .GroupBy(d => d.InvoiceId)
                    .Select(g => new { Id = g.Key, Sum = g.Sum(x => x.LineTotal ?? 0m) })
                    .ToList();
                foreach (var s in sums)
                {
                    var inv = context.Invoices.First(i => i.InvoiceId == s.Id);
                    inv.TotalAmount = s.Sum;
                }
                context.SaveChanges();
            }

            {
                var whHN = context.Warehouses.First(w => w.WarehouseName == "Kho Hà Nội");
                var wood = context.Materials.First(m => m.MaterialCode == "W001");

                bool hasHistory = context.Inventories.Any(i =>
                    i.WarehouseId == whHN.WarehouseId && i.MaterialId == wood.MaterialId && i.CreatedAt < DateTime.Now.AddDays(-1));

                if (!hasHistory)
                {
                    context.Inventories.AddRange(
                        new Inventory { WarehouseId = whHN.WarehouseId, MaterialId = wood.MaterialId, Quantity = 150m, CreatedAt = DateTime.Now.AddDays(-7) },
                        new Inventory { WarehouseId = whHN.WarehouseId, MaterialId = wood.MaterialId, Quantity = 135m, CreatedAt = DateTime.Now.AddDays(-4) },
                        new Inventory { WarehouseId = whHN.WarehouseId, MaterialId = wood.MaterialId, Quantity = 120m, CreatedAt = DateTime.Now.AddDays(-1) }
                    );
                    context.SaveChanges();
                }
            }

            if (!context.Exports.Any(e => e.ExportCode.StartsWith("PRJ-")))
            {
                var staff = context.Users.First(u => u.UserName == "staff01");
                var whHN = context.Warehouses.First(w => w.WarehouseName == "Kho Hà Nội");

                var ex1 = new Export { ExportCode = "PRJ-001", ExportDate = DateTime.Now.AddDays(-10), WarehouseId = whHN.WarehouseId, CreatedBy = staff.UserId, Notes = "Xuất công trình A", Status = "Success", CreatedAt = DateTime.Now.AddDays(-10) };
                var ex2 = new Export { ExportCode = "PRJ-001", ExportDate = DateTime.Now.AddDays(-7), WarehouseId = whHN.WarehouseId, CreatedBy = staff.UserId, Notes = "Xuất công trình A", Status = "Success", CreatedAt = DateTime.Now.AddDays(-7) };
                var ex3 = new Export { ExportCode = "PRJ-002", ExportDate = DateTime.Now.AddDays(-4), WarehouseId = whHN.WarehouseId, CreatedBy = staff.UserId, Notes = "Xuất công trình B", Status = "Success", CreatedAt = DateTime.Now.AddDays(-4) };
                var ex4 = new Export { ExportCode = "PRJ-002", ExportDate = DateTime.Now.AddDays(-1), WarehouseId = whHN.WarehouseId, CreatedBy = staff.UserId, Notes = "Xuất công trình B", Status = "Success", CreatedAt = DateTime.Now.AddDays(-1) };
                context.Exports.AddRange(ex1, ex2, ex3, ex4);
                context.SaveChanges();

                var wood = context.Materials.First(m => m.MaterialCode == "W001");
                var metal = context.Materials.First(m => m.MaterialCode == "M001");

                context.ExportDetails.AddRange(
                    new ExportDetail { ExportId = ex1.ExportId, MaterialId = wood.MaterialId, MaterialCode = wood.MaterialCode, MaterialName = wood.MaterialName, Unit = wood.Unit, UnitPrice = 255000m, Quantity = 10m, LineTotal = 10m * 255000m },
                    new ExportDetail { ExportId = ex1.ExportId, MaterialId = metal.MaterialId, MaterialCode = metal.MaterialCode, MaterialName = metal.MaterialName, Unit = metal.Unit, UnitPrice = 320000m, Quantity = 5m, LineTotal = 5m * 320000m },
                    new ExportDetail { ExportId = ex2.ExportId, MaterialId = wood.MaterialId, MaterialCode = wood.MaterialCode, MaterialName = wood.MaterialName, Unit = wood.Unit, UnitPrice = 255000m, Quantity = 12m, LineTotal = 12m * 255000m },
                    new ExportDetail { ExportId = ex3.ExportId, MaterialId = metal.MaterialId, MaterialCode = metal.MaterialCode, MaterialName = metal.MaterialName, Unit = metal.Unit, UnitPrice = 325000m, Quantity = 7m, LineTotal = 7m * 325000m },
                    new ExportDetail { ExportId = ex4.ExportId, MaterialId = wood.MaterialId, MaterialCode = wood.MaterialCode, MaterialName = wood.MaterialName, Unit = wood.Unit, UnitPrice = 255000m, Quantity = 9m, LineTotal = 9m * 255000m }
                );
                context.SaveChanges();
            }

            var pAgent = context.Partners.First(p => p.PartnerCode == "P004");
            var pRetail = context.Partners.First(p => p.PartnerCode == "P005");
            var pSupp1 = context.Partners.First(p => p.PartnerCode == "P001");
            var pSupp2 = context.Partners.First(p => p.PartnerCode == "P002");

            var userManager = context.Users.First(u => u.UserName == "manager1");
            var userStaff = context.Users.First(u => u.UserName == "staff01");

            var matWood = context.Materials.First(m => m.MaterialCode == "W001");
            var matBrick = context.Materials.First(m => m.MaterialCode == "B001");

            void SeedChartForPartner(int partnerId)
            {
                if (!context.GlAccounts.Any(a => a.PartnerId == partnerId))
                {
                    context.GlAccounts.AddRange(
                        new GlAccount { PartnerId = partnerId, Code = "111", Name = "Tiền mặt", Type = "Asset", IsPosting = true },
                        new GlAccount { PartnerId = partnerId, Code = "112", Name = "Tiền gửi ngân hàng", Type = "Asset", IsPosting = true },
                        new GlAccount { PartnerId = partnerId, Code = "131", Name = "Phải thu khách hàng", Type = "Asset", IsPosting = true },
                        new GlAccount { PartnerId = partnerId, Code = "331", Name = "Phải trả NCC", Type = "Liability", IsPosting = true },
                        new GlAccount { PartnerId = partnerId, Code = "156", Name = "Hàng hóa tồn kho", Type = "Asset", IsPosting = true },
                        new GlAccount { PartnerId = partnerId, Code = "511", Name = "Doanh thu bán hàng", Type = "Revenue", IsPosting = true },
                        new GlAccount { PartnerId = partnerId, Code = "632", Name = "Giá vốn hàng bán", Type = "Expense", IsPosting = true },
                        new GlAccount { PartnerId = partnerId, Code = "133", Name = "Thuế GTGT đầu vào", Type = "Asset", IsPosting = true },
                        new GlAccount { PartnerId = partnerId, Code = "3331", Name = "Thuế GTGT đầu ra", Type = "Liability", IsPosting = true }
                    );
                    context.SaveChanges();
                }
            }

            void SeedPolicyForPartner(int partnerId)
            {
                if (!context.PostingPolicies.Any(p => p.PartnerId == partnerId))
                {
                    var acc = context.GlAccounts.Where(a => a.PartnerId == partnerId).ToDictionary(a => a.Code, a => a.AccountId);
                    context.PostingPolicies.AddRange(
                        new PostingPolicy { PartnerId = partnerId, DocumentType = "SalesInvoice", RuleKey = "Revenue", DebitAccountId = acc["131"], CreditAccountId = acc["511"] },
                        new PostingPolicy { PartnerId = partnerId, DocumentType = "SalesInvoice", RuleKey = "VATOut", DebitAccountId = acc["131"], CreditAccountId = acc["3331"] },
                        new PostingPolicy { PartnerId = partnerId, DocumentType = "PurchaseInvoice", RuleKey = "Inventory", DebitAccountId = acc["156"], CreditAccountId = acc["331"] },
                        new PostingPolicy { PartnerId = partnerId, DocumentType = "PurchaseInvoice", RuleKey = "VATIn", DebitAccountId = acc["133"], CreditAccountId = acc["331"] },
                        new PostingPolicy { PartnerId = partnerId, DocumentType = "Receipt", RuleKey = "Cash", DebitAccountId = acc["111"], CreditAccountId = acc["131"] },
                        new PostingPolicy { PartnerId = partnerId, DocumentType = "Receipt", RuleKey = "Bank", DebitAccountId = acc["112"], CreditAccountId = acc["131"] },
                        new PostingPolicy { PartnerId = partnerId, DocumentType = "Payment", RuleKey = "Cash", DebitAccountId = acc["331"], CreditAccountId = acc["111"] },
                        new PostingPolicy { PartnerId = partnerId, DocumentType = "Payment", RuleKey = "Bank", DebitAccountId = acc["331"], CreditAccountId = acc["112"] }
                    );
                    context.SaveChanges();
                }
            }

            SeedChartForPartner(pAgent.PartnerId);
            SeedChartForPartner(pRetail.PartnerId);
            SeedChartForPartner(pSupp1.PartnerId);
            SeedChartForPartner(pSupp2.PartnerId);

            SeedPolicyForPartner(pAgent.PartnerId);
            SeedPolicyForPartner(pRetail.PartnerId);
            SeedPolicyForPartner(pSupp1.PartnerId);
            SeedPolicyForPartner(pSupp2.PartnerId);

            if (!context.MoneyAccounts.Any(a => a.PartnerId == pAgent.PartnerId))
            {
                context.MoneyAccounts.AddRange(
                    new MoneyAccount { PartnerId = pAgent.PartnerId, Name = "Vietcombank - CN Hà Nội", Type = "Bank", Number = "0123456789", IsActive = true },
                    new MoneyAccount { PartnerId = pAgent.PartnerId, Name = "Quỹ tiền mặt", Type = "Cash", Number = "TM-001", IsActive = true }
                );
                context.SaveChanges();
            }

            Invoice EnsureInvoice(string code, string type, int partnerId, int createdBy,
                      DateTime issueDate, DateTime dueDate, string status, int orderId)
            {
                var existed = context.Invoices.SingleOrDefault(i => i.InvoiceCode == code);
                if (existed != null) return existed;

                var inv = new Invoice
                {
                    InvoiceCode = code,
                    InvoiceType = type,
                    PartnerId = partnerId,
                    CreatedBy = createdBy,
                    IssueDate = issueDate,
                    DueDate = dueDate,
                    ImportStatus = type == "Import" ? status : null,
                    ExportStatus = type == "Export" ? status : null,
                    OrderId = orderId,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    TotalAmount = 0
                };
                context.Invoices.Add(inv);
                context.SaveChanges();
                return inv;
            }

            void EnsureInvoiceDetails(Invoice inv, IEnumerable<(int materialId, int qty, int price)> rows)
            {
                if (!context.InvoiceDetails.Any(d => d.InvoiceId == inv.InvoiceId))
                {
                    foreach (var r in rows)
                        context.InvoiceDetails.Add(new InvoiceDetail { InvoiceId = inv.InvoiceId, MaterialId = r.materialId, Quantity = r.qty, UnitPrice = r.price, LineTotal = r.qty * r.price });
                    context.SaveChanges();
                    var total = context.InvoiceDetails.Where(d => d.InvoiceId == inv.InvoiceId).Sum(d => d.LineTotal ?? (d.Quantity * d.UnitPrice));
                    inv.TotalAmount = total;
                    inv.UpdatedAt = DateTime.Now;
                    context.SaveChanges();
                }
            }

            // 1️⃣ Seed Orders
            var ord1 = context.Orders.SingleOrDefault(o => o.OrderCode == "ORD-001");
            if (ord1 == null)
            {
                ord1 = new Order
                {
                    OrderCode = "ORD-001",
                    CustomerName = "Retail Customer 1",
                    PhoneNumber = "0909123456",
                    DeliveryAddress = "123 Retail St.",
                    Status = "Pending",
                    CreatedBy = userStaff.UserId,
                    CreatedAt = DateTime.Now,
                    SupplierId = pRetail.PartnerId
                };
                context.Orders.Add(ord1);
                context.SaveChanges();
            }

            var ord2 = context.Orders.SingleOrDefault(o => o.OrderCode == "ORD-002");
            if (ord2 == null)
            {
                ord2 = new Order
                {
                    OrderCode = "ORD-002",
                    CustomerName = "Retail Customer 2",
                    PhoneNumber = "0909876543",
                    DeliveryAddress = "456 Retail Ave.",
                    Status = "Pending",
                    CreatedBy = userStaff.UserId,
                    CreatedAt = DateTime.Now,
                    SupplierId = pRetail.PartnerId
                };
                context.Orders.Add(ord2);
                context.SaveChanges();
            }

            var ord3 = context.Orders.SingleOrDefault(o => o.OrderCode == "ORD-003");
            if (ord3 == null)
            {
                ord3 = new Order
                {
                    OrderCode = "ORD-003",
                    CustomerName = "Agent Customer",
                    PhoneNumber = "0912345678",
                    DeliveryAddress = "789 Agent Rd.",
                    Status = "Pending",
                    CreatedBy = userStaff.UserId,
                    CreatedAt = DateTime.Now,
                    SupplierId = pAgent.PartnerId
                };
                context.Orders.Add(ord3);
                context.SaveChanges();
            }

            var invImp1 = EnsureInvoice("IMP-001", "Import", pSupp1.PartnerId, userManager.UserId,
    DateTime.Now.AddDays(-12), DateTime.Now.AddDays(5), "Success", ord1.OrderId);

            var invImp2 = EnsureInvoice("IMP-002", "Import", pSupp2.PartnerId, userManager.UserId,
                DateTime.Now.AddDays(-10), DateTime.Now.AddDays(7), "Success", ord2.OrderId);

            var invExp1 = EnsureInvoice("EXP-001", "Export", pRetail.PartnerId, userStaff.UserId,
                DateTime.Now.AddDays(-9), DateTime.Now.AddDays(10), "Success", ord1.OrderId);

            var invExp2 = EnsureInvoice("EXP-002", "Export", pAgent.PartnerId, userStaff.UserId,
                DateTime.Now.AddDays(-7), DateTime.Now.AddDays(12), "Success", ord3.OrderId);

            // 3️⃣ Seed InvoiceDetails
            EnsureInvoiceDetails(invImp1, new[]
            {
    (matWood.MaterialId, 30, 255000),
    (matBrick.MaterialId, 500, 1200)
});

            EnsureInvoiceDetails(invImp2, new[]
            {
    (matWood.MaterialId, 18, 255000)
});

            EnsureInvoiceDetails(invExp1, new[]
            {
    (matWood.MaterialId, 10, 300000)
});

            EnsureInvoiceDetails(invExp2, new[]
            {
    (matBrick.MaterialId, 400, 2000)
});

            // 3️⃣ Seed InvoiceDetails
            EnsureInvoiceDetails(invImp1, new[]
            {
    (matWood.MaterialId, 30, 255000),
    (matBrick.MaterialId, 500, 1200)
});

            EnsureInvoiceDetails(invImp2, new[]
            {
    (matWood.MaterialId, 18, 255000)
});

            EnsureInvoiceDetails(invExp1, new[]
            {
    (matWood.MaterialId, 10, 300000)
});

            EnsureInvoiceDetails(invExp2, new[]
            {
    (matBrick.MaterialId, 400, 2000)
});

            if (!context.Receipts.Any())
            {
                var bankAgent = context.MoneyAccounts.First(a => a.PartnerId == pAgent.PartnerId && a.Type == "Bank");
                var exp1 = context.Invoices.Single(i => i.InvoiceCode == "EXP-001");
                var exp2 = context.Invoices.Single(i => i.InvoiceCode == "EXP-002");
                context.Receipts.AddRange(
                    new Receipt { Date = DateTime.Now.AddDays(-6), PartnerId = pRetail.PartnerId, InvoiceId = exp1.InvoiceId, Amount = 5000000, Method = "Bank", MoneyAccountId = bankAgent.MoneyAccountId, Reference = "Thu công nợ EXP-001", Status = "Draft" },
                    new Receipt { Date = DateTime.Now.AddDays(-5), PartnerId = pAgent.PartnerId, InvoiceId = exp2.InvoiceId, Amount = 2000000, Method = "Cash", MoneyAccountId = null, Reference = "Thu công nợ EXP-002", Status = "Draft" }
                );
                context.SaveChanges();
            }

            if (!context.Payments.Any())
            {
                var bankAgent = context.MoneyAccounts.First(a => a.PartnerId == pAgent.PartnerId && a.Type == "Bank");
                var imp1 = context.Invoices.Single(i => i.InvoiceCode == "IMP-001");
                context.Payments.AddRange(
                    new Payment { Date = DateTime.Now.AddDays(-6), PartnerId = pSupp1.PartnerId, InvoiceId = imp1.InvoiceId, Amount = 1200000, Method = "Bank", MoneyAccountId = bankAgent.MoneyAccountId, Reference = "Chi NCC IMP-001", Status = "Draft" },
                    new Payment { Date = DateTime.Now.AddDays(-4), PartnerId = pSupp2.PartnerId, InvoiceId = null, Amount = 800000, Method = "Cash", MoneyAccountId = null, Reference = "Chi NCC khác", Status = "Draft" }
                );
                context.SaveChanges();
            }

            if (!context.BankStatements.Any())
            {
                var bankAgent = context.MoneyAccounts.First(a => a.PartnerId == pAgent.PartnerId && a.Type == "Bank");
                var stmt = new BankStatement { PartnerId = pAgent.PartnerId, MoneyAccountId = bankAgent.MoneyAccountId, From = DateTime.Now.AddDays(-10), To = DateTime.Now };
                context.BankStatements.Add(stmt);
                context.SaveChanges();

                var rBank = context.Receipts.FirstOrDefault(r => r.Method == "Bank" && r.MoneyAccountId == bankAgent.MoneyAccountId);
                var pBank = context.Payments.FirstOrDefault(p => p.Method == "Bank" && p.MoneyAccountId == bankAgent.MoneyAccountId);

                var bl1 = new BankStatementLine { BankStatementId = stmt.BankStatementId, Date = DateTime.Now.AddDays(-6), Amount = 5000000, Description = "Khách chuyển khoản", ExternalRef = "TXN-R-001", Status = rBank != null ? "Reconciled" : "Unreconciled", ReceiptId = rBank?.ReceiptId, PaymentId = null };
                var bl2 = new BankStatementLine { BankStatementId = stmt.BankStatementId, Date = DateTime.Now.AddDays(-6), Amount = -1200000, Description = "Chi trả NCC", ExternalRef = "TXN-P-001", Status = pBank != null ? "Reconciled" : "Unreconciled", ReceiptId = null, PaymentId = pBank?.PaymentId };
                var bl3 = new BankStatementLine { BankStatementId = stmt.BankStatementId, Date = DateTime.Now.AddDays(-5), Amount = 750000, Description = "Khách khác chuyển khoản", ExternalRef = "TXN-R-002", Status = "Unreconciled", ReceiptId = null, PaymentId = null };

                context.BankStatementLines.AddRange(bl1, bl2, bl3);
                context.SaveChanges();
            }

            if (!context.JournalEntries.Any())
            {
                var acc111 = context.GlAccounts.First(a => a.PartnerId == pAgent.PartnerId && a.Code == "111");
                var acc131 = context.GlAccounts.First(a => a.PartnerId == pAgent.PartnerId && a.Code == "131");
                var je = new JournalEntry
                {
                    PartnerId = pAgent.PartnerId,
                    PostingDate = DateTime.Now.AddDays(-2),
                    SourceType = "ManualSeed",
                    SourceId = 1,
                    ReferenceNo = "TEST-001",
                    Memo = "Thu tiền mặt test"
                };
                je.Lines.Add(new JournalLine { PartnerId = pAgent.PartnerId, AccountId = acc111.AccountId, Debit = 2000000, Credit = 0 });
                je.Lines.Add(new JournalLine { PartnerId = pAgent.PartnerId, AccountId = acc131.AccountId, Debit = 0, Credit = 2000000 });
                context.JournalEntries.Add(je);
                context.SaveChanges();
            }

            var transportType = context.PartnerTypes.FirstOrDefault(x => x.TypeName == "Đơn vị vận tải")
            ?? context.PartnerTypes.Add(new PartnerType { TypeName = "Đơn vị vận tải" }).Entity;
            context.SaveChanges();

            if (!context.Addresses.Any(a => a.Name == "Main Depot"))
            {
                var depot = new Address { Name = "Main Depot", Line1 = "KCN A", City = "HCM", Lat = 10.8, Lng = 106.7 };
                var whA = new Address { Name = "Kho Partner A", Line1 = "Quận 1", City = "HCM", Lat = 10.78, Lng = 106.70 };
                var whB = new Address { Name = "Kho Partner B", Line1 = "Quận 7", City = "HCM", Lat = 10.73, Lng = 106.72 };
                context.Addresses.AddRange(depot, whA, whB);
                context.SaveChanges();

                var v1 = new Vehicle
                {
                    Code = "TRK-01",
                    PlateNumber = "51C-00001",
                    VehicleClass = "Truck 5T",
                    MinLicenseClass = "C1",
                    PayloadTons = 5.0m,
                    Active = true,
                    PartnerId = 1
                };
                var v2 = new Vehicle
                {
                    Code = "TRK-02",
                    PlateNumber = "51C-00002",
                    VehicleClass = "Truck 8T",
                    MinLicenseClass = "C",
                    PayloadTons = 8.0m,
                    Active = true,
                    PartnerId = 1
                };
                var v3 = new Vehicle
                {
                    Code = "VAN-01",
                    PlateNumber = "51D-12345",
                    VehicleClass = "Van 1.5T",
                    MinLicenseClass = "B",
                    PayloadTons = 1.5m,
                    Active = true,
                    PartnerId = 1
                };
                context.Vehicles.AddRange(v1, v2, v3);
                context.SaveChanges();

                var d1 = new Driver
                {
                    FullName = "Nguyễn Văn Tài",
                    Phone = "0901111111",
                    BirthDate = new DateOnly(1990, 5, 12),
                    Hometown = "Long An",
                    LicenseClass = "C1",
                    Active = true,
                    PartnerId = 1
                };
                var d2 = new Driver
                {
                    FullName = "Trần Văn Lái",
                    Phone = "0902222222",
                    BirthDate = new DateOnly(1987, 11, 3),
                    Hometown = "Tiền Giang",
                    LicenseClass = "C",
                    Active = true,
                    PartnerId = 1
                };
                var d3 = new Driver
                {
                    FullName = "Phạm Quốc Đạt",
                    Phone = "0905555555",
                    BirthDate = new DateOnly(1996, 2, 20),
                    Hometown = "Đồng Nai",
                    LicenseClass = "B",
                    Active = true,
                    PartnerId = 1
                };
                context.Drivers.AddRange(d1, d2, d3);
                context.SaveChanges();

                var p1 = new Porter
                {
                    FullName = "Lê Văn A",
                    Phone = "0903333333",
                    BirthYear = 1995,
                    Hometown = "Bình Dương",
                    Active = true,
                    PartnerId = 1
                };
                var p2 = new Porter
                {
                    FullName = "Phạm Văn B",
                    Phone = "0904444444",
                    BirthYear = 1992,
                    Hometown = "Vĩnh Long",
                    Active = true,
                    PartnerId = 1
                };
                var p3 = new Porter
                {
                    FullName = "Ngô Văn C",
                    Phone = "0906666666",
                    BirthYear = 1998,
                    Hometown = "Hậu Giang",
                    Active = true,
                    PartnerId = 1
                };
                context.Porters.AddRange(p1, p2, p3);
                context.SaveChanges();

                var t1 = new Transport
                {
                    TransportCode = "T-INIT-001",
                    DepotId = depot.AddressId,
                    ProviderPartnerId = 1,
                    Status = TransportStatus.Assigned,
                    StartTimePlanned = DateTimeOffset.Now.AddHours(1),
                    Notes = "Seed trip",
                    VehicleId = v1.VehicleId,
                    DriverId = d1.DriverId
                };
                context.Transports.Add(t1);
                context.SaveChanges();

                var stop0 = new TransportStop { TransportId = t1.TransportId, Seq = 0, StopType = TransportStopType.Depot, AddressId = depot.AddressId, ServiceTimeMin = 0, Status = TransportStopStatus.Planned };
                var stop1 = new TransportStop { TransportId = t1.TransportId, Seq = 1, StopType = TransportStopType.Dropoff, AddressId = whA.AddressId, ServiceTimeMin = 15, Status = TransportStopStatus.Planned };
                var stop2 = new TransportStop { TransportId = t1.TransportId, Seq = 2, StopType = TransportStopType.Dropoff, AddressId = whB.AddressId, ServiceTimeMin = 15, Status = TransportStopStatus.Planned };
                context.TransportStops.AddRange(stop0, stop1, stop2);
                context.SaveChanges();

                var inv1 = context.Invoices.FirstOrDefault(i => i.InvoiceCode == "INV-001") ?? context.Invoices.OrderBy(i => i.InvoiceId).First();
                var inv2 = context.Invoices.FirstOrDefault(i => i.InvoiceCode == "INV-002") ?? context.Invoices.OrderBy(i => i.InvoiceId).Skip(1).FirstOrDefault() ?? inv1;

                //context.TransportInvoices.AddRange(
                //    new TransportInvoice { TransportId = t1.TransportId, InvoiceId = inv1.InvoiceId },
                //    new TransportInvoice { TransportId = t1.TransportId, InvoiceId = inv2.InvoiceId }
                //);
                //context.SaveChanges();

                context.TransportPorters.AddRange(
                    new TransportPorter { TransportId = t1.TransportId, PorterId = p1.PorterId, Role = "Member" },
                    new TransportPorter { TransportId = t1.TransportId, PorterId = p2.PorterId, Role = "Member" }
                );
                context.SaveChanges();

                context.ShippingLogs.AddRange(
                    new ShippingLog { InvoiceId = inv1.InvoiceId, TransportId = t1.TransportId, Status = "Transport.Created", CreatedAt = DateTime.Now },
                    new ShippingLog { InvoiceId = inv2.InvoiceId, TransportId = t1.TransportId, Status = "Transport.Assigned", CreatedAt = DateTime.Now }
                );
                context.SaveChanges();
            }
        }
    }
}