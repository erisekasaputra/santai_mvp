namespace Order.Domain.Enumerations;

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
    OrderCancelledByUser,
    OrderRejectedOrCancelledByMechanic,
}
