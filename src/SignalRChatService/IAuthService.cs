using System.ServiceModel;

namespace SignalRChatService {
    [ServiceContract]
    interface IAuthService {

        [OperationContract]
        bool RegisterUser(string username, string email, string password, out string errorMessage);

        [OperationContract]
        bool LoginUser(string email, string password);

        [OperationContract]
        string GetUsernameFromEmail(string email);

        [OperationContract]
        bool ChangePassword(string email, string newPassword);
    }
}
