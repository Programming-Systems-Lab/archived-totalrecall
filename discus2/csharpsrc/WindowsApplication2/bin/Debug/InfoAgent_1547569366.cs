namespace DynamicPxy {
    using System.Diagnostics;
    using System.Xml.Serialization;
    using System;
    using System.Web.Services.Protocols;
    using System.ComponentModel;
    using System.Web.Services;
    using Microsoft.Web.Services;
    using Microsoft.Web.Services.Security;
    
    
    /// <remarks/>
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Web.Services.WebServiceBindingAttribute(Name="InfoAgentSoap", Namespace="http://tempuri.org/")]
    public class InfoAgent : Microsoft.Web.Services.WebServicesClientProtocol {
        
        /// <remarks/>
        public InfoAgent() {
            this.Url = "http://128.59.14.168/TotalRecall/InfoAgent.asmx";
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapRpcMethodAttribute("http://tempuri.org/JoinMeeting", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/")]
        public string JoinMeeting(string strMeetingRequest, string strVouchers) {
            object[] results = this.Invoke("JoinMeeting", new object[] {
                        strMeetingRequest,
                        strVouchers});
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public System.IAsyncResult BeginJoinMeeting(string strMeetingRequest, string strVouchers, System.AsyncCallback callback, object asyncState) {
            return this.BeginInvoke("JoinMeeting", new object[] {
                        strMeetingRequest,
                        strVouchers}, callback, asyncState);
        }
        
        /// <remarks/>
        public string EndJoinMeeting(System.IAsyncResult asyncResult) {
            object[] results = this.EndInvoke(asyncResult);
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapRpcMethodAttribute("http://tempuri.org/InviteAgent", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/")]
        public string InviteAgent(string strMeetingRequest, string strIAUrl) {
            object[] results = this.Invoke("InviteAgent", new object[] {
                        strMeetingRequest,
                        strIAUrl});
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public System.IAsyncResult BeginInviteAgent(string strMeetingRequest, string strIAUrl, System.AsyncCallback callback, object asyncState) {
            return this.BeginInvoke("InviteAgent", new object[] {
                        strMeetingRequest,
                        strIAUrl}, callback, asyncState);
        }
        
        /// <remarks/>
        public string EndInviteAgent(System.IAsyncResult asyncResult) {
            object[] results = this.EndInvoke(asyncResult);
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapRpcMethodAttribute("http://tempuri.org/SignMeetingRequest", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/")]
        public string SignMeetingRequest(string strMeetingRequest) {
            object[] results = this.Invoke("SignMeetingRequest", new object[] {
                        strMeetingRequest});
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public System.IAsyncResult BeginSignMeetingRequest(string strMeetingRequest, System.AsyncCallback callback, object asyncState) {
            return this.BeginInvoke("SignMeetingRequest", new object[] {
                        strMeetingRequest}, callback, asyncState);
        }
        
        /// <remarks/>
        public string EndSignMeetingRequest(System.IAsyncResult asyncResult) {
            object[] results = this.EndInvoke(asyncResult);
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapRpcMethodAttribute("http://tempuri.org/CreateMeeting", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/")]
        public bool CreateMeeting(string strMeetingID, string strTopic) {
            object[] results = this.Invoke("CreateMeeting", new object[] {
                        strMeetingID,
                        strTopic});
            return ((bool)(results[0]));
        }
        
        /// <remarks/>
        public System.IAsyncResult BeginCreateMeeting(string strMeetingID, string strTopic, System.AsyncCallback callback, object asyncState) {
            return this.BeginInvoke("CreateMeeting", new object[] {
                        strMeetingID,
                        strTopic}, callback, asyncState);
        }
        
        /// <remarks/>
        public bool EndCreateMeeting(System.IAsyncResult asyncResult) {
            object[] results = this.EndInvoke(asyncResult);
            return ((bool)(results[0]));
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/InfoAgentContextUpdate", RequestNamespace="http://tempuri.org/", OneWay=true, Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public void InfoAgentContextUpdate(string strIACtxMsg) {
            this.Invoke("InfoAgentContextUpdate", new object[] {
                        strIACtxMsg});
        }
        
        /// <remarks/>
        public System.IAsyncResult BeginInfoAgentContextUpdate(string strIACtxMsg, System.AsyncCallback callback, object asyncState) {
            return this.BeginInvoke("InfoAgentContextUpdate", new object[] {
                        strIACtxMsg}, callback, asyncState);
        }
        
        /// <remarks/>
        public void EndInfoAgentContextUpdate(System.IAsyncResult asyncResult) {
            this.EndInvoke(asyncResult);
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/MeetingContextUpdate", RequestNamespace="http://tempuri.org/", OneWay=true, Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public void MeetingContextUpdate(string strMtgCtxMsg) {
            this.Invoke("MeetingContextUpdate", new object[] {
                        strMtgCtxMsg});
        }
        
        /// <remarks/>
        public System.IAsyncResult BeginMeetingContextUpdate(string strMtgCtxMsg, System.AsyncCallback callback, object asyncState) {
            return this.BeginInvoke("MeetingContextUpdate", new object[] {
                        strMtgCtxMsg}, callback, asyncState);
        }
        
        /// <remarks/>
        public void EndMeetingContextUpdate(System.IAsyncResult asyncResult) {
            this.EndInvoke(asyncResult);
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/ContextUpdate", RequestNamespace="http://tempuri.org/", OneWay=true, Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public void ContextUpdate(string strCtxResponse) {
            this.Invoke("ContextUpdate", new object[] {
                        strCtxResponse});
        }
        
        /// <remarks/>
        public System.IAsyncResult BeginContextUpdate(string strCtxResponse, System.AsyncCallback callback, object asyncState) {
            return this.BeginInvoke("ContextUpdate", new object[] {
                        strCtxResponse}, callback, asyncState);
        }
        
        /// <remarks/>
        public void EndContextUpdate(System.IAsyncResult asyncResult) {
            this.EndInvoke(asyncResult);
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/SendContextUpdate", RequestNamespace="http://tempuri.org/", OneWay=true, Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public void SendContextUpdate(string strCtxResponse, string strContactID, string strIAUrl) {
            this.Invoke("SendContextUpdate", new object[] {
                        strCtxResponse,
                        strContactID,
                        strIAUrl});
        }
        
        /// <remarks/>
        public System.IAsyncResult BeginSendContextUpdate(string strCtxResponse, string strContactID, string strIAUrl, System.AsyncCallback callback, object asyncState) {
            return this.BeginInvoke("SendContextUpdate", new object[] {
                        strCtxResponse,
                        strContactID,
                        strIAUrl}, callback, asyncState);
        }
        
        /// <remarks/>
        public void EndSendContextUpdate(System.IAsyncResult asyncResult) {
            this.EndInvoke(asyncResult);
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/SendInfoAgentContextUpdate", RequestNamespace="http://tempuri.org/", OneWay=true, Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public void SendInfoAgentContextUpdate(string strIACtxMsg, string strContactID, string strIAUrl) {
            this.Invoke("SendInfoAgentContextUpdate", new object[] {
                        strIACtxMsg,
                        strContactID,
                        strIAUrl});
        }
        
        /// <remarks/>
        public System.IAsyncResult BeginSendInfoAgentContextUpdate(string strIACtxMsg, string strContactID, string strIAUrl, System.AsyncCallback callback, object asyncState) {
            return this.BeginInvoke("SendInfoAgentContextUpdate", new object[] {
                        strIACtxMsg,
                        strContactID,
                        strIAUrl}, callback, asyncState);
        }
        
        /// <remarks/>
        public void EndSendInfoAgentContextUpdate(System.IAsyncResult asyncResult) {
            this.EndInvoke(asyncResult);
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/SendMeetingContextUpdate", RequestNamespace="http://tempuri.org/", OneWay=true, Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public void SendMeetingContextUpdate(string strMtgCtxMsg, string strContactID, string strIAUrl) {
            this.Invoke("SendMeetingContextUpdate", new object[] {
                        strMtgCtxMsg,
                        strContactID,
                        strIAUrl});
        }
        
        /// <remarks/>
        public System.IAsyncResult BeginSendMeetingContextUpdate(string strMtgCtxMsg, string strContactID, string strIAUrl, System.AsyncCallback callback, object asyncState) {
            return this.BeginInvoke("SendMeetingContextUpdate", new object[] {
                        strMtgCtxMsg,
                        strContactID,
                        strIAUrl}, callback, asyncState);
        }
        
        /// <remarks/>
        public void EndSendMeetingContextUpdate(System.IAsyncResult asyncResult) {
            this.EndInvoke(asyncResult);
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/RequestRecommendation", RequestNamespace="http://tempuri.org/", OneWay=true, Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public void RequestRecommendation(string strRecommendationRequest) {
            this.Invoke("RequestRecommendation", new object[] {
                        strRecommendationRequest});
        }
        
        /// <remarks/>
        public System.IAsyncResult BeginRequestRecommendation(string strRecommendationRequest, System.AsyncCallback callback, object asyncState) {
            return this.BeginInvoke("RequestRecommendation", new object[] {
                        strRecommendationRequest}, callback, asyncState);
        }
        
        /// <remarks/>
        public void EndRequestRecommendation(System.IAsyncResult asyncResult) {
            this.EndInvoke(asyncResult);
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/Recommend", RequestNamespace="http://tempuri.org/", OneWay=true, Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public void Recommend(string strRecommendationResponse) {
            this.Invoke("Recommend", new object[] {
                        strRecommendationResponse});
        }
        
        /// <remarks/>
        public System.IAsyncResult BeginRecommend(string strRecommendationResponse, System.AsyncCallback callback, object asyncState) {
            return this.BeginInvoke("Recommend", new object[] {
                        strRecommendationResponse}, callback, asyncState);
        }
        
        /// <remarks/>
        public void EndRecommend(System.IAsyncResult asyncResult) {
            this.EndInvoke(asyncResult);
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/SendResources", RequestNamespace="http://tempuri.org/", OneWay=true, Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public void SendResources(string strResMsg, string strContactID, string strIAUrl) {
            this.Invoke("SendResources", new object[] {
                        strResMsg,
                        strContactID,
                        strIAUrl});
        }
        
        /// <remarks/>
        public System.IAsyncResult BeginSendResources(string strResMsg, string strContactID, string strIAUrl, System.AsyncCallback callback, object asyncState) {
            return this.BeginInvoke("SendResources", new object[] {
                        strResMsg,
                        strContactID,
                        strIAUrl}, callback, asyncState);
        }
        
        /// <remarks/>
        public void EndSendResources(System.IAsyncResult asyncResult) {
            this.EndInvoke(asyncResult);
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/AddResources", RequestNamespace="http://tempuri.org/", OneWay=true, Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public void AddResources(string strResMsg) {
            this.Invoke("AddResources", new object[] {
                        strResMsg});
        }
        
        /// <remarks/>
        public System.IAsyncResult BeginAddResources(string strResMsg, System.AsyncCallback callback, object asyncState) {
            return this.BeginInvoke("AddResources", new object[] {
                        strResMsg}, callback, asyncState);
        }
        
        /// <remarks/>
        public void EndAddResources(System.IAsyncResult asyncResult) {
            this.EndInvoke(asyncResult);
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/RecallMyResources", RequestNamespace="http://tempuri.org/", OneWay=true, Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public void RecallMyResources(string strResCtxMsg) {
            this.Invoke("RecallMyResources", new object[] {
                        strResCtxMsg});
        }
        
        /// <remarks/>
        public System.IAsyncResult BeginRecallMyResources(string strResCtxMsg, System.AsyncCallback callback, object asyncState) {
            return this.BeginInvoke("RecallMyResources", new object[] {
                        strResCtxMsg}, callback, asyncState);
        }
        
        /// <remarks/>
        public void EndRecallMyResources(System.IAsyncResult asyncResult) {
            this.EndInvoke(asyncResult);
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/RecallResources", RequestNamespace="http://tempuri.org/", OneWay=true, Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public void RecallResources(string strResCtxMsg) {
            this.Invoke("RecallResources", new object[] {
                        strResCtxMsg});
        }
        
        /// <remarks/>
        public System.IAsyncResult BeginRecallResources(string strResCtxMsg, System.AsyncCallback callback, object asyncState) {
            return this.BeginInvoke("RecallResources", new object[] {
                        strResCtxMsg}, callback, asyncState);
        }
        
        /// <remarks/>
        public void EndRecallResources(System.IAsyncResult asyncResult) {
            this.EndInvoke(asyncResult);
        }
    }
}
