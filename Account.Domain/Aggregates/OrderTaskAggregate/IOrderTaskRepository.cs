namespace Account.Domain.Aggregates.OrderTaskAggregate;

public interface IOrderTaskRepository
{
    Task<IEnumerable<OrderTaskWaitingMechanicAssign>> GetOrderWithUnassignedMechanic(int rowNumber);
    void UpdateRangeOrders(IEnumerable<OrderTaskWaitingMechanicAssign> orders);
    void UpdateMechanicTask(MechanicOrderTask mechanicTask);
    Task<MechanicOrderTask?> GetMechanicTaskByIdWithSkipLockAsync(Guid id);
    Task CreateOrderTaskWaitingMechanicAssignAsync(OrderTaskWaitingMechanicAssign assignTask);
    Task CreateOrderTaskWaitingMechanicConfirmAsync(OrderTaskWaitingMechanicConfirm confirmTask);
}
