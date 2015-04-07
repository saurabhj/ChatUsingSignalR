using System;
using System.Linq;

namespace SignalRChatService {
    public class AuthService : IAuthService {
        /// <summary>
        /// Registers a new user in the system.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="email">The email.</param>
        /// <param name="password">The password.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <returns></returns>
        public bool RegisterUser(string username, string email, string password, out string errorMessage) {
            errorMessage = String.Empty;

            try {
                using (var db = new SignalRChatEntities()) {
                    // Checking if user exists
                    var user = db.Users.SingleOrDefault(x => x.Username == username.ToLower());
                    if (user != null) {
                        errorMessage = "The specified username already exists in the system.";
                        return false;
                    }

                    user = db.Users.SingleOrDefault(x => x.Email == email.ToLower());
                    if (user != null) {
                        errorMessage = "The specified email address already exists in the system.";
                        return false;
                    }
                    

                    var newUser = new User {
                        Created = DateTime.UtcNow,
                        Email = email.ToLower(),
                        LastLogin = DateTime.UtcNow,
                        Password = SHA1Hash.Generate(password),
                        Username = username.ToLower()
                    };


                    db.Users.Add(newUser);
                    db.SaveChanges();

                    return true;
                }
            }
            catch (Exception ex) {
                //TODO: Log Exception
            }

            return false;
        }

        /// <summary>
        /// Logins the user.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        public bool LoginUser(string email, string password) {
            try {
                using (var db = new SignalRChatEntities()) {
                    var lowerEmail = email.ToLower();
                    var passwordHash = SHA1Hash.Generate(password);

                    var user = db.Users.SingleOrDefault(x => x.Email == lowerEmail && x.Password == passwordHash);

                    if (user != null) {
                        user.LastLogin = DateTime.UtcNow;
                        db.SaveChanges();

                        return true;
                    }
                }
            }
            catch (Exception ex) {
                //TODO: Handle Exception
            }

            return false;
        }

        /// <summary>
        /// Gets the username from email.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <returns></returns>
        public string GetUsernameFromEmail(string email) {
            using (var db = new SignalRChatEntities()) {
                var user = db.Users.SingleOrDefault(x => x.Email == email.ToLower());
                if (user != null) {
                    return user.Username;
                }
            }

            return null;
        }

        /// <summary>
        /// Changes the password.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="newPassword">The new password.</param>
        /// <returns></returns>
        public bool ChangePassword(string username, string newPassword) {
            try {
                using (var db = new SignalRChatEntities()) {
                    var user = db.Users.SingleOrDefault(x => x.Username == username.ToLower());
                    if (user != null) {
                        user.Password = SHA1Hash.Generate(newPassword);
                    }

                    db.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex) {
                //TODO: Log Exception

            }

            return false;
        }
    }
}
