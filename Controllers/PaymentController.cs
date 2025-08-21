using LMS.API.DTOs;
using LMS.API.LMSDbContext;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Razorpay.Api;

namespace LMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : Controller
    {
        private readonly IConfiguration _config;
        private readonly LMSContext _context;
        public PaymentController(IConfiguration config, LMSContext context)
        {
            _config = config;
            _context = context;
        }

        [HttpPost("createOrder")]
        public IActionResult CreateOrder([FromBody] PaymentRequest obj)
        {
            var systemSetting = _context.SystemSettings.First();
            if (systemSetting.PaidCoursesEnabled == false)
                return BadRequest(new { message = "Admin has been disabled the payments for now...! Retry after some time" });

            var client = new RazorpayClient(
                _config["Razorpay:key"],
                _config["Razorpay:secretKey"]
            );

            Dictionary<string, object> options = new Dictionary<string, object>
            {
                {"amount", obj.Amount*100},
                {"currency", "INR"},
                {"receipt", "order_receipt_11"}
            };

            Razorpay.Api.Order order = client.Order.Create(options);
            return Ok(new
            {
                orderId = order["id"].ToString(),
                amount = order["amount"],
                currency = order["currency"]
            });
        }

        [HttpPost("verify-payment")]
        public IActionResult VerifyPayment([FromBody] RazorpayPaymentResponse obj)
        {
            var client = new RazorpayClient(
                _config["Razorpay:key"],
                _config["Razorpay:secretKey"]
                );

            Dictionary<string, string> options = new Dictionary<string, string>();
            options.Add("razorpay_order_id", obj.razorpay_order_id);
            options.Add("razorpay_payment_id", obj.razorpay_payment_id);
            options.Add("razorpay_signature", obj.razorpay_signature);

            Utils.verifyPaymentSignature(options);
            return Ok(new { message = "Payment verified successfully" });

        }

        [HttpPost("save-payment")]
        public async Task<IActionResult> SavePayment([FromBody] PaymentDto dto)
        {
            var exists = await _context.Payments.AnyAsync(p => p.RazorpayPaymentId == dto.RazorpayPaymentId);

            if (exists)
                return BadRequest("Payment already saved");

            var client = new RazorpayClient(_config["Razorpay:key"], _config["Razorpay:secretKey"]);
            var paymentFetch = client.Payment.Fetch(dto.RazorpayPaymentId);

            var method = "";

            if (paymentFetch["method"] == "card")
            {
                method = $"{paymentFetch["card_type"].ToString()} card" ?? null;
            }
            else if (paymentFetch["method"] == "wallet")
            {
                method = $"{paymentFetch["wallet"].ToString()} wallet" ?? null;
            }
            else
            {
                method = paymentFetch["method"];
            }

            if (dto == null || dto.CourseId == 0 || string.IsNullOrWhiteSpace(dto.RazorpayPaymentId))
            {
                return BadRequest("Invalid payment data");
            }

            var payment = new LMS.API.Models.Payment
            {
                StudentId = dto.StudentId,
                CourseId = dto.CourseId,
                Amount = dto.Amount,
                PaymentMethod = method ?? dto.PaymentMethod,
                PaymentStatus = dto.PaymentStatus,
                PaymentDate = dto.PaymentDate,

                RazorpayPaymentId = dto.RazorpayPaymentId,
                RazorpayOrderId = dto.RazorpayOrderId,
                RazorpaySignature = dto.RazorpaySignature,
            };
            _context.Add(payment);
            await _context.SaveChangesAsync();

            return Ok(new { paymentId = payment.PaymentId });
        }

    }
}
