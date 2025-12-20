using Application.Constants.Enums;

namespace Application.MappingProfile
{
    public static class RoleMapper
    {
        public static RoleCodeEnum MapFromDb(string roleName)
        {
            return roleName switch
            {
                "Quản trị viên" => RoleCodeEnum.ADMIN,
                "Quản lý kho" => RoleCodeEnum.MANAGER,
                "Nhân viên kho" => RoleCodeEnum.WAREHOUSE_STAFF,
                "Kế toán" => RoleCodeEnum.ACCOUNTANT,
                "Nhân viên bán hàng" => RoleCodeEnum.SALES,
                "Nhân viên hỗ trợ" => RoleCodeEnum.SUPPORT,
                "Kiểm kho" => RoleCodeEnum.STOCK_AUDITOR,
                "Phân tích viên" => RoleCodeEnum.ANALYST,
                _ => throw new Exception($"Unknown role name: {roleName}")
            };
        }
    }
}
