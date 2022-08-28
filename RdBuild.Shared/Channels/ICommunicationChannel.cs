namespace RdBuild.Client;

/// The class is defining the channel to communication between client and server sides.
/// The channel is responsible for transfer (receive and send) serialized portions of information
/// and could be responsible to dispatch the received information by senders.
/// Data are grouping in packages and sending it in one of two modes: OneWay or SendReceive.
/// One data transaction must be registered by three lambdas - onSend, onReceive and onError,
/// sending expiration time and cancellation token.
/// The transaction will have the transaction Id, setting during the data placing in sending queue.
/// this Id must be passed to remote side at send and receiving at local side as remote side responsing.
/// The communication channels must be one of two kinds - ClientSide and ServerSide.
/// The ClientSide is initiating the transfer, it sends the query and can receive answer. The query
/// and answer are linking by query id.
public class ICommunicationChannel
{
    
}