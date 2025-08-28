namespace H4.Day1.Identity.Data
{
    public partial class Todolist
    {
        public int Int { get; set; }

        public int UserId { get; set; }

        public string Item { get; set; } = null!;

        public virtual Cpr User { get; set; } = null!;
    }
}
