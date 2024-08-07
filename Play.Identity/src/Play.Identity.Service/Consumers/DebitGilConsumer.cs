using System.Threading.Tasks;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using Play.Identity.Contracts;
using Play.Identity.Service.Entities;
using Play.Identity.Service.Exceptions;

namespace Play.Identity.Service.Consumers
{
    public class DebitGilConsuer : IConsumer<DebitGil>
    {
        private readonly UserManager<ApplicationUser> userManager;

        public DebitGilConsuer(UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;
        }

        public async Task Consume(ConsumeContext<DebitGil> context)
        {
            var message = context.Message;

            var user = await userManager.FindByIdAsync(message.UserId.ToString());

            if (user == null)
            {
                throw new UnknownUserException(message.UserId);
            }

            user.Gil -= message.Gil;

            if (user.Gil < 0)
            {
                throw new InsufficientGilException(user.Id, user.Gil);
            }

            await userManager.UpdateAsync(user);

            await context.Publish(new GilDebited(message.CorrelationId));
        }
    }
}