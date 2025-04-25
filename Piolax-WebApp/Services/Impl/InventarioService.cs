using IronXL;
using Piolax_WebApp.DTOs;
using Piolax_WebApp.Models;
using Piolax_WebApp.Repositories;
using Piolax_WebApp.Repositories.Impl;
using QRCoder;
using System.Drawing;
using System.Drawing.Imaging;
using OfficeOpenXml;
using Piolax_WebApp.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;


namespace Piolax_WebApp.Services.Impl
{
    public class InventarioService(IInventarioRepository repository, IMaquinasService maquinasService, IHubContext<AsignacionHub> hubContext) : IInventarioService
    {
        private readonly IInventarioRepository _repository = repository;
        private readonly IMaquinasService _maquinasService = maquinasService;
        private readonly IHubContext<AsignacionHub> _hubContext = hubContext;


        public async Task<Inventario> ConsultarInventarioPorCategoria(int idInventarioCategoria)
        {

            return await _repository.ConsultarInventarioPorCategoria(idInventarioCategoria);
        }


        //Metodo modificado para mostrar los detalles de la refaccion en base al nombre de la misma
        public async Task<InventarioDetalleDTO?> ConsultarRefaccionPorNombre(string nombreProducto)
        {
            return await _repository.ConsultarRefaccionPorNombre(nombreProducto);
        }

        public Task<IEnumerable<Inventario>> ConsultarTodoInventario()
        {
            return _repository.ConsultarTodoInventario();
        }

        public async Task<bool> ExisteProductoInventario(int idRefaccion)
        {
            return await _repository.ExisteProductoInventario(idRefaccion);
        }

        public async Task<bool> ExisteNumParte(string numParte)
        {
            return await _repository.ExisteNumParte(numParte);
        }

        public async Task<Inventario> ConsultarInventarioPorID(int idRefaccion)
        {
            return await _repository.ConsultarInventarioPorID(idRefaccion);
        }

        public async Task<IEnumerable<Inventario>> ConsultarTodosLosProductosPorCategoria(int idInventarioCategoria)
        {
            return await _repository.ConsultarTodosLosProductosPorCategoria(idInventarioCategoria);
        }


        public async Task<Inventario> RegistrarInventario(InventarioDTO inventarioDTO)
        {
            if (inventarioDTO == null)
            {
                throw new ArgumentNullException(nameof(inventarioDTO), "El objeto InventarioDTO no puede ser nulo.");
            }

            if (string.IsNullOrWhiteSpace(inventarioDTO.numParte))
            {
                throw new ArgumentException("El número de parte no puede estar vacío.");
            }

            // Verificar si el producto ya existe en la base de datos
            if (await _repository.ExisteNumParte(inventarioDTO.numParte))
            {
                throw new InvalidOperationException("El producto ya está registrado en el inventario.");
            }

            // Generar el campo `item`
            var item = GenerarItem(inventarioDTO.nombreProducto, inventarioDTO.numParte, inventarioDTO.idArea);
            inventarioDTO.item = item;

            // Generar el código QR si `numParte` es válido
            /*string qrCodeBase64 = !string.IsNullOrEmpty(inventarioDTO.numParte) ? GenerateQRCode(inventarioDTO.numParte) : null;
            inventarioDTO.codigoQR = qrCodeBase64;*/

            //Almacenar el código QR en la base de datos pero ya no es necesario porque se genera de forma dinamica
            inventarioDTO.codigoQR = inventarioDTO.numParte;

            // Calcular el precio total
            inventarioDTO.precioInventarioTotal = inventarioDTO.precioUnitario * inventarioDTO.cantidadActual;

            // Transformar DTO a entidad
            var inventario = new Inventario
            {
                item = inventarioDTO.item,
                descripcion = inventarioDTO.descripcion,
                ubicacion = inventarioDTO.ubicacion,
                idInventarioCategoria = inventarioDTO.idInventarioCategoria,
                cantidadActual = inventarioDTO.cantidadActual,
                cantidadMax = inventarioDTO.cantidadMax,
                cantidadMin = inventarioDTO.cantidadMin,
                piezaCritica = inventarioDTO.piezaCritica,
                nombreProducto = inventarioDTO.nombreProducto,
                numParte = inventarioDTO.numParte,
                proveedor = inventarioDTO.proveedor,
                precioUnitario = inventarioDTO.precioUnitario,
                precioInventarioTotal = inventarioDTO.precioInventarioTotal,
                codigoQR = inventarioDTO.codigoQR,
                proceso = inventarioDTO.proceso,
                idArea = inventarioDTO.idArea,
                idMaquina = inventarioDTO.idMaquina,
                fechaEntrega = inventarioDTO.fechaEntrega,
                inventarioActivoObsoleto = inventarioDTO.inventarioActivoObsoleto,
                EstatusInventario = Enum.TryParse<EstatusInventario>(inventarioDTO.EstatusInventario, true, out var status) ? status : EstatusInventario.Disponible
            };

            return await _repository.RegistrarInventario(inventario);
        }


