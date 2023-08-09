using EntityDynamicAttributes;

namespace Example;

public class MyUser
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public bool HasAddress { get; set; }
    public MyAddressOptions AddressOptions { get; set; }
}

public class MyUserConfig : TypeConfig<MyUser>
{
    public MyUserConfig()
    {
        Property(x => x.FirstName)
            .Required();

        Property(x => x.AddressOptions, new MyAddressOptionsConfig())
            .Hidden(x => !x.Entity.HasAddress);
    }
}
