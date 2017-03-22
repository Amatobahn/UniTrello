using UnityEngine;
using System;
using LitJson;
using System.Collections.Generic;
using System.IO;

/// <summary>
/// UniTrello is a light-weight bridge between Unity and Trello using REST commands to parse information
/// to and from a users Trello board.
/// 
/// Aimed to benefit smaller teams with a cost effective solution for issue tracking and exception handling.
/// </summary>
namespace UniTrello {
    public class Trello {

        private string token;
        private string key;

        private JsonData boardData;
        private JsonData listData;
        private JsonData cardData;

        private const string memberBaseUrl = "https://api.trello.com/1/members/me";
        private const string boardBaseUrl = "https://api.trello.com/1/boards/";
        private const string listBaseUrl = "https://api.trello.com/1/lists/";
        private const string cardBaseUrl = "https://api.trello.com/1/cards/";

        public enum IssueType { Bug, Feedback, Request, BlueScreen, RedScreen, Exception };
        public enum IssueDept { Design, Engineering, Art_UI, Art_3D, Art_2D, Production }

        private string currentBoardId = "";
        private string currentListId = "";
        private string currentCardId = "";

        private JSON json = new JSON();

        /// <summary>
        /// Instantiates a trello link with assigned key and token.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="token"></param>
        public Trello(string key, string token) {
            this.key = key;
            this.token = token;
        }

        /// <summary>
        /// Checks if a WWW objects has resulted in an error, and if so throws an exception to deal with it.
        /// </summary>
        /// <param name="errorMessage">Error message.</param>
        /// <param name="www">The request object.</param>
        public void checkWwwStatus(string errorMessage, WWW www) {
            if (!string.IsNullOrEmpty(www.error)) {
                throw new TrelloException(errorMessage + ": " + www.error);
            }
        }

        /// <summary>
        /// Checks and compares issue type from given IssueType.
        /// </summary>
        /// <param name="issueType"></param>
        /// <returns>String of issue type.</returns>
        public string GetIssueType(IssueType issueType) {
            string type = "";

            switch (issueType) {
                case IssueType.Bug:
                    type = "Bug";
                    break;

                case IssueType.Feedback:
                    type = "Feedback";
                    break;

                case IssueType.Request:
                    type = "Request";
                    break;

                case IssueType.BlueScreen:
                    type = "Blue Screen";
                    break;

                case IssueType.RedScreen:
                    type = "Red Screen";
                    break;

                case IssueType.Exception:
                    type = "Exception";
                    break;
            }

            return type;
        }

        /// <summary>
        /// CHeckes and compares issue department from given issueDept.
        /// </summary>
        /// <param name="issueDept"></param>
        /// <returns>String of issue department.</returns>
        public string GetIssueDept(IssueDept issueDept) {
            string dept = "";

            switch (issueDept) {
                case IssueDept.Art_2D:
                    dept = "Art-2D";
                    break;

                case IssueDept.Art_3D:
                    dept = "Art-3D";
                    break;

                case IssueDept.Art_UI:
                    dept = "Art-UI";
                    break;

                case IssueDept.Design:
                    dept = "Design";
                    break;

                case IssueDept.Engineering:
                    dept = "Engineering";
                    break;

                case IssueDept.Production:
                    dept = "Production";
                    break;
            }

            return dept;
        }

        /// <summary>
        /// Download the list of available boards for the user, these are cached and allow populateLists() to function.
        /// </summary>
        /// <returns>A parsed JSON list of boards.</returns>
        /// 
        public JsonData populateBoards() {
            boardData = null;
            string boards = json.RequestJSON(memberBaseUrl + "?" + "key=" + key + "&token=" + token + "&boards=all");

            var bDict = json.MakeJSONObject(boards);

            //gDebug.gLog(boards);

            boardData = bDict["boards"];
            return boardData;
        }

