using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Constants.Messages
{
    public static class TransportMessages
    {
        // General
        public const string INTERNAL_ERROR = "Đã xảy ra lỗi hệ thống.";
        public const string INVALID_REQUEST = "Dữ liệu yêu cầu không hợp lệ.";

        // Transport
        public const string TRANSPORT_NOT_FOUND = "Không tìm thấy chuyến vận chuyển với ID {0}.";
        public const string TRANSPORT_ALREADY_STARTED_OR_COMPLETED = "Không thể cập nhật chuyến khi đã bắt đầu hoặc hoàn thành.";
        public const string TRANSPORT_CANCELLED = "Chuyến vận chuyển đã bị hủy.";
        public const string TRANSPORT_NOT_ASSIGNED = "Chuyến chưa được gán xe hoặc tài xế.";
        public const string TRANSPORT_STOPS_NOT_COMPLETED = "Không thể hoàn thành chuyến khi chưa xong các điểm dừng.";

        // Resources (Vehicle, Driver, Porter, Partner)
        public const string PROVIDER_PARTNER_NOT_FOUND = "Không tìm thấy đối tác vận chuyển.";
        public const string WAREHOUSE_NOT_FOUND = "Không tìm thấy kho hàng.";
        public const string VEHICLE_NOT_FOUND = "Không tìm thấy phương tiện.";
        public const string DRIVER_NOT_FOUND = "Không tìm thấy tài xế.";
        public const string PARTNER_MISMATCH = "Phương tiện hoặc tài xế không thuộc đối tác của chuyến này.";
        public const string VEHICLE_BUSY = "Xe đang bận trong khoảng thời gian này.";
        public const string DRIVER_BUSY = "Tài xế đang bận trong khoảng thời gian này.";
        public const string PORTER_BUSY = "Bốc xếp (ID: {0}) đang bận trong khoảng thời gian này.";
        public const string DRIVER_LICENSE_INVALID = "Bằng lái của tài xế {0} ({1}) không đủ điều kiện cho xe {2} (Yêu cầu: {3}).";

        // Stop & Invoice
        public const string STOP_MISSING_INVOICE = "Điểm dừng phải có ít nhất một hóa đơn.";
        public const string INVOICE_NOT_FOUND = "Hóa đơn với ID {0} không tồn tại.";
        public const string INVOICE_ADDRESS_MISMATCH = "Các hóa đơn trong cùng một điểm dừng phải có cùng địa chỉ giao hàng.";
        public const string INVOICE_NO_ADDRESS = "Hóa đơn {0} không có địa chỉ giao hàng hợp lệ.";
        public const string INVOICE_NOT_EXPORT = "Hóa đơn {0} không phải là phiếu xuất (Export).";
        public const string INVOICE_ASSIGNED_ELSEWHERE = "Hóa đơn đã được gán cho điểm dừng khác: {0}.";
        public const string STOP_PROOF_REQUIRED = "Chưa có ảnh chứng từ cho điểm dừng này.";
        public const string STOP_CANNOT_REMOVE_DEPOT = "Không thể xóa điểm dừng kho (Depot).";
        public const string STOP_PROOF_BASE64_REQUIRED = "Dữ liệu ảnh (Base64) là bắt buộc.";
    }
}
