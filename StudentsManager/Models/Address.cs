namespace StudentsManager.Models;

public class Address : Identity
{
    public string StreetAddress { get; set; }
    public string ZipCode { get; set; }
    public string City { get; set; }
    public string PhoneNumber { get; set; }
}