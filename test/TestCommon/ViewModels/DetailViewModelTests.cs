using System;
using FluentAssertions;
using Moq;
using TripLog.Exceptions;
using TripLog.Models;
using TripLog.Services;
using TripLog.ViewModels;
using Xunit;

namespace TestCommon.ViewModels
{
    public class DetailViewModelTests
    {
        private static DetailViewModel GetViewModelMock()
        {
            INavService navMock = new Mock<INavService>().Object;
            DetailViewModel viewModelMock = new DetailViewModel(navMock);
            return viewModelMock;
        }

        [Fact]
        public void Init_ParameterProvided_EntryIsSet()
        {
            // Arrange
            DetailViewModel vm = GetViewModelMock();
            vm.Entry = null;
            TripLogEntry mockEntry = new Mock<TripLogEntry>().Object;

            // Act
            vm.Init(mockEntry);

            // Assert
            vm.Entry.Should().NotBeNull("Entry is null after being " +
                "initialized with a valid TripLogEntry object");
        }

        [Fact]
        public void Init_ParameterNotProvided_ThrowsEntryNotProvidedException()
        {
            // Arrange
            DetailViewModel vm = GetViewModelMock();

            // Act
            Action act = () => vm.Init();

            // Assert
            act.Should().Throw<EntryNotProvidedException>();
        }
    }
}
