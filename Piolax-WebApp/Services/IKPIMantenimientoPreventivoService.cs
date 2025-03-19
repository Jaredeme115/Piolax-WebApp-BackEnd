namespace Piolax_WebApp.Services
{
    public interface IKPIMantenimientoPreventivoService 
    {
        Task CalcularYGuardarKPIs(DateTime inicio, DateTime fin);

    }
}