        public async Task<InventarioDetalleDTO> Modificar(int idRefaccion, InventarioModificarDTO inventarioDTO)
        {
            var productoExistente = await _repository.ConsultarInventarioPorID(idRefaccion);
            if (productoExistente == null)
            {
                throw new InvalidOperationException("El producto no existe.");
            }

            // Obtener lista de máquinas del área actual
            var maquinasDisponibles = await _maquinasService.ConsultarPorArea(productoExistente.idArea);

            // Verificar que la máquina seleccionada pertenece al área
            if (!maquinasDisponibles.Any(m => m.idMaquina == inventarioDTO.idMaquina))
            {
                throw new InvalidOperationException("La máquina seleccionada no pertenece al área asignada.");
            }

            // Recalcular el precio total
            var nuevoPrecioInventarioTotal = inventarioDTO.precioUnitario * inventarioDTO.cantidadActual;

            // Mapear solo los campos modificables
            productoExistente.descripcion = inventarioDTO.descripcion;
            productoExistente.ubicacion = inventarioDTO.ubicacion;
            productoExistente.cantidadActual = inventarioDTO.cantidadActual;
            productoExistente.cantidadMax = inventarioDTO.cantidadMax;
            productoExistente.cantidadMin = inventarioDTO.cantidadMin;
            productoExistente.piezaCritica = inventarioDTO.piezaCritica;
            productoExistente.proveedor = inventarioDTO.proveedor;
            productoExistente.precioUnitario = inventarioDTO.precioUnitario;
            productoExistente.precioInventarioTotal = nuevoPrecioInventarioTotal;
            productoExistente.proceso = inventarioDTO.proceso;
            productoExistente.idMaquina = inventarioDTO.idMaquina;
            productoExistente.fechaEntrega = inventarioDTO.fechaEntrega;
            productoExistente.inventarioActivoObsoleto = inventarioDTO.inventarioActivoObsoleto;
            productoExistente.fechaActualizacion = DateTime.UtcNow;

            // Guardar cambios en el repositorio
            var refaccionModificada = await _repository.Modificar(productoExistente);

            // Notificar si el stock esta por debajo del mínimo
            if (refaccionModificada.cantidadActual <= refaccionModificada.cantidadMin && !refaccionModificada.bajoStockNotificado)
            {
                // enviar alerta
                await _hubContext.Clients.Group("Mantenimiento")
                    .SendAsync("LowStockAlert", new
                    {
                        refaccionModificada.idRefaccion,
                        refaccionModificada.nombreProducto,
                        refaccionModificada.cantidadActual,
                        refaccionModificada.cantidadMin
                    });

                // marcar para no notificar de nuevo inmediatamente
                refaccionModificada.bajoStockNotificado = true;
                await _repository.Modificar(productoExistente);
            } else if (refaccionModificada.bajoStockNotificado && refaccionModificada.cantidadActual > refaccionModificada.cantidadMin)
            {
                // Si repusieron por encima del mínimo, resetear flag para futuras alertas
                refaccionModificada.bajoStockNotificado = false;
                await _repository.Modificar(productoExistente);
            }

            // Retornar la refacción modificada en formato DTO
            return new InventarioDetalleDTO
            {
                idRefaccion = refaccionModificada.idRefaccion,
                descripcion = refaccionModificada.descripcion,
                ubicacion = refaccionModificada.ubicacion,
                idInventarioCategoria = refaccionModificada.idInventarioCategoria,
                nombreInventarioCategoria = refaccionModificada.InventarioCategorias?.nombreInventarioCategoria ?? "Sin categoría",
                cantidadActual = refaccionModificada.cantidadActual,
                cantidadMax = refaccionModificada.cantidadMax,
                cantidadMin = refaccionModificada.cantidadMin,
                piezaCritica = refaccionModificada.piezaCritica,
                nombreProducto = refaccionModificada.nombreProducto,
                numParte = refaccionModificada.numParte,
                proveedor = refaccionModificada.proveedor,
                precioUnitario = refaccionModificada.precioUnitario,
                precioInventarioTotal = refaccionModificada.precioInventarioTotal,
                codigoQR = refaccionModificada.codigoQR,
                proceso = refaccionModificada.proceso,
                idArea = refaccionModificada.idArea,
                nombreArea = refaccionModificada.Areas?.nombreArea ?? "Sin área asignada",
                idMaquina = refaccionModificada.idMaquina,
                nombreMaquina = refaccionModificada.Maquinas?.nombreMaquina ?? "Sin máquina asignada",
                fechaEntrega = refaccionModificada.fechaEntrega,
                inventarioActivoObsoleto = refaccionModificada.inventarioActivoObsoleto,
                item = refaccionModificada.item,
                fechaActualizacion = refaccionModificada.fechaActualizacion,
                EstatusInventario = refaccionModificada.EstatusInventario.ToString()
            };
        }

