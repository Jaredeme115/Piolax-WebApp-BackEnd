﻿using Piolax_WebApp.DTOs;
using Piolax_WebApp.Models;

namespace Piolax_WebApp.Services
{
    public interface IInventarioService
    {
        Task<Inventario> RegistrarInventario(InventarioDTO inventarioDTO);
        Task<InventarioDetalleDTO> Modificar(int idRefaccion, InventarioModificarDTO inventarioModificarDTO);
        Task<bool> Eliminar(int idRefaccion);
        Task<IEnumerable<Inventario>> ConsultarTodoInventario();

        //Task<Inventario> ConsultarInventarioConDetalles(int idRefaccion);

        //Metodo modificado para mostrar los detalles de la refaccion en base al nombre de la misma
        Task<InventarioDetalleDTO?> ConsultarRefaccionPorNombre(string nombreProducto);

        Task<Inventario> ConsultarInventarioPorCategoria(int idInventarioCategoria);
        Task<Inventario> ConsultarInventarioPorID(int idRefaccion);
        Task<IEnumerable<Inventario>> ConsultarTodosLosProductosPorCategoria(int idInventarioCategoria);
        Task<bool> ExisteProductoInventario(int idRefaccion);
        Task<bool> ExisteNumParte(string numParte);
        Task ActualizarCantidadInventario(int idRefaccion, int cantidadADescontar);
        Task DescontarInventario(int idRefaccion, int cantidadADescontar);
        Task<int> ConsultarCantidadDisponible(int idRefaccion);
        Task<IEnumerable<Inventario>> ConsultarRefaccionesPorFiltros(bool? piezaCritica, bool? inventarioActivoObsoleto, string? proceso);

        //Consultar Inventario con Detalles
        Task<InventarioDetalleDTO?> ObtenerRefaccionDetalle(int idInventario);

        Task<IEnumerable<string>> ConsultarNombresRefaccionesPorCategoria(int idCategoria);

        Task<string> RegistrarInventarioDesdeExcel(IFormFile filePath);

        //Task<byte[]> ExportarInventarioExcel();

        //Descargar QR
        byte[] GenerateQRCodeBytes(string text);

    }
}
