using Hotel.Inventory.Domain.Entities;

namespace Hotel.Inventory.Api.Data;

public static class RoomData
{
    public static List<Room> GetRooms() =>
    [
        new Room("HTL-001", "SINGLE", "S01", 120m),
        new Room("HTL-001", "SINGLE", "S02", 120m),
        new Room("HTL-001", "DOUBLE", "D01", 180m),
        new Room("HTL-001", "DOUBLE", "D02", 180m),
        new Room("HTL-001", "SUITE", "ST01", 350m),
        new Room("HTL-002", "SINGLE", "S01", 130m),
        new Room("HTL-002", "SINGLE", "S02", 130m),
        new Room("HTL-002", "DOUBLE", "D01", 190m),
        new Room("HTL-002", "DOUBLE", "D02", 190m),
        new Room("HTL-002", "SUITE", "ST01", 380m)
    ];
}
