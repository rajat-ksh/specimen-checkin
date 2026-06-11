using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SpecimenCheckIn.Api.Dto;
using SpecimenCheckIn.Api.Services;

namespace SpecimenCheckIn.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ManifestController(ManifestService manifestService) : ControllerBase
{
    private readonly ManifestService _manifestService = manifestService;

    [HttpGet]
    public async Task<IActionResult> GetManifests()
    {
        var manifests =
            await _manifestService.GetManifestsAsync();

        return Ok(manifests);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetManifest(
    Guid id)
    {
        var manifest =
            await _manifestService.GetManifestAsync(id);

        if (manifest is null)
            return NotFound();

        return Ok(manifest);
    }

    [HttpPost("{manifestId:guid}/specimens/{specimenId:guid}/receive")]
    public async Task<IActionResult> Receive(
    Guid manifestId,
    Guid specimenId)
    {
        await _manifestService.ReceiveSpecimenAsync(
            manifestId,
            specimenId);

        return NoContent();
    }

    [HttpPost("{manifestId:guid}/specimens/{specimenId:guid}/flag")]
    public async Task<IActionResult> Flag(
    Guid manifestId,
    Guid specimenId)
    {
        await _manifestService.FlagSpecimenAsync(
            manifestId,
            specimenId);

        return NoContent();
    }

    [HttpPost("{manifestId:guid}/specimens")]
    public async Task<IActionResult> AddSpecimen(
    Guid manifestId,
    AddSpecimenRequest request)
    {
        await _manifestService.AddOffManifestSpecimenAsync(
            manifestId,
            request);

        return NoContent();
    }

    [HttpPost("{manifestId:guid}/close")]
    public async Task<IActionResult> Close(
    Guid manifestId)
    {
        await _manifestService.CloseManifestAsync(
            manifestId);

        return NoContent();
    }
}
