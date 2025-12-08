using System;
using System.Collections.Generic;
using System.Text;

namespace TrafficGuard.Domain.Enums;

public enum ViolationSeverity
{
    None = 0,           // Sem infração
    Medium = 1,         // Média (até 20%)
    Serious = 2,        // Grave (20% a 50%)
    VerySerious = 3     // Gravíssima (acima de 50%)
}