using Core.Results; 
using MediatR;
using Account.API.Applications.Dtos.RequestDtos;

namespace Account.API.Applications.Queries.GetBusinessUserByUserId;

public record GetBusinessUserByUserIdQuery(Guid Id, FleetsRequestDto? FleetsRequest = null) : IRequest<Result>;
