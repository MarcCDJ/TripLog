using FluentAssertions;
using Moq;
using TripLog.Models;
using TripLog.Services;
using TripLog.ViewModels;
using Xunit;

namespace TestCommon.ViewModels
{
    public class NewEntryViewModelTests
    {
        [Fact]
        public void Init_EntryIsSetWithGeoCoordinates()
        {
            // Arrange
            Mock<INavService> navMock = GetNavMock();
            Mock<ILocationService> locMock = GetLocMock();
            Mock<ITripLogApiDataService> dataMock = GetDataMock();

            NewEntryViewModel evm = GetEntryViewModelMock(navMock.Object, locMock.Object, dataMock.Object);
            evm.Latitude = 0.0;
            evm.Longitude = 0.0;

            // Act
            evm.Init();

            // Assert
            evm.Latitude.Should().Be(123);
            evm.Longitude.Should().Be(321);
        }

        [Fact]
        public void SaveCommand_TitleIsEmpty_CanExecuteReturnsFalse()
        {
            // Arrange
            Mock<INavService> navMock = GetNavMock();
            Mock<ILocationService> locMock = GetLocMock();
            Mock<ITripLogApiDataService> dataMock = GetDataMock();

            NewEntryViewModel evm = GetEntryViewModelMock(navMock.Object, locMock.Object, dataMock.Object);
            evm.Title = "";

            // Act
            bool canSave = evm.SaveCommand.CanExecute(null);

            // Assert
            canSave.Should().BeFalse();
        }

        [Fact]
        public void SaveCommand_AddsEntryToTripLogBackend()
        {
            // Arrange
            Mock<INavService> navMock = GetNavMock();
            Mock<ILocationService> locMock = GetLocMock();
            Mock<ITripLogApiDataService> dataMock = GetDataMock();

            NewEntryViewModel evm = GetEntryViewModelMock(navMock.Object, locMock.Object, dataMock.Object);
            evm.Title = "Mock Entry";

            // Act
            evm.SaveCommand.Execute(null);

            // Assert
            dataMock.Verify(x =>
                x.AddEntryAsync(It.Is<TripLogEntry>(entry =>
                    entry.Title == "Mock Entry")), Times.AtLeast(1));
        }

        [Fact]
        public void SaveCommand_NavigatesBack()
        {
            // Arrange
            Mock<INavService> navMock = GetNavMock();
            Mock<ILocationService> locMock = GetLocMock();
            Mock<ITripLogApiDataService> dataMock = GetDataMock();

            NewEntryViewModel evm = GetEntryViewModelMock(navMock.Object, locMock.Object, dataMock.Object);
            evm.Title = "Mock Entry";

            // Act
            evm.SaveCommand.Execute(null);

            // Assert
            navMock.Verify(x => x.GoBack(), Times.Once);
        }

        #region Helper Methods
        private static Mock<INavService> GetNavMock()
        {
            Mock<INavService> navMock = new Mock<INavService>();
            navMock.Setup(x => x.GoBack()).Verifiable();
            return navMock;
        }

        private static Mock<ILocationService> GetLocMock()
        {
            Mock<ILocationService> lockMock = new Mock<ILocationService>();
            lockMock.Setup(x => x.GetGeoCoordinatesAsync())
                .ReturnsAsync(new GeoCoords
                {
                    Latitude = 123,
                    Longitude = 321
                });
            return lockMock;
        }

        private static Mock<ITripLogApiDataService> GetDataMock()
        {
            Mock<ITripLogApiDataService> dataMock = new Mock<ITripLogApiDataService>();
            dataMock.Setup(x => x.AddEntryAsync(It.Is<TripLogEntry>(entry =>
                    entry.Title == "Mock Entry")))
                .Verifiable();
            return dataMock;
        }

        private static NewEntryViewModel GetEntryViewModelMock(
            INavService navService, ILocationService locationService,
            ITripLogApiDataService tripLogApiDataService)
        {
            NewEntryViewModel entryViewModelMock = new NewEntryViewModel(navService,
                locationService, tripLogApiDataService);
            return entryViewModelMock;
        }
        #endregion
    }
}
