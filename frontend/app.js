const EMAIL_PATTERN = /^[A-Za-z0-9._%+-]+@univ\.edu\.ph$/i;

const events = [
  { id: 1, title: "Tech Talk 2026", venue: "Main Auditorium", date: "2026-11-05T14:00:00", capacity: 100, registered: 2 },
  { id: 2, title: "Career Fair", venue: "Gymnasium", date: "2026-11-20T09:00:00", capacity: 300, registered: 1 },
  { id: 3, title: "Hackathon Night", venue: "Computer Lab 3", date: "2026-12-02T18:00:00", capacity: 2, registered: 2 }
];

const registrations = [];

const eventList = document.getElementById("event-list");
const eventSelect = document.getElementById("event-select");
const catalogStatus = document.getElementById("catalog-status");
const form = document.getElementById("registration-form");
const formMessage = document.getElementById("form-message");

const nameInput = document.getElementById("full-name");
const emailInput = document.getElementById("email");

function formatDate(isoString) {
  return new Date(isoString).toLocaleString("en-PH", {
    dateStyle: "medium",
    timeStyle: "short"
  });
}

function seatsLeft(event) {
  return event.capacity - event.registered;
}

function renderEvents() {
  eventList.innerHTML = "";
  eventSelect.innerHTML = '<option value="">Choose an event</option>';

  events.forEach(function (event) {
    const left = seatsLeft(event);

    const item = document.createElement("li");
    const card = document.createElement("article");
    card.className = "event-card";

    const title = document.createElement("h3");
    title.textContent = event.title;

    const venue = document.createElement("p");
    venue.textContent = "Venue: " + event.venue;

    const when = document.createElement("p");
    when.textContent = "Date: " + formatDate(event.date);

    const seats = document.createElement("p");
    seats.className = "seats";
    seats.textContent = left > 0 ? left + " seats left" : "Fully booked";

    card.append(title, venue, when, seats);
    item.appendChild(card);
    eventList.appendChild(item);

    const option = document.createElement("option");
    option.value = String(event.id);
    option.textContent = left > 0 ? event.title : event.title + " (Full)";
    option.disabled = left <= 0;
    eventSelect.appendChild(option);
  });

  catalogStatus.textContent = events.length + " events available.";
}

function setError(input, errorId, message) {
  document.getElementById(errorId).textContent = message;
  if (message) {
    input.setAttribute("aria-invalid", "true");
  } else {
    input.removeAttribute("aria-invalid");
  }
}

function validateForm() {
  let valid = true;

  const name = nameInput.value.trim();
  const email = emailInput.value.trim();

  if (name.length < 2) {
    setError(nameInput, "name-error", "Please enter your full name.");
    valid = false;
  } else {
    setError(nameInput, "name-error", "");
  }

  if (!EMAIL_PATTERN.test(email)) {
    setError(emailInput, "email-error", "Use your school email ending in @univ.edu.ph.");
    valid = false;
  } else {
    setError(emailInput, "email-error", "");
  }

  if (!eventSelect.value) {
    setError(eventSelect, "event-error", "Please choose an event.");
    valid = false;
  } else {
    setError(eventSelect, "event-error", "");
  }

  return valid;
}

function showMessage(text, type) {
  formMessage.textContent = text;
  formMessage.className = "message " + type;
}

form.addEventListener("submit", function (submitEvent) {
  submitEvent.preventDefault();
  formMessage.textContent = "";

  if (!validateForm()) {
    return;
  }

  const email = emailInput.value.trim().toLowerCase();
  const eventId = Number(eventSelect.value);
  const chosen = events.find(function (event) {
    return event.id === eventId;
  });

  if (!chosen || seatsLeft(chosen) <= 0) {
    showMessage("Sorry, that event is already full.", "failure");
    return;
  }

  const duplicate = registrations.some(function (entry) {
    return entry.email === email && entry.eventId === eventId;
  });

  if (duplicate) {
    showMessage("You are already registered for this event.", "failure");
    return;
  }

  registrations.push({ email: email, eventId: eventId, name: nameInput.value.trim() });
  chosen.registered += 1;

  renderEvents();
  form.reset();
  showMessage("You are registered for " + chosen.title + ".", "success");
});

renderEvents();