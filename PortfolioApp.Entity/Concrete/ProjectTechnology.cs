namespace PortfolioApp.Entity.Concrete;

public class ProjectTechnology
{
    public int ProjectId { get; set; }
    public int TechnologyId { get; set; }

    public virtual Project Project { get; set; } = null!;
    public virtual Technology Technology { get; set; } = null!;
}
