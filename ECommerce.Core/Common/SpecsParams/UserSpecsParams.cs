namespace ECommerce.Core.Common.SpecsParams;

public class UserSpecsParams : BaseSpecsParams
{
    private string? _search;

    public string? Search
    {
        get => _search;
        set => _search = value?.Trim();
    }

    public string? RoleId { get; set; }
}