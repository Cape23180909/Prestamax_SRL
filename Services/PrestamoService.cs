using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Prestamax_SRL.Data;
using Prestamax_SRL.Models;
using System.Drawing;
using System.Linq.Expressions;

namespace Prestamax_SRL.Services
{
    public class PrestamoService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbFactory;
        private readonly IHubContext<Hub> _hubContext; // Inyección de IHubContext

        // Constructor que inicializa _dbFactory y _hubContext
        public PrestamoService(IDbContextFactory<ApplicationDbContext> dbFactory, IHubContext<Hub> hubContext)
        {
            _dbFactory = dbFactory ?? throw new ArgumentNullException(nameof(dbFactory));
            _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext)); // Inicializa _hubContext
        }

        public async Task ActualizarPrestamosDespuesCobro()
        {
            await _hubContext.Clients.All.SendAsync("ActualizarPrestamos");
        }

        // Método Existe
        public async Task<bool> Existe(int prestamoid)
        {
            await using var contexto = await _dbFactory.CreateDbContextAsync(); // Usa el DbFactory
            return await contexto.Prestamos.AnyAsync(p => p.PrestamosId == prestamoid);
        }

        // Método Insertar
        private async Task<bool> Insertar(Prestamos prestamo)
        {
            await using var contexto = await _dbFactory.CreateDbContextAsync(); // Usa el DbFactory
            prestamo.Estado ??= "Activo";
            contexto.Prestamos.Add(prestamo);
            return await contexto.SaveChangesAsync() > 0;
        }

        // Método Modificar
        private async Task<bool> Modificar(Prestamos prestamo)
        {
            await using var contexto = await _dbFactory.CreateDbContextAsync(); // Usa el DbFactory
            contexto.Update(prestamo);
            return await contexto.SaveChangesAsync() > 0;
        }

        // Método Guardar
        public async Task<bool> Guardar(Prestamos prestamo)
        {
            // Enviar mensaje a todos los clientes conectados utilizando IHubContext
            await _hubContext.Clients.All.SendAsync("ActualizarPrestamos");

            if (!await Existe(prestamo.PrestamosId))
                return await Insertar(prestamo);
            else
                return await Modificar(prestamo);
        }

        public async Task<bool> GuardarCobro(Cobros cobro)
        {
            await using var contexto = await _dbFactory.CreateDbContextAsync();
            contexto.Cobros.Add(cobro);
            return await contexto.SaveChangesAsync() > 0;
        }

        // Método Eliminar
        public async Task<bool> Eliminar(int id)
        {
            await using var contexto = await _dbFactory.CreateDbContextAsync(); // Usa el DbFactory
            var eliminado = await contexto.Prestamos
                .Where(p => p.PrestamosId == id)
                .ExecuteDeleteAsync();
            return eliminado > 0;
        }

        // Método Buscar
        public async Task<Prestamos?> Buscar(int id)
        {
            await using var contexto = await _dbFactory.CreateDbContextAsync(); // Usa el DbFactory
            return await contexto.Prestamos
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.PrestamosId == id);
        }

        // Método Listar
        public async Task<List<Prestamos>> Listar(Expression<Func<Prestamos, bool>> criterio)
        {
            await using var contexto = await _dbFactory.CreateDbContextAsync(); // Usa el DbFactory
            return await contexto.Prestamos
                .AsNoTracking()
                .Where(criterio)
                .ToListAsync();
        }

        // Método ObtenerPrestamosPorCliente
        public async Task<List<Prestamos>> ObtenerPrestamosPorCliente(int clienteId)
        {
            await using var contexto = await _dbFactory.CreateDbContextAsync();
            return await contexto.Prestamos
                .Where(p => p.ClienteId == clienteId)
                .ToListAsync();
        }

        // Método ListarPrestamosConClientes
        public async Task<List<Prestamos>> ListarPrestamosConClientes()
        {
            await using var contexto = await _dbFactory.CreateDbContextAsync(); // Usa el DbFactory
            return await contexto.Prestamos
                .Include(p => p.Cliente)
                .ToListAsync();
        }

        public async Task<Prestamos> ObtenerPrestamoPorId(int prestamoId)
        {
            await using var contexto = await _dbFactory.CreateDbContextAsync();
            // Realiza la consulta a la base de datos o sistema de almacenamiento para obtener el préstamo.
            var prestamo = await contexto.Prestamos
                .Include(p => p.Cliente) // Incluye el cliente relacionado, si es necesario
                .FirstOrDefaultAsync(p => p.PrestamosId == prestamoId);

            return prestamo;
        }

        public async Task<bool> ActualizarPrestamo(Prestamos prestamo)
        {
            await using var contexto = await _dbFactory.CreateDbContextAsync();
            contexto.Prestamos.Update(prestamo);

            // Enviar notificación a través del hub si es necesario
            await _hubContext.Clients.All.SendAsync("ActualizarPrestamos");

            return await contexto.SaveChangesAsync() > 0;
        }

        // Método RealizarPago
        public async Task<bool> RegistrarCobro(int prestamoId, decimal montoPagado)
        {
            await using var contexto = await _dbFactory.CreateDbContextAsync();
            var prestamo = await contexto.Prestamos.FindAsync(prestamoId);

            if (prestamo == null)
            {
                return false; // Si el préstamo no existe, retorna false
            }

            // Actualiza el saldo
            prestamo.Saldo -= montoPagado;

            // Verifica que el saldo no sea negativo
            if (prestamo.Saldo < 0)
            {
                prestamo.Saldo = 0; // Evita saldo negativo
            }

            await contexto.SaveChangesAsync();
            return true;
        }

        public async Task<List<Prestamos>> ObtenerPrestamosPorClienteId(int clienteId)
        {
            await using var contexto = await _dbFactory.CreateDbContextAsync();
            // Buscar los préstamos del cliente con el ID proporcionado
            var prestamos = await contexto.Prestamos
                .Where(p => p.ClienteId == clienteId) // Filtra por el clienteId
                .ToListAsync(); // Obtiene la lista de préstamos

            return prestamos;
        }

    }
}