namespace BuildingBlocks.Primitives.Results;

public static class ResultMessages
{
    public const string SucceededResultCannotHaveAnError     = "Succeeded result cannot have an error";
    public const string FailedResultRequiresValidError       = "IsFailed result requires a valid Error object.";
    public const string FailedResultCannotAccessData         = "The data of a failed result can not be accessed.";
    public const string SucceedResultCannotAccessError       = "Succeeded result cannot access error.";
    public const string SucceededDataResultRequiresValidData = "Succeeded data result requires a valid data object.";
    public const string NoErrorProvided                      = "No errors provided.";
    public const string SuccessResultCannotMapToFailedResult = "Cannot map a successful result to a failed data result.";
    public const string ActionCompletedSuccessfully          = "Action completed successfully.";
    public const string ActionFailed                         = "Action failed.";
}