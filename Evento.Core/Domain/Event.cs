using System;
using System.Collections.Generic;

namespace Evento.Core.Domain
{
    //IAggregateRoot
    public class Event : Entity
    {
        private ISet<Ticket> _tickets = new HashSet<Ticket>();
        public string Name { get; protected set; }
        public string Description { get; protected set; }
        public DateTime CreatedAt { get; protected set; }
        public DateTime StartDate { get; protected set; }
        public DateTime EndDate { get; protected set; }
        public DateTime UpdateAt { get; protected set; }
        public IEnumerable<Ticket> Tickets => _tickets;

        protected Event() { }

        public Event(Guid id, string name, string description, DateTime startDate, DateTime endDate)
        {
            Id = id;
            SetName(name);
            SetDesctiption(description);
            StartDate = startDate;
            EndDate = endDate;
            CreatedAt = DateTime.UtcNow;
            UpdateAt = DateTime.UtcNow;
        }

        public void SetName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new Exception($"Event with id: '{Id}' can not have an empty name.");
            }
            Name = name;
            UpdateAt = DateTime.UtcNow;
        }

        public void SetDesctiption(string description)
        {
            if (string.IsNullOrWhiteSpace(description))
            {
                throw new Exception($"Event with id: '{Id}' can not have an empty description.");
            }
            Description = description;
            UpdateAt = DateTime.UtcNow;
        }

        public void AddTickets(int amount, decimal price)
        {
            var seating = _tickets.Count + 1;
            for(var i = 0; i < amount; i++)
            {
                _tickets.Add(new Ticket(this, seating, price));
                seating++;
            }
        }
    }
}