        /// <summary>
        /// Sets the current board to search for lists in.
        /// </summary>
        /// <param name="name">Name of the board we're after.</param>
        public void setCurrentBoard(string name) {
            if (boardData == null) {
                throw new TrelloException("You have not yet populated the list of boards, so one cannot be selected.");
            }

            for (int i = 0; i < boardData.Count; i++) {
                var board = boardData[i];
                if ((string)board["name"] == name) {
                    currentBoardId = (string)board["id"];
                    return;
                }
            }
            currentBoardId = "";
            throw new TrelloException("No such board found.");
        }

        /// <summary>
        /// Populate the lists for the current board, these are cached for easy card uploading later.
        /// </summary>
        /// <returns>A parsed JSON Data object.</returns>
        public LitJson.JsonData populateLists() {
            listData = null;

            if (currentBoardId == "") {
                throw new TrelloException("Cannot retreive the lists, you have not selected a board yet.");
            }

            string lists = json.RequestJSON(boardBaseUrl + currentBoardId + "?" + "key=" + key + "&token=" + token + "&lists=all");

            var lDict = json.MakeJSONObject(lists);

            //gDebug.gLog(lists);

            listData = lDict["lists"];
            return listData;
        }

        /// <summary>
        /// Updates the current list to upload cards to.
        /// </summary>
        /// <param name="name">Name of the list.</param>
        public void setCurrentList(string name) {
            if (listData == null) {
                throw new TrelloException("You have not yet populated the list of lists, so one cannot be selected.");
            }

            for (int i = 0; i < listData.Count; i++) {
                var list = listData[i];
                if ((string)list["name"] == name) {
                    currentListId = (string)list["id"];
                    //gDebug.gLog(currentListId);
                    return;
                }
            }

            currentListId = "";
            throw new TrelloException("No such list found.");
        }

        /// <summary>
        /// Generate JsonData of all cards within current list id.
        /// </summary>
        /// <returns>A list of cards as JsonData</returns>
        public LitJson.JsonData populateCards() {
            cardData = null;

            string cards = json.RequestJSON(listBaseUrl + currentListId + "?" + "key=" + key + "&token=" + token + "&cards=all");

            var cDict = json.MakeJSONObject(cards);

            cardData = cDict["cards"]; //For ID
            //cardData = cDict["url"]; // For Unique URL

            return cardData;
        }

        /// <summary>
        /// Set current card id.
        /// </summary>
        public void setCurrentCard() {
            if (cardData == null) {
                throw new TrelloException("You have not yet populated the list of cards, so one cannot be selected.");
            }
            var card = cardData[0];
            currentCardId = (string)card["id"];
            return;
        }

        /// <summary>
        /// Retrieve a new Trello card objects, with the correct list id populated already.
        /// </summary>
        /// <returns>The card object.</returns>
        public TrelloCard newCard() {
            if (currentListId == "") {
                throw new TrelloException("Cannot create a card when you have not set selected a list.");
            }

            var card = new TrelloCard();
            card.idList = currentListId;
            return card;
        }

        /// <summary>
        /// Retrieve a new Trello card attachment object.
        /// </summary>
        /// <returns></returns>
        public TrelloAttachment newAttachment() {
            var attachment = new TrelloAttachment();
            return attachment;
        }
        
        /// <summary>
        /// Uploads a given TrelloCard object to the Trello servers.
        /// [pos], [name], [desc], [due], [idList], [idLabels], [urlSource], [fileSource]
        /// </summary>
        /// <returns>Your card.</returns>
        /// <param name="card">Your card.</param>
        public TrelloCard uploadCard(TrelloCard card) {
            WWWForm post = new WWWForm();
            post.AddField("key", key);
            post.AddField("token", token);

            post.AddField("pos", card.pos);
            post.AddField("name", card.name);
            post.AddField("desc", card.desc);
            post.AddField("due", card.due);
            post.AddField("idList", card.idList);
            post.AddField("idLabels", card.idLabels);
            post.AddField("urlSource", card.urlSource);
            if (card.fileSource != "") {
                post.AddBinaryData("fileSource", File.ReadAllBytes(card.fileSource));
            }
            
            WWW www = new WWW(cardBaseUrl, post);

            // Wait for request to return
            while (!www.isDone) {
                checkWwwStatus("Could not upload Trello card", www);
            }

            return card;
        }

