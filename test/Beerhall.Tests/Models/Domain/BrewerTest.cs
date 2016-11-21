using System;
using System.Linq;
using Beerhall.Models.Domain;
using Xunit;

namespace Beerhall.Tests.Models.Domain {
    public class BrewerTest : IDisposable {
        private Brewer _bockor;

        public BrewerTest() {
            _bockor = new Brewer("Bockor");
            _bockor.AddBeer("Omer");
            _bockor.AddBeer("Bellegems Bruin");
        }

        #region Constructor
        [Fact]
        public void NewBrewerWithAGivenNameIsCreatedCorrectly() {
            Brewer brewer = new Brewer("Rodenbach");
            Assert.Equal("Rodenbach", brewer.Name);
            Assert.Null(brewer.Turnover);
            Assert.Equal(0, brewer.NrOfBeers);
            Assert.Null(brewer.Location);
            Assert.Null(brewer.Street);
            Assert.Equal(0, brewer.BrewerId);
        }

        [Fact]
        public void NewBrewerWithAGivenAddressIsCreatedCorrectly() {
            Location veurne = new Location { Name = "Veurne", PostalCode = "8630" };
            Brewer brouwer = new Brewer("Bachten de Kupe", veurne, "Kerkstraat 20") { Turnover = 20000 };
            Assert.Equal("Bachten de Kupe", brouwer.Name);
            Assert.Equal("Veurne", brouwer.Location.Name);
            Assert.Equal("Kerkstraat 20", brouwer.Street);
            Assert.Equal(20000, brouwer.Turnover);
        }
        #endregion
        [Fact]
        public void BrewerTurnoverSetToNegativeValueThrowsException() {
            Assert.Throws<ArgumentException>(() => new Brewer("Rodenbach") { Turnover = -2000 });
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("    ")]
        [InlineData(" \t \n \r \t   ")]
        [InlineData("01234567890123456789012345678901234567890123456789*")]
        public void BrewerNameSetToIncorrectValueThrowsException(string name) {
            Assert.Throws<ArgumentException>(() => new Brewer(name));
        }

        [Theory]
        [InlineData("")]
        [InlineData("abc")]
        [InlineData("abc@de")]
        [InlineData("ab.c@de.")]
        public void BrewerEmailSetToInvalidAddressThrowsException(string contactEmail) {
            Assert.Throws<ArgumentException>(() => new Brewer("Rodenbach") { ContactEmail = contactEmail });
        }

        [Fact]
        public void BrewerDateEstablishedSetToFutureDateThrowsException() {
            Assert.Throws<ArgumentException>(() => new Brewer("Rodenbach") { DateEstablished = DateTime.Now.AddDays(1) });
        }

        [Fact]
        public void AddBeerAddsABeer() {
            int nrOfBeersBeforeAdd = _bockor.NrOfBeers;
            _bockor.AddBeer("HoGent beer", 55.0D);
            Assert.Equal(nrOfBeersBeforeAdd + 1, _bockor.NrOfBeers);
        }

        [Fact]
        public void AddBeerWithABeerThatHasADuplicateNameThrowsException() {
            Assert.Throws<ArgumentException>(() => _bockor.AddBeer("Omer"));
        }

        [Fact]
        public void DeleteBeerDeletesTheBeer() {
            int nrOfBeersBeforeDelete = _bockor.NrOfBeers;
            Beer aBeer = _bockor.Beers.First();
            _bockor.DeleteBier(aBeer);
            Assert.Equal(nrOfBeersBeforeDelete - 1, _bockor.NrOfBeers);
        }

        [Fact]
        public void DeleteBeerForNonExistingBeerThrowsException() {
            Beer aBeer = new Beer("Just a beer");
            Assert.Throws<ArgumentException>(() => _bockor.DeleteBier(aBeer));
        }

        public void Dispose() {

        }
    }
}