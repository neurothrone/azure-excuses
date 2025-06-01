using Excuses.Persistence.Shared.Repositories;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Net;

namespace Excuses.FunctionApp.Functions;

public class GetExcuseByIdFunction
{
    private readonly IExcuseRepository _excuseRepository;

    public GetExcuseByIdFunction(IExcuseRepository excuseRepository)
    {
        _excuseRepository = excuseRepository;
    }

    [Function("GetExcuseByIdFunction")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "excuses/{id:int}")]
        HttpRequestData req,
        int id)
    {
        try
        {
            var excuse = await _excuseRepository.GetExcuseById(id);
            if (excuse == null)
            {
                var notFoundResponse = req.CreateResponse(HttpStatusCode.NotFound);
                return notFoundResponse;
            }

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(excuse);
            return response;
        }
        catch (Exception e)
        {
            var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
            await errorResponse.WriteStringAsync($"An error occurred while retrieving the excuse: {e.Message}");
            return errorResponse;
        }
    }
}