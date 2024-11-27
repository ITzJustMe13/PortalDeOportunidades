using BackEnd.Controllers.Data;
using BackEnd.Enums;
using BackEnd.ServiceResponses;
using BackEnd.Interfaces;
using BackEnd.Models.BackEndModels;
using BackEnd.Models.FrontEndModels;
using BackEnd.Models.Mappers;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.WebEncoders.Testing;

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

        /// <summary>
        /// Function that gets all the Opportunities that are Impulsed
        /// </summary>
        /// <returns>Returns a ServiceResponse with a response.Sucess=false and a message 
        /// if something is wrong or a response.Sucess=true and gets all the Dtos Impulsed Opportunities</returns>
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

        /// <summary>
        /// Function that gets an Opportunity by its id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Returns a ServiceResponse with a response.Sucess=false and a message 
        /// if something is wrong or a response.Sucess=true and sends the Opportunity Dto</returns>
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

        /// <summary>
        /// Function that searches Opportunities in the DB by certain filters
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="vacancies"></param>
        /// <param name="minPrice"></param>
        /// <param name="maxPrice"></param>
        /// <param name="category"></param>
        /// <param name="location"></param>
        /// <returns>Returns a ServiceResponse with a response.Sucess=false and a message 
        /// if something is wrong or a response.Sucess=true and returns the search results</returns>
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

        /// <summary>
        /// Function thate creates a Opportunity
        /// </summary>
        /// <param name="opportunityDto">Opportunity Dto</param>
        /// <returns>Returns a ServiceResponse with a response.Sucess=false and a message 
        /// if something is wrong or a response.Sucess=true, creates the Opportunity and 
        /// returns the new Dto</returns>
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
                var errors = ValidateOpportunityParameters(opportunityDto);

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

        /// <summary>
        /// Function that deletes the opportunity by its id
        /// </summary>
        /// <param name="id">Id of the opportunity</param>
        /// <returns>Returns a ServiceResponse with a response.Sucess=false and a message 
        /// if something is wrong or a response.Sucess=true and deletes the opportunity</returns>
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

        /// <summary>
        /// Function that activates the opportunity by its id
        /// </summary>
        /// <param name="id">Id of the opportunity</param>
        /// <returns>Returns a ServiceResponse with a response.Sucess=false and a message 
        /// if something is wrong or a response.Sucess=true and activates the opportunity</returns>
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

        /// <summary>
        /// Function that deactivates the opportunity by its id
        /// </summary>
        /// <param name="id">Id of the opportunity</param>
        /// <returns>Returns a ServiceResponse with a response.Sucess=false and a message 
        /// if something is wrong or a response.Sucess=true and deactivates the opportunity</returns>
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
        /// <summary>
        /// Function that edits the Opportunity by its id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="updatedOpportunity"></param>
        /// <returns>Returns a ServiceResponse with a response.Sucess=false and a message 
        /// if something is wrong or a response.Sucess=true and the updated Opportunity Dto</returns>
        public async Task<ServiceResponse<Opportunity>> EditOpportunityByIdAsync(
            int id,
            Opportunity updatedOpportunity
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
                var errors = ValidateOpportunityParameters(updatedOpportunity);

                if (errors.Any())
                {
                    response.Success = false;
                    response.Message = string.Join("; ", errors);
                    response.Type = "BadRequest";
                    return response;
                }

                // Update fields only if they have valid values
                opportunityModel.Name = updatedOpportunity.name;
                opportunityModel.Description = updatedOpportunity.description;
                opportunityModel.Price = updatedOpportunity.price;
                opportunityModel.Vacancies = updatedOpportunity.vacancies;
                opportunityModel.Category = updatedOpportunity.category;
                opportunityModel.Location = updatedOpportunity.location;
                opportunityModel.Address = updatedOpportunity.address;
                opportunityModel.Date = updatedOpportunity.date;


                if (updatedOpportunity.OpportunityImgs != null)
                {
                    // Remove existing images
                    dbContext.OpportunityImgs.RemoveRange(opportunityModel.OpportunityImgs);

                    // Add new images
                    var newImages = updatedOpportunity.OpportunityImgs.Select(url => new OpportunityImgModel
                    {
                        Image = url.image,
                        OpportunityId = opportunityModel.OpportunityId
                    }).ToList();

                    await dbContext.OpportunityImgs.AddRangeAsync(newImages);
                }

                dbContext.Opportunities.Update(opportunityModel);
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

        /// <summary>
        /// Function to validate opportunity search parameters
        /// </summary>
        /// <param name="vacancies"></param>
        /// <param name="minPrice"></param>
        /// <param name="maxPrice"></param>
        /// <param name="category"></param>
        /// <param name="location"></param>
        /// <returns>A list of error messages. If the list is empty, the validation passed.</returns>
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
        /// Validates the parameters of an Opportunity for creating or editing.
        /// </summary>
        /// <param name="opportunity">The Opportunity object to validate.</param>
        /// <returns>A list of error messages. If the list is empty, the validation passed.</returns>
        public List<string> ValidateOpportunityParameters(Opportunity opportunity)
        {
            var errors = new List<string>();

            // Validate name
            if (string.IsNullOrWhiteSpace(opportunity.name))
            {
                errors.Add("Name cannot be empty.");
            }
            else if (opportunity.name.Length > 100)
            {
                errors.Add("Name should be 100 characters or less.");
            }

            // Validate description
            if (string.IsNullOrWhiteSpace(opportunity.description))
            {
                errors.Add("Description cannot be empty.");
            }
            else if (opportunity.description.Length > 1000)
            {
                errors.Add("Description should be 1000 characters or less.");
            }

            // Validate price
            if (opportunity.price == null || opportunity.price <= 0.00M)
            {
                errors.Add("Price should be at least 0.01.");
            }

            // Validate vacancies
            if (opportunity.vacancies == null || opportunity.vacancies <= 0)
            {
                errors.Add("Vacancies should be at least one.");
            }

            // Validate category
            if (opportunity.category == null || !Enum.IsDefined(typeof(Category), opportunity.category))
            {
                errors.Add("Category is not valid.");
            }

            // Validate location
            if (opportunity.location == null || !Enum.IsDefined(typeof(Location), opportunity.location))
            {
                errors.Add("Location is not valid.");
            }

            // Validate address
            if (!string.IsNullOrWhiteSpace(opportunity.address) && opportunity.address.Length > 200)
            {
                errors.Add("Address should be 200 characters or less.");
            }

            // Validate date
            if (opportunity.date == null || opportunity.date <= DateTime.Today)
            {
                errors.Add("Date must be in the future.");
            }

            return errors;
        }

    }
}
