using Microsoft.EntityFrameworkCore;
using SpecimenCheckIn.Api.Dto;
using SpecimenCheckIn.Api.Enums;
using SpecimenCheckIn.Tests.Infrastructure;

namespace SpecimenCheckIn.Tests.Services;

[TestFixture]
public class ManifestServiceTests
{
    private ManifestServiceTestContext _context = null!;

    [SetUp]
    public void SetUp()
    {
        _context = new ManifestServiceTestContext();
    }

    [TearDown]
    public void TearDown()
    {
        _context.Dispose();
    }

    [Test]
    public async Task GetManifestsAsync_ShouldReturnOnlyCurrentTenantManifests()
    {
        var result =
            await _context.Service.GetManifestsAsync();

        Assert.That(result, Has.Count.EqualTo(1));

        Assert.That(
            result.Single().Code,
            Is.EqualTo("MAN-A-001")
        );

        Assert.That(
            result.Any(x => x.Code == "MAN-B-001"),
            Is.False
        );
    }

    [Test]
    public async Task GetManifestAsync_ShouldReturnManifest_WhenTenantOwnsManifest()
    {
        var result =
            await _context.Service.GetManifestAsync(
                ManifestServiceTestContext.ManifestAId
            );

        Assert.That(result, Is.Not.Null);

        Assert.That(
            result!.Id,
            Is.EqualTo(
                ManifestServiceTestContext.ManifestAId
            )
        );

        Assert.That(
            result.Code,
            Is.EqualTo("MAN-A-001")
        );
    }

