using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Beerhall.Models.Domain;
using Microsoft.AspNetCore.Identity;

namespace Beerhall.Data {
    public class BeerhallDataInitializer {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;

        public BeerhallDataInitializer(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager) {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        public async Task InitializeData() {
            _dbContext.Database.EnsureDeleted();
            if (_dbContext.Database.EnsureCreated()) {
                await InitializeUsers();
                Location bavikhove = new Location { Name = "Bavikhove", PostalCode = "8531" };
                Location roeselare = new Location { Name = "Roeselare", PostalCode = "8800" };
                Location puurs = new Location { Name = "Puurs", PostalCode = "2870" };
                Location leuven = new Location { Name = "Leuven", PostalCode = "3000" };
                Location oudenaarde = new Location { Name = "Oudenaarde", PostalCode = "9700" };
                Location affligem = new Location { Name = "Affligem", PostalCode = "1790" };
                Location[] locations = { bavikhove, roeselare, puurs, leuven, oudenaarde, affligem };
                _dbContext.Locations.AddRange(locations);
                _dbContext.SaveChanges();

                Brewer bavik = new Brewer("Bavik", bavikhove, "Rijksweg 33");
                _dbContext.Brewers.Add(bavik);
                bavik.AddBeer("Bavik Pils", 5.2,
                    "De Bavik Premium Pils wordt gebrouwen met de beste mout en hop en verdient koel geschonken te worden.");
                bavik.AddBeer("Wittekerke", 5.0, "Wittekerke 1/4");
                bavik.AddBeer("Wittekerke Speciale", 5.8);
                bavik.AddBeer("Wittekerke Rosé", 4.3);
                bavik.AddBeer("Ezel Wit", 5.8);
                bavik.AddBeer("Ezel Bruin", 6.5);
                bavik.Turnover = 20000000;
                bavik.DateEstablished = new DateTime(1990, 12, 26);
                bavik.ContactEmail = "info@bavik.be";
                bavik.Description =
                    "Brouwerij De Brabandere kan terugblikken op een rijke geschiedenis, maar kijkt met evenveel vertrouwen naar de toekomst. De droom die stichter Adolphe De Brabandere op het eind van de negentiende eeuw koestert wanneer hij in Bavikhove de fundamenten legt van zijn brouwerij, is realiteit geworden in de succesvolle onderneming van vandaag.Met een rijk assortiment bieren dat gesmaakt wordt door kenners tot ver buiten onze landsgrenzen.Brouwen was, is, en blijft een kunst bij Brouwerij De Brabandere. Beschouw onze talrijke karaktervolle bieren gerust als erfgoed: gemaakt met traditioneel vakmanschap, met authentieke ingrediënten en… veel liefde. Het creëren van een unieke smaaksensatie om te delen met vrienden, dat drijft ons dag in dag uit.  Zonder compromissen.";

                Brewer palm = new Brewer("Palm Breweries");
                _dbContext.Brewers.Add(palm);
                palm.AddBeer("Estimanet", 5.2);
                palm.AddBeer("Steenbrugge Blond", 6.5);
                palm.AddBeer("Palm", 5.4);
                palm.AddBeer("Dobbel Palm", 6.0);
                palm.Turnover = 500000;

                Brewer duvelMoortgat = new Brewer("Duvel Moortgat", puurs, "Breendonkdorp 28");
                _dbContext.Brewers.Add(duvelMoortgat);
                duvelMoortgat.AddBeer("Duvel", 8.5);
                duvelMoortgat.AddBeer("Vedett");
                duvelMoortgat.AddBeer("Maredsous");
                duvelMoortgat.AddBeer("Liefmans Kriekbier");
                duvelMoortgat.AddBeer("La Chouffe", 8.0);
                duvelMoortgat.AddBeer("De Koninck", 5.0);

                Brewer inBev = new Brewer("InBev", leuven, "Brouwerijplein 1");
                _dbContext.Brewers.Add(inBev);
                inBev.AddBeer("Jupiler");
                inBev.AddBeer("Stella Artois");
                inBev.AddBeer("Leffe");
                inBev.AddBeer("Belle-Vue");
                inBev.AddBeer("Hoegaarden");

                Brewer roman = new Brewer("Roman", oudenaarde, "Hauwaart 105");
                _dbContext.Brewers.Add(roman);
                roman.AddBeer("Sloeber", 7.5);
                roman.AddBeer("Black Hole", 5.6);
                roman.AddBeer("Ename", 6.5);
                roman.AddBeer("Romy Pils", 5.1);

                Brewer deGraal = new Brewer("De Graal");
                _dbContext.Brewers.Add(deGraal);

                Brewer deLeeuw = new Brewer("De Leeuw");
                _dbContext.Brewers.Add(deLeeuw);

                _dbContext.SaveChanges();
            }
        }

        private async Task InitializeUsers() {
            string eMailAddress = "beermaster@hogent.be";
            ApplicationUser user = new ApplicationUser { UserName = eMailAddress, Email = eMailAddress };
            await _userManager.CreateAsync(user, "P@ssword1");
            await _userManager.AddClaimAsync(user, new Claim(ClaimTypes.Role, "admin"));

            eMailAddress = "jan@hogent.be";
            user = new ApplicationUser { UserName = eMailAddress, Email = eMailAddress };
            await _userManager.CreateAsync(user, "P@ssword1");
            await _userManager.AddClaimAsync(user, new Claim(ClaimTypes.Role, "customer"));
        }
    }
}

