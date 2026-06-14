namespace PaymentDetailApi.Application.Common
{
    public record CursorPagedResponse<T>(List<T> Items, int? NextCursor);
}
