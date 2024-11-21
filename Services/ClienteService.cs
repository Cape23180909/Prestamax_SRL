using Microsoft.EntityFrameworkCore;
using Prestamax_SRL.Data;
using Prestamax_SRL.Migrations;
using Prestamax_SRL.Models;
using System.Linq.Expressions;

namespace Prestamax_SRL.Services;

public class ClienteService
{
    private readonly ApplicationDbContext _contexto;

    public ClienteService(ApplicationDbContext contexto)
    {
        _contexto = contexto ?? throw new ArgumentNullException(nameof(contexto));
    }
    //Metodo Existe 
    public async Task<bool> Existe(int clienteid)
    {
        return await _contexto.Clientes.AnyAsync(c => c.ClienteId == clienteid);
    }
    //Metodo Insertar
    private async Task<bool> Insertar(Clientes cliente)
    {
        _contexto.Clientes.Add(cliente);
        return await _contexto.SaveChangesAsync() > 0;
    }
    //Metodo Modificar 
    private async Task<bool> Modificar(Clientes cliente)
    {
        _contexto.Update(cliente);
        return await _contexto
            .SaveChangesAsync() > 0;
    }
    //Metod Guardar 
    public async Task<bool> Guardar(Clientes cliente)
    {
        if (!await Existe(cliente.ClienteId))
            return await Insertar(cliente);
        else
            return await Modificar(cliente);
    }
    //Metodo Eliminar
    public async Task<bool> Eliminar(int id)
    {
        var eliminado = await _contexto.Clientes
            .Where(p => p.ClienteId == id)
            .ExecuteDeleteAsync();
        return eliminado > 0;
    }
    //Metodo Buscar 
    public async Task<Clientes?> Buscar(int id)
    {
        return await _contexto.Clientes
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.ClienteId == id);
    }
    //Metodo Listar 
    public async Task<List<Clientes>> Listar(Expression<Func<Clientes, bool>> criterio)
    {
        return await _contexto.Clientes
            .AsNoTracking()
            .Where(criterio)
            .ToListAsync();
    }
    //Metodo para obtener todos los clientes y poder hacer el dicho prestamo del mismo
    public async Task<List<Clientes>> ObtenerTodos()
    {
        return await _contexto.Clientes.ToListAsync();
    }

    public async Task<Clientes> ObtenerClientePorId(int id)
    {
        return await _contexto.Clientes.FirstOrDefaultAsync(c => c.ClienteId == id);
    }
    public async Task<List<Clientes>> ObtenerClientesConPrestamos()
    {
        return await _contexto.Clientes
            .Where(c => c.Prestamos.Any())
            .ToListAsync();
    }

    public async Task<bool> ExisteCedula(string cedula)
    {
        return await _contexto.Clientes.AnyAsync(c => c.Cedula == cedula);
    }

    public async Task<bool> ExisteTelefono(string telefono)
    {
        return await _contexto.Clientes.AnyAsync(c => c.Telefono == telefono);
    }
}