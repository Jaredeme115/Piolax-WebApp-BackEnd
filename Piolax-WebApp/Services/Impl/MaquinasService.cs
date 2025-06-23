using Piolax_WebApp.DTOs;
using Piolax_WebApp.Models;
using Piolax_WebApp.Repositories;
using QRCoder;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using OfficeOpenXml;

namespace Piolax_WebApp.Services.Impl
{
    public class MaquinasService(IMaquinasRepository repository, IAreasRepository areasRepository) : IMaquinasService
    {
        private readonly IMaquinasRepository _repository = repository;
        private readonly IAreasRepository _areasRepository = areasRepository;

        public Task<IEnumerable<Maquinas>> ConsultarTodos()
        {
            return _repository.ConsultarTodos();
        }

        public Task<Maquinas> Consultar(int idMaquina)
        {
            return _repository.Consultar(idMaquina);
        }

        public async Task<Maquinas> Registro(MaquinaDTO maquina)
        {
            var maquinas = new Maquinas
            {
                nombreMaquina = maquina.descripcion,
                codigoQR = maquina.descripcion,
                idArea = maquina.idArea,
                maquinaActiva = maquina.maquinaActiva
            };

            var registrada = await _repository.Registro(maquinas);
            await _areasRepository.ActualizarContadorMaquinasActivas(maquinas.idArea);

            return registrada;
        }

        public async Task<Maquinas> Modificar(int idMaquina, MaquinaDTO maquina)
        {
            var maquinaExistente = await _repository.Consultar(idMaquina);

            if (maquinaExistente == null)
                return null;

            int idAreaAnterior = maquinaExistente.idArea;

            maquinaExistente.nombreMaquina = maquina.descripcion;
            maquinaExistente.codigoQR = maquina.descripcion;
            maquinaExistente.maquinaActiva = maquina.maquinaActiva;
            maquinaExistente.idArea = maquina.idArea;

            var modificada = await _repository.Modificar(idMaquina, maquinaExistente);

            if (idAreaAnterior != maquina.idArea)
            {
                await _areasRepository.ActualizarContadorMaquinasActivas(idAreaAnterior);
            }
            await _areasRepository.ActualizarContadorMaquinasActivas(maquina.idArea);

            return modificada;
        }



        public async Task<Maquinas> Eliminar(int idMaquina)
        {
            var maquina = await _repository.Consultar(idMaquina);
            if (maquina == null) return null;

            var eliminada = await _repository.Eliminar(idMaquina);
            await _areasRepository.ActualizarContadorMaquinasActivas(maquina.idArea);

            return eliminada;
        }

        public async Task<bool> CambiarEstado(int idMaquina, bool nuevaCondicion)
        {
            var maquina = await _repository.Consultar(idMaquina);
            if (maquina == null) return false;

            maquina.maquinaActiva = nuevaCondicion;
            await _repository.Modificar(maquina.idMaquina, maquina);
            await _areasRepository.ActualizarContadorMaquinasActivas(maquina.idArea);

            return true;
        }

        public Task<int> ContarMaquinasActivasPorArea(int idArea)
        {
            return _repository.ContarMaquinasActivasPorArea(idArea);
        }


        public async Task<bool> MaquinaExiste(int idMaquina)
        {
            return await _repository.MaquinaExiste(idMaquina);
        }

        public async Task<bool> MaquinaExisteRegistro(string nombreMaquina)
        {
            return await _repository.MaquinaExisteRegistro(nombreMaquina);
        }

        public async Task<IEnumerable<Maquinas>> ConsultarPorArea(int idArea)
        {
            return await _repository.ConsultarPorArea(idArea);
        }

        //Servicio para cargar empleados desde un archivo Excel
        public async Task<string> RegistrarMaquinasDesdeExcel(IFormFile filePath)
        {
            if (filePath == null || filePath.Length == 0)
                throw new ArgumentException("El archivo proporcionado es inválido o está vacío.");

            if (!filePath.FileName.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("El archivo debe tener formato .xlsx");

            // EPPlus requiere declarar el contexto de licencia
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            var errores = new List<string>();
            int maquinasCargadas = 0;
            int filasProcesadas = 0;

            try
            {
                using var stream = filePath.OpenReadStream();
                using var package = new ExcelPackage(stream);

                var workbook = package.Workbook;
                if (workbook.Worksheets.Count == 0)
                    throw new ApplicationException("El archivo Excel no contiene hojas.");

                // Obtengo la primera hoja
                var worksheet = workbook.Worksheets.First();

                // Determino cuántas filas hay con datos
                var rowCount = worksheet.Dimension?.End.Row ?? 0;

                for (int row = 2; row <= rowCount; row++)
                {
                    filasProcesadas++;
                    try
                    {
              
                        var descripcion = worksheet.Cells[row, 1].Text?.Trim();
                        var idAreaText = worksheet.Cells[row, 2].Text?.Trim();
                        var maquinaActiva = worksheet.Cells[row, 3].Text?.Trim().ToLower() == "si";

                        if (string.IsNullOrWhiteSpace(descripcion) ||
                            string.IsNullOrWhiteSpace(idAreaText))
                        {
                            errores.Add($"Fila {row}: Una o más celdas requeridas están vacías.");
                            continue;
                        }

                        if (!int.TryParse(idAreaText, out var idArea))
                        {
                            errores.Add($"Fila {row}: El ID de área '{idAreaText}' no es un número válido.");
                            continue;
                        }

                        var maquinaDTO = new MaquinaDTO
                        {
                            descripcion = descripcion,
                            idArea = idArea,
                            codigoQR = descripcion, // Generación de QR en tu servicio
                            maquinaActiva = maquinaActiva
                        };

                        if (await MaquinaExisteRegistro(maquinaDTO.descripcion))
                        {
                            errores.Add($"Fila {row}: La máquina '{maquinaDTO.descripcion}' ya está registrada.");
                            continue;
                        }

                        await Registro(maquinaDTO);
                        maquinasCargadas++;
                    }
                    catch (FormatException)
                    {
                        errores.Add($"Fila {row}: Formato inválido en una o más celdas.");
                    }
                    catch (Exception ex)
                    {
                        errores.Add($"Fila {row}: Error inesperado: {ex.Message}");
                    }
                }

                var resultado = $"{maquinasCargadas} máquina(s) cargadas correctamente de {filasProcesadas} fila(s) procesadas.";
                if (errores.Any())
                    resultado += $" Se encontraron errores en {errores.Count} fila(s): {string.Join("; ", errores)}";

                return resultado;
            }
            catch (Exception ex)
            {
                // Propago con detalle interno
                throw new ApplicationException("Error al procesar el archivo Excel.", ex);
            }
        }

        //Metodo para generar el codigo QR para cada maquina cuando sea necesario
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

    }
}
