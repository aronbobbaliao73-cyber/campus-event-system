using CampusEvents.Backend;
using Moq;
using Xunit;

namespace CampusEvents.Tests;

public class RegistrationValidatorTests
{
    private readonly Mock<IEventRepository> _repository = new();

    private RegistrationValidator CreateValidator()
    {
        return new RegistrationValidator(_repository.Object);
    }

    [Theory]
    [InlineData("juan@univ.edu.ph", true)]
    [InlineData("JUAN@UNIV.EDU.PH", true)]
    [InlineData("  maria.santos@univ.edu.ph  ", true)]
    [InlineData("juan@gmail.com", false)]
    [InlineData("@univ.edu.ph", false)]
    [InlineData("juan@@univ.edu.ph", false)]
    [InlineData("ju an@univ.edu.ph", false)]
    [InlineData("juan@univ.edu.ph.fake.com", false)]
    [InlineData("", false)]
    [InlineData(null, false)]
    public void IsValidSchoolEmail_ChecksDomain(string? email, bool expected)
    {
        var validator = CreateValidator();

        Assert.Equal(expected, validator.IsValidSchoolEmail(email));
    }

    [Fact]
    public void Validate_InvalidEmail_FailsWithoutTouchingRepository()
    {
        var validator = CreateValidator();

        var result = validator.Validate(1, 1, "juan@gmail.com");

        Assert.False(result.IsValid);
        _repository.Verify(r => r.EventExists(It.IsAny<int>()), Times.Never);
        _repository.Verify(r => r.IsAlreadyRegistered(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
        _repository.Verify(r => r.SeatsLeft(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public void Validate_MissingEvent_Fails()
    {
        _repository.Setup(r => r.EventExists(99)).Returns(false);
        var validator = CreateValidator();

        var result = validator.Validate(1, 99, "juan@univ.edu.ph");

        Assert.False(result.IsValid);
        Assert.Equal("Event not found.", result.Message);
    }

    [Fact]
    public void Validate_DuplicateRegistration_Fails()
    {
        _repository.Setup(r => r.EventExists(1)).Returns(true);
        _repository.Setup(r => r.IsAlreadyRegistered(2, 1)).Returns(true);
        var validator = CreateValidator();

        var result = validator.Validate(2, 1, "juan@univ.edu.ph");

        Assert.False(result.IsValid);
        Assert.Equal("You are already registered for this event.", result.Message);
    }

    [Fact]
    public void Validate_FullEvent_Fails()
    {
        _repository.Setup(r => r.EventExists(1)).Returns(true);
        _repository.Setup(r => r.IsAlreadyRegistered(2, 1)).Returns(false);
        _repository.Setup(r => r.SeatsLeft(1)).Returns(0);
        var validator = CreateValidator();

        var result = validator.Validate(2, 1, "juan@univ.edu.ph");

        Assert.False(result.IsValid);
        Assert.Equal("This event is full.", result.Message);
    }

    [Fact]
    public void Validate_AllConditionsMet_Succeeds()
    {
        _repository.Setup(r => r.EventExists(1)).Returns(true);
        _repository.Setup(r => r.IsAlreadyRegistered(2, 1)).Returns(false);
        _repository.Setup(r => r.SeatsLeft(1)).Returns(5);
        var validator = CreateValidator();

        var result = validator.Validate(2, 1, "juan@univ.edu.ph");

        Assert.True(result.IsValid);
    }
}