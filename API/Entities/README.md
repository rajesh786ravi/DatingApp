✅ Entity – Key Points
Definition:
    An Entity is a class that maps directly to a database table using an ORM like Entity Framework.
Purpose:
    Represents business objects in the domain and handles persistence (data storage and retrieval).
Database Mapping:
    Each property corresponds to a column in the database.
    Each instance = one row in a table.
Includes:
    Primary keys (e.g., Id)
    Foreign keys
    Navigation properties (e.g., relationships)
    Full set of table columns
    Sometimes audit fields (e.g., CreatedAt, UpdatedBy)
Used In:
    Data layer
    Repositories
    Business logic (sometimes)
Not Used In:
    Directly in APIs or UI (to avoid exposing sensitive or unnecessary data)
Examples:
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
    }   
Has Relationships:
    Can contain navigation properties to other entities (e.g., Product → Category).
May Contain Logic:
    an include business rules or methods (but avoid putting too much logic in them).
Connected to ORM:
    Used by tools like Entity Framework, which tracks changes and translates LINQ to SQL.
Why Not Use in APIs?
    May expose sensitive data (e.g., PasswordHash)
    Performance: includes too much data
    Tight coupling with DB schema