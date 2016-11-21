using System.Collections.Generic;
using System.Linq;
using Beerhall.Controllers;
using Beerhall.Models.Domain;
using Beerhall.Models.ViewModels.BrewerViewModels;
using Beerhall.Tests.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using Xunit;

namespace Beerhall.Tests.Controllers {
    public class BrewerControllerTest {
        private readonly BrewerController _controller;
        private readonly Mock<IBrewerRepository> _brewerRepository;
        private readonly Mock<ILocationRepository> _locationRepository;
        private readonly DummyApplicationDbContext _dummyContext;

        public BrewerControllerTest() {
            _dummyContext = new DummyApplicationDbContext();
            _brewerRepository = new Mock<IBrewerRepository>();
            _locationRepository = new Mock<ILocationRepository>();
            _controller = new BrewerController(_brewerRepository.Object, _locationRepository.Object);
            _controller.TempData = new Mock<ITempDataDictionary>().Object;
        }

        #region -- Index --
        [Fact]
        public void IndexMustPassOrderedListOfBrewersInViewResultModel() {
            _brewerRepository.Setup(m => m.GetAll()).Returns(_dummyContext.Brewers);
            IActionResult actionresult = _controller.Index();
            IList<Brewer> brewersInModel = (actionresult as ViewResult)?.Model as IList<Brewer>;
            Assert.Equal(3, brewersInModel?.Count);
            Assert.Equal("Bavik", brewersInModel?[0].Name);
            Assert.Equal("De Leeuw", brewersInModel?[1].Name);
            Assert.Equal("Duvel Moortgat", brewersInModel?[2].Name);
        }

        [Fact]
        public void IndexMustStoreTotalTurnoverInViewData() {
            _brewerRepository.Setup(m => m.GetAll()).Returns(_dummyContext.Brewers);
            IActionResult actionresult = _controller.Index();
            Assert.Equal(20050000, (actionresult as ViewResult)?.ViewData["TotalTurnover"]);
        }
        #endregion

        #region -- Edit GET --
        [Fact]
        public void EditMustPassBrewerInEditViewModel() {
            _brewerRepository.Setup(m => m.GetBy(1)).Returns(_dummyContext.Bavik);
            IActionResult action = _controller.Edit(1);
            EditViewModel brewerEvm = (action as ViewResult)?.Model as EditViewModel;
            Assert.Equal(1, brewerEvm?.BrewerId);
            Assert.Equal("Bavik", brewerEvm?.Name);
        }

        [Fact]
        public void EditMustReturnSelectListOfGemeentenAndSelectedValue() {
            _brewerRepository.Setup(m => m.GetBy(1)).Returns(_dummyContext.Bavik);
            _locationRepository.Setup(m => m.GetAll()).Returns(_dummyContext.Locations);
            IActionResult action = _controller.Edit(1);
            ViewResult result = action as ViewResult;
            SelectList locationsInViewData = result?.ViewData["Locations"] as SelectList;
            Assert.Equal(3, locationsInViewData.Count());
            Assert.Equal("8531", locationsInViewData?.SelectedValue);
        }

        #endregion

        #region -- Edit POST --
        [Fact]
        public void EditRedirectsToActionIndexWhenSuccessfull() {
            _brewerRepository.Setup(m => m.GetBy(1)).Returns(_dummyContext.Bavik);
            EditViewModel brewerEvm = new EditViewModel(_dummyContext.Bavik);
            brewerEvm.Street = "nieuwe straat 1";
            RedirectToActionResult action = _controller.Edit(brewerEvm) as RedirectToActionResult;
            Assert.Equal("Index", action?.ActionName);
        }

        [Fact]
        public void EditChangesAndPersistsBrewerWhenSuccesful() {
            _brewerRepository.Setup(m => m.GetBy(1)).Returns(_dummyContext.Bavik);
            EditViewModel brewerEvm = new EditViewModel(_dummyContext.Bavik);
            brewerEvm.Street = "nieuwe straat 1";
            _controller.Edit(brewerEvm);
            Brewer bavik = _dummyContext.Bavik;
            Assert.Equal("Bavik", bavik.Name);
            Assert.Equal("nieuwe straat 1", bavik.Street);
            _brewerRepository.Verify(m => m.SaveChanges(), Times.Once());
        }

        [Fact]
        public void EditDoesNotChangeBrewerWhenModelStateContainsErrors() {
            EditViewModel brewerEvm = new EditViewModel(_dummyContext.Bavik);
            _controller.ModelState.AddModelError("", "Error message");
            _controller.Edit(brewerEvm);
            _brewerRepository.Verify(m => m.SaveChanges(), Times.Never());
        }

