using Excuses.Persistence.Shared.Models;
using Excuses.Persistence.Shared.Repositories;

namespace Excuses.WebApi.Endpoints;

public static class ExcuseEndpoints
{
    public static void MapExcuseEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("api/excuses");

        group.MapGet(string.Empty, async (IExcuseRepository excuseRepository) =>
        {
            try
            {
                var excuses = await excuseRepository.GetAllExcuses();
                return Results.Ok(excuses);
            }
            catch (Exception e)
            {
                return Results.Problem($"An error occurred while retrieving excuses: {e.Message}");
            }
        }).WithName("GetAllExcuses");

        group.MapGet("/{id:int:min(0)}", async (int id, IExcuseRepository excuseRepository) =>
        {
            try
            {
                var excuse = await excuseRepository.GetExcuseById(id);
                return excuse == null
                    ? Results.NotFound()
                    : Results.Ok(excuse);
            }
            catch (Exception e)
            {
                return Results.Problem($"An error occurred while retrieving the excuse: {e.Message}");
            }
        }).WithName("GetExcuseById");

        group.MapPost(string.Empty, async (ExcuseInputDto excuse, IExcuseRepository excuseRepository) =>
        {
            try
            {
                var createdExcuse = await excuseRepository.AddExcuse(excuse);
                return Results.Created($"/excuses/{createdExcuse.Id}", createdExcuse);
            }
            catch (Exception e)
            {
                return Results.Problem($"An error occurred while adding the excuse: {e.Message}");
            }
        }).WithName("AddExcuse");

        group.MapPut("/{id:int:min(0)}", async (int id, ExcuseInputDto excuse, IExcuseRepository excuseRepository) =>
        {
            try
            {
                var updatedExcuse = await excuseRepository.UpdateExcuseById(id, excuse);
                return updatedExcuse == null
                    ? Results.NotFound()
                    : Results.Ok(updatedExcuse);
            }
            catch (Exception e)
            {
                return Results.Problem($"An error occurred while updating the excuse: {e.Message}");
            }
        }).WithName("UpdateExcuseById");

        group.MapDelete("/{id:int:min(0)}", async (int id, IExcuseRepository excuseRepository) =>
        {
            try
            {
                var deleted = await excuseRepository.DeleteExcuseById(id);
                return deleted
                    ? Results.NoContent()
                    : Results.NotFound();
            }
            catch (Exception e)
            {
                return Results.Problem($"An error occurred while deleting the excuse: {e.Message}");
            }
        }).WithName("DeleteExcuseById");
    }
}
