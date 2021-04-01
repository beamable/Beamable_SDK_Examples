using System.Collections;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace Beamable.Examples.Tests
{
    /// <summary>
    /// Demonstrates a working example of an EditMode test.
    /// </summary>
    public class BeamableEditModeTestExample
    {
        [UnityTest]
        public IEnumerator UserId_LengthIs16_WhenGetInstance()
        {
            // Arrange
            var promise = Beamable.API.Instance;
            yield return promise.ToYielder();
            var beamableAPI = promise.GetResult();
            
            // Act
            int userIdLength = beamableAPI.User.id.ToString().Length;

            // Assert
            Assert.That(userIdLength, Is.EqualTo(16));
        }
    }
}