using Core.Results;
using Core.Messages;
using MediatR;
using Account.API.Applications.Dtos.RequestDtos;

namespace Account.API.Applications.Queries.GetRegularUserByUserId;

public record GetRegularUserByUserIdQuery(Guid UserId, FleetsRequestDto? FleetsRequest = null) : IRequest<Result>;