    [Test]
    public async Task GetManifestAsync_ShouldReturnNull_WhenManifestBelongsToAnotherTenant()
    {
        var result =
            await _context.Service.GetManifestAsync(
                ManifestServiceTestContext.ManifestBId
            );

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task ReceiveSpecimenAsync_ShouldMarkPendingSpecimenAsReceived()
    {
        await _context.Service.ReceiveSpecimenAsync(
            ManifestServiceTestContext.ManifestAId,
            ManifestServiceTestContext.PendingSpecimenAId
        );

        var specimen =
            await _context.Db.Specimens.SingleAsync(
                x =>
                    x.Id ==
                    ManifestServiceTestContext.PendingSpecimenAId
            );

        Assert.That(
            specimen.Status,
            Is.EqualTo(SpecimenStatus.Received)
        );
    }

    [Test]
    public async Task ReceiveSpecimenAsync_ShouldBeIdempotent_WhenCalledTwice()
    {
        await _context.Service.ReceiveSpecimenAsync(
            ManifestServiceTestContext.ManifestAId,
            ManifestServiceTestContext.PendingSpecimenAId
        );

        var eventCountAfterFirstScan =
            await _context.Db.CheckInEvents.CountAsync(
                x =>
                    x.SpecimenId ==
                    ManifestServiceTestContext.PendingSpecimenAId
            );

        await _context.Service.ReceiveSpecimenAsync(
            ManifestServiceTestContext.ManifestAId,
            ManifestServiceTestContext.PendingSpecimenAId
        );

        var specimen =
            await _context.Db.Specimens.SingleAsync(
                x =>
                    x.Id ==
                    ManifestServiceTestContext.PendingSpecimenAId
            );

        var eventCountAfterDuplicateScan =
            await _context.Db.CheckInEvents.CountAsync(
                x =>
                    x.SpecimenId ==
                    ManifestServiceTestContext.PendingSpecimenAId
            );

        Assert.Multiple(() =>
        {
            Assert.That(
                specimen.Status,
                Is.EqualTo(SpecimenStatus.Received)
            );

            Assert.That(
                eventCountAfterDuplicateScan,
                Is.EqualTo(eventCountAfterFirstScan)
            );
        });
    }

    [Test]
    public async Task ReceiveSpecimenAsync_ShouldRejectMutation_WhenSpecimenBelongsToAnotherTenant()
    {
        _context.Dispose();

        _context =
            new ManifestServiceTestContext(
                ManifestServiceTestContext.LabAId
            );

        Assert.That(
            async () =>
                await _context.Service.ReceiveSpecimenAsync(
                    ManifestServiceTestContext.ManifestBId,
                    ManifestServiceTestContext.PendingSpecimenBId
                ),
            Throws.TypeOf<InvalidOperationException>()
        );

        var specimen =
            await _context.Db.Specimens.SingleAsync(
                x =>
                    x.Id ==
                    ManifestServiceTestContext.PendingSpecimenBId
            );

        Assert.That(
            specimen.Status,
            Is.EqualTo(SpecimenStatus.Pending)
        );
    }

    [Test]
    public async Task FlagSpecimenAsync_ShouldMarkSpecimenAsFlagged()
    {
        await _context.Service.FlagSpecimenAsync(
            ManifestServiceTestContext.ManifestAId,
            ManifestServiceTestContext.PendingSpecimenAId
        );

        var specimen =
            await _context.Db.Specimens.SingleAsync(
                x =>
                    x.Id ==
                    ManifestServiceTestContext.PendingSpecimenAId
            );

        Assert.That(
            specimen.Status,
            Is.EqualTo(SpecimenStatus.Flagged)
        );
    }

    [Test]
    public async Task FlagSpecimenAsync_ShouldCreateMissingDiscrepancy()
    {
        await _context.Service.FlagSpecimenAsync(
            ManifestServiceTestContext.ManifestAId,
            ManifestServiceTestContext.PendingSpecimenAId
        );

        var discrepancy =
            await _context.Db.Discrepancies.SingleAsync(
                x =>
                    x.SpecimenId ==
                    ManifestServiceTestContext.PendingSpecimenAId
            );

        Assert.Multiple(() =>
        {
            Assert.That(
                discrepancy.Type,
                Is.EqualTo(DiscrepancyType.Missing)
            );

            Assert.That(
                discrepancy.Status,
                Is.EqualTo(DiscrepancyStatus.Open)
            );
        });
    }

    [Test]
    public async Task AddOffManifestSpecimenAsync_ShouldCreateAddedSpecimen()
    {
        var request =
            new AddSpecimenRequest
            {
                Code = "SP-OFF-001",
                Patient = "Synthetic Patient E",
                Site = "Kidney",
                Provider = "Dr Demo"
            };

        await _context.Service
            .AddOffManifestSpecimenAsync(
                ManifestServiceTestContext.ManifestAId,
                request
            );

        var specimen =
            await _context.Db.Specimens.SingleAsync(
                x => x.Code == "SP-OFF-001"
            );

        Assert.Multiple(() =>
        {
            Assert.That(
                specimen.Status,
                Is.EqualTo(SpecimenStatus.Added)
            );

            Assert.That(
                specimen.ManifestId,
                Is.EqualTo(
                    ManifestServiceTestContext.ManifestAId
                )
            );
        });
    }

    [Test]
    public async Task AddOffManifestSpecimenAsync_ShouldCreateOffManifestDiscrepancy()
    {
        var request =
            new AddSpecimenRequest
            {
                Code = "SP-OFF-002",
                Patient = "Synthetic Patient F",
                Site = "Kidney",
                Provider = "Dr Demo"
            };

        await _context.Service
            .AddOffManifestSpecimenAsync(
                ManifestServiceTestContext.ManifestAId,
                request
            );

        var specimen =
            await _context.Db.Specimens.SingleAsync(
                x => x.Code == "SP-OFF-002"
            );

        var discrepancy =
            await _context.Db.Discrepancies.SingleAsync(
                x => x.SpecimenId == specimen.Id
            );

        Assert.Multiple(() =>
        {
            Assert.That(
                discrepancy.Type,
                Is.EqualTo(DiscrepancyType.OffManifest)
            );

            Assert.That(
                discrepancy.Status,
                Is.EqualTo(DiscrepancyStatus.Open)
            );
        });
    }

    [Test]
    public void CloseManifestAsync_ShouldRejectClose_WhenPendingSpecimensRemain()
    {
        Assert.That(
            async () =>
                await _context.Service.CloseManifestAsync(
                    ManifestServiceTestContext.ManifestAId
                ),
            Throws.TypeOf<InvalidOperationException>().With.Message.Contains("pending").IgnoreCase
        );
    }

    [Test]
    public async Task CloseManifestAsync_ShouldCloseWithDiscrepancy_WhenAllSpecimensAreReconciled()
    {
        await _context.Service.ReceiveSpecimenAsync(
            ManifestServiceTestContext.ManifestAId,
            ManifestServiceTestContext.PendingSpecimenAId
        );

        await _context.Service.FlagSpecimenAsync(
            ManifestServiceTestContext.ManifestAId,
            ManifestServiceTestContext.SecondPendingSpecimenAId
        );

        await _context.Service.CloseManifestAsync(
            ManifestServiceTestContext.ManifestAId
        );

        var manifest =
            await _context.Db.Manifests.SingleAsync(
                x =>
                    x.Id ==
                    ManifestServiceTestContext.ManifestAId
            );

        Assert.That(
            manifest.Status,
            Is.EqualTo(
                ManifestStatus.ClosedWithDiscrepancy
            )
        );
    }
}