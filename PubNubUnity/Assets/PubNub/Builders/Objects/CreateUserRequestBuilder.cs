using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace PubNubAPI
{
    public class CreateUserRequestBuilder: PubNubNonSubBuilder<CreateUserRequestBuilder, PNUserResult>, IPubNubNonSubscribeBuilder<CreateUserRequestBuilder, PNUserResult>
    {        
        private PNUserSpaceInclude[] CreateUserInclude { get; set;}
        private string CreateUserID { get; set;}
        private string CreateUserName { get; set;}
        private string CreateUserExternalID { get; set;}
        private string CreateUserProfileURL { get; set;}
        private string CreateUserEmail { get; set;}
        private Dictionary<string, object> CreateUserCustom { get; set;}
        
        public CreateUserRequestBuilder(PubNubUnity pn): base(pn, PNOperationType.PNCreateUserOperation){
        }

        #region IPubNubBuilder implementation
        public void Async(Action<PNUserResult, PNStatus> callback)
        {
            this.Callback = callback;
            base.Async(this);
        }
        #endregion

        public CreateUserRequestBuilder Include(PNUserSpaceInclude[] include){
            CreateUserInclude = include;
            return this;
        }

        public CreateUserRequestBuilder ID(string id){
            CreateUserID = id;
            return this;
        }

        public CreateUserRequestBuilder Name(string name){
            CreateUserName = name;
            return this;
        }

        public CreateUserRequestBuilder ExternalID(string externalID){
            CreateUserExternalID = externalID;
            return this;
        }

        public CreateUserRequestBuilder ProfileURL(string profileURL){
            CreateUserProfileURL = profileURL;
            return this;
        }

        public CreateUserRequestBuilder Email(string email){
            CreateUserEmail = email;
            return this;
        }

        public CreateUserRequestBuilder Custom(Dictionary<string, object> custom){
            CreateUserCustom = custom;
            return this;
        }   

        protected override void RunWebRequest(QueueManager qm){
            RequestState requestState = new RequestState ();
            requestState.OperationType = OperationType;
            requestState.httpMethod = HTTPMethod.Post;

            var cub = new { 
                id = CreateUserID, 
                email = CreateUserEmail,
                name = CreateUserName,
                profileUrl = CreateUserProfileURL,
                externalId = CreateUserExternalID,
                custom = CreateUserCustom,
            };

            string jsonUserBody = Helpers.JsonEncodePublishMsg (cub, "", this.PubNubInstance.JsonLibrary, this.PubNubInstance.PNLog);
            #if (ENABLE_PUBNUB_LOGGING)
            this.PubNubInstance.PNLog.WriteToLog (string.Format ("jsonUserBody: {0}", jsonUserBody), PNLoggingMethod.LevelInfo);
            #endif
            requestState.POSTData = jsonUserBody;

            string[] includeString = (CreateUserInclude==null) ? new string[]{} : CreateUserInclude.Select(a=>a.GetDescription().ToString()).ToArray();

            Uri request = BuildRequests.BuildObjectsCreateUserRequest(
                    string.Join(",", includeString),
                    this.PubNubInstance,
                    this.QueryParams
                );
            base.RunWebRequest(qm, request, requestState, this.PubNubInstance.PNConfig.NonSubscribeTimeout, 0, this); 
        }

        protected override void CreatePubNubResponse(object deSerializedResult, RequestState requestState){
            object[] c = deSerializedResult as object[];
            //{"status":200,"data":{"id":"id17","name":"name 17","externalId":null,"profileUrl":null,"email":"email 17","created":"2019-10-25T10:52:58.366074Z","updated":"2019-10-25T10:52:58.366074Z","eTag":"AdnSjuyx7KmDngE"}}
            PNUserResult pnUserResult = new PNUserResult();
            PNStatus pnStatus = new PNStatus();

            Debug.Log("=======>" + deSerializedResult.ToString());

            try{
                Debug.Log("=======> dictionary");
                Dictionary<string, object> dictionary = deSerializedResult as Dictionary<string, object>;
                
                if(dictionary != null) {
                    object objData;
                    dictionary.TryGetValue("data", out objData);
                    if(objData!=null){
                        Dictionary<string, object> objDataDict = objData as Dictionary<string, object>;
                        if(objDataDict != null){
                            pnUserResult = ObjectsHelpers.ExtractUser(objDataDict);
                            Debug.Log("=======> pnUserResult" + pnUserResult);
                        }  else {
                            pnUserResult = null;
                            pnStatus = base.CreateErrorResponseFromException(new PubNubException("objDataDict null"), requestState, PNStatusCategory.PNUnknownCategory);
                        }  
                    }  else {
                        Debug.Log("=======> objData null");
                        pnUserResult = null;
                        pnStatus = base.CreateErrorResponseFromException(new PubNubException("objData null"), requestState, PNStatusCategory.PNUnknownCategory);
                    }                      
                } else {
                    Debug.Log("=======> dictionary null");
                }
            } catch (Exception ex){
                Debug.Log("=======>" + ex.ToString());

                pnUserResult = null;
                pnStatus = base.CreateErrorResponseFromException(ex, requestState, PNStatusCategory.PNUnknownCategory);
            }
            Callback(pnUserResult, pnStatus);

        }

    }
}