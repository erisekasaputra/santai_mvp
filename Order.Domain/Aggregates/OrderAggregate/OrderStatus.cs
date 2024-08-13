namespace Order.Domain.Aggregates.OrderAggregate;

public enum OrderStatus
{
    OrderPlaced,
    PaymentPending,
    PaymentPaid,
    OrderScheduled,
    FindingMechanic,
    MechanicAssigned,
    MechanicDispatched,
    MechanicArrived,
    ServiceInProgress,
    ServiceCompleted,
    ServiceIncompleted,
    OrderCanceled,
}
