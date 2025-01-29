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
            // Validar que el DTO tiene datos válidos
            if (inventarioDTO == null)
                throw new ArgumentNullException(nameof(inventarioDTO), "El objeto InventarioDTO no puede ser nulo.");

            var item = GenerarItem(
                inventarioDTO.nombreProducto,
                inventarioDTO.idInventarioCategoria, //Eliminar
                inventarioDTO.numParte,
                inventarioDTO.idArea
            );

            inventarioDTO.item = item;

            // Generar el código QR
            string qrCodeText = inventarioDTO.numParte; // Puedes personalizar el contenido del código QR
            string qrCodeBase64 = GenerateQRCode(qrCodeText);
            inventarioDTO.codigoQR = qrCodeBase64;


            // Asignar valor a la propiedad precioInventarioTotal
            inventarioDTO.precioInventarioTotal = inventarioDTO.precioUnitario * inventarioDTO.cantidadActual;


            // Transformar el DTO a la entidad Inventario
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

                // Manejar el valor del estado desde el DTO (si es proporcionado) o usar un valor predeterminado
                EstatusInventario = string.IsNullOrWhiteSpace(inventarioDTO.EstatusInventario)
            ? EstatusInventario.Disponible // Valor predeterminado si no se especifica
            : Enum.TryParse<EstatusInventario>(inventarioDTO.EstatusInventario, true, out var status)
                ? status
                : throw new ArgumentException("El estado proporcionado no es válido.", nameof(inventarioDTO.EstatusInventario))
            };


            // Llamar al repositorio para registrar el inventario
            return await _repository.RegistrarInventario(inventario);
        }


        public async Task<Inventario> Modificar(int idRefaccion, InventarioDTO inventarioDTO)
        {
            var productoExistente = await _repository.ConsultarInventarioPorID(idRefaccion);

            if (productoExistente == null)
            {
                throw new Exception("El producto no existe");
            }


            // Generar el código QR
            string qrCodeText = inventarioDTO.numParte; ; // Puedes personalizar el contenido del código QR
            string qrCodeBase64 = GenerateQRCode(qrCodeText);
            inventarioDTO.codigoQR = qrCodeBase64;

            // Asignar valor a la propiedad precioInventarioTotal
            inventarioDTO.precioInventarioTotal = inventarioDTO.precioUnitario * inventarioDTO.cantidadActual;

            // Actualizar los campos del producto existente

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

            return await _repository.Modificar(idRefaccion, productoExistente);

        }

        public async Task ActualizarCantidadInventario(int idRefaccion, int cantidadADescontar)
        {
            await _repository.ActualizarCantidadInventario(idRefaccion, cantidadADescontar);
        }

        public async Task<int> ConsultarCantidadDisponible(int idRefaccion)
        {
            return await _repository.ConsultarCantidadDisponible(idRefaccion);
        }

        public async Task<Inventario> Eliminar(int idRefaccion)
        {
            return await _repository.Eliminar(idRefaccion);
        }

        public string GenerarItem(string nombreProducto, int idInventarioCategoria, string numParte, int idArea)
        {
            // Tomar las iniciales del nombre del producto (las primeras dos palabras)
            var iniciales = string.Join("", nombreProducto.Split(' ').Take(2).Select(palabra => palabra.Substring(0, 1).ToUpper()));

            // Construir el CURP concatenando los valores
            return $"{iniciales}-{idArea}-{idInventarioCategoria}-{numParte}";
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

    }
}
