using MediatR;

namespace Search.Worker.Applications.Commands.UpdateItemCategory;

public record UpdateItemCategoryCommand(Guid Id, string Name, string ImageUrl) : IRequest;
