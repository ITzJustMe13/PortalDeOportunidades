using BackEnd.Controllers.Data;
using BackEnd.Enums;
using BackEnd.GenericClasses;
using BackEnd.Interfaces;
using BackEnd.Models.BackEndModels;
using BackEnd.Models.FrontEndModels;
using BackEnd.Models.Mappers;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;

namespace BackEnd.Services
{
    /// <summary>
    /// This class is responsible for the Opportunity logic of the program
    /// and implements the IOpportunityService Interface
    /// Has a constructor that receives a DBContext
    /// </summary>
    public class OpportunityService : IOpportunityService
    {
        
        
        private readonly ApplicationDbContext dbContext;

        public OpportunityService(ApplicationDbContext context)
        {
            dbContext = context;
        }

        /// <summary>
        /// Function that validates the opportunity search parameters
        /// </summary>
        /// <param name="vacancies"></param>
        /// <param name="minPrice"></param>
        /// <param name="maxPrice"></param>
        /// <param name="category"></param>
        /// <param name="location"></param>
        /// <returns>Returns a List of Strings (errors) or a empty List based 
        /// of the validation of the parameters </returns>
        public async Task<ServiceResponse<IEnumerable<Opportunity>>> GetAllOpportunitiesAsync()
        {
            var response = new ServiceResponse<IEnumerable<Opportunity>>();

            try
            {
                if (dbContext == null)
                {
                    response.Success = false;
                    response.Message = "DB context missing.";
                    response.Type = "NotFound";
                    return response;
                }

                var opportunityModels = await dbContext.Opportunities
                    .Include(o => o.OpportunityImgs) // Include related images
                    .ToListAsync();

                if (!opportunityModels.Any())
                {
                    response.Success = false;
                    response.Message = "No Opportunities were found.";
                    response.Type = "NotFound";
                    return response;
                }

                // Map models to DTOs
                var opportunityDtos = opportunityModels.Select(OpportunityMapper.MapToDto).ToList();
                response.Data = opportunityDtos;
                response.Success = true;
                response.Message = "Opportunities retrieved successfully.";
                response.Type = "Ok";
            }
            catch (ValidationException ex)
            {
                response.Success = false;
                response.Message = "Validation error occurred.";
                response.Type = "BadRequest";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "An unexpected error occurred.";
                response.Type = "InternalServerError";
            }

            return response;
        }

        public async Task<ServiceResponse<IEnumerable<Opportunity>>> GetAllImpulsedOpportunitiesAsync()
        {
            var response = new ServiceResponse<IEnumerable<Opportunity>>();

            try
            {
                if (dbContext == null)
                {
                    response.Success = false;
                    response.Message = "DB context missing.";
                    response.Type = "NotFound";
                    return response;
                }

                var opportunityModels = await dbContext.Opportunities
                    .Where(o => o.IsImpulsed == true)
                    .Include(o => o.OpportunityImgs)
                    .ToListAsync();

                if (!opportunityModels.Any())
                {
                    response.Success = false;
                    response.Message = "No impulsed opportunities were found.";
                    response.Type = "NotFound";
                    return response;
                }

                // Map models to DTOs
                var opportunityDtos = opportunityModels.Select(OpportunityMapper.MapToDto).ToList();
                response.Data = opportunityDtos;
                response.Success = true;
                response.Message = "Impulsed opportunities retrieved successfully.";
                response.Type = "Ok";
            }
            catch (ValidationException ex)
            {
                response.Success = false;
                response.Message = "Validation error occurred.";
                response.Type = "BadRequest";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "An unexpected error occurred.";
                response.Type = "InternalServerError";
            }

            return response;
        }

