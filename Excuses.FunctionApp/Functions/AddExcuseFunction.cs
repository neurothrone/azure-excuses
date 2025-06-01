using Excuses.Persistence.Shared.Models;
using Excuses.Persistence.Shared.Repositories;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Net;
using System.Text.Json;

namespace Excuses.FunctionApp.Functions;

public class AddExcuseFunction
{
    private readonly IExcuseRepository _excuseRepository;

    public AddExcuseFunction(IExcuseRepository excuseRepository)
    {
        _excuseRepository = excuseRepository;
    }

    [Function("AddExcuseFunction")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "excuses")]
        HttpRequestData req)
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

            var newExcuse = await _excuseRepository.AddExcuse(excuse);

            var response = req.CreateResponse(HttpStatusCode.Created);
            await response.WriteAsJsonAsync(newExcuse);
            return response;
        }
        catch (Exception e)
        {
            var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
            await errorResponse.WriteStringAsync($"An error occurred while adding the excuse: {e.Message}");
            return errorResponse;
        }
    }
}