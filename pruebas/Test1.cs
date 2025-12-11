using AdopcionAPI.Controllers.Mascotas;
using Application.IA.Intefaces;
using Application.Mascotas.Intefaces;
using Application.Mascotas.Models.Requests;
using Application.Mascotas.Models.Responses;
using Application.Mascotas.Services;
using AutoMapper;
using MediatR;
using Moq;

namespace pruebas
{
    [TestClass]
    public sealed class Test1
    {
        private Mock<IMascotaService> _MockmascotaService;
        private Mock<IIaDescripcionService> _MockiaDescripcionService;
        private Mock<IMapper> _MockMapper;
        private Mock<IMediator> _MockMediator;
        private MascotasController _MascotasController;

        [TestInitialize]
        public void Setup()
        {
            _MockmascotaService = new Mock<IMascotaService>();
            _MockiaDescripcionService = new Mock<IIaDescripcionService>();
            _MockMapper = new Mock<IMapper>();
            _MockMediator = new Mock<IMediator>();
            _MascotasController = new MascotasController(_MockmascotaService.Object, _MockiaDescripcionService.Object, _MockMapper.Object, _MockMediator.Object);
        }

        [TestMethod]
        public void Test_ReturnOKMascota()
        {
            // Arrange
            FiltrosRequestDTO prueba = new FiltrosRequestDTO
            {
                GeneroId = 1
            };

            _MockMapper.Setup(c => c.Map<FiltrosRequestDTO>(prueba)).Returns(prueba);
            _MockmascotaService.Setup(c => c.ObtenerTodasUserAsync(It.IsAny<FiltrosRequestDTO>())).ReturnsAsync(
                new List<MascotaPreviewDto>
                {
                    new MascotaPreviewDto
                    {
                        Genero = 1
                    }
                });

            // act

            var result = _MascotasController.Test(prueba);

            // assert

            Assert.IsNotNull(result);
        }
    }
}