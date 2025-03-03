using IronXL;
using Piolax_WebApp.DTOs;
using Piolax_WebApp.Models;
using Piolax_WebApp.Repositories;
using Piolax_WebApp.Repositories.Impl;
using QRCoder;
using System.Drawing;
using System.Drawing.Imaging;
using OfficeOpenXml;


namespace Piolax_WebApp.Services.Impl
{
    public class InventarioService(IInventarioRepository repository) : IInventarioService
    {
        private readonly IInventarioRepository _repository = repository;


        public async Task<Inventario> ConsultarInventarioPorCategoria(int idInventarioCategoria)
        {

            return await _repository.ConsultarInventarioPorCategoria(idInventarioCategoria);
        }

        public async Task<Inventario> ConsultarInventarioPorNombre(string nombreProducto)
        {
            return await _repository.ConsultarInventarioPorNombre(nombreProducto);
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
            string qrCodeBase64 = !string.IsNullOrEmpty(inventarioDTO.numParte) ? GenerateQRCode(inventarioDTO.numParte) : null;
            inventarioDTO.codigoQR = qrCodeBase64;

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


        public async Task<Inventario> Modificar(int idRefaccion, InventarioDTO inventarioDTO)
        {
            var productoExistente = await _repository.ConsultarInventarioPorID(idRefaccion);
            if (productoExistente == null)
            {
                throw new InvalidOperationException("El producto no existe.");
            }

            // Si cambia el número de parte, generar nuevo código QR
            if (productoExistente.numParte != inventarioDTO.numParte)
            {
                string qrCodeBase64 = GenerateQRCode(inventarioDTO.numParte);
                inventarioDTO.codigoQR = qrCodeBase64;
            }
            else
            {
                inventarioDTO.codigoQR = productoExistente.codigoQR; // Mantener el QR actual
            }

            // Recalcular el precio total
            inventarioDTO.precioInventarioTotal = inventarioDTO.precioUnitario * inventarioDTO.cantidadActual;

            // Mapear los cambios
            productoExistente.descripcion = inventarioDTO.descripcion;
            productoExistente.ubicacion = inventarioDTO.ubicacion;
            productoExistente.idInventarioCategoria = inventarioDTO.idInventarioCategoria;
            productoExistente.cantidadActual = inventarioDTO.cantidadActual;
            productoExistente.cantidadMax = inventarioDTO.cantidadMax;
            productoExistente.cantidadMin = inventarioDTO.cantidadMin;
            productoExistente.piezaCritica = inventarioDTO.piezaCritica;
            productoExistente.nombreProducto = inventarioDTO.nombreProducto;
            productoExistente.numParte = inventarioDTO.numParte;
            productoExistente.proveedor = inventarioDTO.proveedor;
            productoExistente.precioUnitario = inventarioDTO.precioUnitario;
            productoExistente.precioInventarioTotal = inventarioDTO.precioInventarioTotal;
            productoExistente.proceso = inventarioDTO.proceso;
            productoExistente.idMaquina = inventarioDTO.idMaquina;
            productoExistente.fechaEntrega = inventarioDTO.fechaEntrega;
            productoExistente.inventarioActivoObsoleto = inventarioDTO.inventarioActivoObsoleto;

            return await _repository.Modificar(productoExistente);

        }

        public async Task ActualizarCantidadInventario(int idRefaccion, int cantidadADescontar)
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

        public async Task<IEnumerable<InventarioDetalleDTO>> ObtenerInventarioConDetalles()
        {
            var inventario = await _repository.ConsultarInventarioConDetalles();

            var inventarioDTOs = inventario.Select(i => new InventarioDetalleDTO
            {
                descripcion = i.descripcion,
                ubicacion = i.ubicacion,
                idInventarioCategoria = i.idInventarioCategoria,
                nombreInventarioCategoria = i.InventarioCategorias?.nombreInventarioCategoria ?? "Sin categoría",
                cantidadActual = i.cantidadActual,
                cantidadMax = i.cantidadMax,
                cantidadMin = i.cantidadMin,
                piezaCritica = i.piezaCritica,
                nombreProducto = i.nombreProducto,
                numParte = i.numParte,
                proveedor = i.proveedor,
                precioUnitario = i.precioUnitario,
                precioInventarioTotal = i.precioInventarioTotal,
                codigoQR = i.codigoQR,
                proceso = i.proceso,
                idArea = i.idArea,
                nombreArea = i.Areas?.nombreArea ?? "Sin área asignada",
                idMaquina = i.idMaquina,
                nombreMaquina = i.Maquinas?.nombreMaquina ?? "Sin máquina asignada",
                fechaEntrega = i.fechaEntrega,
                inventarioActivoObsoleto = i.inventarioActivoObsoleto,
                item = i.item,
                fechaActualizacion = i.fechaActualizacion,
                EstatusInventario = i.EstatusInventario.ToString()
            }).ToList();

            return inventarioDTOs;
        }



        public async Task<IEnumerable<Inventario>> ConsultarRefaccionesPorFiltros(bool? piezaCritica, bool? inventarioActivoObsoleto)
        {
            return await _repository.ConsultarRefaccionesPorFiltros(piezaCritica, inventarioActivoObsoleto);
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
                            inventarioDTO.codigoQR = !string.IsNullOrEmpty(inventarioDTO.numParte) ? GenerateQRCode(inventarioDTO.numParte) : null;

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

        // Funcionalidad para generar el código QR
        private string GenerateQRCode(string text)
        {
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
                            byte[] byteImage = ms.ToArray();
                            return Convert.ToBase64String(byteImage);
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
