using Hotel.Inventory.Domain.Entities;
using Hotel.Inventory.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hotel.Inventory.Infrastructure.Repositories;

public class RoomRepository(AppDbContext context) : IRoomRepository
{
    public async Task<Room?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await context.Rooms.FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

    public async Task<Room?> GetByHotelAndRoomNumberAsync(string hotelId, string roomNumber, CancellationToken cancellationToken = default)
        => await context.Rooms.FirstOrDefaultAsync(
            r => r.HotelId == hotelId && r.RoomNumber == roomNumber, cancellationToken);

    public async Task<IReadOnlyList<Room>> GetAllAsync(CancellationToken cancellationToken = default)
        => await context.Rooms.ToListAsync(cancellationToken);

    public async Task<Room?> FindAvailableRoomAsync(string hotelId, string roomType, CancellationToken cancellationToken = default)
        => await context.Rooms.FirstOrDefaultAsync(
            r => r.HotelId == hotelId && r.RoomType == roomType && !r.Availability.IsReserved,
            cancellationToken);

    public async Task AddAsync(Room room, CancellationToken cancellationToken = default)
    {
        await context.Rooms.AddAsync(room, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Room room, CancellationToken cancellationToken = default)
    {
        context.Rooms.Update(room);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var room = await GetByIdAsync(id, cancellationToken);
        if (room is not null)
        {
            context.Rooms.Remove(room);
            await context.SaveChangesAsync(cancellationToken);
        }
    }
}
