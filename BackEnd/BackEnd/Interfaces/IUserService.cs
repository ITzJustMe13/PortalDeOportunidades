﻿
using BackEnd.Models.FrontEndModels;
using BackEnd.Responses;
using BackEnd.ServiceResponses;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.Interfaces
{
    /// <summary>
    /// This interface is responsibile for all the functions of the logic part of User
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// Function responsible to get from the db a Users info by his id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Returns a ServiceResponse with a response.Sucess=false and a message 
        /// if something is wrong or a response.Sucess=true and the User Dto</returns>
        Task<ServiceResponse<User>> GetUserByIDAsync(int id);


        /// <summary>
        /// Function that creates a new User in the DB
        /// </summary>
        /// <param name="user"></param>
        /// <returns>Returns a ServiceResponse with a response.Sucess=false and a message 
        /// if something is wrong or a response.Sucess=true and the created User dto</returns>
        Task<ServiceResponse<User>> CreateNewUserAsync(User user);


        /// <summary>
        /// Function that deletes the user from the DB by his id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Returns a ServiceResponse with a response.Sucess=false and a message 
        /// if something is wrong or a response.Sucess=true and a sucess mensage</returns>
        Task<ServiceResponse<string>> DeleteUserAsync(int id);


        /// <summary>
        /// Function that updates the user based on the updatedUser dto that it receives
        /// </summary>
        /// <param name="id"></param>
        /// <param name="updatedUser"></param>
        /// <returns>Returns a ServiceResponse with a response.Sucess=false and a message 
        /// if something is wrong or a response.Sucess=true and a sucess mensage</returns>
        Task<ServiceResponse<User>> EditUserAsync(int id, User updatedUser);


        /// <summary>
        /// Function that makes the User login
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Returns a ServiceResponse with a response.Sucess=false and a message 
        /// if something is wrong or a response.Sucess=true with a sucess mensage and a
        /// LoginResponse with a jwt token and the user dto it belongs to</returns>
        Task<ServiceResponse<LoginResponse>> LoginAsync(LoginRequest request);


        /// <summary>
        /// Function that exists with the purpose of informing the user if his email is already registered
        /// on the plataform
        /// </summary>
        /// <param name="email"></param>
        /// <returns>Returns a ServiceResponse with a response.Sucess=false and a message 
        /// if something is wrong or a response.Sucess=true and a mensage informing the user
        /// about his choice of email availability accordingly </returns>
        Task<ServiceResponse<bool>> CheckEmailAvailabilityAsync(string email);


        /// <summary>
        /// Function that activates the User account on the DB and plataform
        /// </summary>
        /// <param name="token"></param>
        /// <returns>Returns a ServiceResponse with a response.Sucess=false and a message 
        /// if something is wrong or a response.Sucess=true and a sucess mensage</returns>
        Task<ServiceResponse<string>> ActivateAccountAsync(string token);


        /// <summary>
        /// Function that activates an Impulse on a User's Opportunity
        /// </summary>
        /// <param name="impulse"></param>
        /// <returns>Returns a ServiceResponse with a response.Sucess=false and a message 
        /// if something is wrong or a response.Sucess=true, a sucess mensage and the Impulse Dto</returns>
        Task<ServiceResponse<Impulse>> ImpulseOpportunityAsync(Impulse impulse);


    }
}
