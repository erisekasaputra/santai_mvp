using MediatR;

namespace Search.Worker.Applications.Commands.UpdateItemBrand;

public record UpdateItemBrandCommand(Guid Id, string Name, string ImageUrl) : IRequest; 
