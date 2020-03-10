

public class Connection
{
    public Planet planetFrom, planetTo;

    public float Length => (planetFrom.transform.position - planetTo.transform.position).magnitude;
}
