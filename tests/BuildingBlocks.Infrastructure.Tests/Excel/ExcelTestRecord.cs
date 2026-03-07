namespace BuildingBlocks.Infrastructure.Tests.Excel;

public record ExcelTestRecord
{
    public string Name { get; init; } = "";
    public int Age { get; init; }
    public decimal Salary { get; init; }
}
