using ECommerce.Core.Common.Constants;
using ECommerce.Core.Common.Enums;
using ECommerce.Core.Common.Options;
using ECommerce.Core.Common.SpecsParams;
using ECommerce.Core.Dtos.CheckoutDtos;
using ECommerce.Core.Interfaces;
using ECommerce.Core.Interfaces.Services;
using ECommerce.Core.Models.OrderModule;
using ECommerce.Core.Specifications.OrderSpecifications;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;

namespace ECommerce.Application.Services;

public class CheckoutService : ICheckoutService
{
    private readonly StripeOptions _stripe;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IOrderService _orderService;
    private readonly IConfiguration _config;
    private readonly IStringLocalizer<CheckoutService> _localizer;

    public CheckoutService(
        IUnitOfWork unitOfWork,
        IOrderService orderService,
        IConfiguration config,
        IOptions<StripeOptions> stripeOptions,
        IStringLocalizer<CheckoutService> localizer
    )
    {
        _unitOfWork = unitOfWork;
        _orderService = orderService;
        _config = config;
        _localizer = localizer;
        _stripe = stripeOptions.Value;
        StripeConfiguration.ApiKey = _stripe.SecretKey;
    }

    public async Task<CheckoutResult> CheckoutAsync(int orderId, string userId)
    {
        var orderSpecs = new OrderSpecs(
            specsParams: new OrderSpecsParams { OrderId = orderId, UserId = userId },
            enablePagination: false,
            enableSorting: false,
            enableTracking: true
        );

        orderSpecs.IncludeRelatedData(o => o.User, o => o.DeliveryMethod, o => o.Items);
        var order = await _unitOfWork
            .Repository<Order>()
            .GetAsync(orderSpecs);

        if (order is null)
            return new CheckoutResult() { Message = _localizer[L.Checkout.orderNotFound] };


        var clientUrl = _config["ClientUrl"];

        if (order.PaymentMethod == Core.Common.Enums.PaymentMethod.Cash)
        {
            order.OrderStatus = OrderStatus.Processing;
            await _unitOfWork.SaveChangesAsync();
            return new CheckoutResult()
            {
                Success = true,
                Message = _localizer[L.Checkout.cashSuccess],
                RedirectUrl = $"{clientUrl}/{_stripe.SuccessRoute}"
            };
        }

        if (order.OrderStatus != OrderStatus.Pending)
            return new CheckoutResult
                { Message = _localizer[L.Checkout.alreadyProcessed, order.OrderStatus.ToString()] };

        if (!string.IsNullOrEmpty(order.CheckoutSessionId))
        {
            var existingSessionResult = await RetrieveCheckoutSessionAsync(orderId, userId);
            if (existingSessionResult.Success)
                return existingSessionResult;
        }


        var lineItems = order.Items.Select(i => new SessionLineItemOptions()
        {
            PriceData = new SessionLineItemPriceDataOptions
            {
                Currency = "usd",
                UnitAmount = (long)(i.Price * 100),
                ProductData = new SessionLineItemPriceDataProductDataOptions { Name = i.Product.Name }
            },

            Quantity = i.Quantity,
        }).ToList();

        var shippingOptions = new SessionShippingOptionOptions()
        {
            ShippingRateData = new SessionShippingOptionShippingRateDataOptions
            {
                DisplayName = order.DeliveryMethod.ShortName,
                FixedAmount = new SessionShippingOptionShippingRateDataFixedAmountOptions
                {
                    Amount = (long)(order.DeliveryMethod.Cost * 100),
                    Currency = "usd",
                },
                Type = "fixed_amount",
            }
        };

        var createOptions = new SessionCreateOptions()
        {
            SuccessUrl = $"{clientUrl}/{_stripe.SuccessRoute}",
            CancelUrl = $"{clientUrl}/{_stripe.CancelRoute}",
            PaymentMethodTypes = ["card"],
            Mode = "payment",
            Currency = "usd",
            LineItems = lineItems,
            ShippingOptions = [shippingOptions],
            ClientReferenceId = order.Id.ToString(),
            CustomerEmail = order.User.Email
        };

        var service = new SessionService();
        var session = await service.CreateAsync(createOptions);

        order.CheckoutSessionId = session.Id;
        order.PaymentStatus = PaymentStatus.AwaitingPayment;
        await _unitOfWork.SaveChangesAsync();

        return new CheckoutResult()
        {
            Success = true,
            Message = _localizer[L.Checkout.stripeSessionCreated],
            RedirectUrl = session.Url
        };
    }

    public async Task<CheckoutResult> RetrieveCheckoutSessionAsync(int orderId, string userId)
    {
        var specs = new OrderSpecs(
            specsParams: new OrderSpecsParams { OrderId = orderId, UserId = userId },
            enablePagination: false,
            enableSorting: false,
            enableTracking: true
        );

        var order = await _unitOfWork
            .Repository<Order>()
            .GetAsync(specs);

        if (order is null)
            return new CheckoutResult() { Message = _localizer[L.Checkout.orderNotFound] };

        if (string.IsNullOrEmpty(order.CheckoutSessionId))
        {
            await _orderService.CancelOrderAsync(orderId, userId);
            return new CheckoutResult() { Message = _localizer[L.Checkout.noSessionAssociated] };
        }

        var service = new SessionService();

        var session = await service.GetAsync(order.CheckoutSessionId);

        if (session is null)
            return new CheckoutResult() { Message = _localizer[L.Checkout.sessionNotFound] };

        if (session.Status == "expired")
        {
            await _orderService.CancelOrderAsync(orderId, userId);
            return new CheckoutResult { Message = _localizer[L.Checkout.sessionExpired] };
        }

        if (session.PaymentStatus == "paid")
            return new CheckoutResult() { Message = _localizer[L.Checkout.paymentCompleted] };

        return new CheckoutResult()
        {
            Success = true,
            Message = _localizer[L.Checkout.awaitingPayment],
            RedirectUrl = session.Url
        };
    }

    public async Task<ExpiredCheckoutSessionResult> ExpireCheckoutSessionAsync(string sessionId)
    {
        try
        {
            var service = new SessionService();
            await service.ExpireAsync(sessionId);
            return new ExpiredCheckoutSessionResult
                { ManageToExpire = true, Message = _localizer[L.Checkout.sessionExpiredSuccessfully, sessionId] };
        }
        catch (StripeException ex)
        {
            return new ExpiredCheckoutSessionResult
                { ManageToExpire = false, Message = _localizer[L.Checkout.sessionExpireFailed, ex.Message] };
        }
    }
}