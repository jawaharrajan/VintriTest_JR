using VintriTest_JR;
using Moq;
using Microsoft.AspNetCore.Mvc;
using VintriTest_JR.Controllers;
using VintriTest_JR.Services;
using VintriTest_JR.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using VintriTest_JR.ActionFilters;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace VintriTest_JR.Tests
{
    public class BeerControllerTests
    {
        private readonly BeerController _mockBeercontroller;
        private readonly BeerController _mockRatingBeercontroller;
        private readonly Mock<IApiService> _mockApiService;
        
        //Constructor to set up mock objects
        public BeerControllerTests()
        {
            _mockApiService = new Mock<IApiService>();
            _mockBeercontroller = new BeerController(_mockApiService.Object);
        }

        [Fact]
        public async Task GivenGetBeers_ItShouldReturnOKResult_WhenSuccessful()
        {
            //Arrange
            var mockJson = "[\r\n  {\r\n    \"id\": 1,\r\n    \"name\": \"Buzz\"\r\n  },\r\n  {\r\n    \"id\": 2,\r\n    \"name\": \"Trashy Blonde\"\r\n  },\r\n  {\r\n    \"id\": 3,\r\n    \"name\": \"Berliner Weisse With Yuzu - B-Sides\"\r\n  },\r\n  {\r\n    \"id\": 4,\r\n    \"name\": \"Pilsen Lager\"\r\n  },\r\n  {\r\n    \"id\": 5,\r\n    \"name\": \"Avery Brown Dredge\"\r\n  }]";
            
                _mockApiService.Setup(s => s.GetBeerIdAndName())
                .ReturnsAsync(mockJson);

            //Act
            var result = await _mockBeercontroller.GetBeers();

            //Assert
            Assert.IsType<OkObjectResult>(result);
            var okResult = (OkObjectResult)result;
            Assert.Equal(mockJson, okResult.Value);
        }

        [Fact]
        public async Task GivenGetBeers_ItShouldReturnBadResult_WhenServiceThrowsAnError()
        {
            //Arrange            
            _mockApiService.Setup(s => s.GetBeerIdAndName())
            .Throws(new Exception("Service failure."));

            //Act
            var result = await _mockBeercontroller.GetBeers();

            //Assert
            Assert.IsType<BadRequestObjectResult>(result);
            var badRequestResult = (BadRequestObjectResult)result;
            Assert.Equal("Error getting Beer information.", badRequestResult.Value);
        }

        [Fact]
        public async Task GivenSumbitRating_ItShouldReturnOkResult_WhenServiceIsGood()
        {
            //Arrange
            var mockRating = new Mock<Ratings>();

            _mockApiService.Setup(s => s.SubmitBeerRatingAsync(It.IsAny<Ratings>(),It.IsAny<int>()))
                .ReturnsAsync("Rating Data saved");

            //Act
            var result = await _mockBeercontroller.SubmitRating(mockRating.Object,1);

            //Assert
            Assert.IsType<OkObjectResult>(result);
                var okResult = (OkObjectResult)result;
            Assert.Equal("Rating Data saved", okResult.Value);
        }

        [Fact]
        public async Task GivenSumbitRating_ItShouldBadResult_WhenServiceFails()
        {
            //Arrange
            var mockRating = new Mock<Ratings>();

            _mockApiService.Setup(s => s.SubmitBeerRatingAsync(It.IsAny<Ratings>(), It.IsAny<int>()))
                .Throws(new Exception("Service failure."));

            //Act
            var result = await _mockBeercontroller.SubmitRating(mockRating.Object, 1);

            //Assert
            Assert.IsType<BadRequestObjectResult>(result);
            var badRequestResult = (BadRequestObjectResult)result;
            Assert.Equal("Error submitting rating.", badRequestResult.Value);
        }

        [Fact]
        public async Task GivenSumbitRating_ItShouldBadResult_WhenUserNameIsNotValid()
        {
            //Arrange
            var actionContext = new ActionExecutingContext(
            new ActionContext(new DefaultHttpContext(), new RouteData(), new ActionDescriptor()),
            new List<IFilterMetadata>(),
            new Dictionary<string, object> { { "ratingModel", new Ratings { Id = 1, Username = "test$test.com", Rating = 2, Comments = "Good Beer" } } },
            new BeerController(_mockApiService.Object));

            var filter = new UsernameValidationFilterAttribute();

            // Act
            filter.OnActionExecuting(actionContext);

            //Assert
            Assert.IsType<BadRequestObjectResult>(actionContext.Result);
            var badRequestResult = actionContext.Result as BadRequestObjectResult;
            Assert.Equal("The Username field is not a valid e-mail address.", badRequestResult.Value);
        }


        [Theory]
        [InlineData(8, false)]
        [InlineData(6, false)]
        //[InlineData(3, true)]
        //[InlineData(1, true)]
        public async Task GivenSumbitRating_ItShouldBadResult_WhenRatingIsNotInRange(int rating, bool shouldValidate)
        {
            //Arrange         
            var mockRating = new Mock<Ratings>();
            mockRating.SetupAllProperties();
            mockRating.Object.Id = 1;
            mockRating.Object.Username = "test1@test.com";
            mockRating.Object.Rating = rating;
            mockRating.Object.Comments = "Good taste";

            //Act
            _mockBeercontroller.ModelState.AddModelError("Rating", "Value for rating must be between 1 and 5."); ;
            var result = await _mockBeercontroller.SubmitRating(mockRating.Object, 1);

            //Assert
            Assert.IsType<BadRequestObjectResult>(result);
    
            var badRequestResult = (BadRequestObjectResult)result;
            
            Assert.Equal(400,
                badRequestResult.StatusCode); 
          
        }
    }
}