using AMMS.Application.Helpers;
using AMMS.Application.Interfaces;
using AMMS.Shared.DTOs.PayOS;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AMMS.Application.Services
{
    public sealed class PayOsService : IPayOsService
    {
        private readonly HttpClient _http;
        private readonly PayOsOptions _opt;

        public PayOsService(HttpClient http, IOptions<PayOsOptions> opt)
        {
            _http = http;
            _opt = opt.Value;
        }

        public async Task<string> CreatePaymentLinkAsync(
            int orderCode,
            int amount,
            string description,
            string buyerName,
            string buyerEmail,
            string buyerPhone,
            string returnUrl,
            string cancelUrl,
            CancellationToken ct = default)
        { 
            var dataToSign =
                $"amount={amount}&cancelUrl={cancelUrl}&description={description}&orderCode={orderCode}&returnUrl={returnUrl}";

            var signature = HmacSha256Hex(_opt.ChecksumKey, dataToSign);

            var req = new
            {
                orderCode = orderCode,
                amount = amount,
                description = description,
                buyerName = buyerName,
                buyerEmail = buyerEmail,
                buyerPhone = buyerPhone,
                cancelUrl = cancelUrl,
                returnUrl = returnUrl,
                signature = signature
            };

            using var msg = new HttpRequestMessage(HttpMethod.Post, $"{_opt.BaseUrl}/v2/payment-requests");
            msg.Headers.Add("x-client-id", _opt.ClientId);
            msg.Headers.Add("x-api-key", _opt.ApiKey);
            msg.Content = JsonContent.Create(req);

            var res = await _http.SendAsync(msg, ct);
            res.EnsureSuccessStatusCode();

            var json = await res.Content.ReadFromJsonAsync<PayOsCreateResponse>(cancellationToken: ct);
            if (json?.data?.checkoutUrl == null)
                throw new Exception("PayOS response missing checkoutUrl");

            return json.data.checkoutUrl;
        }

        private static string HmacSha256Hex(string key, string data)
        {
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
            var sb = new StringBuilder(hash.Length * 2);
            foreach (var b in hash) sb.Append(b.ToString("x2"));
            return sb.ToString();
        }
    }
}
