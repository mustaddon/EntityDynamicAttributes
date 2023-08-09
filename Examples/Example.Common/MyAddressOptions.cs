using EntityDynamicAttributes;

namespace Example;

public class MyAddressOptions
{
    public string Address { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string Country { get; set; }
}

public class MyAddressOptionsConfig : TypeConfig<MyAddressOptions>
{
    public MyAddressOptionsConfig()
    {
        Property(x => x.Address)
            .Required(x => !x.IsHidden(xx => xx));

        Property(x => x.City)
            .Required(x => !x.IsHidden(xx => xx));

        Property(x => x.Country)
            .Required(x => !x.IsHidden(xx => xx));
    }
}