        public async Task<ServiceResponse<Opportunity>> GetOpportunityByIdAsync(int id)
        {
            var response = new ServiceResponse<Opportunity>();

            try
            {
                if (dbContext == null)
                {
                    response.Success = false;
                    response.Message = "DB context is missing.";
                    response.Type = "NotFound";
                    return response;
                }

                if (id <= 0)
                {
                    response.Success = false;
                    response.Message = "Given opportunityId is invalid, it should be greater than 0.";
                    response.Type = "BadRequest";
                    return response;
                }

                var opportunity = await dbContext.Opportunities
                    .Include(o => o.OpportunityImgs)
                    .FirstOrDefaultAsync(o => o.OpportunityId == id);

                if (opportunity == null)
                {
                    response.Success = false;
                    response.Message = $"Opportunity with id {id} not found.";
                    response.Type = "NotFound";
                    return response;
                }

                // Map to DTO
                var opportunityDto = OpportunityMapper.MapToDto(opportunity);
                response.Data = opportunityDto;
                response.Success = true;
                response.Message = "Opportunity retrieved successfully.";
                response.Type = "Ok";
            }
            catch (ValidationException ex)
            {
                response.Success = false;
                response.Message = "Validation error occurred.";
                response.Type = "BadRequest";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "An unexpected error occurred.";
                response.Type = "InternalServerError";
            }

            return response;
        }

        public async Task<ServiceResponse<List<Opportunity>>> GetAllOpportunitiesByUserIdAsync(int userId)
        {
            var response = new ServiceResponse<List<Opportunity>>();

            try
            {
                if (dbContext == null)
                {
                    response.Success = false;
                    response.Message = "DB context is missing.";
                    response.Type = "NotFound";
                    return response;
                }

                if (userId <= 0)
                {
                    response.Success = false;
                    response.Message = "Given userId is invalid, it should be greater than 0.";
                    response.Type = "BadRequest";
                    return response;
                }

                var opportunities = await dbContext.Opportunities
                    .Where(e => e.UserID == userId)
                    .Include(o => o.OpportunityImgs)
                    .ToListAsync();

                if (!opportunities.Any())
                {
                    response.Success = false;
                    response.Message = $"Opportunities with userId {userId} not found.";
                    response.Type = "NotFound";
                    return response;
                }

                // Map opportunities to DTOs
                var opportunityDtos = opportunities.Select(OpportunityMapper.MapToDto).ToList();
                response.Data = opportunityDtos;
                response.Success = true;
                response.Message = "Opportunities retrieved successfully.";
                response.Type = "Ok";
            }
            catch (ValidationException ex)
            {
                response.Success = false;
                response.Message = "Validation error occurred.";
                response.Type = "BadRequest";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "An unexpected error occurred.";
                response.Type = "InternalServerError";
            }

            return response;
        }

        public async Task<ServiceResponse<List<Opportunity>>> SearchOpportunitiesAsync(
            string? keyword,
            int? vacancies,
            decimal? minPrice,
            decimal? maxPrice,
            Category? category,
            Location? location
        )
        {
            var response = new ServiceResponse<List<Opportunity>>();

            try
            {
                if (dbContext == null)
                {
                    response.Success = false;
                    response.Message = "DB context is missing.";
                    response.Type = "NotFound";
                    return response;
                }

                // Validate search parameters
                var errors = ValidateSearchParameters(vacancies, minPrice, maxPrice, category, location);
                if (errors.Any())
                {
                    response.Success = false;
                    response.Message = "Invalid search parameters.";
                    response.Type = "BadRequest";
                    return response;
                }

                // Build the query dynamically
                var query = dbContext.Opportunities.AsQueryable();

                if (!string.IsNullOrEmpty(keyword))
                    query = query.Where(o => o.Name.Contains(keyword) || (o.Description != null && o.Description.Contains(keyword)));

                if (vacancies.HasValue)
                    query = query.Where(o => o.Vacancies >= vacancies.Value);

                if (minPrice.HasValue)
                    query = query.Where(o => o.Price >= minPrice.Value);

                if (maxPrice.HasValue)
                    query = query.Where(o => o.Price <= maxPrice.Value);

                if (category.HasValue)
                    query = query.Where(o => o.Category == category.Value);

                if (location.HasValue)
                    query = query.Where(o => o.Location == location.Value);

                // Execute query
                var opportunities = await query.Include(o => o.OpportunityImgs).ToListAsync();

                // Map to DTOs
                var opportunityDtos = opportunities.Select(OpportunityMapper.MapToDto).ToList();

                response.Data = opportunityDtos;
                response.Success = true;
                response.Message = "Opportunities retrieved successfully.";
                response.Type = "Ok";
            }
            catch (ValidationException ex)
            {
                response.Success = false;
                response.Message = "Validation error occurred.";
                response.Type = "BadRequest";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "An unexpected error occurred.";
                response.Type = "InternalServerError";
            }

            return response;
        }

