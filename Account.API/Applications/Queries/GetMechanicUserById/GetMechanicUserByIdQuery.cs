using Core.Results;
using Core.Messages;
using MediatR;
using Account.API.Applications.Dtos.RequestDtos;

namespace Account.API.Applications.Queries.GetMechanicUserById;

public record GetMechanicUserByIdQuery(Guid Id, FleetsRequestDto? FleetsRequest = null) : IRequest<Result>; 
