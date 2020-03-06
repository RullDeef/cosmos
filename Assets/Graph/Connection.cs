

public class Connection
{
    public Planet planetFrom, planetTo;

    public float Length => (planetFrom.position - planetTo.position).magnitude;
}