        public async Task<ServiceResponse<Opportunity>> CreateOpportunityAsync(Opportunity opportunityDto)
        {
            var response = new ServiceResponse<Opportunity>();

            try
            {
                if (dbContext == null)
                {
                    response.Success = false;
                    response.Message = "DB context is missing.";
                    response.Type = "NotFound";
                    return response;
                }

                // Validate input parameters
                var errors = ValidateOpportunityParameters(
                    opportunityDto.name,
                    opportunityDto.description,
                    opportunityDto.price,
                    opportunityDto.vacancies,
                    opportunityDto.category,
                    opportunityDto.location,
                    opportunityDto.address,
                    opportunityDto.date,
                    true // This is a creation request
                );

                if (errors.Any())
                {
                    response.Success = false;
                    response.Message = "Validation failed.";
                    response.Type = "BadRequest";
                    return response;
                }

                // Check if the user exists
                var userExists = await dbContext.Users.AnyAsync(u => u.UserId == opportunityDto.userId);
                if (!userExists)
                {
                    response.Success = false;
                    response.Message = "Invalid User ID. User does not exist.";
                    response.Type = "BadRequest";
                    return response;
                }

                // Initialize review score
                var opportunityModel = OpportunityMapper.MapToModel(opportunityDto);
                opportunityModel.Score = 0.0F;

                // If there are images, map them to the model
                if (opportunityDto.OpportunityImgs != null && opportunityDto.OpportunityImgs.Any())
                {
                    opportunityModel.OpportunityImgs = opportunityDto.OpportunityImgs
                        .Select(OpportunityImgMapper.MapToModel)
                        .ToList();
                }

                // Save the opportunity to the database
                await dbContext.Opportunities.AddAsync(opportunityModel);
                await dbContext.SaveChangesAsync();

                // Map the created model back to DTO
                var createdOpportunityDto = OpportunityMapper.MapToDto(opportunityModel);

                response.Data = createdOpportunityDto;
                response.Success = true;
                response.Message = "Opportunity created successfully.";
                response.Type = "Created";
            }
            catch (ValidationException ex)
            {
                response.Success = false;
                response.Message = "Validation exception occurred.";
                response.Type = "BadRequest";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "An unexpected error occurred.";
                response.Type = "InternalServerError";
            }

            return response;
        }

        public async Task<ServiceResponse<bool>> DeleteOpportunityByIdAsync(int id)
        {
            var response = new ServiceResponse<bool>();

            try
            {
                if (dbContext == null)
                {
                    response.Success = false;
                    response.Message = "DB context is missing.";
                    response.Type = "NotFound";
                    return response;
                }

                if (id <= 0)
                {
                    response.Success = false;
                    response.Message = "Given opportunityId is invalid, it should be greater than 0.";
                    response.Type = "BadRequest";
                    return response;
                }

                var opportunityModel = await dbContext.Opportunities.FindAsync(id);

                if (opportunityModel == null)
                {
                    response.Success = false;
                    response.Message = $"Opportunity with id {id} not found.";
                    response.Type = "NotFound";
                    return response;
                }

                bool hasActiveReservations = await dbContext.Reservations
                    .AnyAsync(r => r.opportunityID == id && r.isActive);

                if (hasActiveReservations)
                {
                    response.Success = false;
                    response.Message = "This Opportunity still has active reservations attached.";
                    response.Type = "BadRequest";
                    return response;
                }

                // No active reservations, safe to delete
                dbContext.Opportunities.Remove(opportunityModel);
                await dbContext.SaveChangesAsync();

                response.Data = true; // Successfully deleted
                response.Success = true;
                response.Message = "Opportunity deleted successfully.";
                response.Type = "NoContent";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "An unexpected error occurred while deleting the opportunity.";
                response.Type = "InternalServerError";
            }

            return response;
        }

