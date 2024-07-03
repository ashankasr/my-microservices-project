using Automatonymous;
using Play.Trading.Service.Contracts;

namespace Play.Trading.Service.StateMachines
{
    public class PurchaseStateMachine : MassTransitStateMachine<PurchaseState>
    {
        public State Accepted { get; private set; }
        public State ItemsGranted { get; private set; }
        public State Completed { get; private set; }
        public State Faulted { get; private set; }

        public Event<PurchaseRequested> PurchaseRequested { get; }
        // public Event<PurchaseAccepted> PurchaseAccepted { get; private set; }
        // public Event<PurchaseRejected> PurchaseRejected { get; private set; }
        // public Event<PurchaseCompleted> PurchaseCompleted { get; private set; }

        public PurchaseStateMachine()
        {
            InstanceState(x => x.CurrentState);

            ConfigureEvents();

            ConfigureInitialState();

            // During(Submitted,
            //     When(PurchaseAccepted)
            //         .Then(context =>
            //         {
            //             context.Instance.LastUpdated = context.Data.LastUpdated;
            //         })
            //         .TransitionTo(Accepted),
            //     When(PurchaseRejected)
            //         .Then(context =>
            //         {
            //             context.Instance.LastUpdated = context.Data.LastUpdated;
            //             context.Instance.ErrorMessage = context.Data.ErrorMessage;
            //         })
            //         .TransitionTo(Rejected)
            // );

            // During(Accepted,
            //     When(PurchaseCompleted)
            //         .Then(context =>
            //         {
            //             context.Instance.LastUpdated = context.Data.LastUpdated;
            //         })
            //         .TransitionTo(Completed)
            // );

            // SetCompletedWhenFinalized();
        }

        private void ConfigureEvents()
        {
            Event(() => PurchaseRequested);
            // Event(() => PurchaseAccepted, x => x.CorrelateById(context => context.Message.CorrelationId));
            // Event(() => PurchaseRejected, x => x.CorrelateById(context => context.Message.CorrelationId));
            // Event(() => PurchaseCompleted, x => x.CorrelateById(context => context.Message.CorrelationId));
        }

        private void ConfigureInitialState()
        {
            Initially(When(PurchaseRequested).Then(context =>
            {
                context.Instance.UserId = context.Data.UserId;
                context.Instance.ItemId = context.Data.ItemId;
                context.Instance.Quantity = context.Data.Quantity;
                context.Instance.CorrelationId = context.Data.CorrelationId;
                context.Instance.Received = DateTime.UtcNow;
                context.Instance.LastUpdated = context.Instance.Received;
            }).TransitionTo(Accepted));
        }
    }
}