public class Family
{
    public string Name { get; set; }
    public string Born { get; set; }
    public List<Phone> Phones { get; set; } = new List<Phone>();
    public List<Address> Addresses { get; set; } = new List<Address>();
}