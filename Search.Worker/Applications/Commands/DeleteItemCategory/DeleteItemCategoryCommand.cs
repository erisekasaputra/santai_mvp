using MediatR;

namespace Search.Worker.Applications.Commands.DeleteItemCategory;

public record DeleteItemCategoryCommand(Guid Id) : IRequest;
