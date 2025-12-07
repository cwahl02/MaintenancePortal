namespace MaintenancePortal.Models;

public enum PageState
{
    Compact,
    Start,
    Center,
    End
}

public class PaginationMetadata
{
    public int Current { get; set; }
    public int PageSize { get; set; }
    public int TotalItems { get; set; }
    public int TotalOpenItems { get; set; }
    public int TotalInProgressItems { get; set; }
    public int TotalClosedItems { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);

    // Boundaries
    public int PageMin => 0;
    public int PageMax => TotalPages - 1;

    public int WindowSize => PageSize / 2;

    public PageState PageState
    {
        get
        {
            return TotalItems < WindowSize ? PageState.Compact :
                   Current < WindowSize ? PageState.Start :
                   Current >= TotalPages - WindowSize ? PageState.End : PageState.Center;
        }
    }


    public bool HasPrevious => Current > 0;
    public bool HasNext => Current < TotalPages - 1;

    public int Skip => Current * PageSize;
    public int Take => PageSize;

    // Generate the pagination list as a list of integers (and -1 for ellipses)
    public List<int> GetPageList()
    {
        var pages = new List<int>();
        int total = TotalPages;
        int current = Current;
        PageState pageState = PageState;  // Use the pre-calculated PageState

        // Define the window size for the page numbers
        int windowSize = 5;

        switch (pageState)
        {
            case PageState.Compact:
                // Show all pages
                for (int i = 1; i <= total; i++)
                {
                    pages.Add(i);
                }
                break;

            // Case 2: Near the start (page 0 + window size)
            case PageState.Start:
                for(int i = 1; i < TotalPages + 1; i++)
                {
                    pages.Add(i);
                }
                // Show first few pages
                //for (int i = 1; i <= windowSize && i <= total; i++)
                //{
                //    pages.Add(i);
                //}

                //// Show ellipses before the last pages if needed
                //if (total > windowSize + 2)
                //    pages.Add(-1); // Ellipses

                //// Show last 2 pages
                //pages.Add(total - 1);
                //pages.Add(total);
                break;

            // Case 3: Near the end (last pages)
            case PageState.End:
                for(int i = Current - (WindowSize / 2); i < TotalItems + 1; i++)
                {
                    pages.Add(i);
                }

                // Show first 2 pages
                //pages.Add(1);
                //pages.Add(2);

                //// Add ellipses after the first pages if needed
                //if (total - windowSize > 3)
                //    pages.Add(-1); // Ellipses

                //// Show the last few pages (window size)
                //for (int i = total - windowSize; i < total; i++)
                //{
                //    pages.Add(i);
                //}
                break;

            case PageState.Center:
                for(int i = Current - (WindowSize / 2); i < Current + (WindowSize / 2) + 1; i++)
                {
                    pages.Add(i);
                }
                // Show first 2 pages
                //pages.Add(1);
                //pages.Add(2);

                //// Add ellipses before the current window
                //if (current - windowSize > 3)
                //    pages.Add(-1); // Ellipses

                //// Show pages around the current page (window size)
                //for (int i = current - windowSize; i <= current + windowSize; i++)
                //{
                //    if (i > 2 && i < total - 2)
                //        pages.Add(i);
                //}

                //// Add ellipses after the current window
                //if (current + windowSize < total - 3)
                //    pages.Add(-1); // Ellipses

                //// Show the last 2 pages
                //pages.Add(total - 1);
                //pages.Add(total);
                break;
        }

        return pages;
    }
}