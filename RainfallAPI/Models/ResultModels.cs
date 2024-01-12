namespace RainfallAPI.Models;

public record RainfallReading(DateTime dateMeasured, decimal amountMeasured);
public record Result(IEnumerable<RainfallReading> reading);
public record ErrorDetail(string propertyName, string message);
public record Error(string message, IEnumerable<ErrorDetail>? detail);
