using Microsoft.AspNetCore.Mvc.Rendering;

public class ReportViewModel
{
    public string SelectedReport { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public SelectList ReportOptions { get; set; }
}
