using IronXL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Win32;
using Piolax_WebApp.DTOs;
using Piolax_WebApp.Models;
using Piolax_WebApp.Services;
using Piolax_WebApp.Services.Impl;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Piolax_WebApp.Controllers
{
    public class EmpleadoController(IEmpleadoService service, ITokenService token, IEmpleadoAreaRolService empleadoAreaRolService, IRefreshTokensService refreshTokensService) : BaseApiController
    {
        private readonly IEmpleadoService _service = service;
        private readonly ITokenService _tokenService = token;
        private readonly IEmpleadoAreaRolService _empleadoAreaRolService = empleadoAreaRolService;
        private readonly IRefreshTokensService _refreshTokensService = refreshTokensService;



        [Authorize (Policy = "AdminOnly")]
        [HttpGet("Consultar")]
        public ActionResult<Empleado?> Consultar(string numNomina)
        {
            return _service.Consultar(numNomina).Result;
        }

        [Authorize (Policy = "AdminOnly")]
        [HttpGet("ConsultarTodos")]
        public async Task<ActionResult<IEnumerable<Empleado>>> ConsultarTodos()
        {
            return Ok(await _service.ConsultarTodos());
        }

        //Método para obtener el detalle de un empleado con sus áreas y roles asignados
        /*[Authorize(Policy = "AdminOnly")]
        [HttpGet("ConsultarTodosConDetalles")]
        public async Task<ActionResult<IEnumerable<EmpleadoAreaRol>>> ConsultarTodosConDetalles()
        {
           
        }*/

        [Authorize(Policy = "AdminOnly")]
        [HttpGet("{numNomina}/DetalleConAreasRoles")]
        public async Task<IActionResult> ObtenerDetalleConAreasRoles(string numNomina)
        {
            // Obtener el empleado
            var empleado = await _service.Consultar(numNomina);
            if (empleado == null)
            {
                return NotFound($"No se encontró el empleado con número de nómina: {numNomina}");
            }

            // Obtener las áreas y roles del empleado
            var areasRoles = await _empleadoAreaRolService.ObtenerAreasRolesPorEmpleado(numNomina);
            if (!areasRoles.Any())
            {
                return NotFound($"No se encontraron áreas o roles asignados para el empleado con número de nómina: {numNomina}");
            }

            // Crear el DTO de respuesta
            var empleadoConAreasRolesDTO = new EmpleadoAreaRolDTO
            {
                NumNomina = empleado.numNomina,
                NombreCompleto = $"{empleado.nombre} {empleado.apellidoPaterno} {empleado.apellidoMaterno}",
                AreasRoles = areasRoles.Select(ar => new AreaRolDTO
                {
                    Area = ar.Area.nombreArea,
                    Rol = ar.Rol.nombreRol
                }).ToList()
            };

            return Ok(empleadoConAreasRolesDTO);
        }

        [Authorize(Policy = "AdminOnly")]
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

            //verificar si el empleado ya tiene un rol asignado en el area

            if (await _empleadoAreaRolService.ValidarRolPorEmpleadoYArea(registro.numNomina, registro.idArea))
            {
                return BadRequest("El empleado ya tiene un rol asignado en el área seleccionada");
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

        [Authorize(Policy = "AdminOnly")]
        [HttpPost("AsignarAreaRol")]
        public async Task<ActionResult> AsignarAreaRol(string numNomina, int idArea, int idRol)
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
                await _empleadoAreaRolService.AsignarAreaRol(numNomina, idArea, idRol);
                return Ok("Área y rol asignados exitosamente al empleado.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al asignar área y rol al empleado: {ex.Message}");
            }
        }

        [Authorize(Policy = "AdminOnly")]
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
        }

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

        //Login con JWT de Acceso y Refresh
        [HttpPost("Login")]
        public async Task<ActionResult<EmpleadoDTO>> Login([FromBody] LoginDTO login)
        {
            if (!await _service.EmpleadoExiste(login.numNomina))
                return Unauthorized("El Empleado no existe" );

            var resultado = _service.EmpleadoExisteLogin(login);

            if (!resultado.esLoginExitoso)
                return Unauthorized("El Password es invalido" );

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

        [Authorize(Policy = "AdminOnly")]
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

        [Authorize(Policy = "AdminOnly")]
        [HttpDelete("EliminarAreaYRolEmpleado")]

        public async Task EliminarAreaYRol(string numNomina, int idArea, int idRol)
        {
            await _empleadoAreaRolService.EliminarAreaRol(numNomina, idArea, idRol);
        }

        [Authorize(Policy = "AdminOnly")]
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
            if (file == null || file.Length == 0)
            {
                return BadRequest("Por favor, sube un archivo Excel válido.");
            }

            var empleadosRegistrados = new List<string>();
            var empleadosNoRegistrados = new List<string>();

            try
            {
                using var stream = file.OpenReadStream();
                var workbook = WorkBook.Load(stream); // Carga el archivo Excel
                var worksheet = workbook.WorkSheets.First(); // Usa la primera hoja del archivo

                for (int row = 2; row <= worksheet.RowCount; row++) // Empieza en la fila 2 (asumiendo encabezados en la fila 1)
                {
                    try
                    {
                        var cell = worksheet[$"A{row}"];
                        if (cell == null || string.IsNullOrWhiteSpace(cell.StringValue))
                        {
                            empleadosNoRegistrados.Add($"Fila {row}: El número de nómina está vacío o no es válido.");
                            continue;
                        }
                        var numNomina = cell.StringValue.Trim();
                        var nombre = worksheet[$"B{row}"].StringValue.Trim();    // Columna B: Nombre
                        var apellidoPaterno = worksheet[$"C{row}"].StringValue.Trim(); // Columna C: Apellido paterno
                        var apellidoMaterno = worksheet[$"D{row}"].StringValue.Trim(); // Columna D: Apellido materno
                        var telefono = worksheet[$"E{row}"].StringValue.Trim();  // Columna E: Teléfono
                        var email = worksheet[$"F{row}"].StringValue.Trim();     // Columna F: Email
                        var fechaIngreso = DateOnly.FromDateTime(DateTime.Parse(worksheet[$"G{row}"].StringValue.Trim())); // Columna G: Fecha de ingreso
                        var password = worksheet[$"H{row}"].StringValue.Trim(); // Columna H: Password
                        var idStatusEmpleado = int.Parse(worksheet[$"I{row}"].StringValue.Trim()); // Columna I: ID del status del empleado
                        var idArea = int.Parse(worksheet[$"J{row}"].StringValue.Trim()); // Columna J: ID del area
                        var idRol = int.Parse(worksheet[$"K{row}"].StringValue.Trim());  // Columna K: ID del rol


                        //Validar si el empleado existe, seguido de validar si el empleado ya tiene un rol asignado en el area.
                        //Si existe el empleado y no tiene un rol asignado en el area, se procede a asignar el area y rol al empleado.

                        if (await _service.EmpleadoExiste(numNomina))
                        {

                            if (await _empleadoAreaRolService.ValidarRolPorEmpleadoYArea(numNomina, idArea))
                            {
                                empleadosNoRegistrados.Add($"Fila {row}: El empleado con número de nómina {numNomina} ya existe y cuenta con un rol en el area {idArea}.");
                            } else
                            {
                                await _empleadoAreaRolService.AsignarAreaRol(numNomina, idArea, idRol);
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

    }
}
