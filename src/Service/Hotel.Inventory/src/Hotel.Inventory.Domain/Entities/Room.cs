using Hotel.Inventory.Domain.Enums;
using Hotel.Inventory.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hotel.Inventory.Domain.Entities;

public class Room
{
    public Guid Id { get; private set; }
    public string HotelId { get; private set; } = string.Empty;
    public string RoomType { get; private set; } = string.Empty;
    public string RoomNumber { get; private set; } = string.Empty;
    public decimal PricePerNight { get; private set; }
    public RoomAvailability Availability { get; private set; } = default!;
    public RoomStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    private Room() { }

    public Room(string hotelId, string roomType, string roomNumber, decimal pricePerNight)
    {
        if (string.IsNullOrWhiteSpace(hotelId))
            throw new ArgumentException("El hotel (HotelId) es obligatorio.", nameof(hotelId));

        if (string.IsNullOrWhiteSpace(roomType))
            throw new ArgumentException("El tipo de habitación (RoomType) es obligatorio.", nameof(roomType));

        if (string.IsNullOrWhiteSpace(roomNumber))
            throw new ArgumentException("El número de habitación (RoomNumber) es obligatorio.", nameof(roomNumber));

        if (pricePerNight <= 0)
            throw new ArgumentOutOfRangeException(nameof(pricePerNight), "El precio por noche debe ser mayor a cero.");

        Id = Guid.NewGuid();
        HotelId = hotelId;
        RoomType = roomType;
        RoomNumber = roomNumber;
        PricePerNight = pricePerNight;
        Availability = RoomAvailability.Free();
        Status = RoomStatus.Active;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateInfo(string roomType, string roomNumber, decimal pricePerNight)
    {
        if (string.IsNullOrWhiteSpace(roomType))
            throw new ArgumentException("El tipo de habitación (RoomType) es obligatorio.", nameof(roomType));

        if (string.IsNullOrWhiteSpace(roomNumber))
            throw new ArgumentException("El número de habitación (RoomNumber) es obligatorio.", nameof(roomNumber));

        if (pricePerNight <= 0)
            throw new ArgumentOutOfRangeException(nameof(pricePerNight), "El precio por noche debe ser mayor a cero.");

        RoomType = roomType;
        RoomNumber = roomNumber;
        PricePerNight = pricePerNight;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Bloquea la habitación para una reserva (equivalente a ReserveStock).
    /// La propia RoomAvailability lanza InvalidOperationException si ya
    /// estaba reservada, por lo que dos solicitudes concurrentes sobre la
    /// misma instancia nunca dejan el agregado en un estado inconsistente.
    /// </summary>
    public void Reserve(Guid reservationId)
    {
        if (Status != RoomStatus.Active)
        {
            throw new InvalidOperationException(
                $"La habitación {RoomNumber} no está activa (estado actual: {Status}) y no puede reservarse.");
        }

        Availability = Availability.Reserve(reservationId);
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>Libera la habitación (compensación SAGA), equivalente a ReleaseStock.</summary>
    public void Release(Guid reservationId)
    {
        Availability = Availability.Release(reservationId);
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>Indica si la habitación puede asignarse a una nueva búsqueda de disponibilidad.</summary>
    public bool IsAvailableFor(string hotelId, string roomType) =>
        Status == RoomStatus.Active &&
        !Availability.IsReserved &&
        HotelId == hotelId &&
        RoomType == roomType;

    public void Discontinue() => Status = RoomStatus.Discontinued;
    public void Reactivate() => Status = RoomStatus.Active;
}
