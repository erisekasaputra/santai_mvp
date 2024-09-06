namespace Order.Domain.Aggregates.OrderAggregate;

public enum OrderStatus
{ 
    PaymentPending,
    PaymentPaid, 
    FindingMechanic,
    MechanicAssigned,
    MechanicAcceptedOrder,
    MechanicDispatched,
    MechanicArrived,
    ServiceInProgress,
    ServiceCompleted,
    ServiceIncompleted, 
    OrderCanceledByUser,
}
