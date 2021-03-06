﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using InvoiceCqrs.Domain.Entities;
using InvoiceCqrs.Messages.Queries.Users;
using InvoiceCqrs.Messages.Shared;
using InvoiceCqrs.Persistence;
using MediatR;

namespace InvoiceCqrs.Handlers.Query.Users
{
    public class SearchUsersHandler : IRequestHandler<SearchUsers, IList<User>>
    {
        private readonly IUnitOfWork _UnitOfWork;

        public SearchUsersHandler(IUnitOfWork unitOfWork)
        {
            _UnitOfWork = unitOfWork;
        }

        public IList<User> Handle(SearchUsers message)
        {
            // There a better way to do this?
            const string query =
                @"  SELECT u.Id, u.Email, u.FirstName, u.LastName
                    FROM Users.[User] u
                    WHERE 
                        (@Email IS NOT NULL AND u.Email = @Email) {CompOper}
                        (@FirstName IS NOT NULL AND u.FirstName = @FirstName) {CompOper}
                        (@UserIdIsDefaultValue = 0 AND u.Id = @UserId) {CompOper}
                        (@LastName IS NOT NULL AND u.LastName = @LastName)";

            var operators = new Dictionary<SearchOptions, string>
            {
                {SearchOptions.MatchAny, "OR"},
                {SearchOptions.MatchAll, "AND"}
            };

            var actualQuery = query.Replace("{CompOper}", operators[message.SearchOption]);
            var results = _UnitOfWork.Query<User>(actualQuery, new
            {
                message.Email,
                message.FirstName,
                message.LastName,
                UserIdIsDefaultValue = message.UserId == default(Guid) ? 1 : 0,
                message.UserId
            });

            return results.ToList();
        }
    }
}
