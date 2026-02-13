namespace MdcHR26Apps.BlazorServer.Models
{
    public class Items
    {
        public int ItemNumber { get; set; }
        public string? ItemName { get; set; } = null!;
        public int ItemPeroportion { get; set; }
        public int ItemSubPeroportion { get; set; }
        public bool ItemCompleteStatus { get; set; }
    }
}
