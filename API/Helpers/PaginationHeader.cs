namespace API;

public class PaginationHeader
{
 
    public PaginationHeader(int currentPage, int totalPages, int itemsPerPage, int totalItems)
    {
        CurrentPage = currentPage;
        TotalItems = totalItems;
        TotalPages = totalPages;
        ItemsPerPage = itemsPerPage;
    }

    public int CurrentPage {get; set;}

    public int TotalPages {get; set;}


    public int ItemsPerPage {get; set;}

    public int TotalItems {get; set;}
}
