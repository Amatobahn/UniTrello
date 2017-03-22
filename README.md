# UniTrello #


### About: ###

Simplified user interfacing between Unity and Trello to create a cost-effective solution for managing issues and exception handling in real-time.
Version 1.0

# API DOCUMENTATION #

###Trello.Trello###
public string key, token.
Instantiates a trello link with assigned key and token.
```
public Trello trello = new Trello (key, token);
```

###Trello.checkWwwStatus###
public string errorMessage, public WWW www.
Checks and compares issue type from given IssueType.
```
WWW www = new WWW(string url, WWWform form);
while(!www.isDone){
     checkWwwStatus("Could not process Trello card", www);
}
```

###Trello.populateBoards###
public JsonData boardData.
Download the list of available boards for the user.
```
//Async
public IEnumerator getBoards() {
     yield return populateBoards();
}
```

###Trello.setCurrentBoard###
public string name.
Sets the current board.
```
string boardName = "Dev Hub";
if(boardName != ""){
     setCurrentBoard(boardName);
}
```

###Trello.populateLists###
public JsonData listData.
Download the list of available lists for the current board.
```
//Async
public IEnumerator getLists() {
     yield return populateLists();
}
```

###Trello.setCurrentList###
public string name.
Sets the current list.
```
//Async
public IEnumerator getLists() {
     string listName = "Bugs";
     yield return populateLists();
     if(listName != ""){
          setCurrentList(listName);
     }
}
```

###Trello.populateCards###
public JsonData cardData.
Download the list of cards from current list.
```
//Async
public IEnumerator getCards() {
     yield return populateCards();
}
```

###Trello.setCurrentCard###
public string name.
Sets the last card created from current accessed list.
```
//Async
public IEnumerator getSetCards() {
     yield return populateCards();
     setCurrentCard();
}
```

###TrelloCard###
public TrelloCard.
Create a new Trello card Object.
Uses the following fields:
string pos | string name | string desc | string due | string idList | string idLabels | string urlSource | string fileSource
```
TrelloCard card = new TrelloCard();
card.pos = "top";
card.name = "My Trello Card";
card.urlSource = "null";
card.idList = currentListId;
card.fileSource = "MyFileUrl.png";
```

###Trello.newCard###
public TrelloCard card.
Create a new Trello card object with the correct list id populated already.
```
Trello trello = new Trello(key, token);
var card = trello.newCard();
```
###Trello.uploadCard###
public TrelloCard card.
Uploads a given TrelloCard object to the Trello servers.
```
Trello trello = new Trello(key, token);
                    
//Async
public IEnumerator UploadCard() {
     yield return trello.populateBoards();
     trello.setCurrentBoard("Dev Hub");
                    
     yield return trello.populateLists();
     trello.setCurrentList("Bugs");
                    
     var card = trello.newCard();
     card.pos = "top";
     card.name = "Test Card";
     card.desc = "Card Description";
     card.idList = currentListId;
                    
     yield return trello.uploadCard(card);
}
```

###Trello.UploadAttachmentToCard###
public TrelloAttachment attachment.
Add attachment to card.
```
if(hasAttachment) {
     trello.UploadAttachmentToCard(attachment);
}
```

###TrelloLabel###
public TrelloLabel.
Create a new Trello card Label.
Uses the following fields:
string color | string name
```
TrelloLabel label = new TrelloLabel();
label.color = "Red";
label.name = "High Priority";
```

###Trello.AddLabelToCard###
public TrelloLabel label.
Add label to last known card.
```
Trello trello = new Trello(key, token);

TrelloLabel label = new TrelloLabel();
label.color = "Red";
label.name = "High Priority";

trello.AddLabelToCard(label);
```

###Trello.InitializeExceptionHandling###
Initialize exception handling for Trello exception cards.
```
Trello trello = new Trello(key, token);
void Awake() {
     trello.InitializeExceptionHandling();
}
```

###SystemInformation.BuildSystemInformation###
public bool deviceInfo, public bool graphicsInfo, public bool processorInfo.
Gathers and compiles a string of user system information.
```
SystemInformation sysInfo = new SystemInformation();
string userInfo = "";
userInfo = sysInfo.buildSystemInformation(true, true, true);
```

###JSON.RequestJSON###
public string jsonUrl.
Query a URL with extension *.json.
```
JSON json = new JSON();

string url = "/home/config.json";
string jsonText = "";

jsonText = json.RequestJSON(url);
```

###JSON.MakeJSONObject###
public string jsonUrl.
Query a URL with extension *.json.
```
JSON json = new JSON();

string url = "/home/config.json";
string jsonText = "";

jsonText = json.RequestJSON(url);
JsonData data = json.MakeJSONObject(jsonText);
```
