using System.Text.Json.Serialization;

public partial class CurrencyType
{
    public static readonly CurrencyType Money = new CurrencyType(nameof(Money));

    public string Id { get; private set; }

    [JsonIgnore]
    public CurrencyData Data => CurrencyController.Instance.GetData(this);

    public CurrencyType(string id)
    {
        Id = id;
    }

    public override string ToString()
    {
        return Id;
    }

    public static bool operator ==(CurrencyType left, CurrencyType right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(CurrencyType left, CurrencyType right)
    {
        return !left.Equals(right);
    }

    public override bool Equals(object obj)
    {
        if (Id == null) return false;
        return Id == (obj as CurrencyType)?.Id;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}
