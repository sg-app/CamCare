namespace CamCare.Models
{
    public class Paginated<T>
    {
        public ICollection<T> Items { get; set; } = [];
        public int TotalCount { get; set; }
    }
}
