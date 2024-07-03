using System.Security.Claims;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Play.Trading.Service.Contracts;
using Play.Trading.Service.Dtos;

namespace Play.Trading.Service.Controllers;

[ApiController]
[Route("purchase")]
[Authorize]
public class PurchaseController : ControllerBase
{
    private readonly IPublishEndpoint publishEndpoint;

    public PurchaseController(IPublishEndpoint publishEndpoint)
    {
        this.publishEndpoint = publishEndpoint;
    }

    [HttpPost]
    public async Task<IActionResult> PostAsync(SubmitPurchaseDto purchase)
    {
        var userId = User.FindFirstValue("sub");
        var correlationId = NewId.NextGuid();

        var message = new PurchaseRequested(
            Guid.Parse(userId),
            purchase.ItemId!.Value,
            purchase.Quantity,
            correlationId
        );

        await publishEndpoint.Publish(message);

        return Accepted();
    }
}