        /// <summary>
        /// Given an exception objects condition and stack trace, a TrelloCard is created and populated with the relevant information from the exception. This is then uploaded to the Trello server.
        /// [name], [desc], [due], [idList], [idLabels]
        /// </summary>
        /// <returns>The exception card.</returns>
        /// <param name="condition">Conditoin.</param>
        /// <param name="stackTrace">Stack Trace.</param>
        private TrelloCard uploadExceptionCard(string condition, string stackTrace) {
            TrelloCard card = new TrelloCard();
            SystemInformation sysInfo = new SystemInformation();
            card.pos = "top";
            card.name = String.Format("[Exception] {0}", condition);
            card.due = DateTime.Now.ToString();
            card.desc = String.Format("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}", condition, Environment.NewLine, "--------", Environment.NewLine, "```", Environment.NewLine, stackTrace, Environment.NewLine, sysInfo.BuildSystemInformation(true, true, true), "```");
            card.idList = currentListId;
            card.fileSource = "";

            MonoBehaviour.print("IN uploadException");

            return uploadCard(card);
        }

        /// <summary>
        /// Add label to last known card.
        /// </summary>
        /// <param name="label"></param>
        /// <returns>Your label.</returns>
        public TrelloLabel AddLabelToCard(TrelloLabel label) {
            WWWForm post = new WWWForm();
            post.AddField("key", key);
            post.AddField("token", token);

            post.AddField("color", label.color);
            post.AddField("name", label.name);

            WWW www = new WWW(cardBaseUrl + currentCardId + "/labels", post);
            // Wait for request to return
            while (!www.isDone) {
                checkWwwStatus("Could not upload Label to Trello card", www);
            }

            return label;
        }
        
        /// <summary>
        /// Add Attachment to card.
        /// </summary>
        /// <param name="cardId"></param>
        /// <param name="filePath"></param>
        public TrelloAttachment UploadAttachmentToCard(TrelloAttachment attachment) {
            WWWForm post = new WWWForm();
            post.AddField("key", key);
            post.AddField("token", token);

            post.AddField("mimeType", attachment.mimeType);
            //post.AddField("file", attachment.file);
            post.AddField("url", attachment.url);
            post.AddBinaryData("file", File.ReadAllBytes(attachment.file));
            post.AddField("name", attachment.name);

            //WWW www = new WWW(cardBaseUrl + currentCardId + "/attachments" + "?" + "key=" + key + "&token=" + token, post);
            WWW www = new WWW(cardBaseUrl + currentCardId + "/attachments", post);

            // Wait for request to return
            while (!www.isDone) {
                checkWwwStatus("Could not upload attachement to Trello card", www);
            }

            return attachment;
        }

        /// <summary>
        /// Initialize exception handling for Trello exception cards
        /// </summary>
        public void InitializeExceptionHandling() {
            Application.logMessageReceived += HandleException;
            MonoBehaviour.print("Initialized Exception Handling");
        }
        
        /// <summary>
        /// Parses exceptions into strings by type and uploads exception card to Trello.
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="stackTrace"></param>
        /// <param name="type"></param>
        private void HandleException(string condition, string stackTrace, LogType type) {
            MonoBehaviour.print("IN HANDLE EXCEPTION");
            Trello trello = new Trello(key, token);
            if (type == LogType.Exception) {
                uploadExceptionCard(condition, stackTrace);
            }
        }
    }

    public class TrelloForm {

        public string summary = "";

        public TrelloForm() {

        }
    }

    public class TrelloCard {

