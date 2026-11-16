namespace MED.Models;

public class MedUser
{
    public int Id { get; set; }

    public string Name { get; set; } = "";

    public string Email { get; set; } = "";

    public string PasswordHash { get; set; } = "";

    public string Role { get; set; } = "";
}

public class Notice
{
    public int Id { get; set; }

    public string Title { get; set; } = "";

    public string Device { get; set; } = "";

    public string Type { get; set; } = "";

    public string Description { get; set; } = "";

    public DateTime Date { get; set; } = DateTime.Today;
}

public class Review
{
    public int Id { get; set; }

    public int EmployeeId { get; set; }

    public string Title { get; set; } = "";

    public List<int> NoticeIds { get; set; } = new();

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

public class NoticeComment
{
    public int Id { get; set; }

    public int NoticeId { get; set; }

    public int EmployeeId { get; set; }

    public string Text { get; set; } = "";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class Finding
{
    public int Id { get; set; }

    public int EmployeeId { get; set; }

    public int ReviewId { get; set; }

    public string Title { get; set; } = "";

    public string Summary { get; set; } = "";

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

public class FindingShare
{
    public int Id { get; set; }

    public int FindingId { get; set; }

    public int CustomerId { get; set; }

    public DateTime SharedAt { get; set; } = DateTime.UtcNow;
}

public class CustomerComment
{
    public int Id { get; set; }

    public int ShareId { get; set; }

    public int CustomerId { get; set; }

    public string Text { get; set; } = "";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class MedDatabase
{
    public List<MedUser> Users { get; set; } = new();

    public List<Notice> Notices { get; set; } = new();

    public List<Review> Reviews { get; set; } = new();

    public List<NoticeComment> NoticeComments { get; set; } = new();

    public List<Finding> Findings { get; set; } = new();

    public List<FindingShare> Shares { get; set; } = new();

    public List<CustomerComment> CustomerComments { get; set; } = new();
}