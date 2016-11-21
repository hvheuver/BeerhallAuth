using Beerhall.Models.Domain;
using System;
using Xunit;


namespace Beerhall.Tests.Models.Domain {
    public class BeerTest {

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("    ")]
        [InlineData(" \t \n \r \t   ")]
        public void NewBeerWithWrongNameThrowsException(string name) {
            Assert.Throws<ArgumentException>(() => new Beer(name));
        }

        [Fact]
        public void AlcoholKnownMustReturnTrueIfAlcoholByVolumeIsSet() {
            double alcoholByVolume = 8.5D;
            Beer beer = new Beer("New beer") { AlcoholByVolume = alcoholByVolume };
            Assert.True(beer.AlcoholKnown);
        }

        [Fact]
        public void AlcoholKnownMustReturnFalseIfAlcoholByVolumeIsNotSet() {
            Beer beer = new Beer("New beer");
            Assert.False(beer.AlcoholKnown);
        }
    }
}