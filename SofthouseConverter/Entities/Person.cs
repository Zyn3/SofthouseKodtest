public class Person
{
    public string Firstname { get; set; }
    public string Lastname { get; set; }
    public List<Phone> Phones { get; set; } = new List<Phone>();
    public List<Address> Addresses { get; set; } = new List<Address>();
    public List<Family> Families { get; set; } = new List<Family>();
}