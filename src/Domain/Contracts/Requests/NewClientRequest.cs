namespace Domain.Contracts.Requests;

public class NewClientRequest
{
        public string Name { get; set; }
        public string DocumentId { get; set; }
        public string PhoneNumber { get; set; }
        public string Observation { get; set; }
}