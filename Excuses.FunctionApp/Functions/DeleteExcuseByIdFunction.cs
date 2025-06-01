using Excuses.Persistence.Shared.Repositories;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Net;

namespace Excuses.FunctionApp.Functions;

public class DeleteExcuseByIdFunction
{
    private readonly IExcuseRepository _excuseRepository;

    public DeleteExcuseByIdFunction(IExcuseRepository excuseRepository)
    {
        _excuseRepository = excuseRepository;
    }

    [Function("DeleteExcuseByIdFunction")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "excuses/{id:int}")]
        HttpRequestData req,
        int id)
    {
        try
        {
            var deleted = await _excuseRepository.DeleteExcuseById(id);

            if (!deleted)
            {
                var notFoundResponse = req.CreateResponse(HttpStatusCode.NotFound);
                return notFoundResponse;
            }

            var response = req.CreateResponse(HttpStatusCode.NoContent);
            return response;
        }
        catch (Exception e)
        {
            var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
            await errorResponse.WriteStringAsync($"An error occurred while deleting the excuse: {e.Message}");
            return errorResponse;
        }
    }
}