        public async Task ActualizarCantidadInventario(int idRefaccion, int cantidadADescontar)
        {
            await _repository.ActualizarCantidadInventario(idRefaccion, cantidadADescontar);
        }

        public async Task DescontarInventario(int idRefaccion, int cantidadADescontar)
        {
            await _repository.ActualizarCantidadInventario(idRefaccion, cantidadADescontar);
        }

        public async Task<int> ConsultarCantidadDisponible(int idRefaccion)
        {
            return await _repository.ConsultarCantidadDisponible(idRefaccion);
        }

        public async Task<bool> Eliminar(int idRefaccion)
        {
            if (idRefaccion <= 0)
            {
                throw new ArgumentException("ID de refacción no válido.");
            }

            var eliminado = await _repository.Eliminar(idRefaccion);

            if (eliminado == null)
            {
                throw new InvalidOperationException("No se encontró la refacción con el ID especificado.");
            }

            return true;
        }

        public async Task<InventarioDetalleDTO?> ObtenerRefaccionDetalle(int idInventario)
        {
            return await _repository.ConsultarRefaccionDetalle(idInventario);
        }


        public async Task<IEnumerable<Inventario>> ConsultarRefaccionesPorFiltros(bool? piezaCritica, bool? inventarioActivoObsoleto, string? proceso)
        {
            return await _repository.ConsultarRefaccionesPorFiltros(piezaCritica, inventarioActivoObsoleto, proceso);
        }

        public async Task<IEnumerable<string>> ConsultarNombresRefaccionesPorCategoria(int idCategoria)
        {
            return await _repository.ConsultarNombresRefaccionesPorCategoria(idCategoria);
        }


