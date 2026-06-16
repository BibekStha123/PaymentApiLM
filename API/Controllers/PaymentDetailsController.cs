using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using PaymentDetailApi.Application.Common;
using PaymentDetailApi.Application.PaymentDetail.Commands;
using PaymentDetailApi.Application.PaymentDetail.Queries;
using PaymentDetailApi.Domain.Payment.Entities;

namespace PaymentDetailApi.API.Controllers
{
    [EnableRateLimiting("sliding")]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class PaymentDetailsController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IMediator _mediator;
        public PaymentDetailsController(IMediator mediator, IHttpClientFactory httpClientFactory)
        {
            _mediator = mediator;
            _httpClientFactory = httpClientFactory;
        }

        public record ChatRequest(string Prompt);

        [HttpPost("chat")]
        //public async Task<IActionResult> Chat([FromBody] ChatRequest request)
        //{
        //    var client = _httpClientFactory.CreateClient("LMStudio");

        //    var tools = new object[]
        //    {
        //        new
        //        {
        //            type = "function",
        //            function = new
        //            {
        //                name = "get_payment",
        //                description = "Get payment details by card owner name",
        //                parameters = new
        //                {
        //                    type = "object",
        //                    properties = new { name = new { type = "string" } },
        //                    required = new[] { "name" }
        //                }
        //            }
        //        },
        //        new
        //        {
        //            type = "function",
        //            function = new
        //            {
        //                name = "delete_payment",
        //                description = "Delete a payment record by its ID",
        //                parameters = new
        //                {
        //                    type = "object",
        //                    properties = new { id = new { type = "integer" } },
        //                    required = new[] { "id" }
        //                }
        //            }
        //        }
        //    };

        //    var messages = new List<object>
        //    {
        //        new { role = "user", content = request.Prompt }
        //    };

        //    var body = new { model = "qwen3.5-9b", messages, tools, temperature = 0.7 };

        //    var response = await client.PostAsJsonAsync("/v1/chat/completions", body);
        //    if (!response.IsSuccessStatusCode)
        //        return StatusCode((int)response.StatusCode, "LM Studio request failed");

        //    var result = await response.Content.ReadFromJsonAsync<JsonElement>();
        //    var choice = result.GetProperty("choices")[0];
        //    var finishReason = choice.GetProperty("finish_reason").GetString();

        //    if (finishReason == "tool_calls")
        //    {
        //        var assistantMessage = choice.GetProperty("message");
        //        var toolCall = assistantMessage.GetProperty("tool_calls")[0];
        //        var toolName = toolCall.GetProperty("function").GetProperty("name").GetString();
        //        var args = JsonSerializer.Deserialize<JsonElement>(
        //            toolCall.GetProperty("function").GetProperty("arguments").GetString()!);

        //        string toolResult;
        //        if (toolName == "delete_payment")
        //        {
        //            var id = args.GetProperty("id").GetInt32();
        //            var deleteResult = await DeletePaymentDetails(id);
        //            toolResult = deleteResult is OkObjectResult ok
        //                ? JsonSerializer.Serialize(ok.Value)
        //                : $"{{\"error\": \"Payment with id {id} not found\"}}";
        //        }
        //        else
        //        {
        //            var name = args.GetProperty("name").GetString();
        //            var payment = await GetPaymentDetailsByName(name ?? "");
        //            toolResult = JsonSerializer.Serialize(payment);
        //        }

        //        messages.Add(JsonSerializer.Deserialize<object>(JsonSerializer.Serialize(assistantMessage))!);
        //        messages.Add(new { role = "tool", content = toolResult });

        //        var followUp = new { model = "qwen3.5-9b", messages, tools, temperature = 0.7 };
        //        var followUpResponse = await client.PostAsJsonAsync("/v1/chat/completions", followUp);
        //        if (!followUpResponse.IsSuccessStatusCode)
        //            return StatusCode((int)followUpResponse.StatusCode, "LM Studio follow-up request failed");

        //        result = await followUpResponse.Content.ReadFromJsonAsync<JsonElement>();
        //        choice = result.GetProperty("choices")[0];

        //        var toolMessage = choice.GetProperty("message").GetProperty("content").GetString();
        //        return Ok(new { reply = toolMessage });
        //    }

        //    // No tool matched — delegate to MCP playwright for web search
        //    var mcpBody = new
        //    {
        //        model = "ibm/granite-4-micro",
        //        input = request.Prompt,
        //        integrations = new[] { "mcp/playwright" },
        //        context_length = 8000,
        //        temperature = 0
        //    };

        //    var mcpResponse = await client.PostAsJsonAsync("/api/v1/chat", mcpBody);
        //    if (!mcpResponse.IsSuccessStatusCode)
        //        return StatusCode((int)mcpResponse.StatusCode, "MCP playwright request failed");

        //    var mcpResult = await mcpResponse.Content.ReadFromJsonAsync<JsonElement>();
        //    var mcpMessage = mcpResult.GetProperty("output").GetString();
        //    return Ok(new { reply = mcpMessage });
        //}

        // GET: api/PaymentDetail
        [HttpGet]
        public async Task<ActionResult<CursorPagedResponse<PaymentDetail>>> GetPaymentDetails([FromQuery] int? cursor, [FromQuery] int limit = 10)
        {
            var result = await _mediator.Send(new GetAllPaymentDetailsQuery(cursor, limit));
            return Ok(result);
        }

        // GET: api/PaymentDetail/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PaymentDetail>> GetPaymentDetails(int id)
        {
            var result = await _mediator.Send(new GetPaymentDetailsByIdQuery(id));

            return Ok(result);
        }

        // GET: api/PaymentDetail/name
        [HttpGet("name/{name}")]
        public async Task<ActionResult<PaymentDetail>> GetPaymentDetailsByName(string name)
        {
            var result = await _mediator.Send(new GetPaymentDetailsByNameQuery(name));

            return Ok(result);
        }

        // PUT: api/PaymentDetail/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutPaymentDetails(int id, PaymentDetails paymentDetails)
        //{
        //    if (id != paymentDetails.Id)
        //    {
        //        return BadRequest();
        //    }

        //    _context.Entry(paymentDetails).State = EntityState.Modified;

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!PaymentDetailsExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return Ok(await _context.PaymentDetails.ToListAsync());
        //}

        // POST: api/PaymentDetail
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<int>> PostPaymentDetails(CreatePaymentDetailCommand command)
        {
            return Ok(await _mediator.Send(command));
        }

        // DELETE: api/PaymentDetail/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<string>> DeletePaymentDetails(int id)
        {
            return Ok(await _mediator.Send(new DeletePaymentDetailCommand(id)));

        }

        //private bool PaymentDetailsExists(int id)
        //{
        //    return _context.PaymentDetails.Any(e => e.Id == id);
        //}
    }
}
