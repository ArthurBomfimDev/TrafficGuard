using Microsoft.EntityFrameworkCore;
using TrafficGuard.Domain.Entities;
using TrafficGuard.Domain.Interfaces;
using TrafficGuard.Infrastructure.Data;

namespace TrafficGuard.Infrastructure.Repositories;

public class ViolationRepository : IViolationRepository
{
    private readonly TrafficGuardDbContext _context;

    public ViolationRepository(TrafficGuardDbContext context)
    {
        _context = context;
    }

    public async Task SaveAsync(TrafficViolation violation)
    {
        await _context.Violations.AddAsync(violation);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<TrafficViolation>> GetByPlateAsync(string plate)
    {
        var plateNormalized = plate.Replace("-", "").ToUpper();

        return await _context.Violations
            .Where(v => v.LicensePlate.Value == plateNormalized)
            .ToListAsync();
    }
}