        // Método para registrar inventario desde un archivo Excel
        public async Task<string> RegistrarInventarioDesdeExcel(IFormFile filePath)
        {
            if (filePath == null || filePath.Length == 0)
                throw new ArgumentException("El archivo es inválido.");

            var errores = new List<string>(); // Para registrar errores de procesamiento
            int productosCargados = 0;
            var productos = new List<Inventario>();

            try
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial; // Habilitar EPPlus para uso no comercial

                using (var package = new ExcelPackage(filePath.OpenReadStream()))
                {
                    var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                    if (worksheet == null)
                        return "El archivo Excel no contiene hojas.";

                    int totalRows = worksheet.Dimension.Rows;

                    for (int row = 2; row <= totalRows; row++) // Itera desde la fila 2 (omitir encabezado)
                    {
                        try
                        {
                            var inventarioDTO = new InventarioDTO
                            {
                                idArea = int.Parse(worksheet.Cells[row, 1].Text),
                                idMaquina = int.Parse(worksheet.Cells[row, 2].Text),
                                nombreProducto = worksheet.Cells[row, 3].Text,
                                numParte = worksheet.Cells[row, 4].Text,
                                descripcion = worksheet.Cells[row, 5].Text,
                                proveedor = worksheet.Cells[row, 6].Text,
                                cantidadMin = string.IsNullOrWhiteSpace(worksheet.Cells[row, 7].Text) ? 1 : int.Parse(worksheet.Cells[row, 7].Text),
                                cantidadMax = string.IsNullOrWhiteSpace(worksheet.Cells[row, 8].Text) ? Math.Max(1, int.Parse(worksheet.Cells[row, 9].Text) + 1) : int.Parse(worksheet.Cells[row, 8].Text),
                                cantidadActual = string.IsNullOrWhiteSpace(worksheet.Cells[row, 9].Text) ? 0 : int.Parse(worksheet.Cells[row, 9].Text),
                                precioUnitario = string.IsNullOrWhiteSpace(worksheet.Cells[row, 10].Text) ? 0.0f : float.Parse(worksheet.Cells[row, 10].Text),
                                proceso = worksheet.Cells[row, 11].Text,
                                ubicacion = worksheet.Cells[row, 12].Text,
                                piezaCritica = !string.IsNullOrWhiteSpace(worksheet.Cells[row, 13].Text) && bool.Parse(worksheet.Cells[row, 13].Text),
                                inventarioActivoObsoleto = string.IsNullOrWhiteSpace(worksheet.Cells[row, 14].Text) || bool.Parse(worksheet.Cells[row, 14].Text),
                                fechaEntrega = string.IsNullOrWhiteSpace(worksheet.Cells[row, 15].Text) ? DateTime.Today.AddMonths(1) : DateTime.Parse(worksheet.Cells[row, 15].Text),
                            };

                            // Validaciones y asignaciones por defecto
                            inventarioDTO.numParte = string.IsNullOrWhiteSpace(inventarioDTO.numParte) ? "No contiene" : inventarioDTO.numParte;
                            inventarioDTO.descripcion = string.IsNullOrWhiteSpace(inventarioDTO.descripcion) ? "No contiene" : inventarioDTO.descripcion;
                            inventarioDTO.proveedor = string.IsNullOrWhiteSpace(inventarioDTO.proveedor) ? "No contiene" : inventarioDTO.proveedor;
                            inventarioDTO.proceso = string.IsNullOrWhiteSpace(inventarioDTO.proceso) ? "No contiene" : inventarioDTO.proceso;
                            inventarioDTO.ubicacion = string.IsNullOrWhiteSpace(inventarioDTO.ubicacion) ? "No contiene" : inventarioDTO.ubicacion;

                            // Ajuste de valores mínimos
                            inventarioDTO.cantidadMin = Math.Max(1, inventarioDTO.cantidadMin);
                            inventarioDTO.cantidadMax = Math.Max(1, inventarioDTO.cantidadMax);

                            // Generación de campos adicionales
                            inventarioDTO.idInventarioCategoria = inventarioDTO.idArea;
                            inventarioDTO.EstatusInventario = "Disponible";

                            // Generar el campo `item`
                            inventarioDTO.item = GenerarItem(inventarioDTO.nombreProducto, inventarioDTO.numParte, inventarioDTO.idArea);

                            // Generar código QR
                            //inventarioDTO.codigoQR = !string.IsNullOrEmpty(inventarioDTO.numParte) ? GenerateQRCode(inventarioDTO.numParte) : null;

                            //Almacenar el código QR en la base de datos pero ya no es necesario porque se genera de forma dinamica
                            inventarioDTO.codigoQR = inventarioDTO.numParte;

                            // Calcular el precio total
                            inventarioDTO.precioInventarioTotal = inventarioDTO.precioUnitario * inventarioDTO.cantidadActual;

                            // Transformar DTO a entidad
                            var inventario = new Inventario
                            {
                                item = inventarioDTO.item,
                                descripcion = inventarioDTO.descripcion,
                                ubicacion = inventarioDTO.ubicacion,
                                idInventarioCategoria = inventarioDTO.idInventarioCategoria,
                                cantidadActual = inventarioDTO.cantidadActual,
                                cantidadMax = inventarioDTO.cantidadMax,
                                cantidadMin = inventarioDTO.cantidadMin,
                                piezaCritica = inventarioDTO.piezaCritica,
                                nombreProducto = inventarioDTO.nombreProducto,
                                numParte = inventarioDTO.numParte,
                                proveedor = inventarioDTO.proveedor,
                                precioUnitario = inventarioDTO.precioUnitario,
                                precioInventarioTotal = inventarioDTO.precioInventarioTotal,
                                codigoQR = inventarioDTO.codigoQR,
                                proceso = inventarioDTO.proceso,
                                idArea = inventarioDTO.idArea,
                                idMaquina = inventarioDTO.idMaquina,
                                fechaEntrega = inventarioDTO.fechaEntrega,
                                inventarioActivoObsoleto = inventarioDTO.inventarioActivoObsoleto,
                                EstatusInventario = Enum.TryParse<EstatusInventario>(inventarioDTO.EstatusInventario, true, out var status) ? status : EstatusInventario.Disponible
                            };

                            productos.Add(inventario);
                            productosCargados++;
                        }
                        catch (Exception ex)
                        {
                            errores.Add($"Error en fila {row}: {ex.Message}");
                        }
                    }
                }

                // Guardar en base de datos
                await _repository.AddRangeAsync(productos);

                var resultado = $"{productosCargados} productos cargados correctamente.";
                if (errores.Any())
                {
                    resultado += $" Se encontraron errores en {errores.Count} filas: {string.Join("; ", errores)}";
                }
                return resultado;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error al procesar el archivo Excel.", ex);
            }
        }

