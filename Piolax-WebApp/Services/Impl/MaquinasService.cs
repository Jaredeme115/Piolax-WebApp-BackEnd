using IronXL;
using Piolax_WebApp.DTOs;
using Piolax_WebApp.Models;
using Piolax_WebApp.Repositories;
using QRCoder;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Piolax_WebApp.Services.Impl
{
    public class MaquinasService(IMaquinasRepository repository) : IMaquinasService
    {
        private readonly IMaquinasRepository _repository = repository;

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
                idArea = maquina.idArea
            };

            return await _repository.Registro(maquinas);
        }

        public async Task<Maquinas> Modificar(int idMaquina, MaquinaDTO maquina)
        {
            var maquinaExistente = await _repository.Consultar(idMaquina);

            if (maquinaExistente == null)
                return null; // Devuelve null si la maquina no existe

            // Actualizamos los datos de la maquina
            maquinaExistente.nombreMaquina = maquina.descripcion;
            maquinaExistente.codigoQR = maquina.descripcion; // Actualiza el texto del QR

            return await _repository.Modificar(idMaquina, maquinaExistente);
        }

        public async Task<Maquinas> Eliminar(int idMaquina)
        {
            return await _repository.Eliminar(idMaquina);
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

            var errores = new List<string>();
            int maquinasCargadas = 0;
            int filasProcesadas = 0;

            try
            {
                if (!filePath.FileName.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase))
                    throw new ArgumentException("El archivo debe tener formato .xlsx");

                var workbook = WorkBook.Load(filePath.OpenReadStream());
                if (workbook.WorkSheets.Count == 0)
                    throw new ApplicationException("El archivo Excel no contiene hojas.");

                var worksheet = workbook.WorkSheets[1];

                for (int row = 2; row <= worksheet.RowCount; row++)
                {
                    filasProcesadas++;
                    try
                    {
                        if (string.IsNullOrWhiteSpace(worksheet[$"A{row}"].StringValue) ||
                            string.IsNullOrWhiteSpace(worksheet[$"B{row}"].StringValue))
                        {
                            errores.Add($"Fila {row}: Una o más celdas requeridas están vacías.");
                            continue;
                        }

                        var maquinaDTO = new MaquinaDTO
                        {
                            idArea = int.Parse(worksheet[$"A{row}"].StringValue),
                            descripcion = worksheet[$"B{row}"].StringValue,
                            codigoQR = worksheet[$"B{row}"].StringValue // Genera el QR desde el servicio
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
