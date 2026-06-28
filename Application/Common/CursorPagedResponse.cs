namespace PaymentDetailApi.Application.Common
{
    public record CursorPagedResponse<T>(List<T> Items, Guid? NextCursor);
}
