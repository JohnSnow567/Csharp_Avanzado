using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Moq;
using Xunit;
using TaskManager.Controllers;
using ApplicationLayer.Services.TaskServices;
using CapaAplicacion.Services.AuthServices;
using CapaInfraestructura.Repositorio.Auth;
using CapaDominio.DTO;
using DomainLayer.Models;
using DomainLayer.DTO;
using CapaInfraestructura.Repositorio.Delegates;
using CapaAplicacion.Services.CacheServices;
using TaskManager.Hubs;
using CapaInfraestructura.Repositorio.Tasks;
using CapaInfraestructura.Repositorio.Cache;

namespace PruebasUnitarias
{
    // Pruebas para AuthController
    public class AuthControllerTests
    {
        [Fact]
        public void Login_CredencialesValidas_ReturnsOkWithToken()
        {
            // Arrange
            var mockAuthService = new Mock<IAuthService>();
            var loginRequest = new LoginRequestDto { Username = "admin", Password = "admin123" };
            var expectedResponse = new LoginResponseDto
            {
                Token = "fake-jwt-token",
                Expiration = DateTime.UtcNow.AddHours(1)
            };

            mockAuthService.Setup(s => s.Authenticate(It.Is<LoginRequestDto>(
                lr => lr.Username == "admin" && lr.Password == "admin123")))
                           .Returns(expectedResponse);

            var controller = new AuthController(mockAuthService.Object);

            // Act
            var result = controller.Login(loginRequest);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var loginResponse = Assert.IsType<LoginResponseDto>(okResult.Value);
            Assert.Equal("fake-jwt-token", loginResponse.Token);
        }

        [Fact]
        public void Login_CredencialesInvalidas_ReturnsUnauthorized()
        {
            // Arrange
            var mockAuthService = new Mock<IAuthService>();
            var loginRequest = new LoginRequestDto { Username = "wrong", Password = "wrongpass" };

            mockAuthService.Setup(s => s.Authenticate(It.IsAny<LoginRequestDto>()))
                           .Returns((LoginResponseDto)null);

            var controller = new AuthController(mockAuthService.Object);

            // Act
            var result = controller.Login(loginRequest);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal("Credenciales inválidas", unauthorizedResult.Value);
        }
    }

    // Pruebas para TareasController
    public class TareasControllerTests
    {

        [Fact]
        public async Task GetTaskAllAsync_ReturnsOkWithTasks()
        {
            // Arrange
            var expectedTasks = new List<Tareas>
            {
                new Tareas { Id = 1, Description = "Task1", DueDate = DateTime.UtcNow.AddDays(5) }
            };

            var response = new Response<Tareas>
            {
                DataList = expectedTasks,
                Successful = true
            };

            var mockTaskService = new Mock<ITaskService>(MockBehavior.Strict);
            mockTaskService.Setup(s => s.GetTaskAllAsync()).ReturnsAsync(response);

            var mockValidador = new Mock<IValidadorTareas>();
            var mockCalculador = new Mock<ICalculadorTareas>();
            var mockCache = new Mock<CacheService>();
            var mockHubContext = new Mock<IHubContext<TareasHub>>();

            var controller = new TareasController(
                mockValidador.Object,
                mockCalculador.Object,
                mockTaskService.Object,
                mockCache.Object,
                mockHubContext.Object);

            // Act
            var actionResult = await controller.GetTaskAllAsync();

            // Assert
            var returnedResponse = Assert.IsType<Response<Tareas>>(actionResult.Value);
            Assert.True(returnedResponse.Successful);
            Assert.NotNull(returnedResponse.DataList);
        }

        [Fact]
        public async Task GetTaskByIdAllAsync_IDValido_ReturnsOkWithTask()
        {
            // Arrange
            var tarea = new Tareas { Id = 1, Description = "Task 1", DueDate = DateTime.UtcNow.AddDays(2) };
            var response = new Response<Tareas>
            {
                SingleData = tarea,
                Successful = true
            };

            var mockTaskService = new Mock<ITaskService>(MockBehavior.Strict);
            mockTaskService.Setup(s => s.GetTaskByIdAllAsync(1)).ReturnsAsync(response);

            var mockValidador = new Mock<IValidadorTareas>();
            var mockCalculador = new Mock<ICalculadorTareas>();
            var mockCache = new Mock<CacheService>();
            var mockHubContext = new Mock<IHubContext<TareasHub>>();

            var controller = new TareasController(
                mockValidador.Object,
                mockCalculador.Object,
                mockTaskService.Object,
                mockCache.Object,
                mockHubContext.Object);

            // Act
            var actionResult = await controller.GetTaskByIdAllAsync(1);

            // Assert
            var returnedResponse = Assert.IsType<Response<Tareas>>(actionResult.Value);
            Assert.True(returnedResponse.Successful);
            Assert.Equal(1, returnedResponse.SingleData.Id);
        }

