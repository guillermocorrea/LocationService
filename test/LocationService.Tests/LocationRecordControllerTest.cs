using LocationService.Controllers;
using LocationService.Models;
using LocationService.Persistence;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace LocationService.Tests
{
    public class LocationRecordControllerTest
    {
        [Fact]
        public void AddLocationAppendsLocation()
        {
            // Arrange
            ILocationRecordRepository repository = new InMemoryLocationRecordRepository();
            var controller = new LocationRecordController(repository);
            var newMemberId = Guid.NewGuid();
            var newLocation = new LocationRecord()
            {
                ID = Guid.NewGuid(),
                Altitude = 35.6f,
                Latitude = 34.2f,
                Longitude = 22f,
                MemberID = newMemberId,
                Timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds()
            };
            // Act
            var result = controller.AddLocation(newMemberId, newLocation) as IActionResult;
            var lastRecord = repository.GetLatestForMember(newMemberId);

            // Assert
            Assert.True(result is CreatedResult);
            Assert.Equal(newLocation.ID, lastRecord.ID);
        }

        [Fact]
        public void GetLocationForMemberReturns()
        {
            // Arrange
            ILocationRecordRepository repository = new InMemoryLocationRecordRepository();
            var controller = new LocationRecordController(repository);
            var newMemberId = Guid.NewGuid();
            var newLocation = new LocationRecord()
            {
                ID = Guid.NewGuid(),
                MemberID = Guid.NewGuid()
            };
            var lastLocation = new LocationRecord()
            {
                ID = Guid.NewGuid(),
                MemberID = Guid.NewGuid()
            };
            repository.Add(newLocation);
            repository.Add(lastLocation);
            
            // Act
            var result = (ICollection<LocationRecord>)(controller.GetLocationsForMember(newMemberId) as ObjectResult).Value;

            // Assert
            Assert.Equal(2, result.Count);
            Assert.True(result.Contains(lastLocation));
            Assert.True(result.Contains(newLocation));
        }

        [Fact]
        public void GetLastLocation()
        {
            // Arrange
            ILocationRecordRepository repository = new InMemoryLocationRecordRepository();
            var controller = new LocationRecordController(repository);
            var newMemberId = Guid.NewGuid();
            var newLocation = new LocationRecord()
            {
                ID = Guid.NewGuid(),
                Timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds(),
                MemberID = Guid.NewGuid()
            };
            var lastLocation = new LocationRecord()
            {
                ID = Guid.NewGuid(),
                Timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds(),
                MemberID = Guid.NewGuid()
            };
            repository.Add(newLocation);
            repository.Add(lastLocation);

            // Act
            var result = (LocationRecord)(controller.GetLatestForMember(newMemberId) as ObjectResult).Value;

            // Assert
            Assert.Equal(lastLocation.ID, result.ID);
        }
    }
}
