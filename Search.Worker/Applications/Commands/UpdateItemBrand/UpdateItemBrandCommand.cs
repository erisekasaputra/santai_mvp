using MediatR;

namespace Search.Worker.Applications.Commands.UpdateItemBrand;

public record UpdateItemBrandCommand(string Id, string Name, string ImageUrl) : IRequest; 
