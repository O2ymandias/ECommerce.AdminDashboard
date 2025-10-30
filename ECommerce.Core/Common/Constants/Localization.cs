using System.Globalization;

namespace ECommerce.Core.Common.Constants;

public static class L
{
    public static readonly CultureInfo[] SupportedCultures = [new CultureInfo("en"), new CultureInfo("ar")];
    public static readonly CultureInfo DefaultCulture = new CultureInfo("en");

    public static class Validation
    {
        public const string Required = "validation.required";
        public const string MaxLength = "validation.maxLength";
        public const string Email = "validation.email";
        public const string Phone = "validation.phone";
        public const string PasswordPolicy = "validation.passwordPolicy";
        public const string InvalidPhoneEgypt = "validation.invalidPhoneEgypt";
        public const string Range = "validation.range";
    }

    public static class ApiErrors
    {
        public const string BadRequest = "apiErrors.badRequest";
        public const string Unauthorized = "apiErrors.unauthorized";
        public const string Forbidden = "apiErrors.forbidden";
        public const string NotFound = "apiErrors.notFound";
        public const string InternalServerError = "apiErrors.internalServerError";
        public const string GenericError = "apiErrors.genericError";
    }

    public static class Auth
    {
        public static class Registration
        {
            public const string EmailAlreadyExists = "auth.registration.emailAlreadyExists";
            public const string UserNameAlreadyExists = "auth.registration.userNameAlreadyExists";
            public const string FailedToCreateUser = "auth.registration.failedToCreateUser";
            public const string FailedToAssignUserToRole = "auth.registration.failedToAssignUserToRole";
            public const string UserCreatedSuccessfully = "auth.registration.userCreatedSuccessfully";
        }

        public static class Login
        {
            public const string InvalidLogin = "auth.login.invalidLogin";
            public const string WelcomeUser = "auth.login.welcomeUser";
        }

        public static class Token
        {
            public const string RefreshTokenRequired = "auth.token.refreshTokenRequired";
            public const string RefreshTokenFailed = "auth.token.refreshTokenFailed";
            public const string InvalidToken = "auth.token.invalidToken";
            public const string InactiveToken = "auth.token.inactiveToken";
            public const string TokenRefreshed = "auth.token.tokenRefreshed";
        }
    }

    public static class Account
    {
        public const string UserNotFound = "account.userNotFound";

        public static class ResetPassword
        {
            public const string EmailSent = "account.resetPassword.emailSent";
            public const string InvalidResetLink = "account.resetPassword.invalidResetLink";
            public const string PasswordResetSuccess = "account.resetPassword.passwordResetSuccess";
        }

        public static class BasicInfo
        {
            public const string NoChanges = "account.basicInfo.noChanges";
            public const string UpdateFailed = "account.basicInfo.updateFailed";
            public const string UpdateSuccess = "account.basicInfo.updateSuccess";
        }

        public static class PasswordChange
        {
            public const string ChangeSuccess = "account.passwordChange.success";
            public const string ChangeFailed = "account.passwordChange.failed";
        }

        public static class EmailChange
        {
            public const string SameEmail = "account.emailChange.sameEmail";
            public const string IncorrectPassword = "account.emailChange.incorrectPassword";
            public const string EmailTaken = "account.emailChange.emailTaken";
            public const string VerificationSent = "account.emailChange.verificationSent";
            public const string InvalidOrExpiredCode = "account.emailChange.invalidOrExpiredCode";
            public const string ChangeFailed = "account.emailChange.changeFailed";
            public const string ChangeSuccess = "account.emailChange.changeSuccess";
        }
    }

    public static class Order
    {
        public const string CartNotFound = "order.cartNotFound";
        public const string CartEmpty = "order.cartEmpty";
        public const string DeliveryMethodUnavailable = "order.deliveryMethodUnavailable";
        public const string ProductNotExists = "order.productNotExists";
        public const string NotEnoughStock = "order.notEnoughStock";
        public const string CreatedSuccessfully = "order.createdSuccessfully";
        public const string notCancellable = "order.notCancellable";
        public const string noProductAssociated = "order.noProductAssociated";
        public const string cancelledSuccessfully = "order.cancelledSuccessfully";
        public const string cancelFailed = "order.cancelFailed";
    }

    public static class Checkout
    {
        public const string orderNotFound = "checkout.orderNotFound";
        public const string cashSuccess = "checkout.cashSuccess";
        public const string alreadyProcessed = "checkout.alreadyProcessed";
        public const string stripeSessionCreated = "checkout.stripeSessionCreated";
        public const string noSessionAssociated = "checkout.noSessionAssociated";
        public const string sessionNotFound = "checkout.sessionNotFound";
        public const string sessionExpired = "checkout.sessionExpired";
        public const string paymentCompleted = "checkout.paymentCompleted";
        public const string awaitingPayment = "checkout.awaitingPayment";
        public const string sessionExpiredSuccessfully = "checkout.sessionExpiredSuccessfully";
        public const string sessionExpireFailed = "checkout.sessionExpireFailed";
    }

    public static class Cart
    {
        public const string CartNotFound = "cart.cartNotFound";
        public const string ProductNotFound = "cart.productNotFound";
        public const string ProductNotInCart = "cart.productNotInCart";
        public const string OutOfStock = "cart.outOfStock";
        public const string AddSuccess = "cart.addSuccess";
        public const string AddFailed = "cart.addFailed";
        public const string IncreaseQuantity = "cart.increaseQuantity";
        public const string RemoveSuccess = "cart.removeSuccess";
        public const string RemoveFailed = "cart.removeFailed";
        public const string QuantityMustBeGreaterThanZero = "cart.quantityMustBeGreaterThanZero";
        public const string UpdateQuantitySuccess = "cart.updateQuantitySuccess";
        public const string UpdateQuantityFailed = "cart.updateQuantityFailed";
    }

    public static class ImageUploadMessages
    {
        public const string noImageProvided = "uploadImage.noImageProvided";
        public const string emptyImage = "uploadImage.emptyImage";
        public const string imageTooLarge = "uploadImage.imageTooLarge";
        public const string unsupportedFileType = "uploadImage.unsupportedFileType";
        public const string uploadError = "uploadImage.uploadError";
    }

    public static class General
    {
        public const string EndpointNotFound = "general.endpointNotFound";
    }
}