using System.ComponentModel.DataAnnotations;

namespace StudentsManager.Models;

public class Identity
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public int Age { get; set; }
    public Address Address { get; set; }
}