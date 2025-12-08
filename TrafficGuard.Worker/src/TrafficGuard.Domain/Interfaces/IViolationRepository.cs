using System;
using System.Collections.Generic;
using System.Text;
using TrafficGuard.Domain.Entities;

namespace TrafficGuard.Domain.Interfaces;

public interface IViolationRepository
{
    Task SaveAsync(TrafficViolation violation);
    Task<IEnumerable<TrafficViolation>> GetByPlateAsync(string plate);
}