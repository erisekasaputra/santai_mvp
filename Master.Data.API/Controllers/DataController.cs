using Master.Data.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Newtonsoft.Json;
namespace Master.Data.API.Controllers;


[ApiController]
[Route("api/v1/master")]
public class DataController : ControllerBase
{
    public DataController()
    {
        
    }

    [HttpGet("order")]
    [OutputCache(Duration = 60)]
    public IResult GetInitializeOrder()
    {
        string filePathPreService = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Order/PreServiceInspection/pre-service-inspection.json");
        if (!System.IO.File.Exists(filePathPreService))
        {
            return TypedResults.NotFound();
        }
        string jsonStringPreService = System.IO.File.ReadAllText(filePathPreService);
        var preService = JsonConvert.DeserializeObject<IEnumerable<PreServiceInspection>>(jsonStringPreService);


        string filePathBasicInspection = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Order/BasicInspection/basic-inspection.json");
        if (!System.IO.File.Exists(filePathBasicInspection))
        {
            return TypedResults.NotFound();
        }
        string jsonStringBasicInspection = System.IO.File.ReadAllText(filePathBasicInspection);
        var basicInspection = JsonConvert.DeserializeObject<IEnumerable<BasicInspection>>(jsonStringBasicInspection);
         

        string filePathServiceFee = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Order/ServiceFee/service-fee.json");
        if (!System.IO.File.Exists(filePathServiceFee))
        {
            return TypedResults.NotFound();
        }
        string jsonStringServiceFee = System.IO.File.ReadAllText(filePathServiceFee);
        var serviceFee = JsonConvert.DeserializeObject<IEnumerable<Fee>>(jsonStringServiceFee);





        if (preService is null || basicInspection is null || serviceFee is null)
        {
            return TypedResults.InternalServerError("Master data has not been configured");
        }

        var master = new MasterData(serviceFee, basicInspection, preService);

        return TypedResults.Ok(JsonConvert.SerializeObject(master));
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

        return TypedResults.Ok(jsonString);
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

        return TypedResults.Ok(jsonString);
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

        return TypedResults.Ok(jsonString);
    }




    [HttpGet("order/cancellation-fee")]
    [OutputCache(Duration = 60)]
    public IResult GetMasterDataCancellationFee()
    {
        string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Order/CancellationFee/cancellation-fee.json");

        if (!System.IO.File.Exists(filePath))
        {
            return TypedResults.NotFound();
        }

        string jsonString = System.IO.File.ReadAllText(filePath);

        return TypedResults.Ok(jsonString);
    }


}
