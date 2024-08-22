using Account.API.SeedWork;
using MediatR;

namespace Account.API.Applications.Queries.GetDeviceIdByStaffId;

public record GetDeviceIdByStaffIdQuery(Guid StaffId) : IRequest<Result>;