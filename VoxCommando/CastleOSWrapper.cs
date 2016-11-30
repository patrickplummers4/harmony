using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CastleOS_API;

namespace VoxCommando
{
    //This wrapper may not be needed although it serves to abstract the CastleOS stuff from the rest of the service.
    //It was originally experimental to get the authentication and connection keep alives working.
    //The user name and token are no longer needed in the individual methods.
    public class CastleOSWrapper
    {
        public string Token;
        public string userName;
        public CastleOSUser authenticatedUser;       

        public CastleOSWrapper()
        {
            
        }

        public string AuthenticateUser(string user, string password)
        {
            authenticatedUser = CastleOS_WCF_API.AuthenticateUser_PlainText(user, password);

            if (authenticatedUser != null)
            {
                // user is authenticated move forward by saving the
                // username and token for use with the web service
                userName = authenticatedUser.username;
                Token = authenticatedUser.securityToken;

                CastleOS_WCF_API.CastleOSUsername = userName;
                CastleOS_WCF_API.CastleOSToken = Token;


            }
            else
            {
                // user authentication failed, please try again
            }

            return Token;
        }

        public List<AutomationDevice> GetDevices(string user, string token) 
        {
            

            return CastleOS_WCF_API.GetAllDevices();
        }

        public void Switch(string deviceID, bool power, string user, string token) 
        {
           

            
                CastleOS_WCF_API.ToggleDevicePower(deviceID, power);
            
        }

        public void Dim(string deviceID, int level, string user, string token)
        {
           

            CastleOS_WCF_API.SetDeviceDimLevel(deviceID, level);
        }

        public void ToggleScene(string sceneId, bool power, string user, string token)
        {
           

            CastleOS_WCF_API.ToggleScenePower(sceneId, power);
        }

        public List<Scene> GetScenes(string user, string token)
        {
            

            return CastleOS_WCF_API.GetAllScenes();
        }

        public void SetColor(string deviceId, double hue, double saturation, double brightness, string user, string token)
        {
            

            CastleOS_WCF_API.SetDeviceColorAndBrightness(deviceId, hue, saturation, brightness);
        }

        //this command is used as a keep alive function
        public void GetAppVersion()
        {
            CastleOS_WCF_API.GetBuildVersion();
        }
    }
}