        [Fact]
        public async Task AddTaskAllAsync_TareaValida_ReturnsOkAndSendsNotification()
        {
            // Arrange
            var tarea = new Tareas { Id = 1, Description = "New Task", DueDate = DateTime.UtcNow.AddDays(3) };

            var mockValidador = new Mock<IValidadorTareas>();
            mockValidador.Setup(v => v.Validar(tarea)).Returns(true);

            var response = new Response<string>
            {
                Message = "La tarea se guardo correctamente...",
                Successful = true
            };

            var mockTaskService = new Mock<ITaskService>(MockBehavior.Strict);
            mockTaskService.Setup(s => s.AddTaskAllAsync(tarea)).ReturnsAsync(response);

            var mockCache = new Mock<CacheService>();

            var mockClientProxy = new Mock<IClientProxy>();
            mockClientProxy
                .Setup(x => x.SendCoreAsync(
                    "TareaCreada",
                    It.IsAny<object[]>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var mockHubClients = new Mock<IHubClients>();
            mockHubClients.Setup(clients => clients.All)
                          .Returns(mockClientProxy.Object);

            var mockHubContext = new Mock<IHubContext<TareasHub>>();
            mockHubContext.Setup(hub => hub.Clients)
                          .Returns(mockHubClients.Object);

            var mockCalculador = new Mock<ICalculadorTareas>();

            var controller = new TareasController(
                mockValidador.Object,
                mockCalculador.Object,
                mockTaskService.Object,
                mockCache.Object,
                mockHubContext.Object);

            // Act
            var actionResult = await controller.AddTaskAllAsync(tarea);

            // Assert
            var resultValue = Assert.IsType<Response<string>>(actionResult.Value);
            Assert.True(resultValue.Successful);
            Assert.Equal("La tarea se guardo correctamente...", resultValue.Message);


            mockClientProxy.Verify(x => x.SendCoreAsync(
                "TareaCreada",
                It.Is<object[]>(o => o.Length == 1 && o[0] == tarea),
                It.IsAny<CancellationToken>()
            ), Times.Once);
        }

        [Fact]
        public async Task UpdateTaskAllAsync_TareaInvalida_ReturnsBadRequest()
        {
            // Arrange
            var tarea = new Tareas { Id = 1, Description = "", DueDate = DateTime.UtcNow.AddDays(3) };

            var mockValidador = new Mock<IValidadorTareas>();
            mockValidador.Setup(v => v.Validar(tarea)).Returns(false);

            var mockCalculador = new Mock<ICalculadorTareas>();
            var mockTaskService = new Mock<ITaskService>(MockBehavior.Strict); 
            var mockCache = new Mock<ICacheService>();
            var mockHubContext = new Mock<IHubContext<TareasHub>>();

            var controller = new TareasController(
                mockValidador.Object,
                mockCalculador.Object,
                mockTaskService.Object,
                mockCache.Object,
                mockHubContext.Object);

            // Act
            var actionResult = await controller.UpdateTaskAllAsync(tarea);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult.Result);
            Assert.Equal("La tarea no es válida.", badRequestResult.Value);
        }

        [Fact]
        public async Task DeleteTaskAllAsync_IDValido_ReturnsOkAndRemovesCache()
        {
            // Arrange
            var response = new Response<string>
            {
                Message = "La tarea se eliminó correctamente...",
                Successful = true
            };

            var dummyResponse = new Response<Tareas>
            {
                SingleData = new Tareas { Id = 1, Description = "Task 1", DueDate = DateTime.UtcNow.AddDays(2) },
                Successful = true
            };

            var mockTaskService = new Mock<ITaskService>(MockBehavior.Strict);
            mockTaskService.Setup(s => s.GetTaskByIdAllAsync(1)).ReturnsAsync(dummyResponse);
            mockTaskService.Setup(s => s.DeleteTaskAllAsync(1)).ReturnsAsync(response);

            var mockValidador = new Mock<IValidadorTareas>();
            var mockCalculador = new Mock<ICalculadorTareas>();

            var mockCache = new Mock<ICacheService>();
            mockCache.Setup(c => c.RemoveTaskCache(1));

            var mockHubContext = new Mock<IHubContext<TareasHub>>();

            var controller = new TareasController(
                mockValidador.Object,
                mockCalculador.Object,
                mockTaskService.Object,
                mockCache.Object,
                mockHubContext.Object);

            // Act
            var actionResult = await controller.DeleteTaskAllAsync(1);

            // Assert
            var resultValue = Assert.IsType<Response<string>>(actionResult.Value);
            Assert.True(resultValue.Successful);
            Assert.Equal("La tarea se eliminó correctamente...", resultValue.Message);

            mockCache.Verify(c => c.RemoveTaskCache(1), Times.Once);
        }

        [Fact]
        public async Task GetDiasRestantesAsync_AccedeAlCache_ReturnsCachedValue()
        {
            // Arrange
            var mockTaskService = new Mock<ITaskService>(MockBehavior.Strict);
            var mockValidador = new Mock<IValidadorTareas>();
            var mockCalculador = new Mock<ICalculadorTareas>();

            var mockCache = new Mock<ICacheService>();
            mockCache.Setup(c => c.TryGetDiasRestantes(1, out It.Ref<int>.IsAny))
                     .Returns((int id, out int value) => { value = 5; return true; });

            var mockHubContext = new Mock<IHubContext<TareasHub>>();

            var controller = new TareasController(
                mockValidador.Object,
                mockCalculador.Object,
                mockTaskService.Object,
                mockCache.Object,
                mockHubContext.Object);

            // Act
            var actionResult = await controller.GetDiasRestantesAsync(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            Assert.Equal(5, okResult.Value);
        }   

        [Fact]
        public async Task AddLowPriorityTaskAsync_DescripcionValida_ReturnsOkAndSendsNotification()
        {
            // Arrange
            string description = "Low priority task";
            var response = new Response<string>
            {
                Message = "La tarea se guardo correctamente...",
                Successful = true
            };

            var mockTaskService = new Mock<ITaskService>(MockBehavior.Strict);
            mockTaskService.Setup(s => s.AddLowPriorityTask(description)).ReturnsAsync(response);

            var mockValidador = new Mock<IValidadorTareas>();
            var mockCalculador = new Mock<ICalculadorTareas>();
            var mockCache = new Mock<CacheService>();

            var mockClientProxy = new Mock<IClientProxy>();
            mockClientProxy
                .Setup(x => x.SendCoreAsync(
                    "TareaCreada",
                    It.IsAny<object[]>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var mockHubClients = new Mock<IHubClients>();
            mockHubClients.Setup(clients => clients.All)
                          .Returns(mockClientProxy.Object);

            var mockHubContext = new Mock<IHubContext<TareasHub>>();
            mockHubContext.Setup(hub => hub.Clients)
                          .Returns(mockHubClients.Object);

            var controller = new TareasController(
                mockValidador.Object,
                mockCalculador.Object,
                mockTaskService.Object,
                mockCache.Object,
                mockHubContext.Object);

            // Act
            var actionResult = await controller.AddLowPriorityTaskAsync(description);

            // Assert
            var resultValue = Assert.IsType<Response<string>>(actionResult.Value);
            Assert.True(resultValue.Successful);

            mockClientProxy.Verify(x => x.SendCoreAsync(
                "TareaCreada",
                It.Is<object[]>(o => o != null && o.Length == 1 && (string)o[0] == description),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task AddHighPriorityTaskAsync_DescripcionValida_ReturnsOkAndSendsNotification()
        {
            // Arrange
            string description = "High priority task";
            var response = new Response<string>
            {
                Message = "La tarea se guardo correctamente...",
                Successful = true
            };

            var mockTaskService = new Mock<ITaskService>();
            mockTaskService.Setup(s => s.AddHighPriorityTask(description))
                           .ReturnsAsync(response);

            var mockValidador = new Mock<IValidadorTareas>();
            var mockCalculador = new Mock<ICalculadorTareas>();
            var mockCache = new Mock<CacheService>();

            var mockClientProxy = new Mock<IClientProxy>();
            mockClientProxy
                .Setup(x => x.SendCoreAsync(
                    "TareaCreada",
                    It.IsAny<object[]>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var mockHubClients = new Mock<IHubClients>();
            mockHubClients.Setup(clients => clients.All)
                          .Returns(mockClientProxy.Object);

            var mockHubContext = new Mock<IHubContext<TareasHub>>();
            mockHubContext.Setup(hub => hub.Clients)
                          .Returns(mockHubClients.Object);

            var controller = new TareasController(
                mockValidador.Object,
                mockCalculador.Object,
                mockTaskService.Object,
                mockCache.Object,
                mockHubContext.Object);

            // Act
            var actionResult = await controller.AddHighPriorityTaskAsync(description);

            // Assert
            var resultValue = Assert.IsType<Response<string>>(actionResult.Value);
            Assert.True(resultValue.Successful);

            mockClientProxy.Verify(x => x.SendCoreAsync(
                "TareaCreada",
                It.Is<object[]>(o => o != null && o.Length == 1 && (string)o[0] == description),
                It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}