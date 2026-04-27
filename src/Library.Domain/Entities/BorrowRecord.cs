namespace Library.Domain.Entities
{
    // Transaction record for book loan
    public class BorrowRecord
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        // Foreign keys linking the book and member
        public Guid BookId { get; set; }
        public Guid MemberId { get; set; }


        public DateTime BorrowDate { get; set; } = DateTime.UtcNow;

        // Nullable: remains null until book is returned
        public DateTime? ReturnDate { get; set; }

        // Tracks status of the loan
        public string Status { get; set; } = "Borrowed";

        // Navigation properties enable Entity Framework to load related data via .Include()
        public virtual Book? Book { get; set; }
        public virtual Member? Member { get; set; }
    }
}