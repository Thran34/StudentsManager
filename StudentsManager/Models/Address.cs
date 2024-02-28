namespace StudentsManager.Models;

public class Address
{
    public int AddressId { get; set; }
    public string StreetAddress { get; set; }
    public string ZipCode { get; set; }
    public string City { get; set; }
    public string PhoneNumber { get; set; }
    public Student Student { get; set; }
    public Teacher Teacher { get; set; }

    public int StudentId { get; set; }
    public int TeacherId { get; set; }
}