        public async Task<ServiceResponse<bool>> ActivateOpportunityByIdAsync(int id)
        {
            var response = new ServiceResponse<bool>();

            try
            {
                if (dbContext == null)
                {
                    response.Success = false;
                    response.Message = "DB context is missing.";
                    response.Type = "NotFound";
                    return response;
                }

                if (id <= 0)
                {
                    response.Success = false;
                    response.Message = "Given opportunityId is invalid, it should be greater than 0.";
                    response.Type = "BadRequest";
                    return response;
                }

                var opportunityModel = await dbContext.Opportunities.FindAsync(id);
                if (opportunityModel == null)
                {
                    response.Success = false;
                    response.Message = $"Opportunity with id {id} not found.";
                    response.Type = "NotFound";
                    return response;
                }

                if (opportunityModel.IsActive)
                {
                    response.Success = false;
                    response.Message = "Opportunity is already active.";
                    response.Type = "BadRequest";
                    return response;
                }

                // Activate the opportunity
                opportunityModel.IsActive = true;
                await dbContext.SaveChangesAsync();

                response.Data = true; // Successfully activated
                response.Success = true;
                response.Message = "Opportunity activated successfully.";
                response.Type = "NoContent";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "An unexpected error occurred while activating the opportunity.";
                response.Type = "InternalServerError";
            }

            return response;
        }

        public async Task<ServiceResponse<bool>> DeactivateOpportunityByIdAsync(int id)
        {
            var response = new ServiceResponse<bool>();

            try
            {
                if (dbContext == null)
                {
                    response.Success = false;
                    response.Message = "DB context is missing.";
                    response.Type = "NotFound";
                    return response;
                }

                if (id <= 0)
                {
                    response.Success = false;
                    response.Message = "Given opportunityId is invalid, it should be greater than 0.";
                    response.Type = "BadRequest";
                    return response;
                }

                var opportunityModel = await dbContext.Opportunities.FindAsync(id);
                if (opportunityModel == null)
                {
                    response.Success = false;
                    response.Message = $"Opportunity with id {id} not found.";
                    response.Type = "NotFound";
                    return response;
                }

                if (!opportunityModel.IsActive)
                {
                    response.Success = false;
                    response.Message = "Opportunity is already inactive.";
                    response.Type = "BadRequest";
                    return response;
                }

                // Deactivate the opportunity
                opportunityModel.IsActive = false;
                await dbContext.SaveChangesAsync();

                response.Data = true; // Successfully deactivated
                response.Success = true;
                response.Message = "Opportunity deactivated successfully.";
                response.Type = "NoContent";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "An unexpected error occurred while deactivating the opportunity.";
                response.Type = "InternalServerError";
            }

            return response;
        }

        public async Task<ServiceResponse<Opportunity>> EditOpportunityByIdAsync(
            int id,
            string? name,
            string? description,
            decimal? price,
            int? vacancies,
            Category? category,
            Location? location,
            string? address,
            DateTime? date,
            List<byte[]>? newImageUrls
        )
        {
            var response = new ServiceResponse<Opportunity>();

            try
            {
                if (dbContext == null)
                {
                    response.Success = false;
                    response.Message = "DB context is missing.";
                    response.Type = "NotFound";
                    return response;
                }

                if (id <= 0)
                {
                    response.Success = false;
                    response.Message = "Given opportunityId is invalid, it should be greater than 0.";
                    response.Type = "BadRequest";
                    return response;
                }

                var opportunityModel = await dbContext.Opportunities
                    .Include(o => o.OpportunityImgs)
                    .FirstOrDefaultAsync(o => o.OpportunityId == id);

                if (opportunityModel == null)
                {
                    response.Success = false;
                    response.Message = $"Opportunity with id {id} not found.";
                    response.Type = "BadRequest";
                    return response;
                }

                // Validate the provided parameters for editing
                var errors = ValidateOpportunityParameters(
                    name,
                    description,
                    price,
                    vacancies,
                    category,
                    location,
                    address,
                    date,
                    false // Indicating this is an edit request
                );

                if (errors.Any())
                {
                    response.Success = false;
                    response.Message = string.Join("; ", errors);
                    response.Type = "BadRequest";
                    return response;
                }

                // Update fields only if they have valid values
                if (!string.IsNullOrEmpty(name)) opportunityModel.Name = name;
                if (!string.IsNullOrEmpty(description)) opportunityModel.Description = description;
                if (price.HasValue) opportunityModel.Price = price.Value;
                if (vacancies.HasValue) opportunityModel.Vacancies = vacancies.Value;
                if (category.HasValue) opportunityModel.Category = category.Value;
                if (location.HasValue) opportunityModel.Location = location.Value;
                if (!string.IsNullOrEmpty(address)) opportunityModel.Address = address;
                if (date.HasValue) opportunityModel.Date = date.Value;

                if (newImageUrls != null)
                {
                    // Remove existing images
                    dbContext.OpportunityImgs.RemoveRange(opportunityModel.OpportunityImgs);

                    // Add new images
                    var newImages = newImageUrls.Select(url => new OpportunityImgModel
                    {
                        Image = url,
                        OpportunityId = opportunityModel.OpportunityId
                    }).ToList();

                    await dbContext.OpportunityImgs.AddRangeAsync(newImages);
                }

                await dbContext.SaveChangesAsync();

                // Map to DTO to return
                var opportunityDto = OpportunityMapper.MapToDto(opportunityModel);
                response.Data = opportunityDto;
                response.Success = true;
                response.Message = "Opportunity edited successfully.";
                response.Type = "Ok";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "An unexpected error occurred while editing the opportunity.";
                response.Type = "InternalServerError";
            }

            return response;
        }


