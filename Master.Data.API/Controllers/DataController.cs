using Core.CustomMessages;
using Core.Results;
using Core.Utilities;
using Master.Data.API.Dtos;
using Master.Data.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Newtonsoft.Json;
namespace Master.Data.API.Controllers;


[ApiController]
[Route("api/v1/master")]
public class DataController : ControllerBase
{
    private readonly ILogger<DataController> _logger;
    public DataController(ILogger<DataController> logger)
    {
        _logger = logger;   
    }

    [HttpGet("order")]
    [OutputCache(Duration = 60)]
    public IResult GetInitializeOrder()
    {
        string filePathPreService = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Order/PreServiceInspection/pre-service-inspection.json");
        if (!System.IO.File.Exists(filePathPreService))
        {
            return TypedResults.NotFound(Result.Failure("Pre service inspection master file does not exists", ResponseStatus.NotFound));
        }
        string jsonStringPreService = System.IO.File.ReadAllText(filePathPreService);
        var preService = JsonConvert.DeserializeObject<IEnumerable<PreServiceInspection>>(jsonStringPreService);


        string filePathBasicInspection = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Order/BasicInspection/basic-inspection.json");
        if (!System.IO.File.Exists(filePathBasicInspection))
        {
            return TypedResults.NotFound(Result.Failure("Basic inspection master file does not exists", ResponseStatus.NotFound));
        }
        string jsonStringBasicInspection = System.IO.File.ReadAllText(filePathBasicInspection);
        var basicInspection = JsonConvert.DeserializeObject<IEnumerable<BasicInspection>>(jsonStringBasicInspection);
         

        string filePathServiceFee = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Order/ServiceFee/service-fee.json");
        if (!System.IO.File.Exists(filePathServiceFee))
        {
            return TypedResults.NotFound(Result.Failure("Service fee master file does not exists", ResponseStatus.NotFound));
        }
        string jsonStringServiceFee = System.IO.File.ReadAllText(filePathServiceFee);
        var serviceFee = JsonConvert.DeserializeObject<IEnumerable<Fee>>(jsonStringServiceFee);





        if (preService is null || basicInspection is null || serviceFee is null)
        {
            return TypedResults.InternalServerError(
                Result.Failure("Master data has not been configured", ResponseStatus.InternalServerError));
        }

        var master = new MasterData(serviceFee, basicInspection, preService);

        return TypedResults.Ok(Result.Success(master, ResponseStatus.Ok));
    }

    [HttpGet("order/pre-service-inspection")]
    [OutputCache(Duration = 60)]
    public IResult GetMasterDataPreServiceInspection()
    { 
        string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Order/PreServiceInspection/pre-service-inspection.json");
         
        if (!System.IO.File.Exists(filePath))
        {
            return TypedResults.NotFound(Result.Failure("Pre service inspection master file does not exists", ResponseStatus.NotFound));
        }

        string jsonString = System.IO.File.ReadAllText(filePath);

        var preService = JsonConvert.DeserializeObject<IEnumerable<PreServiceInspection>>(jsonString);
        return TypedResults.Ok(Result.Success(preService, ResponseStatus.Ok));
    }



    [HttpGet("order/basic-inspection")]
    [OutputCache(Duration = 60)] 
    public IResult GetMasterDataBasicInspection()
    {
        string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Order/BasicInspection/basic-inspection.json");

        if (!System.IO.File.Exists(filePath))
        {
            return TypedResults.NotFound(Result.Failure("Basic inspection master file does not exists", ResponseStatus.NotFound));
        }

        string jsonString = System.IO.File.ReadAllText(filePath);

        var basicInspection = JsonConvert.DeserializeObject<IEnumerable<BasicInspection>>(jsonString);
        return TypedResults.Ok(Result.Success(basicInspection, ResponseStatus.Ok));
    }


