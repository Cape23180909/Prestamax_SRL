using Microsoft.EntityFrameworkCore;
using Prestamax_SRL.Data;
using Prestamax_SRL.Models;
using System.Linq.Expressions;

namespace Prestamax_SRL.Services;

    public class PrestamoService
    {
    private readonly ApplicationDbContext _contexto;

    public PrestamoService (ApplicationDbContext contexto)
    {
        _contexto = contexto ?? throw new ArgumentNullException(nameof(contexto));
    }
    //Metodo Existe 
    public async Task<bool> Existe(int prestamoid)
    {
        return await _contexto.Prestamos.AnyAsync(p => p.PrestamosId == prestamoid);
    }
    //Metodo Insertar
    private async Task<bool> Insertar(Prestamos prestamo)
    {
        _contexto.Prestamos.Add(prestamo);
        return await _contexto.SaveChangesAsync() > 0;
    }
    //Metodo Modificar 
    private async Task<bool> Modificar(Prestamos prestamo)
    {
        _contexto.Update(prestamo);
        return await _contexto
            .SaveChangesAsync() > 0;
    }
    //Metod Guardar 
    public async Task<bool> Guardar(Prestamos prestamo)
    {
        if (!await Existe(prestamo.PrestamosId))
            return await Insertar(prestamo);
        else
            return await Modificar(prestamo);
    }
    //Metodo Eliminar
    public async Task<bool> Eliminar(int id)
    {
        var eliminado = await _contexto.Prestamos
            .Where(p => p.PrestamosId == id)
            .ExecuteDeleteAsync();
        return eliminado > 0;
    }
    //Metodo Buscar 
    public async Task<Prestamos?> Buscar(int id)
    {
        return await _contexto.Prestamos
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.PrestamosId == id);
    }
    //Metodo Listar 
    public async Task<List<Prestamos>> Listar(Expression<Func<Prestamos, bool>> criterio)
    {
        return await _contexto.Prestamos
            .AsNoTracking()
            .Where(criterio)
            .ToListAsync();
    }

    public async Task<List<Prestamos>> ListarPrestamosConClientes()
    {
        return await _contexto.Prestamos
            .Include(p => p.Cliente) 
            .ToListAsync();
    }
}