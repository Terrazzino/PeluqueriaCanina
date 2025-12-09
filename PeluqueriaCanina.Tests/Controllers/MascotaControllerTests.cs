using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

using PeluqueriaCanina.Controllers;
using PeluqueriaCanina.Data;
using PeluqueriaCanina.Models.ClasesDeCliente;
using PeluqueriaCanina.Services;

namespace PeluqueriaCanina.Tests.Controllers
{
    public class MascotaControllerTests
    {
        [Fact]
        public void Crear_Post_DebeAsignarClienteId_AgregarMascota_YRedirigir()
        {
            // Arrange
            var mascotita = new Mascota
            {
                Nombre = "Firulais",
                Raza = "Caniche",
                Peso = 4.5
            };

            // Contexto InMemory
            var options = new DbContextOptionsBuilder<ContextoAcqua>()
                .UseInMemoryDatabase("DB_Test_Mascotas")
                .Options;

            var contexto = new ContextoAcqua(options);

            var usuarioMock = new Mock<IUsuarioActualService>();

            var controller = new MascotaController(contexto, usuarioMock.Object);

            var claims = new[] { new Claim("UsuarioId", "99") };
            var identity = new ClaimsIdentity(claims, "test");
            var principal = new ClaimsPrincipal(identity);

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal }
            };

            // Act
            var result = controller.Crear(mascotita);

            // Assert
            Assert.Equal(99, mascotita.ClienteId);
            Assert.Equal(1, contexto.Mascotas.Count());

            var mascotaEnBD = contexto.Mascotas.First();
            Assert.Equal("Firulais", mascotaEnBD.Nombre);

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
        }
    }
}
