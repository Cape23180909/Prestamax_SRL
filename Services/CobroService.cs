using Microsoft.EntityFrameworkCore;
using Prestamax_SRL.Data;
using Prestamax_SRL.Models;
using System.Linq.Expressions;

namespace Prestamax_SRL.Services;
public class CobroService
{
    private readonly ApplicationDbContext _contexto;

    public CobroService(ApplicationDbContext contexto)
    {
        _contexto = contexto ?? throw new ArgumentNullException(nameof(contexto));
    }

    // Método Existe 
    public async Task<bool> Existe(int cobroId)
    {
        return await _contexto.Cobros.AnyAsync(c => c.CobroId == cobroId);
    }

    // Método Insertar
    private async Task<bool> Insertar(Cobros cobro)
    {
        _contexto.Cobros.Add(cobro);
        return await _contexto.SaveChangesAsync() > 0;
    }

    // Método Modificar 
    private async Task<bool> Modificar(Cobros cobro)
    {
        _contexto.Cobros.Update(cobro);
        return await _contexto.SaveChangesAsync() > 0;
    }

    // Método Guardar 
    public async Task<bool> Guardar(Cobros cobro)
    {
        //try
        //{
        if (!await Existe(cobro.CobroId))
            return await Insertar(cobro);
        else
            return await Modificar(cobro);
        //}
        //catch (DbUpdateException ex)
        //{
        //    // Log the error or notify the error, this could be an integration with a logging service.
        //    throw new InvalidOperationException("Error al guardar el cobro en la base de datos.", ex);
        //}
    }


    // Método Eliminar
    public async Task<bool> Eliminar(int id)
    {
        var cobro = await _contexto.Cobros.FindAsync(id);
        if (cobro == null) return false;

        _contexto.Cobros.Remove(cobro);
        return await _contexto.SaveChangesAsync() > 0;
    }

    // Método Buscar 
    public async Task<Cobros?> Buscar(int id)
    {
        return await _contexto.Cobros
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.CobroId == id);
    }

    // Método Listar 
    public async Task<List<Cobros>> Listar(Expression<Func<Cobros, bool>> criterio)
    {
        return await _contexto.Cobros
            .AsNoTracking()
            .Where(criterio)
            .ToListAsync();
    }

    public async Task GuardarCobro(Cobros cobro)
    {
        _contexto.Cobros.Add(cobro);
        await _contexto.SaveChangesAsync();
    }

    public async Task ActualizarPrestamo(Prestamos prestamo)
    {
        var prestamoExistente = await _contexto.Prestamos.FindAsync(prestamo.PrestamosId);
        if (prestamoExistente != null)
        {
            // Actualizar el saldo u otras propiedades necesarias
            prestamoExistente.Saldo = prestamo.Saldo;
            prestamoExistente.MontoTotalPagar = prestamo.MontoTotalPagar;

            // Guarda los cambios
            _contexto.Prestamos.Update(prestamoExistente);
            await _contexto.SaveChangesAsync();
        }
    }
    public async Task<List<Cobros>> ObtenerCobrosPorCliente(int clienteId)
    {
        return await _contexto.Cobros
            .Where(c => c.ClienteId == clienteId)  // Filtrar por el ID del cliente
            .Include(c => c.Cliente)               // Incluir la información del cliente
            .Include(c => c.Prestamo)              // Incluir información del préstamo asociado
            .ToListAsync();                        // Convertir a lista asincrónicamente
    }

    public List<Cobros> ListarConFiltro(Expression<Func<Cobros, bool>> filtro)
    {
        return _contexto.Cobros.Where(filtro).ToList();
    }
}