using IronXL;
using Piolax_WebApp.DTOs;
using Piolax_WebApp.Models;
using Piolax_WebApp.Repositories;
using Piolax_WebApp.Repositories.Impl;
using QRCoder;
using System.Drawing;
using System.Drawing.Imaging;

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
                var workbook = WorkBook.Load(filePath.OpenReadStream()); // Carga el archivo Excel
                var worksheet = workbook.WorkSheets.First(); // Obtiene la primera hoja del archivo

                for (int row = 2; row <= worksheet.RowCount; row++) // Itera desde la fila 2
                {
                    try
                    {
                        var inventarioDTO = new InventarioDTO
                        {
                            idArea = int.Parse(worksheet[$"A{row}"].StringValue),
                            idMaquina = int.Parse(worksheet[$"B{row}"].StringValue),
                            nombreProducto = worksheet[$"C{row}"].StringValue,
                            numParte = worksheet[$"D{row}"].StringValue,
                            descripcion = worksheet[$"E{row}"].StringValue,
                            proveedor = worksheet[$"F{row}"].StringValue,
                            cantidadMin = string.IsNullOrWhiteSpace(worksheet[$"G{row}"].StringValue) ? 1 : int.Parse(worksheet[$"G{row}"].StringValue),
                            cantidadMax = string.IsNullOrWhiteSpace(worksheet[$"H{row}"].StringValue) ? 1 : int.Parse(worksheet[$"G{row}"].StringValue),
                            ubicacion = worksheet[$"B{row}"].StringValue,
                            idInventarioCategoria = int.Parse(worksheet[$"C{row}"].StringValue),
                            cantidadActual = int.Parse(worksheet[$"D{row}"].StringValue),
                            piezaCritica = bool.Parse(worksheet[$"G{row}"].StringValue),
                            precioUnitario = float.Parse(worksheet[$"K{row}"].StringValue),
                            proceso = worksheet[$"L{row}"].StringValue,
                            fechaEntrega = DateTime.Parse(worksheet[$"O{row}"].StringValue),
                            inventarioActivoObsoleto = bool.Parse(worksheet[$"P{row}"].StringValue),
                            EstatusInventario = worksheet[$"Q{row}"].StringValue
                        };

                        // Verificar el contenido de # de parte
                        if (string.IsNullOrWhiteSpace(inventarioDTO.numParte))
                        {
                            inventarioDTO.numParte = "No contiene";
                        }

                        // Verificar el contenido de descripcion
                        if (string.IsNullOrWhiteSpace(inventarioDTO.descripcion))
                        {
                            inventarioDTO.descripcion = "No contiene";
                        }

                        // Verificar el contenido de proveedor
                        if (string.IsNullOrWhiteSpace(inventarioDTO.proveedor))
                        {
                            inventarioDTO.descripcion = "No contiene";
                        }

                        // Verificar si cantidadMin es 0 y asignar 1 en su lugar
                        if (inventarioDTO.cantidadMin == 0)
                        {
                            inventarioDTO.cantidadMin = 1;
                        }

                        // Verificar si cantidadMax es 0 y asignar # en su lugar
                        if (inventarioDTO.cantidadMax == 0)
                        {
                            inventarioDTO.cantidadMax = 1;
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

                        productos.Add(inventario);
                        productosCargados++;
                    }
                    catch (Exception ex)
                    {
                        // Agrega detalles del error para la fila actual
                        errores.Add($"Error en fila {row}: {ex.Message}");
                    }
                }

                // Reutiliza el método AddRangeAsync para insertar todos los productos
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
