namespace FC.Codeflix.Catalog.Domain.Entity;
public class Category
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public Category(string name, string description, bool _IsActive = true)
    {
        Id = Guid.NewGuid();
        Name = name;
        Description = description;
        IsActive = _IsActive;
        CreatedAt = DateTime.Now;
    }
}
