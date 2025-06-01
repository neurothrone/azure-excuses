using Excuses.Persistence.Shared.Models;
using Excuses.Persistence.Shared.Repositories;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Net;
using System.Text.Json;

namespace Excuses.FunctionApp.Functions;

public class UpdateExcuseByIdFunction
{
    private readonly IExcuseRepository _excuseRepository;

    public UpdateExcuseByIdFunction(IExcuseRepository excuseRepository)
    {
        _excuseRepository = excuseRepository;
    }

    [Function("UpdateExcuseByIdFunction")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, "put", Route = "excuses/{id:int}")]
        HttpRequestData req,
        int id)
    {
        try
        {
            var excuse = await JsonSerializer.DeserializeAsync<ExcuseInputDto>(req.Body);
            if (excuse == null)
            {
                var badRequestResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                await badRequestResponse.WriteStringAsync("Missing or invalid fields");
                return badRequestResponse;
            }

            var updatedExcuse = await _excuseRepository.UpdateExcuseById(id, excuse);
            if (updatedExcuse == null)
            {
                var notFoundResponse = req.CreateResponse(HttpStatusCode.NotFound);
                return notFoundResponse;
            }

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(updatedExcuse);
            return response;
        }
        catch (Exception e)
        {
            var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
            await errorResponse.WriteStringAsync($"An error occurred while updating the excuse: {e.Message}");
            return errorResponse;
        }
    }
}