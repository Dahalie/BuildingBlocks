namespace BuildingBlocks.Primitives.Results;

public enum ErrorType
{
    None           = 0,
    Validation     = 1,
    Authentication = 2,
    Authorization  = 3,
    NotFound       = 4,
    Conflict       = 5,
    Business       = 6,
    Exception      = 7
}