using Hotel.Inventory.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hotel.Inventory.Domain.Interfaces;

public interface IRoomRepository
{
    Task<Room?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Room?> GetByHotelAndRoomNumberAsync(string hotelId, string roomNumber, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Room>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Busca la primera habitación libre de un hotel/tipo dados. Es el
    /// método que usa el flujo del SAGA (CheckRoomAvailabilityConsumer)
    /// para resolver disponibilidad.
    /// </summary>
    Task<Room?> FindAvailableRoomAsync(string hotelId, string roomType, CancellationToken cancellationToken = default);

    Task AddAsync(Room room, CancellationToken cancellationToken = default);

    Task UpdateAsync(Room room, CancellationToken cancellationToken = default);

    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}