    [HttpGet("order/service-fee")]
    [OutputCache(Duration = 60)]
    public IResult GetMasterDataServiceFee()
    {
        string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Order/ServiceFee/service-fee.json");

        if (!System.IO.File.Exists(filePath))
        {
            return TypedResults.NotFound(Result.Failure("Service fee master file does not exists", ResponseStatus.NotFound));
        } 
        string jsonString = System.IO.File.ReadAllText(filePath);
        var serviceFee = JsonConvert.DeserializeObject<IEnumerable<Fee>>(jsonString); 
        return TypedResults.Ok(Result.Success(serviceFee, ResponseStatus.Ok));
    }




    [HttpGet("order/cancellation-fee")]
    [OutputCache(Duration = 60)]
    public IResult GetMasterDataCancellationFee()
    {
        string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Order/CancellationFee/cancellation-fee.json");

        if (!System.IO.File.Exists(filePath))
        {
            return TypedResults.NotFound(Result.Failure("Cancellation fee master file does not exists", ResponseStatus.NotFound));
        }

        string jsonString = System.IO.File.ReadAllText(filePath);
        var cancellationFee = JsonConvert.DeserializeObject<IEnumerable<CancellationFee>>(jsonString);
        return TypedResults.Ok(Result.Success(cancellationFee, ResponseStatus.Ok));
    }



    [HttpPut("order/cancellation-fee")]
    public IResult UpdateCancellationFee(
        [FromBody] CancellationFeeRequest cancellationFeeRequest)
    {
        try
        {
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Order/CancellationFee/cancellation-fee.json");

            string jsonString = JsonConvert.SerializeObject(cancellationFeeRequest.CancellationFees, Formatting.Indented);
            
            System.IO.File.WriteAllText(filePath, jsonString);
            return TypedResults.NoContent();
        }
        catch (Exception ex)
        {
            LoggerHelper.LogError(_logger, ex);
            return TypedResults.InternalServerError(
                Result.Failure(Messages.InternalServerError, ResponseStatus.InternalServerError));
        }
    }


    [HttpPut("order/service-fee")]
    public IResult UpdateServiceFee(
        [FromBody] ServiceFeeRequest serviceFeeRequest)
    {
        try
        {
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Order/ServiceFee/service-fee.json");

            string jsonString = JsonConvert.SerializeObject(serviceFeeRequest.ServiceFees, Formatting.Indented);

            System.IO.File.WriteAllText(filePath, jsonString);
            return TypedResults.NoContent();
        }
        catch (Exception ex)
        {
            LoggerHelper.LogError(_logger, ex);
            return TypedResults.InternalServerError(
                Result.Failure(Messages.InternalServerError, ResponseStatus.InternalServerError));
        }
    }


    [HttpPut("order/basic-inspection")]
    public IResult UpdateBasicInspection(
        [FromBody] BasicInspectionRequest basicInspectionRequest)
    {
        try
        {
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Order/BasicInspection/basic-inspection.json");

            string jsonString = JsonConvert.SerializeObject(basicInspectionRequest.BasicInspections, Formatting.Indented);

            System.IO.File.WriteAllText(filePath, jsonString);
            return TypedResults.NoContent();
        }
        catch (Exception ex)
        {
            LoggerHelper.LogError(_logger, ex);
            return TypedResults.InternalServerError(
                Result.Failure(Messages.InternalServerError, ResponseStatus.InternalServerError));
        }
    }



    [HttpPut("order/pre-service-inspection")]
    public IResult UpdatePreServiceInspection(
        [FromBody] PreServiceInspectionRequest preServiceInspectionRequest)
    {
        try
        {
            string filePath = Path.Combine(
                Directory.GetCurrentDirectory(), 
                "wwwroot", 
                "Order/PreServiceInspection/pre-service-inspection.json");

            string jsonString = JsonConvert.SerializeObject(preServiceInspectionRequest.PreServiceInspections, Formatting.Indented);

            System.IO.File.WriteAllText(filePath, jsonString);
            return TypedResults.NoContent();
        }
        catch (Exception ex)
        {
            LoggerHelper.LogError(_logger, ex);
            return TypedResults.InternalServerError(
                Result.Failure(Messages.InternalServerError, ResponseStatus.InternalServerError));
        }
    }
}
