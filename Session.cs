using Microsoft.Phone.Shell;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace inz
{
        public class Session
        {
            [JsonProperty(PropertyName = "Id")]
            public string Id { get; set; }

            [JsonProperty(PropertyName = "Token")]
            public string Token { get; set; }

            static readonly Session instance = new Session();

            private Session() { }

            public static Session Instance { get { return instance; } }
            
            public async Task<bool> IsActive()
            {
                IMobileServiceTable<Session> sessionTable = App.MobileService.GetTable<Session>();
                MobileServiceCollection<Session, Session> sessions;

                try
                {
                    sessions = await sessionTable
                                .Where(s => s.Id == this.Id)
                                .Take(1)                        
                                .ToCollectionAsync();

                    if (sessions.Count == 1) return true;
                    else return false;
                }
                catch (MobileServiceInvalidOperationException ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                    return false;
                }
            }

            public static async Task<bool> Create()
            {
                try
                {
                    IMobileServiceTable<Session> sessionTable = App.MobileService.GetTable<Session>();
                    Task<bool> active = Session.Instance.IsActive();
                    await active;
                    if (active.Result) await Session.Instance.Delete();
                    await sessionTable.InsertAsync((App.Current as App).session);
                    return true; 
                }
                catch (MobileServiceInvalidOperationException ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                    return false;
                }
            }

            public async Task<bool> Delete()
            {
                try
                {
                    IMobileServiceTable<Session> sessionTable = App.MobileService.GetTable<Session>();
                    if(this.Id==null) return false;
                    else
                    {    
                        Task<Session> previousSession = sessionTable.LookupAsync(this.Id);
                        await previousSession;
                        await sessionTable.DeleteAsync(previousSession.Result);
                        return true;   
                    }
                }
                catch (MobileServiceInvalidOperationException ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                    return false;
                }
            }      

            public static string GenerateToken(int maxSize)
                {
                    RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
                    StringBuilder token = new StringBuilder(maxSize);
                    char[] dict = new char[62];
                    dict = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
                    byte[] bytes = new byte[1];
                    rng.GetBytes(bytes);
                    bytes = new byte[maxSize];
                    rng.GetBytes(bytes);

                    foreach (byte b in bytes)
                    {
                        token.Append(dict[b % (dict.Length)]);
                    }
                    return token.ToString();
                }

            public void Restore()
            {
                this.Id = (string)PhoneApplicationService.Current.State["Id"];
                this.Token = (string)PhoneApplicationService.Current.State["Token"];
            }

            public void Save()
            {
                PhoneApplicationService.Current.State["Id"] = this.Id;
                PhoneApplicationService.Current.State["Token"] = this.Token;
            }
        }
    
    
}
