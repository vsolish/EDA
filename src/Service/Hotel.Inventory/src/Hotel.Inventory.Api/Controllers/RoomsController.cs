using Hotel.Inventory.Application.Commands.CreateRoom;
using Hotel.Inventory.Application.Commands.DeleteRoom;
using Hotel.Inventory.Application.Commands.DiscontinueRoom;
using Hotel.Inventory.Application.Commands.ReactivateRoom;
using Hotel.Inventory.Application.Commands.UpdateRoom;
using Hotel.Inventory.Application.DTOs;
using Hotel.Inventory.Domain.Interfaces;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Hotel.Inventory.Api.Controllers;


/// <summary>
/// Expone la administración de habitaciones. Las lecturas van
/// directamente contra IRoomRepository (no hay reglas de negocio que
/// aplicar al leer); las escrituras pasan por MediatR hacia los
/// comandos que ya usan los métodos de comportamiento de Room
/// (UpdateInfo, Discontinue, Reactivate), incluyendo la validación de
/// FluentValidation vía ValidationBehavior.
///
/// El bloqueo/liberación de habitaciones (Reserve/Release) NO se expone
/// aquí: esas transiciones sólo las dispara el SAGA de reservas a
/// través de CheckRoomAvailabilityConsumer/ReleaseRoomConsumer, para
/// evitar que un cliente HTTP deje el inventario inconsistente con el
/// estado real de una reserva.
/// </summary>

[ApiController]
[Route("api/[controller]")]
public class RoomsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IRoomRepository _repository;

    public RoomsController(IMediator mediator, IRoomRepository repository)
    {
        _mediator = mediator;
        _repository = repository;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var rooms = await _repository.GetAllAsync(cancellationToken);
        return Ok(rooms.Adapt<List<RoomDto>>());
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var room = await _repository.GetByIdAsync(id, cancellationToken);
        return room is null ? NotFound() : Ok(room.Adapt<RoomDto>());
    }

    [HttpGet("by-number")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByHotelAndRoomNumber(
        [FromQuery] string hotelId, [FromQuery] string roomNumber, CancellationToken cancellationToken)
    {
        var room = await _repository.GetByHotelAndRoomNumberAsync(hotelId, roomNumber, cancellationToken);
        return room is null ? NotFound() : Ok(room.Adapt<RoomDto>());
    }

    /// <summary>
    /// Consulta de disponibilidad (misma búsqueda que usa el SAGA en
    /// CheckRoomAvailabilityConsumer), útil para que un front-end
    /// verifique disponibilidad antes de que el cliente confirme la
    /// reserva.
    /// </summary>
    [HttpGet("availability")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> FindAvailable(
        [FromQuery] string hotelId, [FromQuery] string roomType, CancellationToken cancellationToken)
    {
        var room = await _repository.FindAvailableRoomAsync(hotelId, roomType, cancellationToken);

        return room is null
            ? NotFound(new { message = $"No hay habitaciones '{roomType}' disponibles en el hotel {hotelId}." })
            : Ok(room.Adapt<RoomDto>());
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateRoomRequest request, CancellationToken cancellationToken)
    {
        var room = await _mediator.Send(new CreateRoomCommand(request), cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = room.Id }, room);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateRoomRequest request, CancellationToken cancellationToken)
    {
        var room = await _mediator.Send(new UpdateRoomCommand(id, request), cancellationToken);
        return Ok(room);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteRoomCommand(id), cancellationToken);
        return NoContent();
    }

    [HttpPost("{id:guid}/discontinue")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Discontinue(Guid id, CancellationToken cancellationToken)
    {
        var room = await _mediator.Send(new DiscontinueRoomCommand(id), cancellationToken);
        return Ok(room);
    }

    [HttpPost("{id:guid}/reactivate")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Reactivate(Guid id, CancellationToken cancellationToken)
    {
        var room = await _mediator.Send(new ReactivateRoomCommand(id), cancellationToken);
        return Ok(room);
    }
}
