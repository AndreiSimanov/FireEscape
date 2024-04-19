namespace FireEscape.Common;

public class PagedResult<TValue>
{
    public static PagedResult<TValue> Create(TValue[] result, bool isLoadMore)
    {
        return new PagedResult<TValue>(result, isLoadMore);
    }

    protected PagedResult(TValue[] result, bool isLoadMore)
    {
        Result = result;
        IsLoadMore = isLoadMore;
    }

    public TValue[] Result { get; set; }

    public bool IsLoadMore { get; set; }
}
