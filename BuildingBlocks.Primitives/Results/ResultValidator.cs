namespace BuildingBlocks.Primitives.Results;

internal static class ResultValidator
{
    public static void Validate(bool isSucceeded, Error error)
    {
        switch (isSucceeded)
        {
            case true when error != ErrorCreator.None():
                throw new ResultException(ResultMessages.SucceededResultCannotHaveAnError);
            case false when error == ErrorCreator.None():
                throw new ResultException(ResultMessages.FailedResultRequiresValidError);
        }
    }

    public static void Validate<TData>(bool isSucceeded, TData? data)
    {
        if (isSucceeded && data is null)
            throw new ResultException(ResultMessages.SucceededDataResultRequiresValidData);
    }
}