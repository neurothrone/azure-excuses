using Excuses.Persistence.Shared.Repositories;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Net;

namespace Excuses.FunctionApp.Functions;

public class GetAllExcusesFunction
{
    private readonly IExcuseRepository _excuseRepository;

    public GetAllExcusesFunction(IExcuseRepository excuseRepository)
    {
        _excuseRepository = excuseRepository;
    }

    [Function("GetAllExcusesFunction")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "excuses")]
        HttpRequestData req)
    {
        try
        {
            var excuses = await _excuseRepository.GetAllExcuses();

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(excuses);
            return response;
        }
        catch (Exception e)
        {
            var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
            await errorResponse.WriteStringAsync($"An error occurred while retrieving excuses: {e.Message}");
            return errorResponse;
        }
    }
}