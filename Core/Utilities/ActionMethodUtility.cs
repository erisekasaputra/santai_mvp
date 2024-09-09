using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;

namespace Core.Utilities;

public class ActionMethodUtility
{
    private readonly IActionDescriptorCollectionProvider _actionDescriptorCollectionProvider;

    public ActionMethodUtility(IActionDescriptorCollectionProvider actionDescriptorCollectionProvider)
    {
        _actionDescriptorCollectionProvider = actionDescriptorCollectionProvider;
    }

    public string GetHttpMethodByActionName(string actionName, string controllerName)
    {
        // Normalize controller name (remove "Controller" suffix)
        var normalizedControllerName = NormalizeControllerName(controllerName);

        var actionDescriptors = _actionDescriptorCollectionProvider.ActionDescriptors.Items;

        foreach (var descriptor in actionDescriptors)
        {
            if (descriptor is ControllerActionDescriptor controllerActionDescriptor)
            {
                if (controllerActionDescriptor.ActionName.Equals(actionName, StringComparison.OrdinalIgnoreCase) &&
                    controllerActionDescriptor.ControllerName.Equals(normalizedControllerName, StringComparison.OrdinalIgnoreCase))
                {
                    // Retrieve the HTTP methods
                    var httpMethods = controllerActionDescriptor.EndpointMetadata
                        .OfType<HttpMethodAttribute>()
                        .Select(attr => attr.HttpMethods.FirstOrDefault())
                        .ToList();

                    if (httpMethods.Any())
                    {
                        return string.Join(", ", httpMethods); // Return HTTP methods as a comma-separated string
                    }
                }
            }
        }

        return string.Empty; // Action method not found or no HTTP methods defined
    }

    private string NormalizeControllerName(string controllerName)
    {
        if (controllerName.EndsWith("Controller", StringComparison.OrdinalIgnoreCase))
        {
            return controllerName.Substring(0, controllerName.Length - "Controller".Length);
        }
        return controllerName;
    }
}