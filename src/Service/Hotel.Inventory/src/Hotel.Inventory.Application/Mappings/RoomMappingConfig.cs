using Hotel.Inventory.Application.DTOs;
using Hotel.Inventory.Domain.Entities;
using Mapster;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hotel.Inventory.Application.Mappings;

/// <summary>
/// Configura explícitamente el mapeo Room -> RoomDto. Es necesario
/// porque Availability es un Value Object anidado (IsReserved,
/// ReservationId) y Mapster no lo aplana automáticamente a propiedades
/// de primer nivel en el DTO sin esta configuración.
/// </summary>
public class RoomMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Room, RoomDto>()
            .Map(dest => dest.IsReserved, src => src.Availability.IsReserved)
            .Map(dest => dest.ReservationId, src => src.Availability.ReservationId)
            .Map(dest => dest.Status, src => src.Status.ToString());
    }
}
