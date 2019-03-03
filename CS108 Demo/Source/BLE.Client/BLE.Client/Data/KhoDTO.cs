using System;
namespace BLE.Client.Data
{
    public class KhoDTO
    {
        public KhoDTO()
        {
        }
        public string Id { get; set; }

        public String Code { get; set; }
        public string RfId { get; set; }
        public string TrangThai { get; set; }
        public string NhanVienNhap { get; set; }
        public string NhanVienXuat { get; set; }
        public string KhachHang { get; set; }
        public bool KiemKe { get; set; }
        public DateTime NgayNhap { get; set; }
        public DateTime NgayXuat { get; set; }
        public string Name { get; set; }
        public int GiaBan { get; set; }
        public int BillCode { get; set; }
    }
    public class KhoResultDTO
    {
        public KhoResultDTO()
        {
        }
        public string Id { get; set; }

        public String Code { get; set; }
        public string RfId { get; set; }
        public string TrangThai { get; set; }
        public string NhanVienNhap { get; set; }
        //public string NhanVienXuat { get; set; }
        public string KhachHang { get; set; }
        public bool KiemKe { get; set; }
        public DateTime NgayNhap { get; set; }
        //public DateTime NgayXuat { get; set; }

        public string ProductId { get; set; }
        public string Name { get; set; }
        public int GiaBan { get; set; }
    }

    public class TraCuuDTO
    {
        public TraCuuDTO()
        {
        }
        public DateTime TuNgay { get; set; }
        public DateTime DenNgay { get; set; }

    }

    public class LoadByBillCode
    {
        public LoadByBillCode()
        {
        }
        public int BillCode { get; set; }
    }
    public class XuatkhoDTO
    {
        public string NhanVien { get; set; }
        public string KhachHang { get; set; }
        public string Rfid { get; set; }
        public int BillCode { get; set; }
    }
}
