using AMMS.Infrastructure.Entities;
using System.Globalization;
using System.Text;

namespace AMMS.Application.Helpers
{
    public static class DealEmailTemplates
    {
        private static string VND(decimal v)
            => string.Format(new CultureInfo("vi-VN"), "{0:N0} ₫", v);

        public static string QuoteEmail(order_request req, cost_estimate est, decimal deposit,
    string acceptUrl, string rejectUrl)
        {
            var address = $"{req.detail_address}";
            var delivery = req.delivery_date?.ToString("dd/MM/yyyy") ?? "N/A";

            return $@"
<html>
  <body style='font-family: Arial;'>
    <h2>Báo giá đơn hàng in ấn</h2>

    <h3>Thông tin người đặt</h3>
    <ul>
      <li><b>Tên:</b> {req.customer_name}</li>
      <li><b>SĐT:</b> {req.customer_phone}</li>
      <li><b>Email:</b> {req.customer_email}</li>
      <li><b>Địa chỉ:</b> {address}</li>
    </ul>

    <h3>Thông tin giao hàng</h3>
    <ul>
      <li><b>Địa chỉ giao:</b> {address}</li>
      <li><b>Ngày giao dự kiến:</b> {delivery}</li>
    </ul>

    <h3>Thông tin đơn hàng</h3>
    <ul>
      <li><b>Sản phẩm:</b> {req.product_name}</li>
      <li><b>Số lượng:</b> {req.quantity}</li>
      <li><b>Tổng giá trị đơn hàng:</b> {est.final_total_cost:n0} VND</li>
      <li><b>Số tiền đặt cọc:</b> {deposit:n0} VND</li>
    </ul>

    <p>
      <a href='{acceptUrl}' style='padding:10px 16px; background:#16a34a; color:white; text-decoration:none; border-radius:6px;'>
        Đồng ý & Thanh toán cọc
      </a>
      &nbsp;
      <a href='{rejectUrl}' style='padding:10px 16px; background:#dc2626; color:white; text-decoration:none; border-radius:6px;'>
        Từ chối báo giá
      </a>
    </p>

    <p style='color:#666; font-size:12px;'>
      Nếu bạn không thực hiện yêu cầu này, vui lòng bỏ qua email.
    </p>
  </body>
</html>";
        }

        public static string AcceptCustomerEmail(
            order_request req,
            order order,
            cost_estimate est,
            string trackingUrl)
        {
            return $@"
<h2>ĐƠN HÀNG ĐÃ ĐƯỢC PHÊ DUYỆT</h2>

<p>Cảm ơn bạn đã xác nhận báo giá.</p>

<p><b>Mã đơn hàng:</b> <span style='color:blue'>{order.code}</span></p>
<p><b>Sản phẩm:</b> {req.product_name}</p>
<p><b>Số lượng:</b> {req.quantity}</p>
<p><b>Giá trị đơn hàng:</b> {VND(est.final_total_cost)}</p>

<hr/>

<p>
Bạn có thể theo dõi tiến độ sản xuất tại:
<br/>
<a href='{trackingUrl}'>{trackingUrl}</a>
</p>

<p>
Vui lòng lưu lại <b>mã đơn hàng</b> để tra cứu sau này.
</p>

<p>AMMS trân trọng!</p>
";
        }

        public static string AcceptConsultantEmail(
            order_request req,
            order order)
        {
            return $@"
<h3>KHÁCH HÀNG ĐÃ ĐỒNG Ý BÁO GIÁ</h3>

<p>Request ID: {req.order_request_id}</p>
<p>Order Code: <b>{order.code}</b></p>
<p>Sản phẩm: {req.product_name}</p>
<p>Số lượng: {req.quantity}</p>
";
        }
    }
}
