using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace ECommerce.Core.Common.SpecsParams;
public class WishlistItemSpecsParams : BaseSpecsParams
{
	[ValidateNever]
	public string UserId { get; set; }
	public int? ProductId { get; set; }
}
