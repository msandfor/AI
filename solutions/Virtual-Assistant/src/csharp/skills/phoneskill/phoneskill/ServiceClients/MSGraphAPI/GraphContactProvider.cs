// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Graph;
using PhoneSkill.Common;
using PhoneSkill.Models;

namespace PhoneSkill.ServiceClients.MSGraphAPI
{
    public class GraphContactProvider : IContactProvider
    {
        private IGraphServiceClient _graphClient;

        public GraphContactProvider(IGraphServiceClient serviceClient)
        {
            this._graphClient = serviceClient;
        }

        public async Task<IList<ContactCandidate>> GetContacts()
        {
            // TODO implement
            throw new NotImplementedException();
        }

        public async Task<List<User>> GetUserAsync(string name)
        {
            List<User> items = new List<User>();

            var optionList = new List<QueryOption>();
            var filterString = $"startswith(displayName, '{name}') or startswith(givenName,'{name}') or startswith(surname,'{name}') or startswith(mail,'{name}') or startswith(userPrincipalName,'{name}')";
            optionList.Add(new QueryOption("$filter", filterString));

            IGraphServiceUsersCollectionPage users = null;

            // Get the current user's profile.
            try
            {
                users = await this._graphClient.Users.Request(optionList).GetAsync();
            }
            catch (ServiceException ex)
            {
                throw GraphClient.HandleGraphAPIException(ex);
            }

            if (users?.Count > 0)
            {
                foreach (User user in users)
                {
                    // Filter out conference rooms.
                    string displayName = user.DisplayName ?? string.Empty;
                    if (!displayName.StartsWith("Conf Room"))
                    {
                        // Get user properties.
                        items.Add(user);
                    }

                    if (items.Count >= 10)
                    {
                        break;
                    }
                }
            }

            return items;
        }

        public async Task<List<Person>> GetPeopleAsync(string name)
        {
            List<Person> items = new List<Person>();
            var optionList = new List<QueryOption>();
            var filterString = $"\"{name}\"";
            optionList.Add(new QueryOption("$search", filterString));

            IUserPeopleCollectionPage users = null;

            // Get the current user's profile.
            try
            {
                users = await this._graphClient.Me.People.Request(optionList).GetAsync();
            }
            catch (ServiceException ex)
            {
                throw GraphClient.HandleGraphAPIException(ex);
            }

            if (users?.Count > 0)
            {
                foreach (Person user in users)
                {
                    // Filter out conference rooms.
                    string displayName = user.DisplayName ?? string.Empty;
                    if (!displayName.StartsWith("Conf Room"))
                    {
                        // Get user properties.
                        items.Add(user);
                    }
                }
            }

            return items;
        }

        public async Task<List<Contact>> GetContactsAsync(string name)
        {
            List<Contact> items = new List<Contact>();

            var optionList = new List<QueryOption>();
            var filterString = $"startswith(displayName, '{name}') or startswith(givenName,'{name}') or startswith(surname,'{name}')";
            optionList.Add(new QueryOption("$filter", filterString));

            IUserContactsCollectionPage contacts = null;

            // Get the current user's profile.
            try
            {
                contacts = await this._graphClient.Me.Contacts.Request(optionList).GetAsync();
            }
            catch (ServiceException ex)
            {
                throw GraphClient.HandleGraphAPIException(ex);
            }

            if (contacts?.Count > 0)
            {
                foreach (Contact contact in contacts)
                {
                    // Filter out conference rooms.
                    string displayName = contact.DisplayName ?? string.Empty;
                    if (!displayName.StartsWith("Conf Room"))
                    {
                        // Get user properties.
                        items.Add(contact);
                    }
                }
            }

            return items;
        }
    }
}
