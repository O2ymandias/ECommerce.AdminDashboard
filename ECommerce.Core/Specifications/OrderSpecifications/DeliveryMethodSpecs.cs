using ECommerce.Core.Models.OrderModule;

namespace ECommerce.Core.Specifications.OrderSpecifications;

public class DeliveryMethodSpecs(int? deliveryMethodId)
    : BaseSpecification<DeliveryMethod>(d => !deliveryMethodId.HasValue || d.Id == deliveryMethodId.Value);