        [Fact]
        public void EditSelectsDefaultViewWhenModelStateContainsErrors() {
            _locationRepository.Setup(m => m.GetAll()).Returns(_dummyContext.Locations);
            EditViewModel brewerEvm = new EditViewModel(_dummyContext.Bavik);
            _controller.ModelState.AddModelError("", "Error message");
            ViewResult result = _controller.Edit(brewerEvm) as ViewResult;
            SelectList locationsInViewData = result?.ViewData["Locations"] as SelectList;
            Assert.Equal(brewerEvm, result?.Model);
            Assert.Equal(3, locationsInViewData.Count());
        }


        #endregion

        #region -- Create GET --
        [Fact]
        public void CreateMustPassNewBrewerInEditViewModel() {
            IActionResult action = _controller.Create();
            EditViewModel brewerEvm = (action as ViewResult)?.Model as EditViewModel;
            Assert.Equal(0, brewerEvm?.BrewerId);
        }

        [Fact]
        public void CreateMustReturnSelectListOfGemeentenWithNoSelectedValue() {
            _locationRepository.Setup(m => m.GetAll()).Returns(_dummyContext.Locations);
            IActionResult action = _controller.Create();
            ViewResult result = action as ViewResult;
            SelectList locationsInViewData = result?.ViewData["Locations"] as SelectList;
            Assert.Equal(3, locationsInViewData.Count());
            Assert.Null(locationsInViewData?.SelectedValue);
        }

        #endregion

        #region -- Create POST --
        [Fact]
        public void CreateRedirectsToActionIndexWhenSuccessfull() {
            EditViewModel brewerEvm = new EditViewModel(new Brewer("Chimay") {
                Location = _dummyContext.Locations.Last(),
                Street = "TestStraat 10 ",
                Turnover = 8000000
            });
            RedirectToActionResult action = _controller.Create(brewerEvm) as RedirectToActionResult;
            Assert.Equal("Index", action?.ActionName);
        }

        [Fact]
        public void CreateCreatesAndPersistsBrewerWhenSuccesfull() {
            _brewerRepository.Setup(m => m.Add(It.IsAny<Brewer>()));
            EditViewModel brewerEvm = new EditViewModel(_dummyContext.Bavik);
            _controller.Create(brewerEvm);
            _brewerRepository.Verify(m => m.Add(It.IsAny<Brewer>()), Times.Once());
            _brewerRepository.Verify(m => m.SaveChanges(), Times.Once());
        }

        [Fact]
        public void CreateDoesNotCreateBrewerWhenModelStateContainsErrors() {
            EditViewModel newBrewerEvm = new EditViewModel(new Brewer());
            _controller.ModelState.AddModelError("", "Error message");
            _controller.Create(newBrewerEvm);
            _brewerRepository.Verify(m => m.Add(It.IsAny<Brewer>()), Times.Never());
            _brewerRepository.Verify(m => m.SaveChanges(), Times.Never());
        }

        [Fact]
        public void CreateSelectsEditViewWhenModelStateContainsErrors() {
            _locationRepository.Setup(m => m.GetAll()).Returns(_dummyContext.Locations);
            EditViewModel newBrewerEvm = new EditViewModel(new Brewer());
            _controller.ModelState.AddModelError("", "Error message");
            ViewResult result = _controller.Create(newBrewerEvm) as ViewResult;
            SelectList locationsInViewData = result?.ViewData["Locations"] as SelectList;
            Assert.Equal(newBrewerEvm, result?.Model);
            Assert.Equal(3, locationsInViewData.Count());
        }

        #endregion

        #region -- Delete GET --
        [Fact]
        public void DeleteMustPassNameOfBrewerInViewData() {
            _brewerRepository.Setup(m => m.GetBy(1)).Returns(_dummyContext.Bavik);
            _brewerRepository.Setup(m => m.Delete(It.IsAny<Brewer>()));
            IActionResult action = _controller.Delete(1);
            Assert.Equal("Bavik", (action as ViewResult)?.ViewData["name"]);
        }
        #endregion

        #region -- Delete POST --
        [Fact]
        public void DeleteRedirectsToActionIndexWhenSuccessfull() {
            _brewerRepository.Setup(m => m.GetBy(1)).Returns(_dummyContext.Bavik);
            _brewerRepository.Setup(m => m.Delete(It.IsAny<Brewer>()));
            RedirectToActionResult action = _controller.DeleteConfirmed(1) as RedirectToActionResult;
            Assert.Equal("Index", action?.ActionName);
        }

        [Fact]
        public void DeleteDeletesBrewerAndPersistsChangesWhenSuccesfull() {
            _brewerRepository.Setup(m => m.GetBy(1)).Returns(_dummyContext.Bavik);
            _brewerRepository.Setup(m => m.Delete(It.IsAny<Brewer>()));
            _controller.DeleteConfirmed(1);
            _brewerRepository.Verify(m => m.GetBy(1), Times.Once());
            _brewerRepository.Verify(m => m.Delete(It.IsAny<Brewer>()), Times.Once());
            _brewerRepository.Verify(m => m.SaveChanges(), Times.Once());
        }

        #endregion

    }
}
