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
    //Metodo Existe 
    public async Task<bool> Existe(int cobroid)
    {
        return await _contexto.Cobros.AnyAsync(c => c.CobroId == cobroid);
    }
    //Metodo Insertar
    private async Task<bool> Insertar(Cobros cobro)
    {
        _contexto.Cobros.Add(cobro);
        return await _contexto.SaveChangesAsync() > 0;
    }
    //Metodo Modificar 
    private async Task<bool> Modificar(Cobros cobro)
    {
        _contexto.Update(cobro);
        return await _contexto
            .SaveChangesAsync() > 0;
    }
    //Metod Guardar 
    public async Task<bool> Guardar(Cobros cobro)
    {
        if (!await Existe(cobro.CobroId))
            return await Insertar(cobro);
        else
            return await Modificar(cobro);
    }
    //Metodo Eliminar
    public async Task<bool> Eliminar(int id)
    {
        var eliminado = await _contexto.Cobros
            .Where(c => c.CobroId == id)
            .ExecuteDeleteAsync();
        return eliminado > 0;
    }
    //Metodo Buscar 
    public async Task<Cobros?> Buscar(int id)
    {
        return await _contexto.Cobros
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.CobroId == id);
    }
    //Metodo Listar 
    public async Task<List<Cobros>> Listar(Expression<Func<Cobros, bool>> criterio)
    {
        return await _contexto.Cobros
            .AsNoTracking()
            .Where(criterio)
            .ToListAsync();
    }

    public async Task<List<Clientes>> ObtenerTodos()
    {
        return await _contexto.Clientes.ToListAsync();
    }

    public async Task<List<Prestamos>> ObtenerTodoPrestamo()
    {
        return await _contexto.Prestamos.ToListAsync();
    }
}