namespace Library.Domain.Entities
{
    // Inventory tracking for book in catalog
    public class Book
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;

        public string ISBN { get; set; } = string.Empty;

        public int TotalCopies { get; set; }
        public int AvailableCopies { get; set; }

        // True only if all copies are borrowed
        public bool IsBorrowed => AvailableCopies == 0;

        // Concurrency Token: EF Core uses this RowVersion to prevent conflicting updates
        // Only the first SaveChanges will succeed, the second throws DbUpdateConcurrencyException
        [System.ComponentModel.DataAnnotations.Timestamp]
        public byte[]? RowVersion { get; set; }
    }
}