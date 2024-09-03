using MediatR;

namespace Search.Worker.Applications.Commands.DeleteItemBrand;

public record DeleteItemBrandCommand(Guid Id) : IRequest;