        public string pos = "top";
        public string name = "";
        public string desc = "";
        public string due = "null";
        public string idList = "";
        public string idLabels = "";
        public string urlSource = "null";
        public string fileSource = "";

        public TrelloCard() {

        }
    }

    public class TrelloLabel {

        public string color = "";
        public string name = "";

        public TrelloLabel() {

        }
    }

    public class TrelloAttachment {

        public string file = "";
        public string url = "null";
        public string name = "";
        public string mimeType = "";

        public TrelloAttachment() {

        }
    }

    public class TrelloException : Exception {

        public TrelloException() : base() { }

        public TrelloException(string message) : base(message) { }

        public TrelloException(string format, params object[] args) : base(string.Format(format, args)) { }

        public TrelloException(string message, Exception innerException) : base(message, innerException) { }

        public TrelloException(string format, Exception innerException, params object[] args) : base(string.Format(format, args), innerException) { }
    }

    public class JSON {

        Trello trello = new Trello("","");

        /// <summary>
        /// Query a URL with extension *.json.
        /// </summary>
        /// <param name="jsonUrl"></param>
        /// <returns>JSON string</returns>
        public string RequestJSON(string jsonUrl) {
            string jsonText;
            WWW request = new WWW(jsonUrl);

            while (!request.isDone) {
                trello.checkWwwStatus("Connection to the Trello servers was not possible", request);
            }

            if (request.error == null) {
                jsonText = request.text;
                return jsonText;
            }
            else {
                Console.WriteLine(request.error);
                return null;
            }
        }

        /// <summary>
        /// Convert JSON string data to JsonData object.
        /// </summary>
        /// <param name="jsonText"></param>
        /// <returns>JsonData Object.</returns>
        public JsonData MakeJSONObject(string jsonText) {
            JsonData jsonObject;
            jsonObject = JsonMapper.ToObject(jsonText);
            return jsonObject;
        }
    }

    public class SystemInformation {
        
        public string deviceName = SystemInfo.deviceName;
        public string deviceType = SystemInfo.deviceType.ToString();
        public string operatingSystem = SystemInfo.operatingSystem;
        public string systemMemorySize = SystemInfo.systemMemorySize.ToString();

        public string graphicsDeviceName = SystemInfo.graphicsDeviceName;
        public string graphicsDeviceVendor = SystemInfo.graphicsDeviceVendor;
        public string graphicsMemorySize = SystemInfo.graphicsMemorySize.ToString();
        public string graphcicsShaderLevel = SystemInfo.graphicsShaderLevel.ToString();

        public string processorType = SystemInfo.processorType;
        public string processorCount = SystemInfo.processorCount.ToString();
        public string processorFrequency = SystemInfo.processorFrequency.ToString();

        /// <summary>
        /// Gather user system information.
        /// </summary>
        /// <param name="deviceInfo"></param>
        /// <param name="graphicsInfo"></param>
        /// <param name="processorInfo"></param>
        /// <returns>User System Information string.</returns>
        public string BuildSystemInformation(bool deviceInfo, bool graphicsInfo, bool processorInfo) {

            string sysInfo = "";

            if (deviceInfo) {
                sysInfo += String.Format("Device Name: {0}\nDevice Type: {1}\nOperating System: {2}\nPhysical Memory: {3}MB\n\n", deviceName, deviceType, operatingSystem, systemMemorySize);
            }
            if (graphicsInfo) {
                sysInfo += String.Format("Graphics Device Name: {0}\nGraphics Device Vendor: {1}\nGraphics Device Memory: {2}MB\nGraphics Shader Level: {3}\n\n", graphicsDeviceName, graphicsDeviceVendor, graphicsMemorySize, graphcicsShaderLevel);
            }
            if (processorInfo) {
                sysInfo += String.Format("Processor: {0}\nProcessor Threads: {1}\nProcessor Frequency: {2}HZ\n\n", processorType, processorCount, processorFrequency);
            }
            return sysInfo;
        }

    }
}
