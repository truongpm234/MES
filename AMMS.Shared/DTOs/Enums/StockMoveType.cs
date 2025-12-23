using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMMS.Shared.DTOs.Enums
{
    /// <summary>
    /// Loại phiếu kho:
    /// IN     = Nhập kho
    /// OUT    = Xuất kho (ra sản xuất)
    /// RETURN = Trả hàng về kho / trả NCC
    /// </summary>
    public enum StockMoveType
    {
        IN = 1,
        OUT = 2,
        RETURN = 3
    }

    public static class StockMoveTypeExtensions
    {
        /// <summary>
        /// Chuẩn hóa enum -> string code trong DB.
        /// </summary>
        public static string ToCode(this StockMoveType type) => type switch
        {
            StockMoveType.IN => "IN",
            StockMoveType.OUT => "OUT",
            StockMoveType.RETURN => "RETURN",
            _ => "IN"
        };

        /// <summary>
        /// Parse từ string code (IN/OUT/RETURN) sang enum (case-insensitive).
        /// </summary>
        public static bool TryParseCode(string? code, out StockMoveType result)
        {
            result = StockMoveType.IN;

            if (string.IsNullOrWhiteSpace(code))
                return false;

            switch (code.Trim().ToUpper())
            {
                case "IN":
                    result = StockMoveType.IN;
                    return true;
                case "OUT":
                    result = StockMoveType.OUT;
                    return true;
                case "RETURN":
                    result = StockMoveType.RETURN;
                    return true;
                default:
                    return false;
            }
        }
    }
}
