using AutoMapper;
using ECommerce.Core.Common.Pagination;
using ECommerce.Core.Common.SpecsParams;
using ECommerce.Core.Dtos.RatingDtos;
using ECommerce.Core.Interfaces;
using ECommerce.Core.Interfaces.Services;
using ECommerce.Core.Models.ProductModule;
using ECommerce.Core.Specifications.RatingSpecifications;

namespace ECommerce.Application.Services;

public class RatingService(IUnitOfWork unitOfWork, IMapper mapper, ICultureService cultureService)
    : IRatingService
{
    public async Task<RatingPaginationResult> GetRatingsAsync(RatingSpecsParams specsParams)
    {
        var canTranslate = cultureService.CanTranslate;

        var ratingItemsSpecs = new RatingSpecs(
            specsParams: specsParams,
            enablePagination: true,
            enableSorting: true,
            enableTracking: false,
            enableSplittingQuery: canTranslate
        );
        ratingItemsSpecs.IncludeRelatedData(r => r.User, r => r.Product);

        if (canTranslate)
            ratingItemsSpecs.IncludeRelatedData(r => r.Product.Translations);

        var ratings = await unitOfWork
            .Repository<Rating>()
            .GetAllAsync(ratingItemsSpecs);

        var (totalRatings, average) = await GetTotalAndAverageAsync(specsParams.ProductId);

        return new RatingPaginationResult()
        {
            PageNumber = specsParams.PageNumber,
            PageSize = specsParams.PageSize,
            Total = totalRatings,
            Average = average,
            Results = mapper.Map<IReadOnlyList<RatingResult>>(ratings)
        };
    }

    public async Task<double> GetAverageRatingAsync(int productId)
    {
        var (_, average) = await GetTotalAndAverageAsync(productId);
        return average;
    }

    public async Task<bool> AddRatingAsync(RatingInput input, string userId)
    {
        var specs = new RatingSpecs(
            specsParams: new RatingSpecsParams { ProductId = input.ProductId, UserId = userId },
            enablePagination: false,
            enableSorting: false,
            enableTracking: true,
            enableSplittingQuery: false
        );

        var existingRating = await unitOfWork
            .Repository<Rating>()
            .GetAsync(specs);

        if (existingRating is not null)
        {
            existingRating.Stars = input.Stars;
            existingRating.Title = input.Title;
            existingRating.Comment = input.Comment;
            existingRating.CreatedAt = DateTime.UtcNow;
        }
        else
        {
            var newRating = new Rating()
            {
                UserId = userId,
                ProductId = input.ProductId,
                Stars = input.Stars,
                Title = input.Title,
                Comment = input.Comment,
            };
            unitOfWork.Repository<Rating>().Add(newRating);
        }

        return await unitOfWork.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteRatingAsync(int ratingId)
    {
        var specs = new RatingSpecs(
            specsParams: new RatingSpecsParams { RatingId = ratingId },
            enablePagination: false,
            enableSorting: false,
            enableTracking: true,
            enableSplittingQuery: false
        );

        var rating = await unitOfWork
            .Repository<Rating>()
            .GetAsync(specs);

        if (rating is null) return false;

        unitOfWork.Repository<Rating>().Delete(rating);
        return await unitOfWork.SaveChangesAsync() > 0;
    }

    private async Task<(int TotalRatings, double Average)> GetTotalAndAverageAsync(int? productId)
    {
        if (productId is null) return (0, 0);

        var specs = new RatingSpecs(
            specsParams: new RatingSpecsParams { ProductId = productId },
            enablePagination: false,
            enableSorting: false,
            enableTracking: false,
            enableSplittingQuery: false
        );

        var ratings = await unitOfWork
            .Repository<Rating>()
            .GetAllAsync(specs);

        var average = ratings.Count > 0 ? Math.Round(ratings.Average(r => r.Stars), 1) : 0.0;
        var totalRatings = ratings.Count;

        return (totalRatings, average);
    }

    public async Task<LatestRatingsResult> GetLatestRatingsAsync(string userId, int limit)
    {
        var canTranslate = cultureService.CanTranslate;

        var ratingsSpecsParams = new RatingSpecsParams()
        {
            PageNumber = 1,
            PageSize = limit,
            UserId = userId
        };

        var ratingSpecs = new RatingSpecs(
            specsParams: ratingsSpecsParams,
            enablePagination: true,
            enableSorting: true,
            enableTracking: false,
            enableSplittingQuery: canTranslate
        );

        ratingSpecs.IncludeRelatedData(r => r.User, r => r.Product);

        if (canTranslate)
            ratingSpecs.IncludeRelatedData(r => r.Product.Translations);


        var ratings = await unitOfWork
            .Repository<Rating>()
            .GetAllAsync(ratingSpecs);

        var countSpecsParams = new RatingSpecsParams() { UserId = userId };
        var countSpecs = new RatingSpecs(
            specsParams: countSpecsParams,
            enablePagination: false,
            enableSorting: false,
            enableTracking: false,
            enableSplittingQuery: false
        );

        var count = await unitOfWork
            .Repository<Rating>()
            .CountAsync(countSpecs);

        return new LatestRatingsResult
        {
            Count = count,
            LatestRatings = mapper.Map<IReadOnlyList<RatingResult>>(ratings)
        };
    }
}