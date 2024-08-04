using MediatR;

namespace Search.Worker.Applications.Commands.DeleteItemBrand;

public record DeleteItemBrandCommand(string Id) : IRequest;
