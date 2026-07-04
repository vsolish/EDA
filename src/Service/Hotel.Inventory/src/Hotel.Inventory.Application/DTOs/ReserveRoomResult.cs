namespace Hotel.Inventory.Application.DTOs;

public record ReserveRoomResult(
    bool Success,
    Guid? RoomId,
    decimal Amount,
    string Currency,
    string? FailureReason);
