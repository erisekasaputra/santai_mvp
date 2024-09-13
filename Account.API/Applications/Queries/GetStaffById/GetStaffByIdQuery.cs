using Account.API.Applications.Dtos.RequestDtos;
using Core.Results;
using MediatR;

namespace Account.API.Applications.Queries.GetStaffById;

public record GetStaffByIdQuery(Guid StaffId, FleetsRequestDto? FleetsRequest = null) : IRequest<Result>;