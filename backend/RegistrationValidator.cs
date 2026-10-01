namespace CampusEvents.Backend;

public interface IEventRepository
{
    bool EventExists(int eventId);
    int SeatsLeft(int eventId);
    bool IsAlreadyRegistered(int userId, int eventId);
}

public record ValidationResult(bool IsValid, string Message);

public class RegistrationValidator
{
    private const string AllowedDomain = "@univ.edu.ph";

    private readonly IEventRepository _repository;

    public RegistrationValidator(IEventRepository repository)
    {
        _repository = repository;
    }

    public bool IsValidSchoolEmail(string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return false;
        }

        string trimmed = email.Trim();

        if (!trimmed.EndsWith(AllowedDomain, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        string localPart = trimmed.Substring(0, trimmed.Length - AllowedDomain.Length);

        return localPart.Length > 0
            && !localPart.Contains('@')
            && !localPart.Contains(' ');
    }

    public ValidationResult Validate(int userId, int eventId, string? email)
    {
        if (!IsValidSchoolEmail(email))
        {
            return new ValidationResult(false, "Email must end with @univ.edu.ph.");
        }

        if (!_repository.EventExists(eventId))
        {
            return new ValidationResult(false, "Event not found.");
        }

        if (_repository.IsAlreadyRegistered(userId, eventId))
        {
            return new ValidationResult(false, "You are already registered for this event.");
        }

        if (_repository.SeatsLeft(eventId) <= 0)
        {
            return new ValidationResult(false, "This event is full.");
        }

        return new ValidationResult(true, "Registration allowed.");
    }
}