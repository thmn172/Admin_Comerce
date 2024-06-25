using System.Globalization;

namespace SV20T1020460.Web
{
    public static class Converter
    {
        /// <summary>
        /// chuyển chuỗi s sang giá trị kiểu DateTime (nếu không chuyển thành công thì trả về Null)
        /// </summary>
        /// <param name="s"></param>
        /// <param name="formats"></param>
        /// <returns></returns>
        public static DateTime? StringToDateTime(this string s, string formats = "d/M/yyyy;d-M-yyyy;d.M/yyyy")
        {
            try
            {
                return DateTime.ParseExact(s, formats.Split(';'), CultureInfo.InvariantCulture);
            }
            catch { return null; }
        }
    }
}
