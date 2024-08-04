using MediatR;

namespace Search.Worker.Applications.Commands.UpdateItemCategory;

public record UpdateItemCategoryCommand(string Id, string Name, string ImageUrl) : IRequest;
