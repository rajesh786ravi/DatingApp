✅ DTO – Key Points
Definition:
    A DTO is a simple object used to transfer data between layers in an application (e.g., between database and UI/API).
Purpose:
    It carries only the data required for a specific operation or view — no extra or sensitive data.
Optimized for Data Consumption:
    Unlike entities, DTOs are shaped or flattened to match the needs of the frontend or client applications.
Security:
    DTOs hide internal or sensitive fields like passwords, IDs, and relationships that entities may expose.
Loose Coupling:
    DTOs prevent tight coupling between the database and presentation layers by abstracting away the DB structure.
Performance:
    Since DTOs include only necessary fields, they reduce data size and improve API performance.
Validation Support:
    DTOs often have validation attributes (like [Required], [EmailAddress], etc.) for form or API input validation.
Not Connected to Database:
    DTOs do not map to database tables and are not tracked by ORM tools like Entity Framework.
Used In:
    DTOs are used in APIs, service layers, and UI bindings — not in the data access layer.
Different from Entity:
    Although DTOs and entities may have similar fields, their purposes are different — entities handle persistence, DTOs handle communication.