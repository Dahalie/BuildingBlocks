namespace BuildingBlocks.Application.Excel;

public class ExcelReadOptions
{
    public string? SheetName { get; init; }
    public int HeaderRow { get; init; } = 1;
}
