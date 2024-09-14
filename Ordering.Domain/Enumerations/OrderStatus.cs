namespace Ordering.Domain.Enumerations;

public enum OrderStatus
{
    PaymentPending = 0,
    PaymentPaid = 1,
    FindingMechanic = 2,
    MechanicAssigned = 4,  
    MechanicDispatched = 5,
    MechanicArrived = 6,
    ServiceInProgress = 7,
    ServiceCompleted = 8,
    ServiceIncompleted = 9,
    OrderCancelledByUser = 20,
    OrderCancelledByMechanic = 21,
}