        public List<string> ValidateSearchParameters(int? vacancies, decimal? minPrice, decimal? maxPrice, Category? category, Location? location)
        {
            var errors = new List<string>();

            if (vacancies.HasValue && vacancies <= 0)
                errors.Add("Vacancies must be greater than zero.");

            if (minPrice.HasValue && minPrice <= 0.00M)
                errors.Add("MinPrice should be greater than 0.01.");

            if (maxPrice.HasValue && maxPrice <= 0.00M)
                errors.Add("MaxPrice should be greater than 0.01.");

            if (minPrice.HasValue && maxPrice.HasValue)
            {
                if (minPrice > maxPrice)
                {
                    errors.Add("MaxPrice should be greater than MinPrice.");
                }
            }

            if (category.HasValue && !Enum.IsDefined(typeof(Category), category.Value))
                errors.Add("Invalid category specified.");

            if (location.HasValue && !Enum.IsDefined(typeof(Location), location.Value))
                errors.Add("Invalid location specified.");

            return errors;
        }

        /// <summary>
        /// Function that validates the Opportunity parameters for creating or editing an Opportunity
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="price"></param>
        /// <param name="vacancies"></param>
        /// <param name="category"></param>
        /// <param name="location"></param>
        /// <param name="address"></param>
        /// <param name="date"></param>
        /// <param name="isCreation"></param>
        /// <returns>Returns a List of Strings (errors) or a empty List based 
        /// of the validation of the parameters </returns>
        public List<string> ValidateOpportunityParameters(
           string? name,
           string? description,
           decimal? price,
           int? vacancies,
           Category? category,
           Location? location,
           string? address,
           DateTime? date,
           bool isCreation // Indicate if this validation is for creating the Opp
)
        {
            var errors = new List<string>();

            if (isCreation)
            {
                if (string.IsNullOrWhiteSpace(name))
                    errors.Add("Name cannot be empty.");
                else if (name.Length > 100)
                    errors.Add("Name should be 100 characters or less.");

                if (string.IsNullOrWhiteSpace(description))
                    errors.Add("Description cannot be empty.");
                else if (description.Length > 1000)
                    errors.Add("Description should be 1000 characters or less.");
            }

            if (price.HasValue && price <= 0.00M)
                errors.Add("Price should be at least 0.01.");

            if (vacancies.HasValue && vacancies <= 0)
                errors.Add("Vacancies should be at least one.");

            if (category.HasValue && !Enum.IsDefined(typeof(Category), category.Value))
                errors.Add("Category is not valid.");

            if (location.HasValue && !Enum.IsDefined(typeof(Location), location.Value))
                errors.Add("Location is not valid.");

            if (!string.IsNullOrWhiteSpace(address) && address.Length > 200)
                errors.Add("Address should be 200 characters or less.");

            if (date.HasValue && date <= DateTime.Today)
                errors.Add("Date must be in the future.");

            return errors;
        }
    }
}
