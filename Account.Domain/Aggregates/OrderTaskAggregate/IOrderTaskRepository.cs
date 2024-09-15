namespace Account.Domain.Aggregates.OrderTaskAggregate;

public interface IOrderTaskRepository
{
    Task<MechanicOrderTask?> GetMechanicTaskByMechanicIdSkipLockedAsync(Guid mechanicId); 
    Task<MechanicOrderTask?> GetMechanicTaskByMechanicIdSkipLockedUnassignedOrderAsync(Guid mechanicId); 
    Task<IEnumerable<OrderTaskWaitingMechanicAssign>> GetOrdersUnassignedMechanicSkipLockedAsync(int rowNumber); 
    Task<IEnumerable<OrderTaskWaitingMechanicConfirm>> GetOrdersExpiredMechanicConfirmationSkipLockedAsync(int rowNumber); 
    Task CreateOrderTaskWaitingMechanicAssignAsync(OrderTaskWaitingMechanicAssign assignTask);  
    Task CreateOrderTaskWaitingMechanicConfirmAsync(OrderTaskWaitingMechanicConfirm confirmTask); 
    void UpdateMechanicTask(MechanicOrderTask mechanicTask);  
    void UpdateRangeOrdersAssign(IEnumerable<OrderTaskWaitingMechanicAssign> orders); 
    void UpdateRangeOrdersConfirm(IEnumerable<OrderTaskWaitingMechanicConfirm> orders);  
    Task<OrderTaskWaitingMechanicAssign?> GetOrderWaitingMechanicAssignByOrderIdWithSkipLockedAsync(Guid orderId); 
    Task<OrderTaskWaitingMechanicConfirm?> GetOrderWaitingMechanicConfirmByOrderIdWithSkipLockedAsync(Guid orderId); 
    void UpdateOrderAssign(OrderTaskWaitingMechanicAssign order);
    void UpdateOrderConfirm(OrderTaskWaitingMechanicConfirm order);
    Task<MechanicOrderTask?> GetMechanicTaskByMechanicIdUnassignedOrderAsync(Guid mechanicId);
    Task<MechanicOrderTask?> GetMechanicTaskByMechanicIdAsync(Guid mechanicId);
    Task<OrderTaskWaitingMechanicAssign?> GetOrderWaitingMechanicAssignByOrderIdAsync(Guid orderId);
    Task<OrderTaskWaitingMechanicConfirm?> GetOrderWaitingMechanicConfirmByOrderIdAsync(Guid orderId);
    void RemoveMechanicTask(MechanicOrderTask task);
}