        /*public async Task<byte[]> ExportarInventarioExcel()
        {

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial; // Habilitar EPPlus para uso no comercial

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Inventario");

                var inventario = await _repository.ConsultarTodoInventario();

                // 🔹 Definir encabezados
                var headers = new string[] { "ID Refacción", "Nombre", "Area", "Maquina", "Cantidad Actual", "Stock Mínimo", "Stock Máximo", "Precio Unitario", "Precio Total", "Ubicación", "Proveedor" };

                for (int i = 0; i < headers.Length; i++)
                {
                    worksheet.Cells[1, i + 1].Value = headers[i];
                    worksheet.Cells[1, i + 1].Style.Font.Bold = true;
                }

                // 🔹 Llenar los datos del inventario
                int row = 2;
                foreach (var item in inventario)
                {
                    worksheet.Cells[row, 1].Value = item.idRefaccion;
                    worksheet.Cells[row, 2].Value = item.nombreProducto;
                    worksheet.Cells[row, 3].Value = item.Areas?.nombreArea ?? "N/A";
                    worksheet.Cells[row, 4].Value = item.Maquinas?.nombreMaquina ?? "N/A";
                    worksheet.Cells[row, 5].Value = item.cantidadActual;
                    worksheet.Cells[row, 6].Value = item.cantidadMin;
                    worksheet.Cells[row, 7].Value = item.cantidadMax;
                    worksheet.Cells[row, 8].Value = item.precioUnitario;
                    worksheet.Cells[row, 9].Value = item.precioInventarioTotal;
                    worksheet.Cells[row, 10].Value = item.ubicacion;
                    worksheet.Cells[row, 11].Value = item.proveedor;
                    row++;
                }

                // 🔹 Ajustar tamaño de columnas automáticamente
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                return package.GetAsByteArray(); // Retorna los bytes del archivo Excel
            }
        }*/

        // Funcionalidad para generar el código QR
        public byte[] GenerateQRCodeBytes(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                throw new ArgumentException("El texto para generar el código QR no puede estar vacío.");

            using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
            {
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);
                using (QRCode qrCode = new QRCode(qrCodeData))
                {
                    using (Bitmap qrCodeImage = qrCode.GetGraphic(20))
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            qrCodeImage.Save(ms, ImageFormat.Png);
                            return ms.ToArray(); // Devuelve los bytes de la imagen
                        }
                    }
                }
            }
        }


        public string GenerarItem(string nombreProducto, string numParte, int idArea)
        {
            // Tomar las iniciales del nombre del producto (las primeras dos palabras)
            var iniciales = string.Join("", nombreProducto.Split(' ').Take(2).Select(palabra => palabra.Substring(0, 1).ToUpper()));

            // Construir el CURP concatenando los valores
            return $"{iniciales}-{idArea}-{numParte}";
        }

    }
}
