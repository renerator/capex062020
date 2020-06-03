using CapexIdentity.Business;
using CapexIdentity.Entities;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapexIdentity.IdentityStore
{
    public class UserStore : IUserStore<ApplicationUser>, IUserRoleStore<ApplicationUser>
    {
        /* ------------------------------------------------------------------------------------
         * 
         * PMO360
         * Av. Nueva Tajamar 481 Of 1403 - Vitacura, Santiago
         * http://www.pmo360.cl
         * 
         * -----------------------------------------------------------------------------------
         * 
         * CLIENTE          : AMSA - ANTOFAGASTA MINERALS
         * PRODUCTO         : CAPEX
         * RESPONABILIDAD   : IDENTIDAD & ROL MANAGEMENT
         * TIPO             : LOGICA DE NEGOCIO
         * DESARROLLADO POR : PMO360
         * FECHA            : 2018
         * VERSION          : 0.0.1
         * PROPOSITO        : 
         * 
         * 
         */
        #region IUserStore
        public Task CreateAsync(ApplicationUser user)
        {
            if (user != null)
            {
                return Task.Factory.StartNew(() =>
                {
                    user.Id = Guid.NewGuid().ToString();
                    UserController.NewUser(user);
                });
            }
            throw new ArgumentNullException("user");
        }

        public Task DeleteAsync(ApplicationUser user)
        {
            if (user != null)
            {
                return Task.Factory.StartNew(() =>
                {
                    UserController.DeleteUser(user);
                });
            }
            throw new ArgumentNullException("user");
        }

        public void Dispose()
        {

        }

        public Task<ApplicationUser> FindByIdAsync(string userId)
        {
            if (!string.IsNullOrEmpty(userId))
            {
                return Task.Factory.StartNew(() =>
                {
                    return UserController.GetUser(userId);
                });
            }
            throw new ArgumentNullException("userId");
        }

        public Task<ApplicationUser> FindByNameAsync(string userName)
        {
            if (!string.IsNullOrEmpty(userName))
            {
                return Task.Factory.StartNew(() =>
                {
                    return UserController.GetUserByUsername(userName);
                });
            }
            throw new ArgumentNullException("userName");
        }

        public ApplicationUser FindByName(string userName)
        {
            if (!string.IsNullOrEmpty(userName))
            {
                return UserController.GetUserByUsername(userName);
            }
            throw new ArgumentNullException("userName");
        }

        public Task UpdateAsync(ApplicationUser user)
        {
            if (user != null)
            {
                return Task.Factory.StartNew(() =>
                {
                    return UserController.UpdateUser(user);
                });
            }
            throw new ArgumentNullException("userName");
        }
        #endregion

        #region IUserRoleStore
        public Task AddToRoleAsync(ApplicationUser user, string roleName)
        {
            if (user != null)
            {
                return Task.Factory.StartNew(() =>
                {
                    UserRoleController.NewUserRole(user.Id, roleName);
                });
            }
            else
            {
                throw new ArgumentNullException("user");
            }
        }

        public Task RemoveFromRoleAsync(ApplicationUser user, string roleName)
        {
            if (user != null)
            {
                return Task.Factory.StartNew(() =>
                {
                    UserRoleController.DeleteUserRole(user.Id, roleName);
                });
            }
            else
            {
                throw new ArgumentNullException("user");
            }
        }

        public Task<IList<string>> GetRolesAsync(ApplicationUser user)
        {
            if (user != null)
            {
                return Task.Factory.StartNew(() =>
                {
                    IList<string> roles = UserRoleController.GetUserRoles(user.Id);
                    return roles;
                });
            }
            else
            {
                throw new ArgumentNullException("user");
            }
        }

        public Task<bool> IsInRoleAsync(ApplicationUser user, string roleName)
        {
            if (user != null)
            {
                return Task.Factory.StartNew(() =>
                {
                    IList<string> roles = UserRoleController.GetUserRoles(user.Id);
                    foreach (string role in roles)
                    {
                        if (role.ToUpper() == roleName.ToUpper())
                        {
                            return true;
                        }
                    }

                    return false;
                });
            }
            else
            {
                throw new ArgumentNullException("user");
            }
        }
        #endregion

    }
}
