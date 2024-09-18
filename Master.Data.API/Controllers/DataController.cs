using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
namespace Master.Data.API.Controllers;


[ApiController]
[Route("api/v1/master")]
public class DataController : ControllerBase
{
    public DataController()
    {
        
    }



    [HttpGet("order/pre-service-inspection")]
    [OutputCache(Duration = 60)]
    public IResult GetMasterDataPreServiceInspection()
    { 
        string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Order/PreServiceInspection/pre-service-inspection.json");
         
        if (!System.IO.File.Exists(filePath))
        {
            return TypedResults.NotFound();
        }

        string jsonString = System.IO.File.ReadAllText(filePath);  

        return TypedResults.Json(jsonString);
    }



    [HttpGet("order/basic-inspection")]
    [OutputCache(Duration = 60)] 
    public IResult GetMasterDataBasicInspection()
    {
        string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Order/BasicInspection/basic-inspection.json");

        if (!System.IO.File.Exists(filePath))
        {
            return TypedResults.NotFound();
        }

        string jsonString = System.IO.File.ReadAllText(filePath);

        return TypedResults.Json(jsonString);
    }


    [HttpGet("order/service-fee")]
    [OutputCache(Duration = 60)]
    public IResult GetMasterDataServiceFee()
    {
        string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Order/ServiceFee/service-fee.json");

        if (!System.IO.File.Exists(filePath))
        {
            return TypedResults.NotFound();
        }

        string jsonString = System.IO.File.ReadAllText(filePath);

        return TypedResults.Json(jsonString);
    }
}
