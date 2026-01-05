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

        public static string QuoteEmailNeedDesign(
            order_request req,
            cost_estimate est,
            decimal deposit,
            string acceptUrl,
            string rejectUrl,
            string orderDetailUrl)
        {
            var delivery = req.delivery_date?.ToString("dd/MM/yyyy") ?? "N/A";
            var finalTotal = est.final_total_cost;

            return $@"
<div style='font-family:Arial,Helvetica,sans-serif;max-width:720px;margin:24px auto;color:#111;line-height:1.6'>
  <h2 style='margin-top:0'>BÁO GIÁ ĐƠN HÀNG IN ẤN</h2>

  <p>Chào {req.customer_name},</p>
  <p>Chúng tôi gửi đến bạn báo giá cho đơn hàng <b>{req.product_name}</b> với các thông tin như sau:</p>

  <h3>Thông tin đơn hàng</h3>
  <ul>
    <li><b>Mã yêu cầu:</b> AM{req.order_request_id:D6}</li>
    <li><b>Sản phẩm:</b> {req.product_name}</li>
    <li><b>Số lượng:</b> {req.quantity}</li>
    <li><b>Ngày giao dự kiến:</b> {delivery}</li>
  </ul>

  <h3>Thông tin báo giá</h3>
  <ul>
    <li><b>Tổng giá trị:</b> {finalTotal:n0} VND</li>
    <li><b>Tiền cọc (dự kiến thu):</b> {deposit:n0} VND</li>
  </ul>

  <p style='margin:16px 0'>
    Vì bạn chọn phương án <b>Tự gửi file thiết kế</b>, vui lòng nhấn vào liên kết dưới đây để
    xem chi tiết đơn hàng và tải lên/cập nhật file thiết kế:
  </p>

  <p style='text-align:center;margin:18px 0'>
    <a href='{orderDetailUrl}'
       style='display:inline-block;padding:10px 18px;border-radius:6px;
              background:#2563eb;color:#ffffff;text-decoration:none;font-weight:600'>
      Xem chi tiết đơn hàng & gửi thiết kế
    </a>
  </p>

  <p style='margin:18px 0'>
    Sau khi xem chi tiết và thống nhất, bạn có thể:
  </p>
  <ul>
    <li>
      <a href='{acceptUrl}' style='color:#16a34a;font-weight:600'>
        Đồng ý báo giá & tiến hành thanh toán tiền cọc
      </a>
    </li>
    <li>
      <a href='{rejectUrl}' style='color:#dc2626'>
        Từ chối báo giá
      </a>
    </li>
  </ul>

  <p>Nếu bạn có bất kỳ thắc mắc nào, hãy phản hồi lại email này để được hỗ trợ.</p>

  <p>Trân trọng,<br/>Đội ngũ AMMS</p>
</div>";
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
