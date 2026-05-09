using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PaymentDetailApi.Models;

namespace PaymentDetailApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentDetailController : ControllerBase
    {
        private readonly PaymentDetailsContext _context;
        private readonly IHttpClientFactory _httpClientFactory;

        public PaymentDetailController(PaymentDetailsContext context, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
        }

        public record ChatRequest(string Prompt);

        [HttpPost("chat")]
        public async Task<IActionResult> Chat([FromBody] ChatRequest request)
        {
            var client = _httpClientFactory.CreateClient("LMStudio");

            var tools = new[]
            {
                new
                {
                    type = "function",
                    function = new
                    {
                        name = "get_payment_by_name",
                        description = "Get payment details by card owner name",
                        parameters = new
                        {
                            type = "object",
                            properties = new
                            {
                                name = new { type = "string" }
                            },
                            required = new[] { "name" }
                        }
                    }
                }
            };

            var messages = new List<object>
            {
                new { role = "user", content = request.Prompt }
            };

            var body = new { model = "qwen3.5-9b", messages, tools, temperature = 0.7 };

            var response = await client.PostAsJsonAsync("/v1/chat/completions", body);
            if (!response.IsSuccessStatusCode)
                return StatusCode((int)response.StatusCode, "LM Studio request failed");

            var result = await response.Content.ReadFromJsonAsync<JsonElement>();
            var choice = result.GetProperty("choices")[0];
            var finishReason = choice.GetProperty("finish_reason").GetString();

            if (finishReason == "tool_calls")
            {
                var assistantMessage = choice.GetProperty("message");
                var toolCall = assistantMessage.GetProperty("tool_calls")[0];
                var args = JsonSerializer.Deserialize<JsonElement>(
                    toolCall.GetProperty("function").GetProperty("arguments").GetString()!);
                var name = args.GetProperty("name").GetString();

                var payment = await GetPaymentDetailsByName(name ?? "");
                var toolResult = JsonSerializer.Serialize(payment);

                messages.Add(JsonSerializer.Deserialize<object>(JsonSerializer.Serialize(assistantMessage))!);
                messages.Add(new { role = "tool", content = toolResult });

                var followUp = new { model = "qwen3.5-9b", messages, tools, temperature = 0.7 };
                var followUpResponse = await client.PostAsJsonAsync("/v1/chat/completions", followUp);
                if (!followUpResponse.IsSuccessStatusCode)
                    return StatusCode((int)followUpResponse.StatusCode, "LM Studio follow-up request failed");

                result = await followUpResponse.Content.ReadFromJsonAsync<JsonElement>();
                choice = result.GetProperty("choices")[0];
            }

            var message = choice.GetProperty("message").GetProperty("content").GetString();
            return Ok(new { reply = message });
        }

        // GET: api/PaymentDetail
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PaymentDetails>>> GetPaymentDetails()
        {
            return await _context.PaymentDetails.ToListAsync();
        }

        // GET: api/PaymentDetail/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PaymentDetails>> GetPaymentDetails(int id)
        {
            var paymentDetails = await _context.PaymentDetails.FindAsync(id);

            if (paymentDetails == null)
            {
                return NotFound();
            }

            return paymentDetails;
        }

        // GET: api/PaymentDetail/name
        [HttpGet("name/{name}")]
        public async Task<ActionResult<PaymentDetails>> GetPaymentDetailsByName(string name)
        {
            var paymentDetails = await _context.PaymentDetails.FirstOrDefaultAsync(p => p.CardOwnerName == name);

            if (paymentDetails == null)
            {
                return NotFound();
            }

            return paymentDetails;
        }

        // PUT: api/PaymentDetail/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPaymentDetails(int id, PaymentDetails paymentDetails)
        {
            if (id != paymentDetails.Id)
            {
                return BadRequest();
            }

            _context.Entry(paymentDetails).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PaymentDetailsExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(await _context.PaymentDetails.ToListAsync());
        }

        // POST: api/PaymentDetail
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<PaymentDetails>> PostPaymentDetails(PaymentDetails paymentDetails)
        {
            _context.PaymentDetails.Add(paymentDetails);
            await _context.SaveChangesAsync();

            return Ok(await _context.PaymentDetails.ToListAsync());
        }

        // DELETE: api/PaymentDetail/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePaymentDetails(int id)
        {
            var paymentDetails = await _context.PaymentDetails.FindAsync(id);
            if (paymentDetails == null)
            {
                return NotFound();
            }

            _context.PaymentDetails.Remove(paymentDetails);
            await _context.SaveChangesAsync();

            return Ok(await _context.PaymentDetails.ToListAsync());
        }

        private bool PaymentDetailsExists(int id)
        {
            return _context.PaymentDetails.Any(e => e.Id == id);
        }
    }
}
