Notes:

1) In the context of this implementation:
- Server: Runs on premise, will response to requests and return data
- Client: Runs entity that will make requests, as of this writing it is the main web site and API, however in future this could
  be stand alone clients

2) The server side will have access to actual keys used to connect to RPC Server
- Question: Are SAS tokens faster?  If so may want to consider using these

3) In AppSettings there is a RunTimeTokenBroker that will generate tokens that will allow a specific 
   on premise instance access to subscribe to messages from a specific topic.  

4) In AppSettings it will return a key that will allow for posting to the SB that will monitor
   for responses, this will be a Send Only key and will be assigned key for SB, not a 
   generated token.  The idea here is that even if this does get compromised, it won't 
   be a big deal.  To make any use out of this, the rouge process would have to intercept
   a request from the server (not possible) to on premise and then craft a bogus response
   with the proper correlation id.  Possible issue here is DOS type attack but if they
   have the service bus address, they could do this anyway.


Defining settings in ITransceiverConnectionSettings

- RPCAdmin: Is only used by the server side to create topics as well as SAS tokens sent down to on premise
            for servers to subscribe to topic.

- RPCClientTransmitter: Settings for SB to Send a request to a server, note here topic will be created from TopicPrefix 
                         stored in Resource of ConnectionSttings

- RPCClientReceiver: Settings for the requester to subscribe to events that were sent in response to a request, topic
                     here will be fixed based on client such as web or api.  Subscription will be in the Url field
					 and will most likely be application.

- RPCServerTransmitter: Settings that will allow the server to post a response to the client that initated the request
                        Note the topic will come in with the request.  These are keys associated event hub not 
						SAS generated tokens (see above note) and are never on the on premise installation.  They
						are sent down in token broker when the on premise side requests then. 

Note: RPCServerReceiver are not setup in configugration but are generated as a SAS token in the TokenBroker and
      sent down with expiration period when the on premise side requests them.