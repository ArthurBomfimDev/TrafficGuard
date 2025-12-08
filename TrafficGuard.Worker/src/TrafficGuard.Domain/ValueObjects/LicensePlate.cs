using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using TrafficGuard.Domain.Exceptions;

namespace TrafficGuard.Domain.ValueObjects;

public record LicensePlate
{
    public string Value { get; }

    private const string Pattern = @"^[A-Z]{3}[0-9][0-9A-Z][0-9]{2}$";

    private LicensePlate(string value)
    {
        Value = value;
    }

    public static LicensePlate Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("License plate cannot be empty.");

        var normalized = value.Replace("-", "").Trim().ToUpper();

        if (!Regex.IsMatch(normalized, Pattern))
            throw new DomainException($"Invalid license plate format: {value}");

        return new LicensePlate(normalized);
    }

    public override string ToString() => Value;

    public static implicit operator string(LicensePlate plate) => plate.Value;
}