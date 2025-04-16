using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using OfficeOpenXml;
using Piolax_WebApp.DTOs;
using Piolax_WebApp.Models;                   
using Piolax_WebApp.Services;
using Piolax_WebApp.Services.Impl;
using System.IdentityModel.Tokens.Jwt;
using System.Net.NetworkInformation;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Piolax_WebApp.Controllers
{
    public class EmpleadoController(IEmpleadoService service, ITokenService token, IEmpleadoAreaRolService empleadoAreaRolService, IRefreshTokensService refreshTokensService) : BaseApiController
    {
        private readonly IEmpleadoService _service = service;
        private readonly ITokenService _tokenService = token;
        private readonly IEmpleadoAreaRolService _empleadoAreaRolService = empleadoAreaRolService;
        private readonly IRefreshTokensService _refreshTokensService = refreshTokensService;


        [Authorize]
        [HttpGet("ListadoEmpleados")]
        public async Task<ActionResult<List<EmpleadoAreaRolDTO>>> ConsultarTodosConDetalles()
        {
            var empleados = await _service.ConsultarTodos();

            var usuariosConAreasRoles = new List<EmpleadoAreaRolDTO>();

            foreach (var u in empleados)
            {
                var areasRoles = await _empleadoAreaRolService.ObtenerAreasRolesPorEmpleado(u.numNomina);
                var areaPrincipal = areasRoles.FirstOrDefault(ar => ar.esAreaPrincipal);
                var areasSecundarias = areasRoles.Where(ar => !ar.esAreaPrincipal).ToList();

                var empleadoAreaRolDTO = new EmpleadoAreaRolDTO
                {
                    numNomina = u.numNomina,
                    nombre = u.nombre,
                    apellidoPaterno = u.apellidoPaterno,
                    apellidoMaterno = u.apellidoMaterno,
                    telefono = u.telefono,
                    fechaIngreso = u.fechaIngreso,
                    email = u.email,
                    areaPrincipal = areaPrincipal != null ? new AreaRolDTO { Area = areaPrincipal.Area.nombreArea, Rol = areaPrincipal.Rol.nombreRol } : null,
                    areasSecundarias = areasSecundarias.Select(ar => new AreaRolDTO { Area = ar.Area.nombreArea, Rol = ar.Rol.nombreRol }).ToList()
                };

                usuariosConAreasRoles.Add(empleadoAreaRolDTO);
            }

            return Ok(usuariosConAreasRoles);
        }

        //[Authorize]
        [HttpGet("ConsultarEmpleadoConDetalles/{numNomina}")]
        public async Task<ActionResult<EmpleadoInfoDTO>> ConsultarEmpleadoConDetalles(string numNomina)
        {
            var empleadoDetalles = await _service.ConsultarEmpleadoConDetalles(numNomina);
            if (empleadoDetalles == null)
            {
                return NotFound();
            }
            return Ok(empleadoDetalles);
        }

        //[Authorize(Policy = "AdminOnly")]
        [HttpGet("Consultar")]
        public ActionResult<Empleado?> Consultar(string numNomina)
        {
            return _service.Consultar(numNomina).Result;
        }

        //[Authorize(Policy = "AdminOnly")]
        [HttpGet("ConsultarTodos")]
        public async Task<ActionResult<IEnumerable<Empleado>>> ConsultarTodos()
        {
            return Ok(await _service.ConsultarTodos());
        }


        //[Authorize(Policy = "AdminOnly")] Este metodo no debe llevar autorización debido a que debe permitir a un empleado registrarse
        [HttpPost("Registro")]
        public async Task<ActionResult<Empleado>> Registro([FromBody] RegistroDTO registro)
        {
            if (await _service.EmpleadoExiste(registro.numNomina))
            {
                return BadRequest("El numero de nomina ya esta registrado");

            }

            // Asignar el valor 1 al idStatusEmpleado si no se proporciona
            if (registro.idStatusEmpleado == 0)
            {
                registro.idStatusEmpleado = 1;
            }

            // Verificar si el empleado ya tiene un rol asignado en el area

            if (await _empleadoAreaRolService.ValidarRolPorEmpleadoYArea(registro.numNomina, registro.idArea))
            {
                return BadRequest("El empleado ya tiene un rol asignado en el área seleccionada");
            }

            if (await _empleadoAreaRolService.TieneAreaPrincipal(registro.numNomina))
            {
                return BadRequest("El empleado ya tiene un área principal asignada");
            }
            else
            {
                registro.esAreaPrincipal = true;
            }

            // Registrar el empleado junto con área y rol
            try
            {
                await _empleadoAreaRolService.RegistrarEmpleadoConAreaYRol(registro);

                return Ok(new { message = "Empleado registrado exitosamente con área y rol asignados." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Error al registrar empleado: {ex.Message}" });
            }
        }

        //[Authorize(Policy = "AdminOnly")]
        [HttpPost("AsignarAreaRol")]
        public async Task<ActionResult> AsignarAreaRol([FromBody] string numNomina, int idArea, int idRol, bool esAreaPrincipal)
        {
            try
            {
                // Verifica si el empleado existe
                if (!await _service.EmpleadoExiste(numNomina))
                {
                    return NotFound("El empleado no existe");
                }

                //verificar si el empleado ya tiene un rol asignado en el area

                if (await _empleadoAreaRolService.ValidarRolPorEmpleadoYArea(numNomina, idArea))
                {
                    return BadRequest("El empleado ya tiene un rol asignado en el área seleccionada");
                }

                // Asigna el área y rol al empleado
                await _empleadoAreaRolService.AsignarAreaRol(numNomina, idArea, idRol, esAreaPrincipal);
                return Ok("Área y rol asignados exitosamente al empleado.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al asignar área y rol al empleado: {ex.Message}");
            }
        }

        //[Authorize(Policy = "AdminOnly")]
        [HttpPut("ModificarEmpleadoConAreaYRol/{numNomina}")]
        public async Task<IActionResult> ModificarEmpleadoAreaRol(string numNomina, [FromBody] RegistroDTO registro)
        {
            try
            {
                var empleadoExistente = await _service.Consultar(numNomina);
                if (empleadoExistente == null)
                {
                    return NotFound(new { mensaje = "El empleado no existe." });
                }

                // 🔹 Llamar directamente al Service sin manejar lógica aquí
                await _empleadoAreaRolService.ModificarEmpleadoAreaRol(numNomina, registro);

                return Ok(new { mensaje = "Empleado modificado exitosamente.", numNomina = numNomina });
            }
            catch (DbUpdateException dbEx)
            {
                return StatusCode(500, new { mensaje = "Error al guardar los cambios en la base de datos.", error = dbEx.InnerException?.Message ?? dbEx.Message });
            }
            catch (InvalidOperationException invEx)
            {
                return BadRequest(new { mensaje = "Operación no válida.", error = invEx.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al modificar el empleado.", error = ex.InnerException?.Message ?? ex.Message });
            }
        }


        /*[Authorize(Policy = "AdminOnly")]
        [HttpPut("ModificarEmpleadoConAreaYRol/{numNomina}")]
        public async Task<ActionResult> ModificarEmpleadoAreaRol(string numNomina, RegistroDTO registro)
        {
            try
            {
                // Verifica si el empleado existe
                if (!await _service.EmpleadoExiste(numNomina))
                {
                    return NotFound("El empleado no existe");
                }

                // Modifica el empleado y asigna el área y rol
                await _empleadoAreaRolService.ModificarEmpleadoAreaRol(numNomina, registro);
                return Ok("Empleado modificado exitosamente junto con su área y rol.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al modificar el empleado: {ex.Message}");
            }
        }*/

        [Authorize(Policy = "AdminOnly")]
        [HttpDelete("{numNomina}")]
        public async Task<ActionResult<Empleado>> Eliminar(string numNomina)
        {
            if (!await _service.EmpleadoExiste(numNomina))
            {
                return NotFound("El empleado no existe");
            }

            return Ok(await _service.Eliminar(numNomina));
        }

        //Login con JWT de Acceso y Refresh (Tampoco debe de llevar authorize)
        [HttpPost("Login")]
        public async Task<ActionResult<EmpleadoDTO>> Login([FromBody] LoginDTO login)
        {
            if (!await _service.EmpleadoExiste(login.numNomina))
                return Unauthorized("El Empleado no existe");

            var resultado = _service.EmpleadoExisteLogin(login);

            if (!resultado.esLoginExitoso)
                return Unauthorized("El Password es invalido");

            var token = _tokenService.CrearToken(resultado.empleado);
            var refreshToken = await _refreshTokensService.GenerateRefreshToken(resultado.empleado.idEmpleado);

            var empleadoDTO = new EmpleadoDTO
            {
                numNomina = resultado.empleado.numNomina,
                token = token,
                refreshToken = refreshToken.token
            };

            return Ok(empleadoDTO);
        }

        //[Authorize(Policy = "AdminOnly")]
        [HttpGet("ConsultarPorStatus/{idStatusEmpleado}")]
        public async Task<ActionResult<IEnumerable<Empleado>>> ConsultarPorStatus(int idStatusEmpleado)
        {
            var empleados = await _service.ConsultarPorStatus(idStatusEmpleado);
            if (empleados == null || !empleados.Any())
            {
                return NotFound("No se encontraron empleados con el status especificado.");
            }

            return Ok(empleados);
        }

        //[Authorize(Policy = "AdminOnly")]
        [HttpDelete("EliminarAreaYRolEmpleado")]

        public async Task EliminarAreaYRol(string numNomina, int idArea, int idRol)
        {
            await _empleadoAreaRolService.EliminarAreaRol(numNomina, idArea, idRol);
        }


        [HttpPost("RefreshToken")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDTO refreshTokenDTO)
        {
            // Verificar si el refresh token existe y no está expirado
            var refreshToken = await _refreshTokensService.GetRefreshToken(refreshTokenDTO.refreshToken);
            if (refreshToken == null || refreshToken.expiresAt <= DateTime.Now)
            {
                return Unauthorized("Refresh token inválido o expirado");
            }

            // Obtener al empleado asociado al refresh token
            var empleado = await _service.ConsultarPorId(refreshToken.idEmpleado);
            if (empleado == null)
            {
                return Unauthorized("Empleado no encontrado");
            }

            // Generar un nuevo token JWT y un nuevo refresh token
            var newToken = _tokenService.CrearToken(empleado);
            var newRefreshToken = await _refreshTokensService.GenerateRefreshToken(empleado.idEmpleado);

            // Marcar el refresh token anterior como revocado
            await _refreshTokensService.RevokeRefreshToken(refreshTokenDTO.refreshToken);

            // Retornar el nuevo token y refresh token
            return Ok(new
            {
                token = newToken,
                refreshToken = newRefreshToken.token
            });
        }

        //Método para registrar empleados desde un archivo Excel
        [Authorize(Policy = "AdminOnly")]
        [HttpPost("RegistrarDesdeExcel")]
        public async Task<IActionResult> RegistrarDesdeExcel(IFormFile file)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            if (file == null || file.Length == 0)
            {
                return BadRequest("Por favor, sube un archivo Excel válido.");
            }

            var empleadosRegistrados = new List<string>();
            var empleadosNoRegistrados = new List<string>();

            try
            {
                using var stream = file.OpenReadStream();
                using var package = new ExcelPackage(stream); // Carga el archivo Excel usando EPPlus
                var worksheet = package.Workbook.Worksheets[0]; // Usa la primera hoja del archivo

                int rowCount = worksheet.Dimension.Rows; // Obtiene el número de filas en la hoja
                for (int row = 2; row <= rowCount; row++) // Empieza en la fila 2 (asumiendo encabezados en la fila 1)
                {
                    try
                    {
                        var numNomina = worksheet.Cells[row, 1]?.Text?.Trim(); // Columna A: Número de nómina
                        if (string.IsNullOrWhiteSpace(numNomina))
                        {
                            empleadosNoRegistrados.Add($"Fila {row}: El número de nómina está vacío o no es válido.");
                            continue;
                        }
                        var nombre = worksheet.Cells[row, 2]?.Text?.Trim(); // Columna B: Nombre
                        if (string.IsNullOrWhiteSpace(nombre))
                        {
                            empleadosNoRegistrados.Add($"Fila {row}: El nombre está vacío o no es válido.");
                            continue;
                        }
                        var apellidoPaterno = worksheet.Cells[row, 3]?.Text?.Trim(); // Columna C: Apellido paterno
                        if (string.IsNullOrWhiteSpace(apellidoPaterno))
                        {
                            empleadosNoRegistrados.Add($"Fila {row}: El apellido paterno está vacío o no es válido.");
                            continue;
                        }
                        var apellidoMaterno = worksheet.Cells[row, 4]?.Text?.Trim(); // Columna D: Apellido materno
                        if (string.IsNullOrWhiteSpace(apellidoMaterno))
                        {
                            empleadosNoRegistrados.Add($"Fila {row}: El apellido materno está vacío o no es válido.");
                            continue;
                        }
                        var telefono = worksheet.Cells[row, 5]?.Text?.Trim(); // Columna E: Teléfono
                        if (string.IsNullOrWhiteSpace(telefono))
                        {
                            empleadosNoRegistrados.Add($"Fila {row}: El telefono está vacío o no es válido.");
                            continue;
                        }
                        var email = worksheet.Cells[row, 6]?.Text?.Trim(); // Columna F: Email
                        if (string.IsNullOrWhiteSpace(email))
                        {
                            empleadosNoRegistrados.Add($"Fila {row}: El email está vacío o no es válido.");
                            continue;
                        }
                        var fechaIngreso = DateOnly.FromDateTime(DateTime.Parse(worksheet.Cells[row, 7]?.Text?.Trim())); // Columna G: Fecha de ingreso
                        var password = worksheet.Cells[row, 8]?.Text?.Trim(); // Columna H: Password
                        if (string.IsNullOrWhiteSpace(password))
                        {
                            empleadosNoRegistrados.Add($"Fila {row}: El password está vacío o no es válido.");
                            continue;
                        }
                        var idStatusEmpleado = int.Parse(worksheet.Cells[row, 9]?.Text?.Trim() ?? "0"); // Columna I: ID del status
                        var idArea = int.Parse(worksheet.Cells[row, 10]?.Text?.Trim() ?? "0"); // Columna J: ID del área
                        var idRol = int.Parse(worksheet.Cells[row, 11]?.Text?.Trim() ?? "0"); // Columna K: ID del rol
                        var esAreaPrincipal = true;

                        // Validar si el empleado ya tiene un área principal asignada
                        if (await _empleadoAreaRolService.TieneAreaPrincipal(numNomina))
                        {
                            empleadosNoRegistrados.Add($"Fila {row}: El empleado con número de nómina {numNomina} ya tiene un área principal asignada.");
                            continue;
                        }

                        //Validar si el empleado existe, seguido de validar si el empleado ya tiene un rol asignado en el area.
                        //Si existe el empleado y no tiene un rol asignado en el area, se procede a asignar el area y rol al empleado.

                        if (await _service.EmpleadoExiste(numNomina))
                        {

                            if (await _empleadoAreaRolService.ValidarRolPorEmpleadoYArea(numNomina, idArea))
                            {
                                empleadosNoRegistrados.Add($"Fila {row}: El empleado con número de nómina {numNomina} ya existe y cuenta con un rol en el area {idArea}.");
                            }
                            else
                            {
                                await _empleadoAreaRolService.AsignarAreaRol(numNomina, idArea, idRol, esAreaPrincipal);
                                empleadosRegistrados.Add($"Fila {row}: Área y rol asignados correctamente al empleado con número de nómina {numNomina}.");
                            }

                        }
                        else
                        {

                            // Registrar empleado
                            var registroDTO = new RegistroDTO
                            {
                                numNomina = numNomina,
                                nombre = nombre,
                                apellidoPaterno = apellidoPaterno,
                                apellidoMaterno = apellidoMaterno,
                                telefono = telefono,
                                email = email,
                                fechaIngreso = fechaIngreso,
                                password = password,
                                idArea = idArea,
                                idRol = idRol,
                                idStatusEmpleado = 1 // Activo por defecto
                            };

                            await _empleadoAreaRolService.RegistrarEmpleadoConAreaYRol(registroDTO);
                            empleadosRegistrados.Add($"Fila {row}: Empleado {nombre} {apellidoPaterno} registrado correctamente.");
                        }
                    }
                    catch (Exception ex)
                    {
                        empleadosNoRegistrados.Add($"Fila {row}: Error al procesar el registro. Detalles: {ex.Message}");
                    }
                }

                return Ok(new
                {
                    Registrados = empleadosRegistrados,
                    NoRegistrados = empleadosNoRegistrados
                });
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al procesar el archivo Excel: {ex.Message}");
            }
        }

        //Método para consultar lista de empleados en base a un area

        [HttpGet("ObtenerEmpleadosPorArea/{idArea}")]
        public async Task<ActionResult<IEnumerable<EmpleadoNombreCompletoDTO>>> ObtenerEmpleadosPorArea(int idArea)
        {
            var empleados = await _empleadoAreaRolService.ObtenerEmpleadosPorArea(idArea);

            if (empleados == null || !empleados.Any())
            {
                return NotFound("No se encontraron empleados en el área especificada.");
            }

            // Transformar Empleado a EmpleadoNombreCompletoDTO
            var empleadosDTO = empleados.Select(emp => new EmpleadoNombreCompletoDTO
            {
                idEmpleado = emp.idEmpleado,
                nombre = emp.nombre,
                apellidoPaterno = emp.apellidoPaterno,
                apellidoMaterno = emp.apellidoMaterno,
                nombreCompleto = $"{emp.nombre} {emp.apellidoPaterno} {emp.apellidoMaterno}".Trim()
            }).ToList();

            return Ok(empleadosDTO);
        }
        [Authorize]
        [HttpGet("ObtenerIdEmpleadoDesdeToken")]
        public IActionResult ObtenerIdEmpleadoDesdeToken()
        {
            var idEmpleadoClaim = User.Claims.FirstOrDefault(c => c.Type == "idEmpleado");
            if (idEmpleadoClaim == null)
            {
                return Unauthorized("No se encontró el idEmpleado en el token.");
            }

            return Ok(int.Parse(idEmpleadoClaim.Value));
        }



    }
}


//------------------------ Métodos A Borrar ------------------------

/*[Authorize (Policy = "AdminOnly")]
[HttpGet("Consultar")]
public ActionResult<Empleado?> Consultar(string numNomina)
{
    return _service.Consultar(numNomina).Result;
}*/

/*[Authorize (Policy = "AdminOnly")]
[HttpGet("ConsultarTodos")]
public async Task<ActionResult<IEnumerable<Empleado>>> ConsultarTodos()
{
    return Ok(await _service.ConsultarTodos());
}*/